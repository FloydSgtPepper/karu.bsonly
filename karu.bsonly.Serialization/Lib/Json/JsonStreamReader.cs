// using System.Diagnostics;
// using karu.bsonly.Serialization.Interface;

// namespace karu.bsonly.Serialization;

// // does not really handle a stream but only a completed stream
// public class JsonStreamReader : IDocumentDeserializer
// {

//   const long MIN_ENTRY_SIZE = 5; //  type id, 2bytes index str, at least 1 byte data + EOD

//   private const int SIZE_OF_SIZE_FIELD = sizeof(int);
//   const int MIN_DOC_SIZE = SIZE_OF_SIZE_FIELD + 1; //  size field + EOD

//   private bool _next_entry = false; // iterator points to an uncomsumed entry

//   private readonly int _max_document_size;

//   private readonly bool _out_of_order_evaluation = false;

//   private byte[] _buffer = new byte[BsonConstants.SIZE_OF_GUID]; // cache


//   private Stream _stream;

//   int _doc_start;

//   long _doc_length;

//   public JsonStreamReader(Stream stream, int max_doc_size, bool out_of_order_evaluation)
//   {
//     // bson_doc is a top level document, starting with the size

//     if (stream.Length < MIN_ENTRY_SIZE || stream.Position > stream.Length - MIN_DOC_SIZE) // is it allowed to have a bson doc with no entries???
//       throw new ArgumentException("doc is invalid");

//     _stream = stream;
//     _max_document_size = max_doc_size;
//     _out_of_order_evaluation = out_of_order_evaluation;
//     var size = ReadSize();
//     if (stream.Length != size)
//       throw new ArgumentException("bson document is not stored properly");

//     if (size > _max_document_size)
//       throw new ArgumentException("bson document is too big");
//     _doc_length = size;

//     if (InitIterator())
//     {
//       _next_entry = true;
//     }
//   }

//   public JsonStreamReader(Stream stream, int max_doc_size, bool out_of_order_evaluation, bool is_doc)
//   {
//     if (is_doc)
//     {
//       // doc is a document part, its starting with an entry
//       if (stream.Length < MIN_ENTRY_SIZE) // I think this is true, but check against null class
//         throw new ArgumentException("doc is invalid");

//       if (stream.Length > max_doc_size)
//         throw new ArgumentException("bson document is too big");

//       _stream = stream;
//       _max_document_size = max_doc_size;
//       _out_of_order_evaluation = out_of_order_evaluation;
//       if (InitIterator())
//       {
//         _next_entry = true;
//       }
//     }
//     else
//     {
//       // value type
//       _stream = stream;
//       _max_document_size = max_doc_size;
//       _out_of_order_evaluation = out_of_order_evaluation;
//       _doc_start = (int)_stream.Position;
//     }
//   }

//   public bool HasEntry(ReadOnlySpan<byte> key, byte type_id)
//   {
//     if (_stream.Position < _stream.Length - MIN_ENTRY_SIZE)
//     {
//       var read_type_id = ReadTypeId();
//       var read_key = ReadKeyString();

//       if (read_type_id == type_id && read_key.SequenceEqual(key))
//       {
//         _next_entry = true;
//         return true;
//       }
//     }

//     if (!_out_of_order_evaluation)
//       return false;

//     _next_entry = SearchEntry(key, type_id);
//     return _next_entry;
//   }

//   public byte HasEntry(ReadOnlySpan<byte> key)
//   {
//     if (_stream.Position < _stream.Length - MIN_ENTRY_SIZE)
//     {
//       var read_type_id = ReadTypeId();
//       var read_key = ReadKeyString();

//       if (read_key.SequenceEqual(key))
//       {
//         _next_entry = true;
//         return read_type_id;
//       }
//     }

//     if (!_out_of_order_evaluation)
//       return BsonConstants.BSON_TYPE_EOD;

//     return SearchEntry(key);
//   }


//   public bool HasNextEntry()
//   {
//     if (_stream.Position > _doc_length - MIN_DOC_SIZE)
//       return false;

//     // read next type id byte
//     var value = (byte)_stream.ReadByte();
//     _stream.Position--;

//     return !IsEod(value);
//   }

//   public bool SkipEntry(ReadOnlySpan<byte> key)
//   {
//     var read_type_id = ReadTypeId();
//     var read_key = ReadKeyString();
//     if (key.SequenceEqual(read_key))
//     {
//       ConsumeValue(read_type_id);
//       return true;
//     }

//     _next_entry = false;
//     return false;
//   }

//   public (ReadOnlyMemory<byte> key_string, byte type) NextEntry()
//   {
//     if (_stream.Position < _stream.Length - MIN_ENTRY_SIZE)
//     {
//       var read_type_id = ReadTypeId();
//       var read_key = ReadKeyString();

//       if (read_key.SequenceEqual(key))
//       {
//         _next_entry = true;
//         return read_type_id;
//       }
//     }

//     if (!_out_of_order_evaluation)
//       return BsonConstants.BSON_TYPE_EOD;

