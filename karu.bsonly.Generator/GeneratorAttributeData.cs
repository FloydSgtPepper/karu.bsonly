namespace karu.bsonly.Generator
{

  public enum Attributes : short
  {
    ApiIgnore = 0,
    ApiElement = 1,
    ApiOrder = 2,
    ApiType = 3,
    ApiName = 4,
    ApiUtf8 = 5,
    NOT_SUPPORTED = 127
  };


  public class GeneratorAttributeData
  {
    public static Attributes SupportedAttributes(string? name)
    {
      if (name == null)
        return Attributes.NOT_SUPPORTED;

      if (name == "ApiIgnore")
        return Attributes.ApiIgnore;

      if (name == "ApiOrder")
        return Attributes.ApiOrder;

      if (name == "ApiType")
        return Attributes.ApiType;

      if (name == "ApiName")
        return Attributes.ApiName;

      if (name == "ApiElement")
        return Attributes.ApiElement;

      if (name == "ApiUtf8")
        return Attributes.ApiUtf8;

      return Attributes.NOT_SUPPORTED;
    }
  }
}

// var sourceText = SourceText.From($$"""
// namespace FooBar;
// partial class Bar
// {
//     partial void Serialize()
//     {
//         // generated code
//     }
// }
// """, Encoding.UTF8);
// source.W(sourceText);

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