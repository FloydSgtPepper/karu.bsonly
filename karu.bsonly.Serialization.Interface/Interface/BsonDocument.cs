using System;
using System.Diagnostics;

namespace karu.bsonly.Serialization.Interface
{
  public class BsonDocument : IDisposable
  {
    // implementation:
    //  _iterator._position is always < _doc.length so that one byte can be read without length checking

    internal enum IterState : byte
    {
      Invalid,
      AtElement,
      AfterElement
    }

    internal struct Iter
    {
      public int _position;
      public int _doc_end;

      public int _doc_start;

      public byte _element_type;

      public ReadOnlyMemory<byte> _element_key;
      public IterState _state;

    }

    public const long MIN_ENTRY_SIZE = 5; //  type id, 2bytes index str, at least 1 byte data + EOD

    public const int SIZE_OF_SIZE_FIELD = sizeof(int);
    public const int MIN_DOC_SIZE = SIZE_OF_SIZE_FIELD + 1; //  size field + EOD -> https://bsonspec.org/spec.html

    private byte[] _doc;

    private BsonDocument? _parent = null;


    private Iter _iterator;

    private bool _out_of_order_evaluation;

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
      if (_disposed) return;

      if (disposing)
      {
        // Dispose managed state (managed objects)
        _parent = null;
        _doc = Array.Empty<byte>();
      }
      // Free unmanaged resources

      _disposed = true;
    }

    public void Dispose()
    {
      // Dispose of unmanaged resources
      Dispose(true);

      // Suppress finalization
      GC.SuppressFinalize(this);
    }

    public BsonDocument(byte[] bson_doc, bool out_of_order_evaluation)
    {
      if (bson_doc.Length < MIN_DOC_SIZE)
        throw new ArgumentException("bson_doc is invalid");
      _doc = bson_doc;
      _iterator._position = 0;
      _iterator._doc_end = bson_doc.Length;
      _iterator._state = IterState.Invalid;

      var doc_size = ReadSize();
      if (_iterator._doc_end != doc_size)
        throw new ArgumentException("bson_doc has invalid size");
      _iterator._doc_end = doc_size; // doc_size could be smaller than bson_doc.Length
      _iterator._doc_start = _iterator._position;

      SetIterator();

      _out_of_order_evaluation = out_of_order_evaluation;
    }

    /// <summary>
    /// Optimization: Create a BsonDocument over a part of the bson_doc buffer
    ///   The document starts with a value (type byte) instead of document size.
    ///    Expected input for bson_doc e.g.: 0x02 <utf8_key> <size> <utf8 text>
    ///     or 0x12 <utf8 key> <int64>
    /// </summary>
    /// <param name="bson_doc">the document buffer</param>
    /// <param name="start_idx">start index of document in the buffer</param>
    /// <param name="count">number of bytes which contain the document</param>
    /// <param name="out_of_order_evaluation">whether to allow out of order evaluation</param>
    /// <exception cref="ArgumentException"></exception>
    public BsonDocument(byte[] bson_doc, int start_idx, int count, bool out_of_order_evaluation)
    {
      if (start_idx + count > bson_doc.Length)
        throw new ArgumentException("parameter start_idx, count and bson_doc are inconsistent");

      if (start_idx < 0 || count < 0)
        throw new ArgumentException("parameter start_idx, count and bson_doc are inconsistent");

      if (count < MIN_ENTRY_SIZE)
        throw new ArgumentException("the document is too short");

      _doc = bson_doc;
      _iterator._position = start_idx;
      _iterator._doc_start = _iterator._position;
      _iterator._doc_end = start_idx + count;
      _iterator._state = IterState.Invalid;

      if (_iterator._doc_end > bson_doc.Length)
        throw new ArgumentException("bson_doc has invalid size");

      SetIterator();
      _out_of_order_evaluation = out_of_order_evaluation;
    }

    /// <summary>
    /// create a child document. The parent must be at a Bson Binary, Bson Array or Bson Document
    /// position.
    /// </summary>
    /// <param name="parent"></param>
    /// <exception cref="ArgumentException"></exception>
    private BsonDocument(BsonDocument parent)
    {
      _parent = parent;
      _doc = parent._doc;
      _out_of_order_evaluation = parent._out_of_order_evaluation;
      _iterator._position = _parent._iterator._position;
      _iterator._doc_start = _iterator._position;
      _iterator._state = IterState.Invalid;

      _iterator._doc_end = _doc.Length;
      var size = ReadSize();
      _iterator._doc_end = _iterator._position - SIZE_OF_SIZE_FIELD + size;
      if (_iterator._doc_end - _iterator._doc_start < MIN_DOC_SIZE)
        throw new ArgumentException("the document is too small");
      _iterator._doc_start = _iterator._position;

      if (_iterator._doc_end >= _doc.Length)
        throw new ArgumentException($"sub document at position {_iterator._doc_start} is invalid");

      SetIterator();
    }

