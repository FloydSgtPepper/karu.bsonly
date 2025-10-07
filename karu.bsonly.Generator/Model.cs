namespace karu.bsonly.Generator
{
  public record Property
  {
    public const int IGNORE_VALUE = int.MinValue;
    public const int DEAULT_ORDER_VALUE = -1;

    public string Type { get; init; }
    public string Name { get; init; }

    public string BsonName { get; init; }
    public string BsonType { get; init; }
    public int Order { get; init; }

    // public string Debug { get; init; }

    public Property(string type, string name, string bson_name, string bson_type, int order_or_ignore, string debug = "")
    {
      Type = type;
      Name = name;
      BsonName = bson_name;
      BsonType = bson_type;
      Order = order_or_ignore;
      // Debug = debug;
    }
  }


  public record Model
  {
    public string Namespace { get; init; } = string.Empty;

    public string ClassName { get; init; } = string.Empty;

    public string WrappedClass { get; init; } = string.Empty;

    public EquatableArray<Property> Properties { get; init; } = EquatableArray<Property>.Empty();

    public bool WriteEncode { get; init; }

    public bool WriteDecode { get; init; }
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