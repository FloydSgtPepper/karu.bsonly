using System.Text;
using karu.bsonly.Serialization.Interface;


namespace karu.bsonly.Serialization;

static public partial class Serializer
{
  public static void Serialize(IBaseSerializer serializer, ReadOnlySpan<byte> key, List<ulong> value)
  {
    var array_writer = serializer.SerializeArray(key);
    for (var idx = 0; idx < value.Count; ++idx)
      array_writer.Add((long)value[idx]);
    array_writer.Finish();
  }
  public static void Serialize(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, ref List<ulong> value)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_ARRAY))
    {
      value.Clear();
      var array_reader = deserializer.ArrayReader();
      var type = array_reader.NextEntryType();
      while (type == BsonConstants.BSON_TYPE_INT64)
      {
        var long_value = array_reader.ReadLong();
        value.Add((ulong)long_value);
        type = array_reader.NextEntryType();
      }
      array_reader.Finish();
    }
    else
      throw new KeyNotAvailableException($"no entry {System.Text.Encoding.UTF8.GetString(key)} of type \"List<ulong>\" found");
  }
  public static List<ulong>? SerializeListOfULong(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, DeserializationContext context)
  {
    var list_type = deserializer.HasEntry(key);

    if (list_type == BsonConstants.BSON_TYPE_ARRAY)
    {
      var value = new List<ulong>();
      var array_reader = deserializer.ArrayReader();
      var type = array_reader.NextEntryType();
      while (type == BsonConstants.BSON_TYPE_INT64)
      {
        var long_value = array_reader.ReadLong();
        value.Add((ulong)long_value);
        type = array_reader.NextEntryType();
      }
      array_reader.Finish();
      return value;
    }
    // if (list_type == BsonConstants.BSON_TYPE_BINARY && context.Configuration.Sequences == Sequences.BINARY)
    // {
    //   deserializer.ReadBinary()
    // }


    return null;
  }


  public static void Serialize(IBaseSerializer serializer, ReadOnlySpan<byte> key, List<long> value)
  {
    var array_writer = serializer.SerializeArray(key);
    for (var idx = 0; idx < value.Count; ++idx)
      array_writer.Add(value[idx]);
    array_writer.Finish();
  }
  public static void Serialize(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, ref List<long> value)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_ARRAY))
    {
      value.Clear();
      var array_reader = deserializer.ArrayReader();
      var type = array_reader.NextEntryType();
      while (type == BsonConstants.BSON_TYPE_INT64)
      {
        var long_value = array_reader.ReadLong();
        value.Add(long_value);
        type = array_reader.NextEntryType();
      }
      array_reader.Finish();
    }
    else
      throw new KeyNotAvailableException($"no entry {System.Text.Encoding.UTF8.GetString(key)} of type \"List<long>\" found");
  }
  public static List<long> SerializeListOfLong(IBaseDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_ARRAY))
    {
      var value = new List<long>();
      var array_reader = deserializer.ArrayReader();
      var type = array_reader.NextEntryType();
      while (type == BsonConstants.BSON_TYPE_INT64)
      {
        var long_value = array_reader.ReadLong();
        value.Add(long_value);
        type = array_reader.NextEntryType();
      }
      array_reader.Finish();
      return value;
    }
    else
      throw new KeyNotAvailableException($"no entry {System.Text.Encoding.UTF8.GetString(key)} of type \"List<long>\" found");
  }


  public static void Serialize(IBaseSerializer serializer, ReadOnlySpan<byte> key, List<uint> value)
  {
    var array_writer = serializer.SerializeArray(key);
    for (var idx = 0; idx < value.Count; ++idx)
      array_writer.Add((int)value[idx]);
    array_writer.Finish();
  }
  public static void Serialize(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, ref List<uint> value)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_ARRAY))
    {
      value.Clear();
      var array_reader = deserializer.ArrayReader();
      var type = array_reader.NextEntryType();
      while (type == BsonConstants.BSON_TYPE_INT32)
      {
        var int_value = array_reader.ReadInt();
        value.Add((uint)int_value);
        type = array_reader.NextEntryType();
      }
      array_reader.Finish();
    }
    else
      throw new KeyNotAvailableException($"no entry {System.Text.Encoding.UTF8.GetString(key)} of type \"List<uint>\" found");
  }
  public static List<uint> SerializeListOfUInt(IBaseDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_ARRAY))
    {
      var value = new List<uint>();
      var array_reader = deserializer.ArrayReader();
      var type = array_reader.NextEntryType();
      while (type == BsonConstants.BSON_TYPE_INT32)
      {
        var int_value = array_reader.ReadInt();
        value.Add((uint)int_value);
        type = array_reader.NextEntryType();
      }
      array_reader.Finish();
      return value;
    }
    else
      throw new KeyNotAvailableException($"no entry {System.Text.Encoding.UTF8.GetString(key)} of type \"List<uint>\" found");
  }


  public static void Serialize(IBaseSerializer serializer, ReadOnlySpan<byte> key, List<int> value)
  {
    var array_writer = serializer.SerializeArray(key);
    for (var idx = 0; idx < value.Count; ++idx)
      array_writer.Add(value[idx]);
    array_writer.Finish();
  }
  public static void Serialize(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, ref List<int> value)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_ARRAY))
    {
      value.Clear();
      var array_reader = deserializer.ArrayReader();
      var type = array_reader.NextEntryType();
      while (type == BsonConstants.BSON_TYPE_INT32)
      {
        var int_value = array_reader.ReadInt();
        value.Add(int_value);
        type = array_reader.NextEntryType();
      }
      array_reader.Finish();
    }
    else
      throw new KeyNotAvailableException($"no entry {System.Text.Encoding.UTF8.GetString(key)} of type \"List<int>\" found");
  }
  public static List<int> SerializeListOfInt(IBaseDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_ARRAY))
    {
      var value = new List<int>();
      var array_reader = deserializer.ArrayReader();
      var type = array_reader.NextEntryType();
      while (type == BsonConstants.BSON_TYPE_INT32)
      {
        var int_value = array_reader.ReadInt();
        value.Add(int_value);
        type = array_reader.NextEntryType();
      }
      array_reader.Finish();
      return value;
    }
    else
      throw new KeyNotAvailableException($"no entry {System.Text.Encoding.UTF8.GetString(key)} of type \"List<int>\" found");
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