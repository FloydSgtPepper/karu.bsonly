namespace karu.bsonly.Generator
{
  class AttrProperty
  {
    // public string Debug = "";

    public string BsonName;
    public string BsonType;
    public int Order;

    public string SerializationMethod;
    public byte BinaryType;
    public byte UserType;

    public AttrProperty(string name, string type, int order)
    {
      BsonName = name;
      BsonType = type;
      Order = order;
      SerializationMethod = string.Empty;
      BinaryType = BsonlyBinaryDataAttribute.DefaultType;
      UserType = BsonlyBinaryDataAttribute.DefaultType;
    }

    public AttrProperty(string serialization_method, byte binary_type, byte user_type)
    {
      BsonName = string.Empty;
      BsonType = string.Empty;
      Order = Property.DEAULT_ORDER_VALUE;
      SerializationMethod = serialization_method;
      BinaryType = binary_type;
      UserType = user_type;
    }

    public AttrProperty()
    {
      BsonName = string.Empty;
      BsonType = string.Empty;
      Order = Property.DEAULT_ORDER_VALUE;
      SerializationMethod = string.Empty;
      BinaryType = BsonlyBinaryDataAttribute.DefaultType;
      UserType = BsonlyBinaryDataAttribute.DefaultType;
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