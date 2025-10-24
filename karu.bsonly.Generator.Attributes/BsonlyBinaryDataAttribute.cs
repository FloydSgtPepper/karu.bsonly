
namespace karu.bsonly.Generator
{
  [System.AttributeUsage(System.AttributeTargets.Field)]
  public class BsonlyBinaryDataAttribute : System.Attribute
  {
    public const byte DefaultType = 255;

    public string SerializationMethod;

    public byte BinaryType;

    public byte UserType;


    public BsonlyBinaryDataAttribute(string serialization_method, byte binary_type = DefaultType, byte user_type = DefaultType)
    {
      SerializationMethod = serialization_method;
      BinaryType = binary_type;
      UserType = user_type;

    }

    public BsonlyBinaryDataAttribute(byte binary_type, byte user_type)
    {
      SerializationMethod = string.Empty;
      BinaryType = binary_type;
      UserType = user_type;
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