//     return SearchEntry(key);
//   }


//   public void ReadNull()
//   {
//     _stream.ReadByte();
//   }

//   public long ReadLong()
//   {
//     if (_stream.Read(_buffer, 0, sizeof(long)) == sizeof(long))
//       return BitConverter.ToInt64(_buffer);

//     throw new BufferUnderrunException();
//   }
//   public int ReadInt()
//   {
//     if (_stream.Read(_buffer, 0, sizeof(int)) == sizeof(int))
//       return BitConverter.ToInt32(_buffer);

//     throw new BufferUnderrunException();
//   }

//   public double ReadDouble()
//   {
//     if (_stream.Read(_buffer, 0, sizeof(double)) == sizeof(double))
//       return BitConverter.ToDouble(_buffer);

//     throw new BufferUnderrunException();
//   }

//   public bool ReadBool()
//   {
//     return _stream.ReadByte() != 0;
//   }

//   private ReadOnlySpan<byte> ReadString(int size)
//   {
//     Debug.Assert(size > 0);

//     if (size > int.MaxValue)
//       throw new ArgumentException("Can only read ReadOnlySpan<byte> with up to int.MaxValue characters");

//     if (size > _buffer.Length)
//     {
//       // if there is one long ReadOnlySpan<byte> there might be more
//       // -> therefor increase buffer
//       _buffer = new byte[size];
//     }

//     if (_stream.Read(_buffer, 0, size) != size)
//       throw new BufferUnderrunException($"expected to read {size} bytes but the stream did not deliver");
//     if (_buffer[size - 1] != 0)
//       throw new BsonSerializationException("ReadOnlySpan<byte> size information not consistent");
//     return _buffer.AsSpan(0, size - 1);
//   }

//   private ReadOnlySpan<byte> ReadData(int size)
//   {
//     Debug.Assert(size > 0);

//     if (size > int.MaxValue)
//       throw new ArgumentException("Can only read ReadOnlySpan<byte> with up to int.MaxValue characters");

//     if (size > _buffer.Length)
//     {
//       // if there is one long ReadOnlySpan<byte> there might be more
//       // -> therefor increase buffer
//       _buffer = new byte[size];
//     }

//     if (_stream.Read(_buffer, 0, size) != size)
//       throw new BufferUnderrunException($"expected to read {size} bytes but the stream did not deliver");
//     return _buffer.AsSpan<byte>(0, size);
//   }

//   public ReadOnlySpan<byte> ReadString()
//   {
//     return ReadString(ReadSize());
//   }

//   public Guid ReadGuid()
//   {
//     var guid_data = ReadBinary(BsonConstants.BSON_BINARY_SUBTYPE_GUID);
//     return new Guid(guid_data);

//     // json
//     // }
//     // else
//     // {
//     //   var guid_as_string = ReadString(BsonConstants.SIZE_OF_GUID);
//     //   if (Guid.TryParseExact(guid_as_string, "D", out var guid_value))
//     //     return guid_value;
//     //   throw new TypeException("ReadOnlySpan<byte> could not be parsed as guid");
//     // }
//   }

//   public ReadOnlySpan<byte> ReadBinary(byte user_type)
//   {
//     // ISSUE: todo todo
//     var size = ReadSize();
//     if (user_type != ReadTypeId())
//       return ReadOnlySpan<byte>.Empty;

//     return ReadData(size);
//   }

//   public ReadOnlySpan<byte> ReadRawBinary()
//   {
//     // TODO: how to handle raw binary
//     var size = ReadSize();

//     return _buffer.AsSpan(0, _buffer.Length); // size;
//   }

//   public ReadOnlySpan<byte> ReadRawDocument()
//   {
//     return new ReadOnlySpan<byte>(new byte[] { 0, 0, 0, });
//   }

//   public ReadOnlySpan<byte> ReadRawArray()
//   {
//     return new ReadOnlySpan<byte>(new byte[] { 0, 0, 0, });
//   }

//   public IArrayDeserializer ReadArray()
//   {
//     var size = ReadSize();
//     var array_reader = new MemoryArrayReader(_stream, _max_document_size, _out_of_order_evaluation);
//     return array_reader;
//   }

//   // public IBasicDeserializer ReadDocument()
//   // {
//   //   var size = ReadSize();
//   //   var reader = new MemoryDocReader(_stream, _settings, is_doc: true); // also give the buffer?
//   //   return reader;
//   // }

//   public void ReadDocument(object value)
//   {
//     var size = ReadSize();
//     var reader = new MemoryDocReader(_stream, _max_document_size, _out_of_order_evaluation, is_doc: true); // also give the buffer?
//     ObjectSerialization.Deserialize(reader, value);
//     ConsumeEndByte();
//   }

//   public void ReadDocument<T>(T value) where T : ISerializable
//   {
//     var size = ReadSize();
//     var reader = new MemoryDocReader(_stream, _max_document_size, _out_of_order_evaluation, is_doc: true); // also give the buffer?
//     value.Deserialize(reader, new DeserializationContext());
//     ConsumeEndByte();
//   }

