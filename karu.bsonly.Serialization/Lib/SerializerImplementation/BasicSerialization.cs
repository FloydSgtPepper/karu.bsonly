using System.Text;
using karu.bsonly.Serialization.Interface;


namespace karu.bsonly.Serialization;

static public partial class Serializer
{
  // bool
  public static void Serialize(IDocumentSerializer serializer, byte[] key, bool value)
    => Serialize(serializer, key.AsSpan(), value);
  public static void Serialize(IDocumentSerializer serializer, ReadOnlySpan<byte> key, bool value)
  {
    serializer.WriteBool(key).WriteBool(value);
  }
  public static void Serialize(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, ref bool value)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_BOOL))
      value = deserializer.ReadBool();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"bool\" found");
  }
  public static bool SerializeBool(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_BOOL))
      return deserializer.ReadBool();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"bool\" found");
  }

  // int
  public static void Serialize(IDocumentSerializer serializer, byte[] key, int value)
    => Serialize(serializer, key.AsSpan(), value);
  public static void Serialize(IDocumentSerializer serializer, ReadOnlySpan<byte> key, int value)
  {
    serializer.WriteInt(key).WriteInt(value);
  }
  public static int SerializeInt(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_INT32))
      return deserializer.ReadInt();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"int\" found");
  }
  public static void Serialize(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, ref int value)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_INT32))
      value = deserializer.ReadInt();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"int\" found");
  }

  // long
  public static void Serialize(IDocumentSerializer serializer, byte[] key, long value)
  => Serialize(serializer, key.AsSpan(), value);
  public static void Serialize(IDocumentSerializer serializer, ReadOnlySpan<byte> key, long value)
  {
    serializer.WriteLong(key).WriteLong(value);
  }
  public static void Serialize(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, ref long value)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_INT64))
      value = deserializer.ReadLong();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"long\" found");
  }
  public static long SerializeLong(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_INT64))
      return deserializer.ReadLong();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"long\" found");
  }

  // string
  // public static void Serialize(IDocumentSerializer serializer, byte[] key, byte[] utf8_string)
  //   => Serialize(serializer, key.AsSpan(), utf8_string.AsSpan());
  public static void Serialize(IDocumentSerializer serializer, ReadOnlySpan<byte> key, ReadOnlySpan<byte> utf8_string)
  {
    serializer.WriteString(key).WriteString(utf8_string);
  }
  public static void Serialize(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, ref byte[] utf8_string)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_UTF8))
      utf8_string = deserializer.ReadString().ToArray();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"string\" found");
  }
  public static ReadOnlySpan<byte> SerializeUtf8String(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_UTF8))
      return deserializer.ReadString();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"string\" found");
  }

  public static void Serialize(IDocumentSerializer serializer, byte[] key, string value)
    => Serialize(serializer, key.AsSpan(), value);
  public static void Serialize(IDocumentSerializer serializer, ReadOnlySpan<byte> key, string value)
  {
    serializer.WriteString(key).WriteString(Encoding.UTF8.GetBytes(value));
  }
  public static void Serialize(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, ref string value)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_UTF8))
      value = Encoding.UTF8.GetString(deserializer.ReadString());
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"string\" found");
  }
  public static string SerializeString(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_UTF8))
      return Encoding.UTF8.GetString(deserializer.ReadString());
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"string\" found");
  }

  // double
  public static void Serialize(IDocumentSerializer serializer, byte[] key, double value)
    => Serialize(serializer, key.AsSpan(), value);
  public static void Serialize(IDocumentSerializer serializer, ReadOnlySpan<byte> key, double value)
  {
    serializer.WriteDouble(key).WriteDouble(value);
  }
  public static void Serialize(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, ref double value)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_DOUBLE))
      value = deserializer.ReadDouble();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"double\" found");
  }
  public static double SerializeDouble(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_DOUBLE))
      return deserializer.ReadDouble();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"double\" found");
  }

  // Binary
  public static void Serialize(IDocumentSerializer serializer, byte[] key, byte[] value, byte binary_subtype = BsonConstants.BSON_BINARY_SUBTYPE_BINARY)
    => Serialize(serializer, key.AsSpan(), value, binary_subtype);
  public static void Serialize(IDocumentSerializer serializer, ReadOnlySpan<byte> key, byte[] value, byte binary_subtype = BsonConstants.BSON_BINARY_SUBTYPE_BINARY)
  {
    serializer.WriteBinary(key).WriteBinary(value, binary_subtype);
  }
  public static void Serialize(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, byte[] value, byte binary_subtype = BsonConstants.BSON_BINARY_SUBTYPE_BINARY)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_BINARY) && deserializer.BinarySubType() == binary_subtype)
      value = deserializer.ReadBinary().ToArray();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"Guid\" found");
  }
  public static ReadOnlySpan<byte> SerializeBinary(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, byte binary_subtype = BsonConstants.BSON_BINARY_SUBTYPE_BINARY)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_BINARY) && deserializer.BinarySubType() == binary_subtype)
      return deserializer.ReadBinary();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"Guid\" found");
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