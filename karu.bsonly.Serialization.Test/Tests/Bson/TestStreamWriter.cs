using System.Diagnostics;
using karu.bsonly.Serialization;
using System.Text.RegularExpressions;
using karu.bsonly.Serialization.Interface;


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

    public void Serialize(IBaseSerializer serializer, SerializationContext context)
    {
      Serializer.Serialize(serializer, "StrProperty"u8, StrProperty);
      Serializer.Serialize(serializer, "LongProperty"u8, LongProperty);
      Serializer.Serialize(serializer, "IntProperty"u8, IntProperty);
      Serializer.Serialize(serializer, "BoolProperty"u8, BoolProperty);
      Serializer.Serialize(serializer, "DoubleProperty"u8, DoubleProperty);
    }

    public void Deserialize(IBaseDeserializer serializer, DeserializationContext context)
    {
      throw new NotImplementedException();
    }
  }

  [TestMethod]
  public void WriteLong()
  {
    const int max_size = 64 * 1024 * 1024;
    var expected_bson = Regex.Replace(@"0x15000000 12 61206c6f6e6700 2a00000000000000 00", "\\s+", "");

    var serializer = new StreamWriter(max_size);
    serializer.WriteLong("a long"u8, 42L);
    var actual = serializer.Finish();

    Assert.AreEqual(expected_bson, Utils.HexConverter.ByteArrayToHexString(actual));
  }

  [TestMethod]
  public void WriteInt()
  {
    const int max_size = 64 * 1024 * 1024;
    var expected_bson = Regex.Replace(@"0x10000000 10 6120696e7400 2a000000 00", "\\s+", "");

    var serializer = new StreamWriter(max_size);
    serializer.WriteInt("a int"u8, 42);
    var actual = serializer.Finish();

    Debug.WriteLine($"actual: {actual}");
    Assert.AreEqual(expected_bson, Utils.HexConverter.ByteArrayToHexString(actual));
  }

  [TestMethod]
  public void WriteBool()
  {
    const int max_size = 64 * 1024 * 1024;
    var expected_bson = Regex.Replace(@"0x0e000000 08 6120626f6f6c00 01 00", "\\s+", "");

    var serializer = new StreamWriter(max_size);
    serializer.WriteBool("a bool"u8, true);
    var actual = serializer.Finish();

    Debug.WriteLine($"actual: {actual}");
    Assert.AreEqual(expected_bson, Utils.HexConverter.ByteArrayToHexString(actual));
  }


  [TestMethod]
  public void WriteString()
  {
    const int max_size = 64 * 1024 * 1024;
    var expected_bson = Regex.Replace(@"0x24000000 02 6120737472696e6700 11000000 74686973206973206120737472696e6700 00", "\\s+", "");

    var serializer = new StreamWriter(max_size);
    serializer.WriteString("a string"u8, "this is a string"u8);
    var actual = serializer.Finish();

    Debug.WriteLine($"actual: {actual}");
    Assert.AreEqual(expected_bson, Utils.HexConverter.ByteArrayToHexString(actual));
  }

  [TestMethod]
  public void WriteDouble()
  {
    const int max_size = 64 * 1024 * 1024;
    var expected_bson = Regex.Replace(@"0x17000000 01 6120646f75626c6500 1f85eb51b81e0940 00", "\\s+", "");

    var serializer = new StreamWriter(max_size);
    serializer.WriteDouble("a double"u8, 3.14);
    var actual = serializer.Finish();

    Debug.WriteLine($"actual: {actual}");
    Assert.AreEqual(expected_bson, Utils.HexConverter.ByteArrayToHexString(actual));
  }

  [TestMethod]
  public void WriteNull()
  {
    const int max_size = 64 * 1024 * 1024;
    var expected_bson = Regex.Replace(@"0x0e000000 0a 61206e756c6c00 00 00", "\\s+", "");

    var serializer = new StreamWriter(max_size);
    serializer.WriteNull("a null"u8);
    var actual = serializer.Finish();

    Debug.WriteLine($"actual: {actual}");
    Assert.AreEqual(expected_bson, Utils.HexConverter.ByteArrayToHexString(actual));
  }

  [TestMethod]
  public void WriteBinary()
  {
    const int max_size = 64 * 1024 * 1024;
    var expected_bson = Regex.Replace(@"0x18000000 05 612062696e61727900 04000000 58 01020304 00", "\\s+", "");

    var serializer = new StreamWriter(max_size);
    serializer.WriteBinary("a binary"u8, new byte[] { 1, 2, 3, 4 }, 88);
    var actual = serializer.Finish();

    Debug.WriteLine($"actual: {actual}");
    Assert.AreEqual(expected_bson, Utils.HexConverter.ByteArrayToHexString(actual));
  }

  [TestMethod]
  public void WriteGuid()
  {
    const int max_size = 64 * 1024 * 1024;
    var expected_bson = Regex.Replace(@"0x21000000 05 546865496400 10000000 04 551957ca 08f4 4a15 9a9f bc6453b3c28d 00", "\\s+", "");
    var bson_doc = Utils.HexConverter.HexStringToByteArray(expected_bson);

    var serializer = new karu.bsonly.Serialization.StreamWriter(max_size);
    serializer.WriteGuid("TheId"u8, Guid.Parse("551957ca-08f4-4a15-9a9f-bc6453b3c28d"));
    var actual = serializer.Finish();

    Assert.AreEqual(expected_bson, Utils.HexConverter.ByteArrayToHexString(actual));
  }

  [TestMethod]
  public void WriteDocument()
  {
    const int max_size = 64 * 1024 * 1024;
    var expected_bson = Regex.Replace(@"0x7e000000 03 6120646f6375656d6e7400 6d000000
                                                                 02 53747250726f706572747900 09000000 6120737472696e6700
                                                                 12 4c6f6e6750726f706572747900 1100000000000000
                                                                 10 496e7450726f706572747900 d0070000
                                                                 08 426f6f6c50726f706572747900 00
                                                                 01 446f75626c6550726f706572747900 0000000000c06540
                                                            00
                                                00", "\\s+", "");
    var bson_doc = Utils.HexConverter.HexStringToByteArray(expected_bson);

    var serializer = new StreamWriter(max_size);

    var tc = new TestClass
    {
      StrProperty = "a string",
      LongProperty = 17,
      IntProperty = 2000,
      BoolProperty = false,
      DoubleProperty = 174
    };

    serializer.WriteDocument<TestClass>("a docuemnt"u8, tc);
    var actual = serializer.Finish();

    Debug.WriteLine($"actual: {actual}");
    Assert.AreEqual(expected_bson, Utils.HexConverter.ByteArrayToHexString(actual));
  }

  [TestMethod]
  public void WriteRawDocument()
  {
    const int max_size = 64 * 1024 * 1024;
    var expected_bson = Regex.Replace(@"0x19000000 03 6120646f6375656d6e7400 03000000 031301 00 00", "\\s+", "");
    var bson_doc = Utils.HexConverter.HexStringToByteArray(expected_bson);

    var serializer = new StreamWriter(max_size);

    serializer.WriteRawDocument("a docuemnt"u8, [3, 19, 1]);
    var actual = serializer.Finish();

    Debug.WriteLine($"actual: {actual}");
    Assert.AreEqual(expected_bson, Utils.HexConverter.ByteArrayToHexString(actual));
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