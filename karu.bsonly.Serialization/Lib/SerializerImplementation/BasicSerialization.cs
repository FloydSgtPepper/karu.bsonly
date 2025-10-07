using System.Text;
using karu.bsonly.Serialization.Interface;


namespace karu.bsonly.Serialization;

static public partial class Serializer
{
  public static void Serialize(IBaseSerializer serializer, ReadOnlySpan<byte> key, bool value)
  {
    serializer.WriteBool(key, value);
  }
  public static void Serialize(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, ref bool value)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_BOOL))
      value = deserializer.ReadBool();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"bool\" found");
  }
  public static bool SerializeBool(IBaseDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_BOOL))
      return deserializer.ReadBool();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"bool\" found");
  }

  // int
  public static void Serialize(IBaseSerializer serializer, ReadOnlySpan<byte> key, int value)
  {
    serializer.WriteInt(key, value);
  }
  public static int SerializeInt(IBaseDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_INT32))
      return deserializer.ReadInt();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"int\" found");
  }
  public static void Serialize(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, ref int value)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_INT32))
      value = deserializer.ReadInt();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"int\" found");
  }

  // long
  public static void Serialize(IBaseSerializer serializer, ReadOnlySpan<byte> key, long value)
  {
    serializer.WriteLong(key, value);
  }
  public static void Serialize(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, ref long value)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_INT64))
      value = deserializer.ReadLong();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"long\" found");
  }
  public static long SerializeLong(IBaseDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_INT64))
      return deserializer.ReadLong();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"long\" found");
  }

  // string
  public static void Serialize(IBaseSerializer serializer, ReadOnlySpan<byte> key, ReadOnlySpan<byte> utf8_string)
  {
    serializer.WriteString(key, utf8_string);
  }
  public static void Serialize(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, ref byte[] utf8_string)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_UTF8))
      utf8_string = deserializer.ReadString().ToArray();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"string\" found");
  }
  public static ReadOnlySpan<byte> SerializeUtf8String(IBaseDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_UTF8))
      return deserializer.ReadString();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"string\" found");
  }
  public static void Serialize(IBaseSerializer serializer, ReadOnlySpan<byte> key, string value)
  {
    serializer.WriteString(key, Encoding.UTF8.GetBytes(value));
  }
  public static void Serialize(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, ref string value)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_UTF8))
      value = Encoding.UTF8.GetString(deserializer.ReadString());
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"string\" found");
  }
  public static string SerializeString(IBaseDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_UTF8))
      return Encoding.UTF8.GetString(deserializer.ReadString());
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"string\" found");
  }


  // double
  public static void Serialize(IBaseSerializer serializer, ReadOnlySpan<byte> key, double value)
  {
    serializer.WriteDouble(key, value);
  }
  public static void Serialize(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, ref double value)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_DOUBLE))
      value = deserializer.ReadDouble();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"double\" found");
  }
  public static double SerializeDouble(IBaseDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_DOUBLE))
      return deserializer.ReadDouble();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"double\" found");
  }

  // double
  public static void Serialize(IBaseSerializer serializer, ReadOnlySpan<byte> key, Guid value)
  {
    serializer.WriteGuid(key, value);
  }
  public static void Serialize(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, ref Guid value)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_BINARY))
      value = deserializer.ReadGuid();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"Guid\" found");
  }
  public static Guid SerializeGuid(IBaseDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_BINARY))
      return deserializer.ReadGuid();
    else
      throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"Guid\" found");
  }

  // generics
  // public static void Serialize(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, ref object value, byte type_id, SerializeObj serialize_type)
  // {
  //   if (deserializer.HasEntry(key, type_id))
  //   {
  //     value = serialize_type(deserializer);
  //   }
  //   else
  //     throw new KeyNotAvailableException($"no entry {key} of type \"string\" found");
  // }

  // public static void Serialize<T>(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, ref List<T> value) where T : ISerializeable, new()
  // {
  //   if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_ARRAY))
  //   {
  //     // ISSUE: check _settings whether arrays are encoded as binary
  //     var array_deserializer = deserializer.ReadArray();
  //     while (array_deserializer.HasNextEntry())
  //     {
  //       var item = new T();
  //       Serialize<T>(array_deserializer.NextEntry(), "", ref item);
  //       value.Add(item);
  //     }
  //   }
  //   else
  //     throw new KeyNotAvailableException($"no entry {key} of type \"array\" found");
  // }

  // public static void Serialize<T>(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, ref T value) where T : ISerializeable, new()
  // {
  //   System.Diagnostics.Debug.WriteLine("generic deserialization function");
  //   if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_DOCUMENT))
  //   {
  //     var document = deserializer.ReadDocument();
  //     value.Serialize(document, "");
  //   }
  //   else
  //     throw new KeyNotAvailableException($"no entry {key} of type \"document\" found");
  // }

  // public static void Serialize<T>(IBaseSerializer serializer, ReadOnlySpan<byte> key, T value) where T : ISerializeable, new()
  // {
  //   var doc_writer = serializer.DocumentWriter(key);
  //   value.Serialize(serializer, key);
  //   doc_writer.Finish();
  // }

  // public static byte[] CreatBsonDoc<T>(IBaseSerializer serializer, ReadOnlySpan<byte> key, T value) where T : ISerializeable, new()
  // {
  //   value.Serialize(serializer, key);
  //   return serializer.Finish();
  // }


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