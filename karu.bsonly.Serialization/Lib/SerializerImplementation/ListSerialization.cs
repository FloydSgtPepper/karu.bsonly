using System.Text;
using karu.bsonly.Serialization.Interface;


namespace karu.bsonly.Serialization;

static public partial class Serializer
{

  // ULONG
  public static void Serialize(IDocumentSerializer serializer, byte[] key, List<ulong> value)
    => Serialize(serializer, key.AsSpan(), value);
  public static void Serialize(IDocumentSerializer serializer, ReadOnlySpan<byte> key, List<ulong> value)
  {
    var binary_sequence = serializer.Context().Configuration.Sequences == Sequences.BINARY;

    if (binary_sequence)
    {
      const int element_size = sizeof(ulong);
      var data = new byte[value.Count * element_size];
      for (var idx = 0; idx < value.Count; ++idx)
        BitConverter.TryWriteBytes(data.AsSpan(idx * element_size, element_size), value[idx]);

      serializer.WriteBinary(key).WriteBinary(data.AsSpan(), BsonConstants.BSON_USER_TYPE_SEQ_UINT_64);
    }
    else
    {
      var array_writer = serializer.WriteArray(key);
      for (var idx = 0; idx < value.Count; ++idx)
        serializer.WriteLong(array_writer.NextKey()).WriteLong((long)value[idx]);
      array_writer.Finish();
    }
  }

