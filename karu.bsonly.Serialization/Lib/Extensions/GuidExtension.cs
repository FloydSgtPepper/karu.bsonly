using System.Reflection;
using System.Runtime.CompilerServices;
using karu.bsonly.Serialization.Interface;


namespace karu.bsonly.Serialization.Extension;

static public class GuidExtension
{
  public static void Serialize(this Guid extendee, IDocumentSerializer serializer, ReadOnlySpan<byte> key)
  {
    Serializer.Serialize(serializer, key, extendee);
  }

  public static void Deserialize(this Guid extendee, IDocumentDeserializer serializer, ReadOnlySpan<byte> key)
  {
    Serializer.Serialize(serializer, key, ref extendee);
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