    public static BsonDocument SubDocument(BsonDocument parent)
    {
      return new BsonDocument(parent);
    }

    public BsonDocument SubDocument()
    {
      return new BsonDocument(this);
    }

    public BsonDocument ValuesDoc()
    {
      // FIXME: only needed because DocReader and BaseReader are not clearly defined
      return new BsonDocument(_doc, _iterator._doc_start, _iterator._doc_end - _iterator._doc_start, _out_of_order_evaluation);
    }

    public int ReadSize()
    {
      if (_iterator._position + SIZE_OF_SIZE_FIELD < _iterator._doc_end)
      {
        var size_field = new ReadOnlySpan<byte>(_doc, _iterator._position, SIZE_OF_SIZE_FIELD);
        _iterator._position += SIZE_OF_SIZE_FIELD;
        return BitConverter.ToInt32(size_field);
      }

      throw new BufferUnderrunException($"Expected to read {SIZE_OF_SIZE_FIELD}bytes from position {_iterator._position} but reached end of document at position {_iterator._doc_end}");
    }

    public int CurrentPosition()
    {
      if (_iterator._state != IterState.Invalid)
        return _iterator._position;

      return -1;
    }

    public BsonDocument NextElement()
    {
      var cur_pos = _iterator._position;
      var (key, type) = NextEntry();
      ConsumeValue(type);
      var new_pos = _iterator._position;
      return new BsonDocument(_doc, cur_pos, new_pos - cur_pos, _out_of_order_evaluation);
    }

    public void Finish()
    {
      _iterator._position = _iterator._doc_end - 1;
      _iterator._state = IterState.Invalid;
      var byte_read = _doc[_iterator._position];
      Debug.Assert(byte_read == BsonConstants.BSON_TYPE_EOD);
    }

    public bool HasEntry(ReadOnlySpan<byte> key, byte type_id)
    {
      if (_iterator._state == IterState.Invalid)
        return false;

      var start_pos = _iterator._position;

      // maybe already there?
      if (_iterator._state == IterState.AtElement)
      {
        if (type_id == _iterator._element_type && key.SequenceEqual(_iterator._element_key.Span))
          return true;

        ConsumeValueInternal(_iterator._element_type);
      }

      // try the next elements type and key
      if (_iterator._position < _iterator._doc_end - MIN_ENTRY_SIZE)
      {
        var read_type_id = ReadTypeId();
        var read_key = ReadKeyString();

        if (read_type_id == type_id && read_key.SequenceEqual(key))
        {
          SetIteratorKeyAndType(key, type_id);
          return true;
        }
      }

      if (!_out_of_order_evaluation)
      {
        _iterator._position = start_pos;
        return false;
      }

      if (SearchEntry(key, type_id))
      {
        SetIteratorKeyAndType(key, type_id);
        return true;
      }

      _iterator._position = start_pos;
      return false;
    }

    public byte HasEntry(ReadOnlySpan<byte> key)
    {
      if (_iterator._state == IterState.Invalid)
        return BsonConstants.BSON_TYPE_EOD;

      var start_pos = _iterator._position;

      // maybe already there?
      if (_iterator._state == IterState.AtElement)
      {
        if (key.SequenceEqual(_iterator._element_key.Span))
          return _iterator._element_type;

        ConsumeValueInternal(_iterator._element_type);
      }

      // try the next elements type and key
      if (_iterator._position < _iterator._doc_end - MIN_ENTRY_SIZE)
      {
        var read_type_id = ReadTypeId();
        var read_key = ReadKeyString();

        if (read_key.SequenceEqual(key))
        {
          SetIteratorKeyAndType(key, read_type_id);
          return read_type_id;
        }
      }

      if (!_out_of_order_evaluation)
      {
        _iterator._position = start_pos;
        return BsonConstants.BSON_TYPE_EOD;
      }

      var type_id = SearchEntry(key);
      if (type_id != BsonConstants.BSON_TYPE_EOD)
      {
        SetIteratorKeyAndType(key, type_id);
        return type_id;
      }

      _iterator._position = start_pos;
      return BsonConstants.BSON_TYPE_EOD;
    }

    public bool HasNextEntry()
    {
      if (_iterator._position > _iterator._doc_end - MIN_DOC_SIZE)
        return false;

      // read next type id byte
      var value = _doc[_iterator._position];

      return !IsEod(value);
    }

