using karu.bsonly.Serialization.Interface;


namespace karu.bsonly.Serialization;


static public partial class Serializer
{
  // ISerializable T
  public static void Serialize<T>(IDocumentSerializer serializer, byte[] key, T value) where T : ISerializable
    => Serialize(serializer, key.AsSpan(), value);

  public static void Serialize<T>(IDocumentSerializer serializer, ReadOnlySpan<byte> key, T value) where T : ISerializable
  {
    var doc_writer = serializer.WriteDocument(key);
    value.Serialize(doc_writer);
    doc_writer.FinishSubDocument();
  }

  public static void Serialize<T>(IDocumentDeserializer deserializer, byte[] key, T value) where T : ISerializable
    => Serialize(deserializer, key.AsSpan(), value);
  public static void Serialize<T>(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, T value) where T : ISerializable
  {
    if (deserializer.HasEntry(key, BsonConstants.BSON_TYPE_DOCUMENT))
    {
      var doc_reader = deserializer.DocumentReader();
      value.Deserialize(doc_reader);
      doc_reader.Finish();
      // doc_reader.ReadDocument<T>(value, new DeserializationContext()); // FIXME: Context must come from outside
    }
    else
      throw new KeyNotAvailableException($"no entry {System.Text.Encoding.UTF8.GetString(key)} of type \"document\" found");
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