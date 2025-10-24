using System.Diagnostics;
using System.Text;
using karu.bsonly.Serialization.Interface;
using Microsoft.VisualBasic;


namespace karu.bsonly.Serialization;

// static public partial class Serializer
// {
//   public static void Serialize(IDocumentSerializer serializer, ReadOnlySpan<byte> key, Guid value)
//   {
//     switch (serializer.Context().Configuration.GuidRepresentation)
//     {
//       case GuidRepresentation.STANDARD:
//         var bytes = value.ToByteArray(bigEndian: true);
//         serializer.WriteBinary(key).WriteBinary(bytes.AsSpan(), BsonConstants.BSON_BINARY_SUBTYPE_GUID);
//         break;
//       case GuidRepresentation.CSHARP_LEGACY:
//         var legacy_bytes = value.ToByteArray(bigEndian: false);
//         serializer.WriteBinary(key).WriteBinary(legacy_bytes.AsSpan(), BsonConstants.BSON_BINARY_SUBTYPE_GUID_CSHARP_LEGACY);
//         break;
//     }
//   }

//   public static void Serialize(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, ref Guid value)
//   {
//     if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_BINARY))
//     {
//       var bin_value = deserializer.ReadRawBinary();
//       if (bin_value.Length == BsonConstants.SIZE_OF_GUID + 1)
//       {
//         if (bin_value[0] == BsonConstants.BSON_BINARY_SUBTYPE_GUID)
//         {
//           value = new Guid(bin_value.Slice(1)); // FIXME: need to test  
//         }
//         if (bin_value[0] == BsonConstants.BSON_BINARY_SUBTYPE_GUID_CSHARP_LEGACY)
//         {
//           value = new Guid(bin_value.Slice(1), bigEndian: true); // FIXME: need to test  
//         }
//       }
//     }
//     else
//       throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"Guid\" found");
//   }
//   public static Guid SerializeGuid(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key)
//   {
//     if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_BINARY))
//     {
//       var bin_value = deserializer.ReadRawBinary();
//       if (bin_value.Length == BsonConstants.SIZE_OF_GUID + 1)
//       {
//         if (bin_value[0] == BsonConstants.BSON_BINARY_SUBTYPE_GUID)
//         {
//           return new Guid(bin_value.Slice(1), bigEndian: true); // FIXME: need to test  
//         }
//         if (bin_value[0] == BsonConstants.BSON_BINARY_SUBTYPE_GUID_CSHARP_LEGACY)
//         {
//           return new Guid(bin_value.Slice(1), bigEndian: false); // FIXME: need to test  
//         }
//       }
//       throw new KeyNotAvailableException($"key \"{System.Text.Encoding.UTF8.GetString(key)}\" cannot be deserialized into type \"Guid\"");
//     }
//     else
//       throw new KeyNotAvailableException($"no entry \"{System.Text.Encoding.UTF8.GetString(key)}\" of type \"Guid\" found");
//   }
// }


// Dictionary<string, T> will be serialized as Document
/*
template <typename Key, typename Value, typename MapType>
struct MapTypeDescription : BSON::TypeDescriptionCommon
{
using MyType = MapType;

static void serialize(Writer writer, const MyType& value)
{
if constexpr (BinaryUserType::sequence_type<Key> && BinaryUserType::sequence_type<Value>)
{
if (writer.options().sequences == BSON::Sequences::BINARY)
{
  std::vector<Key> keys;
  std::vector<Value> values;
  keys.reserve(value.size());
  values.reserve(value.size());
  for (const auto& entry : value)
  {
    keys.emplace_back(entry.first);
    values.emplace_back(entry.second);
  }
  Writer::Object object(writer);
  object.write("keys").write_binary<Key>({keys.data(), keys.data() + keys.size()});
  object.write("values").write_binary<Value>({values.data(), values.data() + values.size()});
  return;
}
}

Writer::Array array(writer);
for (const auto& entry : value)
{
array.append(assert_t<Key>(entry.first));
array.append(assert_t<Value>(entry.second));
}
}
*/
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