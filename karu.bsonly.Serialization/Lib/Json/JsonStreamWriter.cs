// using System.Diagnostics;
// using karu.bsonly.Serialization.Interface;

// namespace karu.bsonly.Serialization;

// public class JsonStreamWriter : IBaseSerializer
// {
//   private readonly int _max_document_size;

//   private bool _first_member;

//   private Stream _stream;

//   private byte[] _buffer = new byte[20]; // cache int64 min: -9223372036854775808

//   public JsonStreamWriter(int max_doc_size)
//   {
//     _max_document_size = max_doc_size;
//     _stream = new MemoryStream();
//     _stream.WriteByte((byte)'{');
//     _first_member = true;
//   }

//   public JsonStreamWriter(Stream stream, int max_doc_size)
//   {
//     _max_document_size = max_doc_size;
//     _stream = stream;
//     _first_member = true;
//   }

//   private void WriteKeyString(ReadOnlySpan<byte> key_string)
//   {
//     if (!_first_member)
//       _stream.WriteByte((byte)',');
//     _first_member = false;
//     _stream.WriteByte((byte)' ');
//     WriteString(key_string);
//     _stream.WriteByte((byte)' ');
//     _stream.WriteByte((byte)':');
//     _stream.WriteByte((byte)' ');
//   }

//   public void WriteLong(ReadOnlySpan<byte> key_string, long value)
//   {
//     WriteKeyString(key_string);
//     if (!value.TryFormat(_buffer, out var bytes_written))
//     {
//       throw new BsonFormatException($"failed to convert {value} to string, {bytes_written} where written");
//     }
//     _stream.Write(_buffer, 0, bytes_written);
//     _stream.WriteByte((byte)' ');
//   }

//   public void WriteInt(ReadOnlySpan<byte> key_string, int value)
//   {
//     WriteKeyString(key_string);
//     if (!value.TryFormat(_buffer, out var bytes_written))
//     {
//       throw new BsonFormatException($"failed to convert {value} to string, {bytes_written} where written");
//     }
//     _stream.Write(_buffer, 0, bytes_written);
//     _stream.WriteByte((byte)' ');
//   }

//   public void WriteDouble(ReadOnlySpan<byte> key_string, double value)
//   {
//     WriteKeyString(key_string);
//     if (!value.TryFormat(_buffer, out var bytes_written))
//     {
//       throw new BsonFormatException($"failed to convert {value} to string, {bytes_written} where written");
//     }
//     _stream.Write(_buffer, 0, bytes_written);
//     _stream.WriteByte((byte)' ');
//   }

//   public void WriteBool(ReadOnlySpan<byte> key_string, bool value)
//   {
//     WriteKeyString(key_string);
//     if (value)
//       _stream.Write(System.Text.Encoding.ASCII.GetBytes("true"));
//     else
//       _stream.Write(System.Text.Encoding.ASCII.GetBytes("false"));
//     _stream.WriteByte((byte)' ');
//   }

//   public void WriteNull(ReadOnlySpan<byte> key_string)
//   {
//     WriteKeyString(key_string);
//     _stream.Write(System.Text.Encoding.ASCII.GetBytes("null"));
//     _stream.WriteByte((byte)' ');
//   }

//   public void WriteString(ReadOnlySpan<byte> key_string, ReadOnlySpan<byte> value)
//   {
//     WriteKeyString(key_string);
//     WriteString(value);
//     _stream.WriteByte((byte)' ');
//   }

//   public void WriteGuid(ReadOnlySpan<byte> key_string, Guid value)
//   {
//     // if (_settings.Sequences == Sequences.BINARY)
//     // {
//     var value_as_array = value.ToByteArray();
//     Debug.Assert(BsonSerialization.SIZE_OF_GUID == value_as_array.Length);

//     // Standard Bson Guid representation
//     Array.Reverse(value_as_array, 0, 4);
//     Array.Reverse(value_as_array, 4, 2);
//     Array.Reverse(value_as_array, 6, 2);

//     WriteBinary(key_string, BsonSerialization.BSON_BINARY_SUBTYPE_GUID, value_as_array.AsSpan());

//     // json
//     // }
//     // else
//     // {
//     //   var value_as_string = value.ToString();
//     //   WriteString(key, value_as_string);
//     // }
//   }

