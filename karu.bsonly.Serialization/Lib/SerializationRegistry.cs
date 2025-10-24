using karu.bsonly.Serialization.Interface;

namespace karu.bsonly.Serialization;

/// <summary>
/// factory function to create a wrapper
/// </summary>
/// <param name="wrapped_value">the object to be wrapped</param>
/// <returns>the wrapper object</returns>


public class SerializationRegistry : ISerializationFunctionRegistry
{
  private Dictionary<Type, ISerializationProvider> _serialization_functions = new();


  public void Register(Type type, ISerializationProvider serialization_provider)
  {
    _serialization_functions[type] = serialization_provider;
  }

  public ISerializationProvider? Serializer(Type type)
  {
    if (_serialization_functions.TryGetValue(type, out var provider))
      return provider;

    return null;
  }

  public static SerializationRegistry DefaultInitialization()
  {
    var registry = new SerializationRegistry();

    registry.Register(typeof(List<ulong>), new Provider.SerializationOfListOfULong());
    registry.Register(typeof(List<long>), new Provider.SerializationOfListOfLong());
    registry.Register(typeof(List<uint>), new Provider.SerializationOfListOfUInt());
    registry.Register(typeof(List<int>), new Provider.SerializationOfListOfInt());

    return registry;
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