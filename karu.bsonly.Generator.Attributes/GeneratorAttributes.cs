
using System;

namespace karu.bsonly.Generator
{
  [System.AttributeUsage(System.AttributeTargets.Class)]
  public class WrapAttribute : System.Attribute
  {
    public string Name { get; }

    public WrapAttribute(string name)
    {
      Name = name;
    }
  }

  /// <summary>
  /// Function attribute.
  /// Can be applied to the En- and/or Deserialization function
  /// </summary>
  // [System.AttributeUsage(System.AttributeTargets.Method)]
  // public class SerializeAttribute : System.Attribute
  // {
  // }

  [System.AttributeUsage(System.AttributeTargets.Field | AttributeTargets.Property)]
  public class ApiElementAttribute : System.Attribute
  {
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Order { get; set; } = -1;

    public ApiElementAttribute(string name, string type, int order = -1)
    {
      Name = name;
      Type = type;
      Order = order;
    }

    public ApiElementAttribute(string name, int order = -1)
    {
      Name = name;
      Type = string.Empty;
      Order = order;
    }
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class ApiNameAttribute : System.Attribute
  {
    public string Name { get; set; } = string.Empty;

    public ApiNameAttribute(string name)
    {
      Name = name;
    }
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class BsonlyTypeAttribute : System.Attribute
  {
    public string Type { get; set; } = string.Empty;

    public BsonlyTypeAttribute(string type)
    {
      Type = type;
    }
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class ApiOrderAttribute : System.Attribute
  {
    public int Order { get; set; } = -1;

    public ApiOrderAttribute(int order)
    {
      Order = order;
    }
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class ApiIgnoreAttribute : System.Attribute
  {
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