//   public void WriteBinary(ReadOnlySpan<byte> key_string, byte binary_subtype, ReadOnlySpan<byte> binary_data)
//   {
//     WriteTypeId(BsonSerialization.BSON_TYPE_BINARY);
//     WriteString(key_string);
//     var start_position = _stream.Position;
//     WriteSize(0); // dummy size
//     WriteTypeId(binary_subtype);
//     _stream.Write(binary_data);
//     WriteSizeAt(start_position);
//   }

//   public void WriteRawBinary(ReadOnlySpan<byte> key_string, ReadOnlySpan<byte> binary_data)
//   {
//     // ISSUE: is this a good interface??
//     // what should be part of the binary_data and what should be written by the serializer
//     WriteTypeId(BsonSerialization.BSON_TYPE_BINARY);
//     WriteString(key_string);
//     var start_position = _stream.Position;
//     WriteSize(0); // dummy size
//     _stream.Write(binary_data);
//     WriteSizeAt(start_position);
//   }


//   // public void WriteDocument(ReadOnlySpan<byte> key_string, object value)
//   // {
//   //   WriteTypeId(BsonSerialization.BSON_TYPE_DOCUMENT);
//   //   WriteString(key_string);
//   //   var pos = _stream.Position;
//   //   WriteSize(0); // dummy size
//   //   ObjectSerializer.Serialize(this, value);
//   //   FinishDocument(pos);
//   // }

//   public void WriteDocument<T>(ReadOnlySpan<byte> key_string, T document) where T : ISerializable
//   {
//     WriteTypeId(BsonSerialization.BSON_TYPE_DOCUMENT);
//     WriteString(key_string);
//     var pos = _stream.Position;
//     WriteSize(0);
//     document.Serialize(this, new SerializationContext());
//     FinishDocument(pos);
//   }

//   public void WriteRawDocument(ReadOnlySpan<byte> key_string, ReadOnlySpan<byte> document)
//   {
//     WriteKeyString(key_string);
//     _stream.WriteByte((byte)'{');
//     _stream.WriteByte((byte)' ');
//     _stream.Write(document);
//     _stream.WriteByte((byte)' ');
//     _stream.WriteByte((byte)'}');
//   }

//   public IBasicSerializer WriteDocument(ReadOnlySpan<byte> key_string)
//   {
//     WriteTypeId(BsonSerialization.BSON_TYPE_DOCUMENT);
//     WriteString(key_string);
//     return new StreamWriter(_stream, _max_document_size);
//   }

//   private void WriteTypeId(byte type_id)
//   {
//     _stream.WriteByte(type_id);
//   }

//   private void WriteString(ReadOnlySpan<byte> str)
//   {
//     _stream.WriteByte((byte)'"');
//     _stream.Write(str);
//     _stream.WriteByte((byte)'"');
//   }

//   private void WriteSize(int size)
//   {
//     var size_bytes = BitConverter.GetBytes(size);
//     _stream.Write(size_bytes);
//   }

//   private void WriteSizeAt(long start_position)
//   {
//     var size = _stream.Position - start_position - 1 - sizeof(int);
//     if (size < _max_document_size)
//     {
//       _stream.Seek(start_position, SeekOrigin.Begin);
//       WriteSize((int)size);
//       _stream.Seek(0, SeekOrigin.End);
//       return;
//     }

//     throw new ArgumentException("the bson document is too big");
//   }

//   private void FinishDocument(long start_position)
//   {
//     _stream.WriteByte((byte)'}');

//     var size = _stream.Position - start_position;
//     if (size < _max_document_size)
//     {
//       _stream.Seek(start_position, SeekOrigin.Begin);
//       WriteSize((int)size);
//       _stream.Seek(0, SeekOrigin.End);
//       return;
//     }

//     throw new ArgumentException("the bson document is too big");

//   }

//   public byte[] Finish()
//   {
//     _stream.WriteByte((byte)'}');

//     var bytes = Array.Empty<byte>();
//     if (_stream is MemoryStream memory_stream)
//     {
//       bytes = memory_stream.ToArray();
//     }
//     else
//     {
//       using (var mem_stream = new MemoryStream())
//       {
//         _stream.CopyTo(mem_stream);
//         bytes = mem_stream.ToArray();
//         _stream.Seek(0, SeekOrigin.End);
//       }
//     }

//     return bytes;
//   }

//   public IBasicSerializer WriteArray(ReadOnlySpan<byte> key_string)
//   {
//     WriteTypeId(BsonSerialization.BSON_TYPE_ARRAY);
//     WriteString(key_string);
//     return new StreamArrayWriter(_stream, _max_document_size);
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