using karu.bsonly.Serialization;
using karu.bsonly.Serialization.Interface;

namespace karu.bsonly.Serialization.Benchmarks;


// does not implement ISerializable and has no Serialize / Deserialize functions
// and cannot be changed
public class WrappedClassInner
{
  public string StrProperty = string.Empty;
  public long LongProperty = 0;
  public int IntProperty = 0;
}

// [ApiWrap("WrappedClassInner")]
partial class WrapperClassInner : ISerializable
{
  // written by user
  public string StrProperty = string.Empty;
  public long LongProperty = 0;
  public int IntProperty = 0;

  // written by user
  public WrapperClassInner(WrappedClassInner wc)
  {
    StrProperty = wc.StrProperty;
    LongProperty = wc.LongProperty;
    IntProperty = wc.IntProperty;
  }
  // written by user
  public void SetWrappedClassValues(WrappedClassInner wc)
  {
    wc.StrProperty = StrProperty;
    wc.LongProperty = LongProperty;
    wc.IntProperty = IntProperty;
  }

  /* generated if not provided */
  public void Deserialize(IBaseDeserializer reader, DeserializationContext context)
  {
    // Serializer.Serialize(reader, "StrProperty"u8, ref this.StrProperty);
    // Serializer.Serialize(reader, "LongProperty"u8, ref this.LongProperty);
    // Serializer.Serialize(reader, "IntProperty"u8, ref this.IntProperty);
  }

  /* generated if not provided */
  public void Serialize(IBaseSerializer writer, SerializationContext context)
  {
    Serializer.Serialize(writer, "StrProperty"u8, this.StrProperty);
    Serializer.Serialize(writer, "LongProperty"u8, this.LongProperty);
    Serializer.Serialize(writer, "IntProperty"u8, this.IntProperty);
  }
}

// generated in Serializer class
public static class WrapperSerializer
{
  public static void Serialize(IBaseSerializer serializer, ReadOnlySpan<byte> key, WrappedClassInner value)
  {
    // var wrapped_value = new WrapperClassInner(value);
    // Serializer.Serialize<WrapperClassInner>(serializer, key, wrapped_value);
  }

  public static void Serialize(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, WrappedClassInner value)
  {
    // var wrapped_value = new WrapperClassInner(value);
    // Serializer.Serialize<WrapperClassInner>(deserializer, key, wrapped_value);
    // wrapped_value.SetWrappedClassValues(value);
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