    public bool SkipEntry(ReadOnlySpan<byte> key)
    {
      // FIXME: unit test needed

      var start_pos = _iterator._position;
      byte type_id = BsonConstants.BSON_TYPE_EOD;

      if (_iterator._state == IterState.AtElement && key.SequenceEqual(key))
      {
        type_id = _iterator._element_type;
      }
      else if (_iterator._state == IterState.AfterElement)
      {
        type_id = ReadTypeId();
        var read_key = ReadKeyString();
        if (key.SequenceEqual(read_key))
        {
          _iterator._state = IterState.AtElement;
          // no need to save key and type since we will move forward anyway
        }
        else
        {
          _iterator._position = start_pos;
          // _iterator._state = IterState.Invalid; - forgiving
        }
      }

      if (_iterator._state == IterState.AtElement)
      {
        ConsumeValue(type_id);
        return true;
      }

      return false;
    }

    public (ReadOnlyMemory<byte> key_string, byte type) NextEntry()
    {
      // note: out_of_order_evaluation: TODO:

      if (_iterator._state == IterState.AtElement)
        return (_iterator._element_key, _iterator._element_type);

      if (_iterator._position < _iterator._doc_end - MIN_ENTRY_SIZE)
      {
        var read_type_id = ReadTypeId();

        if ((read_type_id == BsonConstants.BSON_TYPE_BOOL
            || read_type_id == BsonConstants.BSON_TYPE_NULL
            || read_type_id == BsonConstants.BSON_TYPE_DOUBLE
            || read_type_id == BsonConstants.BSON_TYPE_INT32
            || read_type_id == BsonConstants.BSON_TYPE_INT64
            || read_type_id == BsonConstants.BSON_TYPE_UTF8
            || read_type_id == BsonConstants.BSON_TYPE_ARRAY
            || read_type_id == BsonConstants.BSON_TYPE_BINARY
            || read_type_id == BsonConstants.BSON_TYPE_DOCUMENT
            )
          && ReadAndSetKeyString())
        {
          _iterator._element_type = read_type_id;
          return (_iterator._element_key, _iterator._element_type);
        }
      }

      Finish();
      return (ReadOnlyMemory<byte>.Empty, BsonConstants.BSON_TYPE_EOD);
    }

    public byte BinarySubType()
    {
      if (_iterator._state == IterState.AtElement && _iterator._element_type == BsonConstants.BSON_TYPE_BINARY)
      {
        var subtype = _doc[_iterator._position + SIZE_OF_SIZE_FIELD];
        return subtype;
      }

      throw new BsonSerializationException("not at a binary element");
    }

    public void ConsumeValue(byte type_id)
    {
      ConsumeValueInternal(type_id);
      _iterator._state = IterState.AfterElement;
      if (_iterator._position >= _iterator._doc_end)
        Finish();
    }

    public ReadOnlySpan<byte> CurrentElement()
    {
      if (_iterator._state != IterState.AtElement)
      {
        Finish();
        return ReadOnlySpan<byte>.Empty;
      }

      var (start, size) = ConsumeValueInternal(_iterator._element_type);
      _iterator._state = IterState.AfterElement;
      return new ReadOnlySpan<byte>(_doc, start, size);
    }

    private byte ReadTypeId()
    {
      var value = _doc[_iterator._position];
      _iterator._position++;
      return value;
    }

    private void ResetIterator()
    {
      _iterator._position = _iterator._doc_start;
    }


    private bool SetIterator()
    {
      var type_value = _doc[_iterator._position];
      ++_iterator._position;
      if (IsEod(type_value))
        return false;

      _iterator._element_type = type_value;
      return ReadAndSetKeyString(); // sets _iterator._key and _iterator._is_at_element
    }

    /// <summary>
    /// read null terminated key string
    /// </summary>
    /// <returns></returns>
    private ReadOnlySpan<byte> ReadKeyString()
    {
      var start_pos = _iterator._position;

      var value = _doc[_iterator._position];
      _iterator._position++;

      while (value != 0 && _iterator._position < _iterator._doc_end)
      {
        value = _doc[_iterator._position];
        _iterator._position++;
      }

      if (value == 0)
      {
        // we have reached the string end marker
        return new ReadOnlySpan<byte>(_doc, start_pos, _iterator._position - start_pos - 1);
      }
      else
      {
        return ReadOnlySpan<byte>.Empty;
      }
    }

