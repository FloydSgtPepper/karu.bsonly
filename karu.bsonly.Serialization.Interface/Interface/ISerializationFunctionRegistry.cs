using System;

namespace karu.bsonly.Serialization.Interface
{

  public interface ISerializationProvider
  {
    public void SerializationFunction(IDocumentSerializer serializer, ReadOnlySpan<byte> key, object value, Type type_of_value);
    public void DeserializationFunction(IDocumentDeserializer serializer, ReadOnlySpan<byte> key, ref object? value, Type type_of_value);
  }


  public interface ISerializationFunctionRegistry
  {
    void Register(Type type, ISerializationProvider provider_object);

    public ISerializationProvider? Serializer(Type type);
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