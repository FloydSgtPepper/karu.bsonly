using System.Diagnostics;
using karu.bsonly.Generator;
using karu.bsonly.Test.Utils;

using karu.bsonly.Serialization;
using System.Text.RegularExpressions;
using karu.bsonly.Serialization.Interface;

namespace karu.bsonly.Serialization.Test;


[TestClass]
public class TestObjectSerialization
{
  internal class TestStructWithObject : ISerializable
  {
    public long LongProperty;

    public required object? ObjectProperty;

    public int IntProperty;

    public void Deserialize(IBaseDeserializer deserializer, DeserializationContext context)
    {
      if (deserializer.HasEntry("LongProperty"u8, BsonConstants.BSON_TYPE_INT64))
        LongProperty = deserializer.ReadLong();

      // ObjectProperty not nulllable
      // object? tmp = ObjectProperty;
      // Serializer.SerializeObjectType(deserializer, "ObjectProperty"u8, ref tmp);
      // if (tmp != null)
      //   ObjectProperty = tmp;
      object? obj_tmp = null;
      Serializer.Serialize(deserializer, "ObjectProperty"u8, ref obj_tmp, typeof(object), context);
      if (deserializer.HasEntry("IntProperty"u8, BsonConstants.BSON_TYPE_INT32))
        IntProperty = deserializer.ReadInt();
    }

    public void Serialize(IBaseSerializer serializer, SerializationContext context)
    {
      serializer.WriteLong("LongProperty"u8, LongProperty);
      Serializer.SerializeObjectType(serializer, "ObjectProperty"u8, ObjectProperty, context);
      serializer.WriteInt("IntProperty"u8, IntProperty);
    }
  };


  internal class TestClassWithObject2 : ISerializable
  {
    public required object? ObjectProperty;

    public void Deserialize(IBaseDeserializer deserializer, DeserializationContext context)
    {
      object? obj_tmp = null;
      Serializer.Serialize(deserializer, "ObjectProperty"u8, ref obj_tmp, typeof(object), context);
    }

    public void Serialize(IBaseSerializer serializer, SerializationContext context)
    {
      Serializer.SerializeObjectType(serializer, "ObjectProperty"u8, ObjectProperty, context);
    }
  };

  private SerializationContext _context = SerializationContext.Default;
  [TestInitialize]
  public void Initialize()
  {
    _context.SerializationRegistry = new SerializationRegistry();
    _context.SerializationRegistry.Register(typeof(WrappedClassInner), new WrapperClassInner());
  }

  [TestMethod]
  public void WriteWrappedClass()
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

    var bson_doc = ApiSerializer.Serialize(tc, _context);
    Debug.WriteLine($"hex {HexConverter.ByteArrayToHexString(bson_doc)}");
    Debug.WriteLine($"hex {expected}");

