using karu.bsonly.Serialization.Interface;


namespace karu.bsonly.Serialization;

// TODO: 
//   - Wrapper class
//   - serialize with delegate function
// 
static public partial class Serializer
{
  // ISerializable T
  public static void Serialize<T>(IBaseSerializer serializer, ReadOnlySpan<byte> key, T value) where T : ISerializable
  {
    serializer.WriteDocument<T>(key, value);
  }
  public static void Serialize<T>(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, T value) where T : ISerializable
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_DOCUMENT))
    {
      var doc_reader = new MemoryDocReader(deserializer.GetBsonDoc());
      doc_reader.ReadDocument<T>(value, new DeserializationContext());
    }
    else
      throw new KeyNotAvailableException($"no entry {System.Text.Encoding.UTF8.GetString(key)} of type \"document\" found");
  }


  // public static List<bool> SerializeListOfBool(IBasicDeserializer deserializer, ReadOnlySpan<byte> key)
  // {
  //   if (deserializer.HasEntry(key, BsonSerialization.BSON_TYPE_ARRAY))
  //   {
  //     var value = new List<bool>();
  //     var array = deserializer.ReadArray();
  //     while (array.HasEntry("", BsonSerialization.BSON_TYPE_BOOL))
  //     {
  //       value.Add(array.ReadBool());
  //     }
  //     return value;
  //   }
  //   else
  //     throw new KeyNotAvailableException($"no entry {key} of type \"long\" found");
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