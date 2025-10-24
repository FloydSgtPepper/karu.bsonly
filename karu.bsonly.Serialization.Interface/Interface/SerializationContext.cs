namespace karu.bsonly.Serialization.Interface
{

  public class SerializationContext
  {


    //public WrapperRegistryFct? WrapperRegistry = null;

    public ISerializationFunctionRegistry? SerializationRegistry = null;

    public BsonSettings Configuration = new BsonSettings();

    public static SerializationContext Default = new SerializationContext
    {
      SerializationRegistry = null,
      Configuration = BsonSettings.BSON_API
    };

    public SerializationContext()
    {
      SerializationRegistry = null;
      Configuration = new BsonSettings();
    }
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