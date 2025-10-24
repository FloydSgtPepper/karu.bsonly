using System.Diagnostics;
using karu.hexutil;
using System.Text.RegularExpressions;
using karu.bsonly.Serialization.Interface;


namespace karu.bsonly.Serialization.Test;

[TestClass]
public class TestObjectSerialization_new
{
  [TestInitialize]
  public void Initialize()
  {
  }
  internal class TestClassLevel2 : ISerializable
  {
    public string StrProperty = "level 2";

    public void Serialize(IDocumentSerializer writer)
    {
      Serializer.Serialize(writer, "StrLevel2"u8, StrProperty);
    }

    public void Deserialize(IDocumentDeserializer reader)
    {
      throw new NotImplementedException();
    }
  }

  internal class TestClassLevel1 : ISerializable
  {
    public string StrProperty = "inner class";
    public long LongProperty = 949430;

    public TestClassLevel2 Level2Tc = new();

    public void Serialize(IDocumentSerializer writer)
    {
      Serializer.Serialize(writer, "StrInner"u8, StrProperty);
      Serializer.Serialize(writer, "LongInner"u8, LongProperty);
      Serializer.Serialize(writer, "Level2"u8, Level2Tc);
    }

    public void Deserialize(IDocumentDeserializer reader)
    {
      throw new NotImplementedException();
    }
  }

  internal class TestClass : ISerializable
  {
    public string StrProperty = "this is a string";
    public TestClassLevel1 InnerClass = new();
    public long LongProperty = 900;

    public void Serialize(IDocumentSerializer writer)
    {
      Serializer.Serialize(writer, "StrProperty"u8, StrProperty);
      Serializer.Serialize(writer, "InnerClass"u8, InnerClass);
      //Serializer_new.Serialize(writer, "LongProperty"u8, LongProperty);
      writer.WriteLong("LongProperty"u8).WriteLong(LongProperty);
    }
    public void Deserialize(IDocumentDeserializer reader)
    {
      throw new NotImplementedException();
    }
  }

  internal class TestClassList : ISerializable
  {
    public List<long> ListData = new List<long> { 5, 6, 7 };

    public void Serialize(IDocumentSerializer writer)
    {
      Serializer.Serialize(writer, "ListData"u8, (object)ListData);
    }

    public void Deserialize(IDocumentDeserializer reader)
    {
      throw new NotImplementedException();
    }
  }

  private byte[] SerializeData()
  {
    var context = SerializationContext.Default;

    var writer = new StreamDocWriter(context);
    writer.WriteLong("long_val"u8).WriteLong(42L);
    writer.WriteString("string_val"u8).WriteString("foo bar"u8);

    var parameters = new List<object?> { Guid.Parse("b9fc8faa-8e08-4bc6-a5ff-c0f7495a3e2a"), new List<ulong> { 5, 7 } };

    var param_writer = writer.WriteArray("params"u8);
    for (int idx = 0; idx < parameters.Count; ++idx)
    {
      //var base_writer_array = param_writer.Write(BsonConstants.BSON_TYPE_INT64);
      Serializer.Serialize(writer, param_writer.NextKey(), parameters[idx]);
    }
    param_writer.Finish();

    return writer.Finish();
  }

  private byte[] BsonlySerializer_new<T>(StreamDocWriter serializer, T value) where T : ISerializable
  {
    value.Serialize(serializer);
    return serializer.Finish();
  }

  [TestMethod]
  public void WriteTestClass()
  {
    var expected_bson = Regex.Replace(
       @"0x9f000000
            02 53747250726f706572747900 11000000 74686973206973206120737472696e6700
            03 496e6e6572436c61737300   52000000
              02 537472496e6e657200     0c000000 696e6e657220636c61737300
              12 4c6f6e67496e6e657200   b67c0e0000000000
              03 4c6576656c3200 18000000
                02 5374724c6576656c3200 08000000 6c6576656c203200 
              00
            00
            12 4c6f6e6750726f706572747900 8403000000000000
           00", "\\s+", "");

    TestClass tc = new();
    var serializer = new StreamDocWriter(SerializationContext.Default);
    var bson_doc = BsonlySerializer.Serialize(tc, serializer);

    Debug.WriteLine($"actual: {HexConverter.ByteArrayToHexString(bson_doc)}");
    Assert.AreEqual(expected_bson, HexConverter.ByteArrayToHexString(bson_doc));
  }

  [TestMethod]
  public void WriteListOfLong()
  {
    var expected_bson = Regex.Replace(
       @"0x35000000
            04 4c6973744461746100 26000000
               12 3000 0500000000000000
               12 3100 0600000000000000
               12 3200 0700000000000000
            00
          00", "\\s+", "");

    var value = new TestClassList();
    var serializer = new StreamDocWriter(SerializationContext.Default);
    var bson_doc = BsonlySerializer.Serialize(value, serializer);

    Debug.WriteLine($"actual: {HexConverter.ByteArrayToHexString(bson_doc)}");
    Assert.AreEqual(expected_bson, HexConverter.ByteArrayToHexString(bson_doc));
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
