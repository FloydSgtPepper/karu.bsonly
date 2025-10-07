using karu.bsonly.Serialization.Interface;

namespace karu.bsonly.Serialization;

static class BasicWriter
{
  public static void WriteLong(Stream stream, ReadOnlySpan<byte> key_string, long value)
  {
    WriteTypeId(stream, BsonConstants.BSON_TYPE_INT64);
    WriteString(stream, key_string);
    var buffer = BitConverter.GetBytes(value);
    stream.Write(buffer);
  }

  public static void WriteInt(Stream stream, ReadOnlySpan<byte> key_string, int value)
  {
    WriteTypeId(stream, BsonConstants.BSON_TYPE_INT32);
    WriteString(stream, key_string);
    var buffer = BitConverter.GetBytes(value);
    stream.Write(buffer);
  }

  public static void WriteDouble(Stream stream, ReadOnlySpan<byte> key_string, double value)
  {
    WriteTypeId(stream, BsonConstants.BSON_TYPE_DOUBLE);
    WriteString(stream, key_string);
    var buffer = BitConverter.GetBytes(value);
    stream.Write(buffer);
  }

  public static void WriteBool(Stream stream, ReadOnlySpan<byte> key_string, bool value)
  {
    WriteTypeId(stream, BsonConstants.BSON_TYPE_BOOL);
    WriteString(stream, key_string);
    if (value)
      stream.WriteByte(1);
    else
      stream.WriteByte(0);
  }

  public static void WriteNull(Stream stream, ReadOnlySpan<byte> key_string)
  {
    WriteTypeId(stream, BsonConstants.BSON_TYPE_NULL);
    WriteString(stream, key_string);
    stream.WriteByte(0);
  }

  public static void WriteString(Stream stream, ReadOnlySpan<byte> key_string, ReadOnlySpan<byte> value)
  {
    WriteTypeId(stream, BsonConstants.BSON_TYPE_UTF8);
    WriteString(stream, key_string);

    WriteSize(stream, value.Length + 1);
    stream.Write(value);
    stream.WriteByte(0);
  }

  public static void WriteGuid(Stream stream, ReadOnlySpan<byte> key_string, Guid value)
  {
    // if (_settings.Sequences == Sequences.BINARY)
    // {
    var value_as_array = value.ToByteArray();
    // Debug.Assert(BsonConstants.SIZE_OF_GUID == value_as_array.Length);

    // Standard Bson Guid representation
    Array.Reverse(value_as_array, 0, 4);
    Array.Reverse(value_as_array, 4, 2);
    Array.Reverse(value_as_array, 6, 2);

    WriteBinary(stream, key_string, value_as_array.AsSpan(), BsonConstants.BSON_BINARY_SUBTYPE_GUID);

    // json
    // }
    // else
    // {
    //   var value_as_string = value.ToString();
    //   WriteString(key, value_as_string);
    // }
  }

  public static void WriteBinary(Stream stream, ReadOnlySpan<byte> key_string, ReadOnlySpan<byte> binary_data, byte binary_subtype)
  {
    WriteTypeId(stream, BsonConstants.BSON_TYPE_BINARY);
    WriteString(stream, key_string);
    var start_position = stream.Position;
    WriteSize(stream, 0); // dummy size
    WriteTypeId(stream, binary_subtype);
    stream.Write(binary_data);
    WriteSizeAt(stream, start_position);
  }

  public static void WriteRawDocument(Stream stream, ReadOnlySpan<byte> key_string, ReadOnlySpan<byte> document)
  {
    WriteTypeId(stream, BsonConstants.BSON_TYPE_DOCUMENT);
    WriteString(stream, key_string);
    WriteSize(stream, document.Length);
    stream.Write(document);
    WriteEod(stream);
  }

  private static void WriteTypeId(Stream stream, byte type_id)
  {
    stream.WriteByte(type_id);
  }
  private static void WriteEod(Stream stream)
  {
    stream.WriteByte(BsonConstants.BSON_TYPE_EOD);
  }

  private static void WriteString(Stream stream, ReadOnlySpan<byte> str)
  {
    stream.Write(str);
    stream.WriteByte(0);
  }

  private static void WriteSize(Stream stream, int size)
  {
    var size_bytes = BitConverter.GetBytes(size);
    stream.Write(size_bytes);
  }

  private static void WriteSizeAt(Stream stream, long start_position)
  {
    var size = stream.Position - start_position - 1 - sizeof(int);
    stream.Seek(start_position, SeekOrigin.Begin);
    WriteSize(stream, (int)size);
    stream.Seek(0, SeekOrigin.End);
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