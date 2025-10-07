using System.Diagnostics;
using karu.bsonly.Serialization.Interface;

namespace karu.bsonly.Serialization;

public class StreamWriter : IBaseSerializer
{
  private readonly int _max_document_size;

  private Stream _stream;

  private readonly long _start_position;

  public StreamWriter(int max_doc_size)
  {
    _max_document_size = max_doc_size;
    _stream = new MemoryStream();

    _start_position = _stream.Position;
    WriteSize(0); // dummy write
  }

  public StreamWriter(Stream stream, int max_doc_size, bool value_writer = false)
  {
    // FIXME: instead of value_writer parameter have a IBaseSerializer and IDocSerializer
    //  IArraySerializer -> has IBaseSerializer (member) to write data
    // IDocSerializer -> has IBaseSerializer (member) to write data 
    // -> actually: should IBaseWriter have IArraySerializer / IDocSerializer or should
    //   IArraySerialiezr/IDocSerializer contain an IBaseSerializer???

    _max_document_size = max_doc_size;
    _stream = stream;

    _start_position = _stream.Position;

    if (!value_writer)
      WriteSize(0); // dummy write
  }

  public void WriteLong(ReadOnlySpan<byte> key_string, long value)
  {
    WriteTypeId(BsonConstants.BSON_TYPE_INT64);
    WriteString(key_string);
    var buffer = BitConverter.GetBytes(value);
    _stream.Write(buffer);
  }

  public void WriteInt(ReadOnlySpan<byte> key_string, int value)
  {
    WriteTypeId(BsonConstants.BSON_TYPE_INT32);
    WriteString(key_string);
    var buffer = BitConverter.GetBytes(value);
    _stream.Write(buffer);
  }

  public void WriteDouble(ReadOnlySpan<byte> key_string, double value)
  {
    WriteTypeId(BsonConstants.BSON_TYPE_DOUBLE);
    WriteString(key_string);
    var buffer = BitConverter.GetBytes(value);
    _stream.Write(buffer);
  }

  public void WriteBool(ReadOnlySpan<byte> key_string, bool value)
  {
    WriteTypeId(BsonConstants.BSON_TYPE_BOOL);
    WriteString(key_string);
    if (value)
      _stream.WriteByte(1);
    else
      _stream.WriteByte(0);
  }

  public void WriteNull(ReadOnlySpan<byte> key_string)
  {
    WriteTypeId(BsonConstants.BSON_TYPE_NULL);
    WriteString(key_string);
    _stream.WriteByte(0);
  }

  public void WriteString(ReadOnlySpan<byte> key_string, ReadOnlySpan<byte> value)
  {
    WriteTypeId(BsonConstants.BSON_TYPE_UTF8);
    WriteString(key_string);

    WriteSize(value.Length + 1);
    _stream.Write(value);
    _stream.WriteByte(0);
  }

  public void WriteGuid(ReadOnlySpan<byte> key_string, Guid value)
  {
    // if (_settings.Sequences == Sequences.BINARY)
    // {
    var value_as_array = value.ToByteArray();
    Debug.Assert(BsonConstants.SIZE_OF_GUID == value_as_array.Length);

    // Standard Bson Guid representation
    Array.Reverse(value_as_array, 0, 4);
    Array.Reverse(value_as_array, 4, 2);
    Array.Reverse(value_as_array, 6, 2);

    WriteBinary(key_string, value_as_array.AsSpan(), BsonConstants.BSON_BINARY_SUBTYPE_GUID);

    // json
    // }
    // else
    // {
    //   var value_as_string = value.ToString();
    //   WriteString(key, value_as_string);
    // }
  }

  public void WriteBinary(ReadOnlySpan<byte> key_string, ReadOnlySpan<byte> binary_data, byte binary_subtype)
  {
    WriteTypeId(BsonConstants.BSON_TYPE_BINARY);
    WriteString(key_string);
    var start_position = _stream.Position;
    WriteSize(0); // dummy size
    WriteTypeId(binary_subtype);
    _stream.Write(binary_data);
    WriteSizeAt(start_position);
  }

  public void WriteDocument<T>(ReadOnlySpan<byte> key_string, T document) where T : ISerializable
  {
    WriteTypeId(BsonConstants.BSON_TYPE_DOCUMENT);
    WriteString(key_string);
    var pos = _stream.Position;
    WriteSize(0);
    document.Serialize(this, new SerializationContext());
    FinishDocument(pos);
  }

  public void WriteRawDocument(ReadOnlySpan<byte> key_string, ReadOnlySpan<byte> document)
  {
    WriteTypeId(BsonConstants.BSON_TYPE_DOCUMENT);
    WriteString(key_string);
    WriteSize(document.Length);
    _stream.Write(document);
    WriteEod();
  }

  private void WriteTypeId(byte type_id)
  {
    _stream.WriteByte(type_id);
  }
  private void WriteEod()
  {
    _stream.WriteByte(BsonConstants.BSON_TYPE_EOD);
  }

  private void WriteString(ReadOnlySpan<byte> str)
  {
    _stream.Write(str);
    _stream.WriteByte(0);
  }

  private void WriteSize(int size)
  {
    var size_bytes = BitConverter.GetBytes(size);
    _stream.Write(size_bytes);
  }

  private void WriteSizeAt(long start_position)
  {
    var size = _stream.Position - start_position - 1 - sizeof(int);
    if (size < _max_document_size)
    {
      _stream.Seek(start_position, SeekOrigin.Begin);
      WriteSize((int)size);
      _stream.Seek(0, SeekOrigin.End);
      return;
    }

    throw new ArgumentException("the bson document is too big");
  }

  private void FinishDocument(long start_position)
  {
    WriteEod();

    var size = _stream.Position - start_position;
    if (size < _max_document_size)
    {
      _stream.Seek(start_position, SeekOrigin.Begin);
      WriteSize((int)size);
      _stream.Seek(0, SeekOrigin.End);
      return;
    }

    throw new ArgumentException("the bson document is too big");
  }

  public byte[] Finish()
  {
    WriteEod();
    var size = _stream.Position - _start_position;

    if (size < _max_document_size)
    {
      _stream.Seek(_start_position, SeekOrigin.Begin);
      WriteSize((int)size);
      _stream.Seek(_start_position, SeekOrigin.Begin);

      var bytes = Array.Empty<byte>();
      if (_stream is MemoryStream memory_stream)
      {
        bytes = memory_stream.ToArray();
      }
      else
      {
        using (var mem_stream = new MemoryStream())
        {
          _stream.CopyTo(mem_stream);
          bytes = mem_stream.ToArray();
        }
      }

      _stream.Seek(0, SeekOrigin.End);
      return bytes;
    }

    throw new ArgumentException("the bson document is too big");
  }

  public IArraySerializer SerializeArray(ReadOnlySpan<byte> key_string)
  {
    WriteTypeId(BsonConstants.BSON_TYPE_ARRAY);
    WriteString(key_string);

    return new StreamArrayWriter(_stream, _max_document_size);
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