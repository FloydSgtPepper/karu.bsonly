using karu.bsonly.Serialization;
using karu.bsonly.Serialization.Interface;

namespace karu.bsonly.Serialization.Benchmarks;


// does not implement ISerializable
class TestClassInnerSimple
{
  public string StrProperty = string.Empty;
  public long LongProperty = 0;
  public int IntProperty = 0;


  public void Deserialize(IDocumentDeserializer reader)
  {
    // Serializer.Serialize(reader, "StrProperty"u8, ref StrProperty);
    // Serializer.Serialize(reader, "LongProperty"u8, ref LongProperty);
    // Serializer.Serialize(reader, "IntProperty"u8, ref IntProperty);
  }

  public void Serialize(IDocumentSerializer writer)
  {
    Serializer.Serialize(writer, "StrProperty"u8, StrProperty);
    Serializer.Serialize(writer, "LongProperty"u8, LongProperty);
    Serializer.Serialize(writer, "IntProperty"u8, IntProperty);
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