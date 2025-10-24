using System.Collections.Specialized;
using System.Diagnostics;
using karu.bsonly.Serialization;
using karu.bsonly.Serialization.Interface;
using karu.hexutil;
using System.Text.RegularExpressions;
using System.Text;

namespace karu.bsonly.Serialization.Test;


[TestClass]
public class TestMemoryDocReader
{

  DeserializationContext _context = new DeserializationContext();

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

    public void Serialize(IDocumentSerializer writer)
    {
    }

    public void Deserialize(IDocumentDeserializer reader)
    {
      if (reader.HasEntry("StrProperty"u8, BsonConstants.BSON_TYPE_UTF8))
        StrProperty = Encoding.UTF8.GetString(reader.ReadString());
      if (reader.HasEntry("LongProperty"u8, BsonConstants.BSON_TYPE_INT64))
        LongProperty = reader.ReadLong();
      if (reader.HasEntry("IntProperty"u8, BsonConstants.BSON_TYPE_INT32))
        IntProperty = reader.ReadInt();
      if (reader.HasEntry("BoolProperty"u8, BsonConstants.BSON_TYPE_BOOL))
        BoolProperty = reader.ReadBool();
      if (reader.HasEntry("DoubleProperty"u8, BsonConstants.BSON_TYPE_DOUBLE))
        DoubleProperty = reader.ReadDouble();
    }
  }

  [TestMethod]
  public void InitializationTest1()
  {
    var bson_str = "0x12000000 02 62617200 04000000 666f6f00 00".Replace(" ", "");
    var bson_doc = HexConverter.HexStringToByteArray(bson_str);
    // ctor must not throw
    var serializer = new MemoryDocReader(bson_doc, DeserializationContext.Default);
    Assert.IsTrue(serializer.HasEntry("bar"u8, BsonConstants.BSON_TYPE_UTF8));

    // minimal size - empty doc - not throwing
    bson_str = "0x05000000 00".Replace(" ", "");
    bson_doc = HexConverter.HexStringToByteArray(bson_str);
    serializer = new MemoryDocReader(bson_doc, DeserializationContext.Default); // must not throw

    var (key, type) = serializer.NextEntry();
    Assert.AreEqual(BsonConstants.BSON_TYPE_EOD, type);
    Assert.IsTrue(key.IsEmpty);
  }

  [TestMethod]
  public void InitializationTestWrongDocSize()
  {
    // wrong doc size - must throw exception
    var bson_str = "0x10000000 02 62617200 04000000 666f6f00 00".Replace(" ", "");
    var bson_doc = HexConverter.HexStringToByteArray(bson_str);
    // ctor must not throw
    Assert.ThrowsExactly<ArgumentException>(() => new MemoryDocReader(bson_doc, DeserializationContext.Default));
  }

  [TestMethod]
  public void InitializationTestDocTooSmall()
  {
    // doc too small -- must throw
    var bson_str = "0x04000000";
    var bson_doc = HexConverter.HexStringToByteArray(bson_str);
    Assert.ThrowsExactly<ArgumentException>(() => new MemoryDocReader(bson_doc, DeserializationContext.Default));
  }

  [TestMethod]
  public void TwoElementsTest()
  {
    var bson_str = "0x1f000000 02 62617200 04000000 666f6f00 02 61617200 04000000 6f6f6f00 00".Replace(" ", "");
    var bson_doc = HexConverter.HexStringToByteArray(bson_str);

    var serializer = new MemoryDocReader(bson_doc, DeserializationContext.Default);

    Assert.IsTrue(serializer.HasEntry("bar"u8, BsonConstants.BSON_TYPE_UTF8));
    Assert.IsTrue(serializer.ReadString().SequenceEqual("foo"u8));

    Assert.IsTrue(serializer.HasEntry("aar"u8, BsonConstants.BSON_TYPE_UTF8));
    Assert.IsTrue(serializer.ReadString().SequenceEqual("ooo"u8));

    serializer = new MemoryDocReader(bson_doc, DeserializationContext.Default);
    Assert.IsTrue(serializer.SkipEntry("bar"u8));
    Assert.IsTrue(serializer.HasEntry("aar"u8, BsonConstants.BSON_TYPE_UTF8));
    Assert.IsTrue(serializer.ReadString().SequenceEqual("ooo"u8));

  }

  [TestMethod]
  public void BasicDeserialization()
  {
    var tc = new TestClass
    {
      StrProperty = "prop1",
      LongProperty = 17,
      IntProperty = 42,
      BoolProperty = true,
      DoubleProperty = 3.14
    };
    var bson_doc = HexConverter.HexStringToByteArray(Regex.Replace(@"6a000000 
        02 53747250726f7065727479000 6000000 70726f703100
        12 4c6f6e6750726f706572747900 1100000000000000
        10 496e7450726f706572747900 2a000000
        08 426f6f6c50726f706572747900 01
        01 446f75626c6550726f706572747900 1f85eb51b81e0940
        00", "\\s+", ""));
    var serializer = new MemoryDocReader(bson_doc, DeserializationContext.Default);

    Assert.IsTrue(serializer.HasEntry("StrProperty"u8, BsonConstants.BSON_TYPE_UTF8));
    Assert.IsTrue(serializer.ReadString().SequenceEqual("prop1"u8));

    Assert.IsTrue(serializer.HasEntry("LongProperty"u8, BsonConstants.BSON_TYPE_INT64));
    Assert.AreEqual(17, serializer.ReadLong());

    Assert.IsTrue(serializer.HasEntry("IntProperty"u8, BsonConstants.BSON_TYPE_INT32));
    Assert.AreEqual(42, serializer.ReadInt());

    Assert.IsTrue(serializer.HasEntry("BoolProperty"u8, BsonConstants.BSON_TYPE_BOOL));
    Assert.IsTrue(serializer.ReadBool());

    Assert.IsTrue(serializer.HasEntry("DoubleProperty"u8, BsonConstants.BSON_TYPE_DOUBLE));
    Assert.AreEqual(3.14, serializer.ReadDouble());
  }

  [TestMethod]
  public void OutOfOrderDeserialization()
  {
    var tc = new TestClass
    {
      StrProperty = "prop1",
      LongProperty = 17,
      IntProperty = 42,
      BoolProperty = true,
      DoubleProperty = 3.14
    };

    var bson_doc = HexConverter.HexStringToByteArray(Regex.Replace(@"6a000000 
        02 53747250726f7065727479000 6000000 70726f703100
        12 4c6f6e6750726f706572747900 1100000000000000
        10 496e7450726f706572747900 2a000000
        08 426f6f6c50726f706572747900 01
        01 446f75626c6550726f706572747900 1f85eb51b81e0940
        00", "\\s+", ""));

    var OutOfOrderEvaluation = DeserializationContext.Default;
    OutOfOrderEvaluation.Configuration.OutOfOrderEvaluation = true;
    var serializer = new MemoryDocReader(bson_doc, OutOfOrderEvaluation);


    Assert.IsTrue(serializer.HasEntry("DoubleProperty"u8, BsonConstants.BSON_TYPE_DOUBLE));
    Assert.AreEqual(3.14, serializer.ReadDouble());

    Assert.IsTrue(serializer.HasEntry("LongProperty"u8, BsonConstants.BSON_TYPE_INT64));
    Assert.AreEqual(17, serializer.ReadLong());

    Assert.IsTrue(serializer.HasEntry("IntProperty"u8, BsonConstants.BSON_TYPE_INT32));
    Assert.AreEqual(42, serializer.ReadInt());

    Assert.IsTrue(serializer.HasEntry("StrProperty"u8, BsonConstants.BSON_TYPE_UTF8));
    Assert.IsTrue(serializer.ReadString().SequenceEqual("prop1"u8));

    Assert.IsTrue(serializer.HasEntry("BoolProperty"u8, BsonConstants.BSON_TYPE_BOOL));
    Assert.IsTrue(serializer.ReadBool());


    serializer = new MemoryDocReader(bson_doc, DeserializationContext.Default);
    Assert.IsFalse(serializer.HasEntry("StrProperty"u8, BsonConstants.BSON_TYPE_DOUBLE));

    serializer = new MemoryDocReader(bson_doc, OutOfOrderEvaluation);
    Assert.IsFalse(serializer.HasEntry("foobar"u8, BsonConstants.BSON_TYPE_DOUBLE));

  }

  [TestMethod]
  public void EmptyKeyStringForInOrderEvaluation()
  {
    var bson_str = "0x19000000 02 00 04000000 666f6f00 02 00 04000000 6f6f6f00 00".Replace(" ", "");
    var bson_doc = HexConverter.HexStringToByteArray(bson_str);

    var serializer = new MemoryDocReader(bson_doc, DeserializationContext.Default);

    Assert.IsTrue(serializer.HasEntry(ReadOnlySpan<byte>.Empty, BsonConstants.BSON_TYPE_UTF8));
    Assert.IsTrue(serializer.ReadString().SequenceEqual("foo"u8));

    Assert.IsTrue(serializer.HasEntry(ReadOnlySpan<byte>.Empty, BsonConstants.BSON_TYPE_UTF8));
    Assert.IsTrue(serializer.ReadString().SequenceEqual("ooo"u8));
  }

  [TestMethod]
  public void ReadLong()
  {
    var expected_bson = Regex.Replace(@"0x15000000 12 61206c6f6e6700 2a00000000000000 00", "\\s+", "");
    var bson_doc = HexConverter.HexStringToByteArray(expected_bson);

    var serializer = new MemoryDocReader(bson_doc, DeserializationContext.Default);

    Assert.IsTrue(serializer.HasEntry("a long"u8, BsonConstants.BSON_TYPE_INT64));
    Assert.AreEqual(42L, serializer.ReadLong());
  }

  [TestMethod]
  public void ReadInt()
  {
    var expected_bson = Regex.Replace(@"0x10000000 10 6120696e7400 2b000000 00", "\\s+", "");
    var bson_doc = HexConverter.HexStringToByteArray(expected_bson);

    var serializer = new MemoryDocReader(bson_doc, DeserializationContext.Default);

    Assert.IsTrue(serializer.HasEntry("a int"u8, BsonConstants.BSON_TYPE_INT32));
    Assert.AreEqual(43L, serializer.ReadInt());
  }

  [TestMethod]
  public void ReadBool()
  {
    var expected_bson = Regex.Replace(@"0x0e000000 08 6120626f6f6c00 01 00", "\\s+", "");
    var bson_doc = HexConverter.HexStringToByteArray(expected_bson);

    var serializer = new MemoryDocReader(bson_doc, DeserializationContext.Default);

    Assert.IsTrue(serializer.HasEntry("a bool"u8, BsonConstants.BSON_TYPE_BOOL));
    Assert.IsTrue(serializer.ReadBool());
  }


  [TestMethod]
  public void ReadString()
  {
    var expected_bson = Regex.Replace(@"0x24000000 02 6120737472696e6700 11000000 74686973206973206120737472696e6700 00", "\\s+", "");
    var bson_doc = HexConverter.HexStringToByteArray(expected_bson);

    var serializer = new MemoryDocReader(bson_doc, DeserializationContext.Default);

    Assert.IsTrue(serializer.HasEntry("a string"u8, BsonConstants.BSON_TYPE_UTF8));
    Assert.IsTrue("this is a string"u8.SequenceEqual(serializer.ReadString()));
  }

  [TestMethod]
  public void ReadDouble()
  {
    var expected_bson = Regex.Replace(@"0x17000000 01 6120646f75626c6500 1f85eb51b81e0940 00", "\\s+", "");
    var bson_doc = HexConverter.HexStringToByteArray(expected_bson);

    var serializer = new MemoryDocReader(bson_doc, DeserializationContext.Default);

    Assert.IsTrue(serializer.HasEntry("a double"u8, BsonConstants.BSON_TYPE_DOUBLE));
    Assert.AreEqual(3.14, serializer.ReadDouble());
  }

  [TestMethod]
  public void ReadNull()
  {
    var expected_bson = Regex.Replace(@"0x0e000000 0a 61206e756c6c00 00 00", "\\s+", "");
    var bson_doc = HexConverter.HexStringToByteArray(expected_bson);

    var serializer = new MemoryDocReader(bson_doc, DeserializationContext.Default);

    Assert.IsTrue(serializer.HasEntry("a null"u8, BsonConstants.BSON_TYPE_NULL));
    serializer.ReadNull();
  }

  [TestMethod]
  public void ReadBinary()
  {
    var expected_bson = Regex.Replace(@"0x18000000 05 612062696e61727900 04000000 58 01020304 00", "\\s+", "");
    var bson_doc = HexConverter.HexStringToByteArray(expected_bson);

    var serializer = new MemoryDocReader(bson_doc, DeserializationContext.Default);

    Assert.IsTrue(serializer.HasEntry("a binary"u8, BsonConstants.BSON_TYPE_BINARY));
    Assert.AreEqual(88, serializer.BinarySubType());
    Assert.IsTrue(serializer.ReadBinary().SequenceEqual(new byte[] { 1, 2, 3, 4 }));
  }

  [TestMethod]
  public void ReadRawBinary()
  {
    var expected_bson = Regex.Replace(@"0x18000000 05 612062696e61727900 04000000 58 01020304 00", "\\s+", "");
    var bson_doc = HexConverter.HexStringToByteArray(expected_bson);

    var serializer = new MemoryDocReader(bson_doc, DeserializationContext.Default);

    Assert.IsTrue(serializer.HasEntry("a binary"u8, BsonConstants.BSON_TYPE_BINARY));
    Assert.IsTrue(serializer.ReadRawBinary().SequenceEqual(new byte[] { 88, 1, 2, 3, 4 }));
  }

  [TestMethod]
  public void ReadRawDocument()
  {
    var expected_bson = Regex.Replace(@"0x19000000 03 6120646f63756d656e7400 08000000 031301 00 00", "\\s+", "");
    var bson_doc = HexConverter.HexStringToByteArray(expected_bson);

    var serializer = new MemoryDocReader(bson_doc, DeserializationContext.Default);

    Assert.IsTrue(serializer.HasEntry("a document"u8, BsonConstants.BSON_TYPE_DOCUMENT));
    Assert.IsTrue(serializer.ReadRawDocument().SequenceEqual(new byte[] { 3, 19, 1 }));
  }

  [TestMethod]
  public void ReadDocument()
  {
    var tc_ex = new TestClass
    {
      StrProperty = "a string",
      LongProperty = 17,
      IntProperty = 2000,
      BoolProperty = false,
      DoubleProperty = 174
    };

    var expected_bson = Regex.Replace(@"0x7e000000 03 6120646f63756d656e7400 6d000000
                                                                 02 53747250726f706572747900 09000000 6120737472696e6700
                                                                 12 4c6f6e6750726f706572747900 1100000000000000
                                                                 10 496e7450726f706572747900 d0070000
                                                                 08 426f6f6c50726f706572747900 00
                                                                 01 446f75626c6550726f706572747900 0000000000c06540
                                                            00
                                                00", "\\s+", "");
    var bson_doc = HexConverter.HexStringToByteArray(expected_bson);

    var deserializer = new MemoryDocReader(bson_doc, DeserializationContext.Default);

    Assert.IsTrue(deserializer.HasEntry("a document"u8, BsonConstants.BSON_TYPE_DOCUMENT));
    var doc_deserializer = deserializer.DocumentReader();
    var (key, type) = doc_deserializer.NextEntry();
    Assert.IsTrue(key.Span.SequenceEqual("StrProperty"u8));
    Assert.AreEqual(BsonConstants.BSON_TYPE_UTF8, type);

    doc_deserializer.Finish(); // must assert
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