    Assert.AreEqual(expected, HexConverter.ByteArrayToHexString(bson_doc));
  }

  //   public void ReadWrappedClass()
  //   {
  //     var expected = new WrappedClass
  //     {
  //       LongProperty = 4711,
  //       InnerClass = new WrappedClassInner { StrProperty = "foo_bar", IntProperty = 1437226410/*55AA55AA*/, LongProperty = 4534534534 },
  //       IntProperty = -1
  //     };
  //     string bson = Regex.Replace(@"0x7d000000 12 4c6f6e6750726f706572747900 6712000000000000 
  //                                    03 496e6e6572436c61737300 45000000
  //                                      02 53747250726f706572747900 08000000 666f6f5f62617200
  //                                      12 4c6f6e6750726f706572747900 8681470e01000000
  //                                      10 496e7450726f706572747900 aa55aa55
  //                                    00
  //                                    10 496e7450726f706572747900 ffffffff
  //                                    00", "\\s+", "");


  //     var registry = new WrapperRegistry();
  //     registry.Register(typeof(WrappedClassInner), (value) => { return new WrapperClassInner((WrappedClassInner)value); });

  //     var context = new DeserializationContext
  //     {
  //       WrapperRegistry = () => { return registry; }
  //     };

  //     var actual = new WrappedClass();

  //     ApiSerializer.Deserialize(HexConverter.HexStringToByteArray(bson), context, actual);

  //     Assert.AreEqual(expected.LongProperty, actual.LongProperty);
  //     Assert.AreEqual(expected.IntProperty, actual.IntProperty);
  //     Assert.AreEqual(expected.InnerClass.StrProperty, actual.InnerClass.StrProperty);
  //     Assert.AreEqual(expected.InnerClass.IntProperty, actual.InnerClass.IntProperty);
  //     Assert.AreEqual(expected.InnerClass.LongProperty, actual.InnerClass.LongProperty);
  //   }


  //   [TestMethod]
  //   public void WriteString()
  //   {
  //     string expected = Regex.Replace(@"0x40000000 
  //                                         12 4c6f6e6750726f706572747900 2a00000000000000
  //                                         02 4f626a65637450726f706572747900 11000000 74686973206973206120737472696e6700
  //                                       00", "\\s+", "");

  //     var tc = new TestStructWithObject { LongProperty = 42, ObjectProperty = "this is a string" };

  //     var context = new SerializationContext
  //     {
  //       WrapperRegistry = () => { return null!; },
  //       IsJson = false
  //     };

  //     var bson_doc = ApiSerializer.Serialize(tc, context);
  //     Debug.WriteLine($"hex {HexConverter.ByteArrayToHexString(bson_doc)}");
  //     Debug.WriteLine($"hex {expected}");

  //     Assert.AreEqual(expected, HexConverter.ByteArrayToHexString(bson_doc));
  //   }

  //   [TestMethod]
  //   public void ReadString()
  //   {
  //     string bson = Regex.Replace(@"0x40000000 
  //                                     12 4c6f6e6750726f706572747900 2a00000000000000
  //                                     02 4f626a65637450726f706572747900 11000000 74686973206973206120737472696e6700
  //                                   00", "\\s+", "");

  //     var expected = new TestStructWithObject { LongProperty = 42, ObjectProperty = "this is a string" };

  //     var context = new DeserializationContext
  //     {
  //       WrapperRegistry = () => { return null!; },
  //       IsJson = false
  //     };

  //     var tc = new TestStructWithObject { ObjectProperty = 0 };
  //     ApiSerializer.Deserialize(HexConverter.HexStringToByteArray(bson), context, tc);

  //     Assert.AreEqual(expected.LongProperty, tc.LongProperty);
  //     Assert.AreEqual(expected.ObjectProperty.GetType(), typeof(string));
  //     Assert.AreEqual((string)expected.ObjectProperty, (string)tc.ObjectProperty);
  //   }

  //   [TestMethod]
  //   public void WriteInt()
  //   {
  //     string expected = Regex.Replace(@"0x2f000000
  //                                         12 4c6f6e6750726f706572747900 2a00000000000000
  //                                         10 4f626a65637450726f706572747900 05000000
  //                                       00", "\\s+", "");

  //     var tc = new TestStructWithObject { LongProperty = 42, ObjectProperty = (int)5 };

  //     var context = new SerializationContext
  //     {
  //       WrapperRegistry = () => { return null!; },
  //       IsJson = false
  //     };

  //     var bson_doc = ApiSerializer.Serialize(tc, context);
  //     Debug.WriteLine($"hex {HexConverter.ByteArrayToHexString(bson_doc)}");
  //     Debug.WriteLine($"hex {expected}");

  //     Assert.AreEqual(expected, HexConverter.ByteArrayToHexString(bson_doc));
  //   }

  //   [TestMethod]
  //   public void ReadInt()
  //   {
  //     string bson = Regex.Replace(@"0x2f000000
  //                                         12 4c6f6e6750726f706572747900 2a00000000000000
  //                                         10 4f626a65637450726f706572747900 05000000
  //                                       00", "\\s+", "");

  //     var expected = new TestStructWithObject { LongProperty = 42, ObjectProperty = (int)5 };

  //     var context = new DeserializationContext
  //     {
  //       WrapperRegistry = () => { return null!; },
  //       IsJson = false
  //     };

  //     var tc = new TestStructWithObject { ObjectProperty = 0 };
  //     ApiSerializer.Deserialize(HexConverter.HexStringToByteArray(bson), context, tc);

  //     Assert.AreEqual(expected.LongProperty, tc.LongProperty);
  //     Assert.AreEqual(expected.ObjectProperty.GetType(), typeof(int));
  //     Assert.AreEqual((int)expected.ObjectProperty, (int)tc.ObjectProperty);
  //   }

  //   [TestMethod]
  //   public void WriteNull()
  //   {
  //     string expected = Regex.Replace(@"0x2c000000
  //                                         12 4c6f6e6750726f706572747900 2a00000000000000
  //                                         0a 4f626a65637450726f706572747900 00
  //                                       00", "\\s+", "");

  //     var tc = new TestStructWithObject { LongProperty = 42, ObjectProperty = null };

  //     var context = new SerializationContext
  //     {
  //       WrapperRegistry = () => { return null!; },
  //       IsJson = false
  //     };

  //     var bson_doc = ApiSerializer.Serialize(tc, context);
  //     Debug.WriteLine($"hex {HexConverter.ByteArrayToHexString(bson_doc)}");
  //     Debug.WriteLine($"hex {expected}");

  //     Assert.AreEqual(expected, HexConverter.ByteArrayToHexString(bson_doc));
  //   }

  //   [TestMethod]
  //   public void ReadNull()
  //   {
  //     string bson = Regex.Replace(@"0x2c000000
  //                                         12 4c6f6e6750726f706572747900 2a00000000000000
  //                                         0a 4f626a65637450726f706572747900 00
  //                                       00", "\\s+", "");

  //     var expected = new TestStructWithObject { LongProperty = 42, ObjectProperty = null };

  //     var context = new DeserializationContext
  //     {
  //       WrapperRegistry = () => { return null!; },
  //       IsJson = false
  //     };

  //     var tc = new TestStructWithObject { ObjectProperty = 0 };
  //     ApiSerializer.Deserialize(HexConverter.HexStringToByteArray(bson), context, tc);

  //     Assert.AreEqual(expected.LongProperty, tc.LongProperty);
  //     Assert.IsTrue(tc.ObjectProperty == null);
  //   }


  //   [TestMethod]
  //   public void WriteObject()
  //   {
  //     string expected = Regex.Replace(@"0x4d000000
  //                                         12 4c6f6e6750726f706572747900 2a00000000000000
  //                                         03 4f626a65637450726f706572747900 22000000 
  //                                           02 4f626a65637450726f706572747900 09000000 6120737472696e6700 
  //                                         00
  //                                       00", "\\s+", "");

  //     var tc = new TestStructWithObject
  //     {
  //       LongProperty = 42,
  //       ObjectProperty = new TestClassWithObject2 { ObjectProperty = "a string" }
  //     };

  //     var context = new SerializationContext
  //     {
  //       WrapperRegistry = () => { return null!; },
  //       IsJson = false
  //     };

  //     var bson_doc = ApiSerializer.Serialize(tc, context);
  //     Debug.WriteLine($"hex {HexConverter.ByteArrayToHexString(bson_doc)}");
  //     Debug.WriteLine($"hex {expected}");

  //     Assert.AreEqual(expected, HexConverter.ByteArrayToHexString(bson_doc));
  //   }

  //   [TestMethod]
  //   public void ReadObject()
  //   {
  //     string bson = Regex.Replace(@"0x4d000000
  //                                         12 4c6f6e6750726f706572747900 2a00000000000000
  //                                         03 4f626a65637450726f706572747900 22000000 
  //                                           02 4f626a65637450726f706572747900 09000000 6120737472696e6700 
  //                                         00
  //                                       00", "\\s+", "");

  //     var expected = new TestStructWithObject
  //     {
  //       LongProperty = 42,
  //       ObjectProperty = new TestClassWithObject2 { ObjectProperty = "a string" }
  //     };

  //     var context = new DeserializationContext
  //     {
  //       WrapperRegistry = () => { return null!; },
  //       IsJson = false
  //     };

  //     var tc = new TestStructWithObject { ObjectProperty = new TestClassWithObject2 { ObjectProperty = 0 } };
  //     ApiSerializer.Deserialize(HexConverter.HexStringToByteArray(bson), context, tc);

  //     Assert.AreEqual(expected.LongProperty, tc.LongProperty);
  //     Assert.AreEqual(expected.ObjectProperty.GetType(), typeof(TestClassWithObject2));

  //     var inner_class = tc.ObjectProperty as TestClassWithObject2;
  //     Assert.AreEqual("a string", (string)inner_class!.ObjectProperty!);
  //   }

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