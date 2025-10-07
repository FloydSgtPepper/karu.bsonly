
using System.Diagnostics;
using karu.bsonly.Serialization;
using karu.bsonly.Test;
using karu.bsonly.Serialization.Interface;
using System.Text.RegularExpressions;

namespace karu.bsonly.Serialization.Test;


[TestClass]
public class TestJsonStreamReader
{

  [TestInitialize]
  public void Initialize()
  {
  }

  internal class TestClass : ISerializable
  {
    public string StrProperty = string.Empty;
    public long LongProperty = 0;
    public int IntProperty = 0;
    public bool BoolProperty = false;
    public double DoubleProperty = 0.0;

    public void Serialize(IBaseSerializer writer, SerializationContext context)
    {
      Serializer.Serialize(writer, "StrProperty"u8, StrProperty);
      Serializer.Serialize(writer, "LongProperty"u8, LongProperty);
      Serializer.Serialize(writer, "IntProperty"u8, IntProperty);
      Serializer.Serialize(writer, "BoolProperty"u8, BoolProperty);
      Serializer.Serialize(writer, "DoubleProperty"u8, DoubleProperty);
    }

    public void Deserialize(IBaseDeserializer reader, DeserializationContext context)
    {
      Serializer.Serialize(reader, "StrProperty"u8, ref StrProperty);
      Serializer.Serialize(reader, "LongProperty"u8, ref LongProperty);
      Serializer.Serialize(reader, "IntProperty"u8, ref IntProperty);
    }
  }

  //   [TestMethod]
  //   public void BasicMemberSerialization()
  //   {
  //     // bson
  //     // 0x75000000
  //     //   02 53747250726f706572747900 11000000 74686973206973206120737472696e6700 
  //     //   12 4c6f6e6750726f706572747900 215f000000000000
  //     //   10 496e7450726f706572747900 2a000000
  //     //   08 426f6f6c50726f706572747900 01
  //     //   01 446f75626c6550726f706572747900 1f85eb51b81e0940
  //     // 00

  //     var tc = new TestClass { StrProperty = "this is a string", LongProperty = 24353, IntProperty = 42, BoolProperty = true, DoubleProperty = 3.14 };
  // #if USE_MONGO_DRIVER
  //     var mongo_bson = tc.ToBson();
  //     Debug.WriteLine($"mongo {Utils.HexConverter.ByteArrayToHexString(mongo_bson)}");
  // #else
  //     var mongo_bson_str = "0x750000000253747250726f7065727479001100000074686973206973206120737472696e6700124c6f6e6750726f706572747900215f00000000000010496e7450726f7065727479002a00000008426f6f6c50726f7065727479000101446f75626c6550726f7065727479001f85eb51b81e094000";
  //     var mongo_bson = Utils.HexConverter.HexStringToByteArray(mongo_bson_str);
  // #endif

  //     var serializer = new StreamWriter(new BsonSettings { OutOfOrderEvaluation = false, MaxSize = 64 * 1024 * 1024 });
  //     var our_doc = MyBasicSerializer.CreatBsonDoc(serializer, "", tc);
  //     Debug.WriteLine($"our   {Utils.HexConverter.ByteArrayToHexString(our_doc)}");

  //     Assert.IsTrue(our_doc.ReadOnlySpan<byte>.EmptyEqual(mongo_bson));
  //   }

  //   [TestMethod]
  //   public void ClassWithClassMemberSerialization()
  //   {
  //     //mongo 
  //     // 0x74000000
  //     //    02 53747250726f706572747900 11000000 74686973206973206120737472696e6700 
  //     //    03 496e6e6572436c61737300 41000000
  //     //       02 53747250726f706572747900 04000000 666f6f00
  //     //       12 4c6f6e6750726f706572747900 2a00000000000000
  //     //       10 496e7450726f706572747900 03000000
  //     //    00
  //     // 00
  //     var tc = new ClassWithClassMember
  //     {
  //       StrProperty = "this is a string",
  //       InnerClass = new InnerClass
  //       {
  //         StrProperty = "foo",
  //         LongProperty = 42,
  //         IntProperty = 3
  //       }
  //     };

  // #if USE_MONGO_DRIVER
  //     var mongo_bson = tc.ToBson();
  //     Debug.WriteLine($"mongo {Utils.HexConverter.ByteArrayToHexString(mongo_bson)}");
  // #else
  //     var mongo_bson_str = "0x740000000253747250726f7065727479001100000074686973206973206120737472696e670003496e6e6572436c61737300410000000253747250726f70657274790004000000666f6f00124c6f6e6750726f7065727479002a0000000000000010496e7450726f706572747900030000000000";
  //     var mongo_bson = Utils.HexConverter.HexStringToByteArray(mongo_bson_str);
  // #endif

  //     var serializer = new StreamWriter(new BsonSettings { OutOfOrderEvaluation = false, MaxSize = 64 * 1024 * 1024 });
  //     var our_doc = MyBasicSerializer.CreatBsonDoc(serializer, "", tc);
  //     Debug.WriteLine($"our   {Utils.HexConverter.ByteArrayToHexString(our_doc)}");

  //     Assert.IsTrue(our_doc.SequenceEqual(mongo_bson));
  //   }


  [TestMethod]
  public void WriteGuid()
  {
    const int max_size = 64 * 1024 * 1024;
    var expected_bson = Regex.Replace(@"0x21000000 05 546865496400 10000000 04 551957ca 08f4 4a15 9a9f bc6453b3c28d 00", "\\s+", "");
    var bson_doc = bsonly.Test.Utils.HexConverter.HexStringToByteArray(expected_bson);

    var serializer = new Serialization.StreamWriter(max_size);
    serializer.WriteGuid("TheId"u8, Guid.Parse("551957ca-08f4-4a15-9a9f-bc6453b3c28d"));
    var actual = serializer.Finish();

    Assert.AreEqual(expected_bson, bsonly.Test.Utils.HexConverter.ByteArrayToHexString(actual));
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