    /// <summary>
    /// read null terminated key string
    /// </summary>
    /// <returns></returns>
    private bool ReadAndSetKeyString()
    {
      var start_pos = _iterator._position;

      var value = (byte)1;
      while (value != 0 && _iterator._position < _iterator._doc_end)
      {
        value = _doc[_iterator._position];
        _iterator._position++;
      }

      if (value == 0)
      {
        // we have reached the string end marker
        _iterator._state = IterState.AtElement;
        _iterator._element_key = new ReadOnlyMemory<byte>(_doc, start_pos, _iterator._position - start_pos - 1);
        return true;
      }
      else
      {
        _iterator._state = IterState.Invalid;
        _iterator._element_key = ReadOnlyMemory<byte>.Empty;
        return false;
      }
    }

    private bool SearchEntry(ReadOnlySpan<byte> key, byte type_id)
    {
      ResetIterator();

      while (true)
      {
        if (_iterator._position > _iterator._doc_end - 2/*2: (empty)key + type id*/)
          break;

        var read_type_id = ReadTypeId();
        var read_key = ReadKeyString();
        Debug.WriteLine($"key: {System.Text.Encoding.UTF8.GetString(read_key)} pos: {_iterator._position}");

        if (read_type_id == type_id && read_key.SequenceEqual(key))
          return true;

        ConsumeValueInternal(read_type_id);
      }

      return false;
    }

    private byte SearchEntry(ReadOnlySpan<byte> key)
    {
      ResetIterator();

      while (true)
      {
        if (_iterator._position > _iterator._doc_end - 2/*2: (empty)key + type id*/)
          break;

        var read_type_id = ReadTypeId();
        var read_key = ReadKeyString();
        Debug.WriteLine($"key: {System.Text.Encoding.UTF8.GetString(read_key)} pos: {_iterator._position}");

        if (read_key.SequenceEqual(key))
          return read_type_id;

        ConsumeValueInternal(read_type_id);
      }

      return BsonConstants.BSON_TYPE_EOD;
    }

    private (int start, int size) ConsumeValueInternal(byte type_id)
    {
      var start_pos = _iterator._position;
      switch (type_id)
      {
        case BsonConstants.BSON_TYPE_DOUBLE:
          _iterator._position += sizeof(double);
          return (start_pos, sizeof(double));

        case BsonConstants.BSON_TYPE_UTF8:
          var str_size = ReadSize();
          _iterator._position += str_size;
          return (start_pos + SIZE_OF_SIZE_FIELD, str_size - 1);

        case BsonConstants.BSON_TYPE_BOOL:
          _iterator._position += sizeof(bool);
          return (start_pos, sizeof(bool));

        case BsonConstants.BSON_TYPE_NULL:
          _iterator._position += 1;
          return (start_pos, 1);

        case BsonConstants.BSON_TYPE_INT32:
          _iterator._position += sizeof(int);
          return (start_pos, sizeof(int));

        case BsonConstants.BSON_TYPE_INT64:
          _iterator._position += sizeof(long);
          return (start_pos, sizeof(long));

        case BsonConstants.BSON_TYPE_BINARY:
          var bin_size = ReadSize();
          _iterator._position += bin_size /*- SIZE_OF_SIZE_FIELD*/ + 1/*subtype*/;
          return (start_pos + SIZE_OF_SIZE_FIELD, bin_size + 1);

        case BsonConstants.BSON_TYPE_DOCUMENT:
          var value_bytes = ReadSize();
          _iterator._position += value_bytes - SIZE_OF_SIZE_FIELD;
          return (start_pos + SIZE_OF_SIZE_FIELD, value_bytes - SIZE_OF_SIZE_FIELD);

        case BsonConstants.BSON_TYPE_ARRAY:
          var array_bytes = ReadSize();
          _iterator._position += array_bytes - SIZE_OF_SIZE_FIELD;
          return (start_pos + SIZE_OF_SIZE_FIELD, array_bytes - SIZE_OF_SIZE_FIELD);

        case BsonConstants.BSON_TYPE_EOD:
          _iterator._position++;
          return (start_pos, 1);

      }
      return (start_pos, 0);
    }

    private bool IsEod(byte value)
    {
      return value == BsonConstants.BSON_TYPE_EOD;
    }

    private void SetIteratorKeyAndType(ReadOnlySpan<byte> key, byte type_id)
    {
      _iterator._state = IterState.AtElement;
      _iterator._element_key = new ReadOnlyMemory<byte>(_doc, _iterator._position - 1 - key.Length, key.Length);
      _iterator._element_type = type_id;
    }

  }
}

#region Copyright notice and license

// Copyright 2025 The bsonly Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion