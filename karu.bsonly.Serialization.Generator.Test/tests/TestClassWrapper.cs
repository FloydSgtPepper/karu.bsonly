using System.Diagnostics;
using karu.bsonly.Serialization.Interface;
using karu.bsonly.Serialization;
using karu.bsonly.Test.Utils;

using System.Text.RegularExpressions;

namespace karu.bsonly.Serialization.Test;


[TestClass]
public class TestWrapperRegistry
{
  private SerializationContext _write_context = SerializationContext.Default;
  private DeserializationContext _read_context = DeserializationContext.Default;


  [TestInitialize]
  public void Initialize()
  {


    var registry = new SerializationRegistry();
    registry.Register(typeof(WrappedClassInner), new WrapperClassInner());

    _write_context.SerializationRegistry = registry;
    _read_context.SerializationRegistry = registry;
  }


  [TestMethod]
  public void WriteClassWithClassMemberGenerated()
  {
    string expected = Regex.Replace(@"0x7d000000 12 4c6f6e6750726f706572747900 6712000000000000 
                                        03 496e6e6572436c61737300 45000000
                                          02 53747250726f706572747900 08000000 666f6f5f62617200
                                          12 4c6f6e6750726f706572747900 8681470e01000000
                                          10 496e7450726f706572747900 aa55aa55
                                        00
                                        10 496e7450726f706572747900 ffffffff
                                        00", "\\s+", "");

    var tc = new WrappedClass
    {
      LongProperty = 4711,
      InnerClass = new WrappedClassInner { StrProperty = "foo_bar", IntProperty = 1437226410/*55AA55AA*/, LongProperty = 4534534534 },
      IntProperty = -1
    };

    var bson_doc = BsonlySerializer.Serialize(tc, _write_context);
    Debug.WriteLine($"hex {HexConverter.ByteArrayToHexString(bson_doc)}");
    Debug.WriteLine($"hex {expected}");

    Assert.AreEqual(expected, HexConverter.ByteArrayToHexString(bson_doc));
  }

  [TestMethod]
  public void ReadClassWithClassMemberObject()
  {
    string input = Regex.Replace(@"0x7d000000 12 4c6f6e6750726f706572747900 6712000000000000 
                                   03 496e6e6572436c61737300 45000000
                                     02 53747250726f706572747900 08000000 666f6f5f62617200
                                     12 4c6f6e6750726f706572747900 8681470e01000000
                                     10 496e7450726f706572747900 aa55aa55
                                   00
                                   10 496e7450726f706572747900 ffffffff
                                   00", "\\s+", "");

    TestClassSimple value = new();
    var bson_doc = HexConverter.HexStringToByteArray(input);
    BsonlySerializer.Deserialize(bson_doc, value, _read_context);

    Assert.AreEqual(4711, value.LongProperty);
    Assert.AreEqual(-1, value.IntProperty);
    Assert.AreEqual("foo_bar", value.InnerClass.StrProperty);
    Assert.AreEqual(1437226410, value.InnerClass.IntProperty);
    Assert.AreEqual(4534534534, value.InnerClass.LongProperty);
  }

  // [TestMethod]
  // public void ReadClassWithClassMemberGenerated()
  // {
  //   string input = Regex.Replace(@"0x7d000000 12 4c6f6e6750726f706572747900 6712000000000000 
  //                                  03 496e6e6572436c61737300 45000000
  //                                    02 53747250726f706572747900 08000000 666f6f5f62617200
  //                                    12 4c6f6e6750726f706572747900 8681470e01000000
  //                                    10 496e7450726f706572747900 aa55aa55
  //                                  00
  //                                  10 496e7450726f706572747900 ffffffff
  //                                  00", "\\s+", "");

  //   TestClassGenerated value = new();
  //   BsonlySerializer.Deserialize(HexConverter.HexStringToByteArray(input), new DeserializationContext(), value);

  //   Assert.AreEqual(4711, value.LongProperty);
  //   Assert.AreEqual(-1, value.IntProperty);
  //   Assert.AreEqual("foo_bar", value.InnerClass.StrProperty);
  //   Assert.AreEqual(1437226410, value.InnerClass.IntProperty);
  //   Assert.AreEqual(4534534534, value.InnerClass.LongProperty);
  // }
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