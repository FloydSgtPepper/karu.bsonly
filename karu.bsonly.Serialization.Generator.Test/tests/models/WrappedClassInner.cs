using karu.bsonly.Serialization;
using karu.bsonly.Serialization.Interface;
using karu.bsonly.Generator;

namespace karu.bsonly.Serialization.Test;

// does not implement ISerializable and has no Serialize / Deserialize functions
// and cannot be changed
public class WrappedClassInner
{
  public string StrProperty = string.Empty;
  public long LongProperty = 0;
  public int IntProperty = 0;
}

// [ApiWrap("WrappedClassInner")]
partial class WrapperClassInner : ISerializationProvider, ISerializable
{
  // written by user
  public string StrProperty = string.Empty;
  public long LongProperty = 0;
  public int IntProperty = 0;

  public void Serialize(IDocumentSerializer serializer)
  {
    Serializer.Serialize(serializer, "StrProperty"u8, this.StrProperty);
    Serializer.Serialize(serializer, "LongProperty"u8, this.LongProperty);
    Serializer.Serialize(serializer, "IntProperty"u8, this.IntProperty);
  }

  public void Deserialize(IDocumentDeserializer deserializer)
  {
    Serializer.Serialize(deserializer, "StrProperty"u8, ref this.StrProperty);
    Serializer.Serialize(deserializer, "LongProperty"u8, ref this.LongProperty);
    Serializer.Serialize(deserializer, "IntProperty"u8, ref this.IntProperty);
  }

  /* generated if not provided */
  public void DeserializationFunction(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key,ref object? value, Type type)
  {
    Deserialize(deserializer);
    value = SetWrappedValues();
  }

  /* generated if not provided */
  public void SerializationFunction(IDocumentSerializer serializer, ReadOnlySpan<byte> key, object value, Type type)
  {
    var tc = value as WrappedClassInner;
    if (tc != null)
    {
      SetWrapperValues(tc);
      Serializer.Serialize(serializer, key, this);
      return;
    }

    throw new ArgumentException($"expected value argument of type {typeof(WrappedClassInner)} but got {type}");
  }

  public WrapperClassInner(WrappedClassInner wrapped)
  {
    SetWrapperValues(wrapped);
  }

  public WrapperClassInner()
  {
  }


  // -- written by user --
  private void SetWrapperValues(WrappedClassInner wrapped)
  {
    StrProperty = wrapped.StrProperty;
    LongProperty = wrapped.LongProperty;
    IntProperty = wrapped.IntProperty;
  }
  public void SetWrappedValues(WrappedClassInner wrapped)
  {
    wrapped.StrProperty = StrProperty;
    wrapped.LongProperty = LongProperty;
    wrapped.IntProperty = IntProperty;
  }


  // written by user
  public WrappedClassInner SetWrappedValues()
  {
    return new WrappedClassInner
    {
      StrProperty = this.StrProperty,
      LongProperty = this.LongProperty,
      IntProperty = this.IntProperty
    };
  }
}

// generated in Serializer class
public static class WrapperSerializer
{
  public static void Serialize(IDocumentSerializer serializer, ReadOnlySpan<byte> key, WrappedClassInner value)
  {
    var wrapped_value = new WrapperClassInner(value);
    Serializer.Serialize<WrapperClassInner>(serializer, key, wrapped_value);
  }

  public static void Serialize(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, WrappedClassInner value)
  {
    var wrapped_value = new WrapperClassInner(value);
    Serializer.Serialize<WrapperClassInner>(deserializer, key, wrapped_value);
    wrapped_value.SetWrappedValues(value);
  }
}

// Serialize.Serialze( ... WrappedClassInner value)
// .. call Serialize.Seralize( .. Wrap_WrappedClassInner ..)

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