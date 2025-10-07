
using karu.bsonly.Serialization.Interface;

namespace karu.bsonly.Serialization;

public class StreamArrayWriter : IArraySerializer
{
  // bson array fo ints
  // "0x2d000000 04 6c6973743100 21000000 10 3000 01000000 10 3100 02000000 10 3200 03000000 10 300 05000000 00 00"

  private readonly int _max_document_size;

  private Stream _stream;

  private int index_key = 0;

  private byte[] _buffer = new byte[11]; // int32.max = 2,147,483,647

  private readonly long _start_position;

  public StreamArrayWriter(Stream stream, int max_doc_size)
  {
    _max_document_size = max_doc_size;
    _stream = stream;

    _start_position = _stream.Position;
    WriteSize(0); // dummy write
  }

  public IBaseSerializer Append()
  {
    WriteTypeId(BsonConstants.BSON_TYPE_ARRAY);

    WriteString(WriteIndexAsString(index_key));
    ++index_key;

    return new StreamWriter(_stream, _max_document_size);
  }

  public void Add(long value)
  {
    BasicWriter.WriteLong(_stream, WriteIndexAsString(index_key), value);
    ++index_key;
  }

  public void Add(int value)
  {
    BasicWriter.WriteInt(_stream, WriteIndexAsString(index_key), value);
    ++index_key;
  }

  public void Add(ReadOnlySpan<byte> value)
  {
    BasicWriter.WriteString(_stream, WriteIndexAsString(index_key), value);
    ++index_key;
  }

  public void Add(bool value)
  {
    BasicWriter.WriteBool(_stream, WriteIndexAsString(index_key), value);
    ++index_key;
  }

  public void Add(double value)
  {
    BasicWriter.WriteDouble(_stream, WriteIndexAsString(index_key), value);
    ++index_key;
  }

  public void AddNull()
  {
    BasicWriter.WriteNull(_stream, WriteIndexAsString(index_key));
    ++index_key;
  }

  public void Add(Guid value)
  {
    BasicWriter.WriteGuid(_stream, WriteIndexAsString(index_key), value);
    ++index_key;
  }

  public void AddBinary(ReadOnlySpan<byte> value, byte binary_subtype)
  {
    BasicWriter.WriteBinary(_stream, WriteIndexAsString(index_key), value, binary_subtype);
    ++index_key;
  }

  public void AddDocument<T>(T value) where T : ISerializable
  {
    var writer = new StreamWriter(_stream, _max_document_size);
    writer.WriteDocument(WriteIndexAsString(index_key), value);
    ++index_key;
  }

  public (IBaseSerializer value_serializer, byte[] key_string) SerializeValue()
  {
    var writer = new StreamWriter(_stream, _max_document_size, value_writer: true);
    byte[] key = WriteIndexAsString(index_key).ToArray();
    ++index_key;
    return (writer, key);
  }


  public void Finish()
  {
    WriteTypeId(BsonConstants.BSON_TYPE_EOD);
    var size = _stream.Position - _start_position;

    if (size < _max_document_size)
    {
      _stream.Seek(_start_position, SeekOrigin.Begin);
      WriteSize((int)size);
      _stream.Seek(0, SeekOrigin.End);
      return;
    }

    throw new ArgumentException("the bson document is too big");
  }

  private void WriteTypeId(byte type_id)
  {
    _stream.WriteByte(type_id);
  }

  private ReadOnlySpan<byte> WriteIndexAsString(int index)
  {
    int bytes_written;
    index.TryFormat(_buffer.AsSpan(), out bytes_written);
    if (bytes_written < _buffer.Length)
    {
      return _buffer.AsSpan(0, bytes_written);
    }

    return ReadOnlySpan<byte>.Empty;
  }
  private void WriteString(ReadOnlySpan<byte> utf8_string)
  {
    _stream.Write(utf8_string);
    _stream.WriteByte(0);
  }

  private void WriteSize(int size)
  {
    var size_bytes = BitConverter.GetBytes(size);
    _stream.Write(size_bytes);
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