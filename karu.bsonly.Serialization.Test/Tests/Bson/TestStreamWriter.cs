using System.Diagnostics;
using karu.bsonly.Serialization;
using System.Text.RegularExpressions;
using karu.bsonly.Serialization.Interface;
using karu.hexutil;


namespace karu.bsonly.Serialization.Test;


[TestClass]
public class TestBasicWriter
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

    public void Serialize(IDocumentSerializer serializer)
    {
      Serializer.Serialize(serializer, "StrProperty"u8, StrProperty);
      Serializer.Serialize(serializer, "LongProperty"u8, LongProperty);
      Serializer.Serialize(serializer, "IntProperty"u8, IntProperty);
      Serializer.Serialize(serializer, "BoolProperty"u8, BoolProperty);
      Serializer.Serialize(serializer, "DoubleProperty"u8, DoubleProperty);
    }

    public void Deserialize(IDocumentDeserializer serializer)
    {
      throw new NotImplementedException();
    }
  }

  [TestMethod]
  public void WriteLong()
  {
    var expected_bson = Regex.Replace(@"0x15000000 12 61206c6f6e6700 2a00000000000000 00", "\\s+", "");

    var serializer = new StreamDocWriter(SerializationContext.Default);
    serializer.WriteLong("a long"u8).WriteLong(42L);
    var actual = serializer.Finish();

    Assert.AreEqual(expected_bson, HexConverter.ByteArrayToHexString(actual));
  }

  [TestMethod]
  public void WriteInt()
  {
    var expected_bson = Regex.Replace(@"0x10000000 10 6120696e7400 2a000000 00", "\\s+", "");

    var serializer = new StreamDocWriter(SerializationContext.Default);
    serializer.WriteInt("a int"u8).WriteInt(42);
    var actual = serializer.Finish();

    Assert.AreEqual(expected_bson, HexConverter.ByteArrayToHexString(actual));
  }

  [TestMethod]
  public void WriteBool()
  {
    var expected_bson = Regex.Replace(@"0x0e000000 08 6120626f6f6c00 01 00", "\\s+", "");

    var serializer = new StreamDocWriter(SerializationContext.Default);
    serializer.WriteBool("a bool"u8).WriteBool(true);
    var actual = serializer.Finish();

    Debug.WriteLine($"actual: {HexConverter.ByteArrayToHexString(actual)}");
    Assert.AreEqual(expected_bson, HexConverter.ByteArrayToHexString(actual));
  }


  [TestMethod]
  public void WriteString()
  {
    var expected_bson = Regex.Replace(@"0x24000000 02 6120737472696e6700 11000000 74686973206973206120737472696e6700 00", "\\s+", "");

    var serializer = new StreamDocWriter(SerializationContext.Default);
    serializer.WriteString("a string"u8).WriteString("this is a string"u8);
    var actual = serializer.Finish();

    Debug.WriteLine($"actual: {HexConverter.ByteArrayToHexString(actual)}");
    Assert.AreEqual(expected_bson, HexConverter.ByteArrayToHexString(actual));
  }

  [TestMethod]
  public void WriteDouble()
  {
    var expected_bson = Regex.Replace(@"0x17000000 01 6120646f75626c6500 1f85eb51b81e0940 00", "\\s+", "");

    var serializer = new StreamDocWriter(SerializationContext.Default);
    serializer.WriteDouble("a double"u8).WriteDouble(3.14);
    var actual = serializer.Finish();

    Debug.WriteLine($"actual: {HexConverter.ByteArrayToHexString(actual)}");
    Assert.AreEqual(expected_bson, HexConverter.ByteArrayToHexString(actual));
  }

  [TestMethod]
  public void WriteNull()
  {
    var expected_bson = Regex.Replace(@"0x0d000000 0a 61206e756c6c00 00", "\\s+", "");

    var serializer = new StreamDocWriter(SerializationContext.Default);
    serializer.WriteNull("a null"u8);
    var actual = serializer.Finish();

    Debug.WriteLine($"actual: {HexConverter.ByteArrayToHexString(actual)}");
    Assert.AreEqual(expected_bson, HexConverter.ByteArrayToHexString(actual));
  }

  // [TestMethod]
  // public void WriteBinary()
  // {
  //   var expected_bson = Regex.Replace(@"0x18000000 05 612062696e61727900 04000000 58 01020304 00", "\\s+", "");

  //   var serializer = new StreamDocWriter(SerializationContext.Default);
  //   serializer.WriteBinary("a binary"u8, new byte[] { 1, 2, 3, 4 }, 88);
  //   var actual = serializer.Finish();

  //   Debug.WriteLine($"actual: {actual}");
  //   Assert.AreEqual(expected_bson, HexConverter.ByteArrayToHexString(actual));
  // }

  // [TestMethod]
  // public void WriteGuid()
  // {
  //   var expected_bson = Regex.Replace(@"0x21000000 05 546865496400 10000000 04 551957ca 08f4 4a15 9a9f bc6453b3c28d 00", "\\s+", "");
  //   var bson_doc = HexConverter.HexStringToByteArray(expected_bson);

  //   var serializer = new StreamDocWriter(SerializationContext.Default);
  //   serializer.WriteGuid("TheId"u8, Guid.Parse("551957ca-08f4-4a15-9a9f-bc6453b3c28d"));
  //   var actual = serializer.Finish();

  //   Assert.AreEqual(expected_bson, HexConverter.ByteArrayToHexString(actual));
  // }

  // [TestMethod]
  // public void WriteDocument()
  // {
  //   var expected_bson = Regex.Replace(@"0x7e000000 03 6120646f6375656d6e7400 6d000000
  //                                                                02 53747250726f706572747900 09000000 6120737472696e6700
  //                                                                12 4c6f6e6750726f706572747900 1100000000000000
  //                                                                10 496e7450726f706572747900 d0070000
  //                                                                08 426f6f6c50726f706572747900 00
  //                                                                01 446f75626c6550726f706572747900 0000000000c06540
  //                                                           00
  //                                               00", "\\s+", "");
  //   var bson_doc = HexConverter.HexStringToByteArray(expected_bson);

  //   var serializer = new StreamDocWriter(SerializationContext.Default);
  //   var tc = new TestClass
  //   {
  //     StrProperty = "a string",
  //     LongProperty = 17,
  //     IntProperty = 2000,
  //     BoolProperty = false,
  //     DoubleProperty = 174
  //   };

  //   serializer.WriteDocument<TestClass>("a docuemnt"u8, tc);
  //   var actual = serializer.Finish();

  //   Debug.WriteLine($"actual: {actual}");
  //   Assert.AreEqual(expected_bson, HexConverter.ByteArrayToHexString(actual));
  // }

  // [TestMethod]
  // public void WriteRawDocument()
  // {
  //   var expected_bson = Regex.Replace(@"0x19000000 03 6120646f6375656d6e7400 03000000 031301 00 00", "\\s+", "");
  //   var bson_doc = HexConverter.HexStringToByteArray(expected_bson);

  //   var serializer = new StreamDocWriter(SerializationContext.Default);

  //   serializer.WriteRawDocument("a docuemnt"u8, [3, 19, 1]);
  //   var actual = serializer.Finish();

  //   Debug.WriteLine($"actual: {actual}");
  //   Assert.AreEqual(expected_bson, HexConverter.ByteArrayToHexString(actual));
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