  public static List<ulong> SerializeListOfULong(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    var list_type = deserializer.HasEntry(key);

    if (list_type == BsonConstants.BSON_TYPE_ARRAY)
    {
      List<ulong> value = new();
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

    if (list_type == BsonConstants.BSON_TYPE_BINARY && deserializer.BinarySubType() == BsonConstants.BSON_USER_TYPE_SEQ_UINT_64)
    {
      var value = new List<ulong>();
      var element_size = sizeof(long);
      var bin_data = deserializer.ReadBinary();
      var nb_of_elements = bin_data.Length / element_size;
      value.Clear();
      if (value.Capacity < nb_of_elements)
        value.Capacity = nb_of_elements;

      for (int idx = 0; idx < nb_of_elements; ++idx)
        value.Add(BitConverter.ToUInt64(bin_data.Slice(idx * 8, element_size)));

      return value;
    }

    throw new KeyNotAvailableException($"no entry {System.Text.Encoding.UTF8.GetString(key)} of type \"List<ulong>\" found");
  }

  public static void Serialize(IDocumentDeserializer deserializer, byte[] key, ref List<ulong> value)
     => value = SerializeListOfULong(deserializer, key.AsSpan());
  public static void Serialize(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, ref List<ulong> value)
     => value = SerializeListOfULong(deserializer, key);

  // LONG
  public static void Serialize(IDocumentSerializer serializer, byte[] key, List<long> value)
    => Serialize(serializer, key.AsSpan(), value);
  public static void Serialize(IDocumentSerializer serializer, ReadOnlySpan<byte> key, List<long> value)
  {
    var binary_sequence = serializer.Context().Configuration.Sequences == Sequences.BINARY;

    if (binary_sequence)
    {
      const int element_size = sizeof(long);
      var data = new byte[value.Count * element_size];
      for (var idx = 0; idx < value.Count; ++idx)
        BitConverter.TryWriteBytes(data.AsSpan(idx * element_size, element_size), value[idx]);

      serializer.WriteBinary(key).WriteBinary(data.AsSpan(), BsonConstants.BSON_USER_TYPE_SEQ_INT_64);
    }
    else
    {
      var array_writer = serializer.WriteArray(key);
      for (var idx = 0; idx < value.Count; ++idx)
        serializer.WriteLong(array_writer.NextKey()).WriteLong(value[idx]);
      array_writer.Finish();
    }
  }
  public static List<long> SerializeListOfLong(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    var list_type = deserializer.HasEntry(key);

    if (list_type == BsonConstants.BSON_TYPE_ARRAY)
    {
      List<long> value = new();
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

    if (list_type == BsonConstants.BSON_TYPE_BINARY && deserializer.Context().Configuration.Sequences == Sequences.BINARY)
    {
      if (deserializer.BinarySubType() == BsonConstants.BSON_USER_TYPE_SEQ_INT_64)
      {
        var value = new List<long>();
        var element_size = sizeof(long);
        var bin_data = deserializer.ReadBinary();
        var nb_of_elements = bin_data.Length / element_size;
        value.Clear();
        if (value.Capacity < nb_of_elements)
          value.Capacity = nb_of_elements;

        for (int idx = 0; idx < nb_of_elements; ++idx)
          value.Add(BitConverter.ToInt64(bin_data.Slice(idx * 8, element_size)));

        return value;
      }
    }

    throw new KeyNotAvailableException($"no entry {System.Text.Encoding.UTF8.GetString(key)} of type \"List<long>\" found");
  }

  public static void Serialize(IDocumentDeserializer deserializer, byte[] key, ref List<long> value)
     => value = SerializeListOfLong(deserializer, key.AsSpan());
  public static void Serialize(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, ref List<long> value)
     => value = SerializeListOfLong(deserializer, key);

  // UINT
  public static void Serialize(IDocumentSerializer serializer, byte[] key, List<uint> value)
    => Serialize(serializer, key.AsSpan(), value);
  public static void Serialize(IDocumentSerializer serializer, ReadOnlySpan<byte> key, List<uint> value)
  {
    var binary_sequence = serializer.Context().Configuration.Sequences == Sequences.BINARY;

    if (binary_sequence)
    {
      const int element_size = sizeof(uint);
      var data = new byte[value.Count * element_size];
      for (var idx = 0; idx < value.Count; ++idx)
        BitConverter.TryWriteBytes(data.AsSpan(idx * element_size, element_size), value[idx]);

      serializer.WriteBinary(key).WriteBinary(data.AsSpan(), BsonConstants.BSON_USER_TYPE_SEQ_UINT_32);
    }
    else
    {
      var array_writer = serializer.WriteArray(key);
      for (var idx = 0; idx < value.Count; ++idx)
        serializer.WriteInt(array_writer.NextKey()).WriteInt((int)value[idx]);
      array_writer.Finish();
    }
  }
  public static List<uint> SerializeListOfUInt(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    var list_type = deserializer.HasEntry(key);

    if (list_type == BsonConstants.BSON_TYPE_ARRAY)
    {
      List<uint> value = new();
      var array_reader = deserializer.ArrayReader();
      var type = array_reader.NextEntryType();
      while (type == BsonConstants.BSON_TYPE_INT32)
      {
        var long_value = array_reader.ReadInt();
        value.Add((uint)long_value);
        type = array_reader.NextEntryType();
      }
      array_reader.Finish();
      return value;
    }

    if (list_type == BsonConstants.BSON_TYPE_BINARY && deserializer.Context().Configuration.Sequences == Sequences.BINARY)
    {
      if (deserializer.BinarySubType() == BsonConstants.BSON_USER_TYPE_SEQ_UINT_32)
      {
        var value = new List<uint>();
        var element_size = sizeof(uint);
        var bin_data = deserializer.ReadBinary();
        var nb_of_elements = bin_data.Length / element_size;
        value.Clear();
        if (value.Capacity < nb_of_elements)
          value.Capacity = nb_of_elements;

        for (int idx = 0; idx < nb_of_elements; ++idx)
          value.Add(BitConverter.ToUInt32(bin_data.Slice(idx * 8, element_size)));

        return value;
      }
    }

    throw new KeyNotAvailableException($"no entry {System.Text.Encoding.UTF8.GetString(key)} of type \"List<uint>\" found");
  }

  public static void Serialize(IDocumentDeserializer deserializer, byte[] key, ref List<uint> value)
     => value = SerializeListOfUInt(deserializer, key.AsSpan());
  public static void Serialize(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, ref List<uint> value)
     => value = SerializeListOfUInt(deserializer, key);


  // INT
  public static void Serialize(IDocumentSerializer serializer, byte[] key, List<int> value)
    => Serialize(serializer, key.AsSpan(), value);
  public static void Serialize(IDocumentSerializer serializer, ReadOnlySpan<byte> key, List<int> value)
  {
    var binary_sequence = serializer.Context().Configuration.Sequences == Sequences.BINARY;

    if (binary_sequence)
    {
      const int element_size = sizeof(int);
      var data = new byte[value.Count * element_size];
      for (var idx = 0; idx < value.Count; ++idx)
        BitConverter.TryWriteBytes(data.AsSpan(idx * element_size, element_size), value[idx]);

      serializer.WriteBinary(key).WriteBinary(data.AsSpan(), BsonConstants.BSON_USER_TYPE_SEQ_INT_32);
    }
    else
    {
      var array_writer = serializer.WriteArray(key);
      for (var idx = 0; idx < value.Count; ++idx)
        serializer.WriteInt(array_writer.NextKey()).WriteInt(value[idx]);
      array_writer.Finish();
    }
  }
  public static List<int> SerializeListOfInt(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    var list_type = deserializer.HasEntry(key);

    if (list_type == BsonConstants.BSON_TYPE_ARRAY)
    {
      List<int> value = new();
      var array_reader = deserializer.ArrayReader();
      var type = array_reader.NextEntryType();
      while (type == BsonConstants.BSON_TYPE_INT32)
      {
        var long_value = array_reader.ReadInt();
        value.Add(long_value);
        type = array_reader.NextEntryType();
      }
      array_reader.Finish();
      return value;
    }

    if (list_type == BsonConstants.BSON_TYPE_BINARY && deserializer.Context().Configuration.Sequences == Sequences.BINARY)
    {
      if (deserializer.BinarySubType() == BsonConstants.BSON_USER_TYPE_SEQ_INT_32)
      {
        var value = new List<int>();
        var element_size = sizeof(int);
        var bin_data = deserializer.ReadBinary();
        var nb_of_elements = bin_data.Length / element_size;
        value.Clear();
        if (value.Capacity < nb_of_elements)
          value.Capacity = nb_of_elements;

        for (int idx = 0; idx < nb_of_elements; ++idx)
          value.Add(BitConverter.ToInt32(bin_data.Slice(idx * 8, element_size)));

        return value;
      }
    }

    throw new KeyNotAvailableException($"no entry {System.Text.Encoding.UTF8.GetString(key)} of type \"List<int>\" found");
  }

  public static void Serialize(IDocumentDeserializer deserializer, byte[] key, ref List<int> value)
     => value = SerializeListOfInt(deserializer, key.AsSpan());
  public static void Serialize(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, ref List<int> value)
     => value = SerializeListOfInt(deserializer, key);

  // char
  public static void Serialize(IDocumentSerializer serializer, byte[] key, List<char> value)
    => Serialize(serializer, key.AsSpan(), value);
  public static void Serialize(IDocumentSerializer serializer, ReadOnlySpan<byte> key, List<char> value)
  {
    var binary_sequence = serializer.Context().Configuration.Sequences == Sequences.BINARY;

    if (binary_sequence)
    {
      var data = new byte[value.Count];
      for (var idx = 0; idx < value.Count; ++idx)
        data[idx] = (byte)value[idx];

      serializer.WriteBinary(key).WriteBinary(data.AsSpan(), BsonConstants.BSON_USER_TYPE_SEQ_INT_8);
    }
    else
    {
      var array_writer = serializer.WriteArray(key);
      for (var idx = 0; idx < value.Count; ++idx)
        serializer.WriteInt(array_writer.NextKey()).WriteInt((int)value[idx]);
      array_writer.Finish();
    }
  }

  public static List<char> SerializeListOfChar(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    var list_type = deserializer.HasEntry(key);

    if (list_type == BsonConstants.BSON_TYPE_ARRAY)
    {
      List<char> value = new();
      var array_reader = deserializer.ArrayReader();
      var type = array_reader.NextEntryType();
      while (type == BsonConstants.BSON_TYPE_INT32)
      {
        var int_value = array_reader.ReadInt();
        value.Add((char)int_value);
        type = array_reader.NextEntryType();
      }
      array_reader.Finish();
      return value;
    }

    if (list_type == BsonConstants.BSON_TYPE_BINARY && deserializer.Context().Configuration.Sequences == Sequences.BINARY)
    {
      if (deserializer.BinarySubType() == BsonConstants.BSON_USER_TYPE_SEQ_INT_8)
      {
        var value = new List<char>();
        var bin_data = deserializer.ReadBinary();
        if (value.Capacity < bin_data.Length)
          value.Capacity = bin_data.Length;

        for (int idx = 0; idx < bin_data.Length; ++idx)
          value.Add((char)bin_data[idx]);

        return value;
      }
    }

    throw new KeyNotAvailableException($"no entry {System.Text.Encoding.UTF8.GetString(key)} of type \"List<char>\" found");
  }

  public static void Serialize(IDocumentDeserializer deserializer, byte[] key, ref List<char> value)
     => value = SerializeListOfChar(deserializer, key.AsSpan());
  public static void Serialize(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, ref List<char> value)
     => value = SerializeListOfChar(deserializer, key);



  // byte
  public static void Serialize(IDocumentSerializer serializer, byte[] key, List<byte> value)
    => Serialize(serializer, key.AsSpan(), value);
  public static void Serialize(IDocumentSerializer serializer, ReadOnlySpan<byte> key, List<byte> value)
  {
    var binary_sequence = serializer.Context().Configuration.Sequences == Sequences.BINARY;

    if (binary_sequence)
    {
      var data = new byte[value.Count];
      for (var idx = 0; idx < value.Count; ++idx)
        data[idx] = value[idx];

      serializer.WriteBinary(key).WriteBinary(data.AsSpan(), BsonConstants.BSON_USER_TYPE_SEQ_UINT_8);
    }
    else
    {
      var array_writer = serializer.WriteArray(key);
      for (var idx = 0; idx < value.Count; ++idx)
        serializer.WriteInt(array_writer.NextKey()).WriteInt((int)value[idx]);
      array_writer.Finish();
    }
  }
  public static List<byte> SerializeListOfByte(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key)
  {
    var list_type = deserializer.HasEntry(key);

    if (list_type == BsonConstants.BSON_TYPE_ARRAY)
    {
      List<byte> value = new();
      var array_reader = deserializer.ArrayReader();
      var type = array_reader.NextEntryType();
      while (type == BsonConstants.BSON_TYPE_INT32)
      {
        var int_value = array_reader.ReadInt();
        value.Add((byte)int_value);
        type = array_reader.NextEntryType();
      }
      array_reader.Finish();
      return value;
    }

    if (list_type == BsonConstants.BSON_TYPE_BINARY && deserializer.Context().Configuration.Sequences == Sequences.BINARY)
    {
      if (deserializer.BinarySubType() == BsonConstants.BSON_USER_TYPE_SEQ_UINT_8)
      {
        var value = new List<byte>();
        var bin_data = deserializer.ReadBinary();
        if (value.Capacity < bin_data.Length)
          value.Capacity = bin_data.Length;

        for (int idx = 0; idx < bin_data.Length; ++idx)
          value.Add(value[idx]);

        return value;
      }
    }

    throw new KeyNotAvailableException($"no entry {System.Text.Encoding.UTF8.GetString(key)} of type \"List<byte>\" found");
  }

  public static void Serialize(IDocumentDeserializer deserializer, byte[] key, ref List<byte> value)
     => value = SerializeListOfByte(deserializer, key.AsSpan());
  public static void Serialize(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, ref List<byte> value)
     => value = SerializeListOfByte(deserializer, key);
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