using System.Text;
using karu.bsonly.Serialization.Interface;


namespace karu.bsonly.Serialization;

static public partial class Serializer
{
  // c++ std::vector<char> or std::vector<byte> -> therefor user type SEQ_INT8 -> 0x84

  // Serialize for utf8_string has same parameter :-(
  public static void SerializeByteArray(IDocumentSerializer serializer, ReadOnlySpan<byte> key, ReadOnlySpan<byte> data)
  {
    serializer.WriteBinary(key).WriteBinary(data, BsonConstants.BSON_USER_TYPE_SEQ_INT_8);
  }

  public static void SerializeByteArray(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, out byte[] value)
  {
    var list_type = deserializer.HasEntry(key);

    if (list_type == BsonConstants.BSON_TYPE_ARRAY)
    {
      var array_reader = deserializer.ArrayReader();
      var type = array_reader.NextEntryType();
      var list_value = new List<byte>();
      while (type == BsonConstants.BSON_TYPE_INT32)
      {
        var long_value = array_reader.ReadInt();
        list_value.Add((byte)long_value);
        type = array_reader.NextEntryType();
      }
      array_reader.Finish();
      var byte_array = new byte[list_value.Count];
      for (int idx = 0; idx < byte_array.Length; ++idx)
        byte_array[idx] = list_value[idx];

      value = byte_array;
      return;
    }

    if (list_type == BsonConstants.BSON_TYPE_BINARY
       && ((deserializer.BinarySubType() == BsonConstants.BSON_USER_TYPE_SEQ_INT_8)
          || (deserializer.BinarySubType() == BsonConstants.BSON_USER_TYPE_SEQ_UINT_8)))
    {
      var bin_data = deserializer.ReadBinary();
      value = bin_data.ToArray();
      return;
    }

    throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"byte[]\" found");
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