//   public void ConsumeEndByte()
//   {
//     if (ReadTypeId() != BsonConstants.BSON_TYPE_EOD)
//       throw new ArgumentException("invalid bson document");
//   }

//   private bool IsEod(byte value)
//   {
//     if (value == BsonConstants.BSON_TYPE_EOD)
//       return true;
//     return false;
//   }

//   private bool SearchEntry(ReadOnlySpan<byte> key, byte type_id)
//   {
//     ResetIterator();

//     bool searching = true;
//     do
//     {
//       if (_stream.Position > _doc_length - 2/*2: (empty)key + type id*/)
//       {
//         searching = false;
//         _next_entry = false;
//         break;
//       }

//       var read_type_id = ReadTypeId();
//       var read_key = ReadKeyString();
//       if (read_type_id == type_id && read_key.SequenceEqual(key))
//       {
//         searching = false;
//         _next_entry = true;
//         break;
//       }
//       ConsumeValue(read_type_id);
//     } while (searching);

//     return _next_entry;
//   }

//   private byte SearchEntry(ReadOnlySpan<byte> key)
//   {
//     ResetIterator();

//     bool searching = true;
//     do
//     {
//       if (_stream.Position > _doc_length - 2/*2: (empty)key + type id*/)
//       {
//         _next_entry = false;
//         return BsonConstants.BSON_TYPE_EOD;
//       }

//       var read_type_id = ReadTypeId();
//       var read_key = ReadKeyString();
//       if (read_key.SequenceEqual(key))
//       {
//         _next_entry = true;
//         return read_type_id;
//       }
//       ConsumeValue(read_type_id);
//     } while (true);
//   }


//   private byte ReadOneByte()
//   {
//     var value = _stream.ReadByte();
//     return (byte)value;
//   }

//   private byte ReadTypeId()
//   {
//     return ReadOneByte();
//   }

//   private void ResetIterator()
//   {
//     _stream.Position = _doc_start;
//   }

//   private bool InitIterator()
//   {
//     var value = (byte)_stream.ReadByte();
//     if (IsEod(value))
//       return false;

//     _doc_start = (int)(_stream.Position - 1);

//     _next_entry = true;

//     var key = ReadKeyString(); // sets _next_entry
//     if (_next_entry != false)
//     {
//       ResetIterator();
//       return true;
//     }

//     return false;
//   }


//   /// <summary>
//   /// read null terminated key ReadOnlySpan<byte>
//   /// </summary>
//   /// <returns></returns>
//   private ReadOnlySpan<byte> ReadKeyString()
//   {
//     int value;
//     int idx = 0;
//     do
//     {
//       value = _stream.ReadByte();
//       _buffer[idx] = (byte)value;
//       ++idx;

//       // TODO: is this overflow handling needed? maybe we can check the bson document size instead
//       // in the ctor
//       if (idx >= _buffer.Length)
//       {
//         if (idx == int.MaxValue)
//           break;

//         int new_size;
//         if (_buffer.Length > (int.MaxValue - 1) / 2)
//           new_size = int.MaxValue - 1;
//         else
//           new_size = _buffer.Length * 2;

//         var new_buffer = new byte[new_size];
//         Array.Copy(_buffer, new_buffer, _buffer.Length);
//         _buffer = new_buffer;
//       }
//     } while (value != 0);

//     if (value == 0 && idx == 1)
//       return ReadOnlySpan<byte>.Empty;
//     if (value == 0)
//     {
//       return _buffer.AsSpan(0, idx - 1);
//     }
//     else
//     {
//       _next_entry = false;
//       return ReadOnlySpan<byte>.Empty;
//     }
//   }

//   private void ConsumeValue(byte type_id)
//   {
//     switch (type_id)
//     {
//       case BsonConstants.BSON_TYPE_DOUBLE:
//         ReadDouble();
//         break;
//       case BsonConstants.BSON_TYPE_UTF8:
//         ReadString();
//         break;
//       case BsonConstants.BSON_TYPE_BOOL:
//         ReadBool();
//         break;
//       case BsonConstants.BSON_TYPE_NULL:
//         ReadNull();
//         break;
//       case BsonConstants.BSON_TYPE_INT32:
//         ReadInt();
//         break;
//       case BsonConstants.BSON_TYPE_INT64:
//         ReadLong();
//         break;
//       case BsonConstants.BSON_BINARY_SUBTYPE_GUID:
//         ReadGuid();
//         break;
//       case BsonConstants.BSON_TYPE_BINARY:
//         // ReadBinary();
//         break;
//       case BsonConstants.BSON_TYPE_DOCUMENT:
//         // ReadDocument();
//         break;
//     }
//   }

//   private int ReadSize()
//   {
//     if (_stream.Read(_buffer, 0, sizeof(int)) == sizeof(int))
//       return BitConverter.ToInt32(_buffer);

//     throw new BufferUnderrunException();
//   }
// }

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