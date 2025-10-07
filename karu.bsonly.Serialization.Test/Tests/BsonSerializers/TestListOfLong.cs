using System.Diagnostics;
using karu.bsonly.Serialization;
using System.Text.RegularExpressions;
using karu.bsonly.Serialization.Interface;
using System.Net;


namespace karu.bsonly.Serialization.Test;


[TestClass]
public class TestListOfLong
{
  DeserializationContext _deser_context = new DeserializationContext();

  SerializationContext _ser_context = new SerializationContext();

  [TestInitialize]
  public void Initialize()
  {
  }

  internal class TestClass1 : ISerializable
  {
    public long LongProperty = 0;
    public List<ulong>? ListOfLong = null;
    public int IntProperty = 0;

    public void Serialize(IBaseSerializer serializer, SerializationContext context)
    {
      Serializer.Serialize(serializer, "LongProperty"u8, LongProperty);
      if (ListOfLong != null)
        Serializer.Serialize(serializer, "ListOfLong"u8, ListOfLong);
      Serializer.Serialize(serializer, "IntProperty"u8, IntProperty);
    }

    public void Deserialize(IBaseDeserializer serializer, DeserializationContext context)
    {
      LongProperty = Serializer.SerializeLong(serializer, "LongProperty"u8);
      ListOfLong = Serializer.SerializeListOfULong(serializer, "ListOfLong"u8, context);
      IntProperty = Serializer.SerializeInt(serializer, "IntProperty"u8);
    }
  }

  internal class TestClass2 : ISerializable
  {
    public long LongProperty = 0;
    public List<ulong>? ListOfLong = null;
    public int IntProperty = 0;

    public void Serialize(IBaseSerializer serializer, SerializationContext context)
    {
      Serializer.Serialize(serializer, "LongProperty"u8, LongProperty);
      if (ListOfLong != null)
        Serializer.Serialize(serializer, "ListOfLong"u8, ListOfLong);
      Serializer.Serialize(serializer, "IntProperty"u8, IntProperty);
    }

    public void Deserialize(IBaseDeserializer deserializer, DeserializationContext context)
    {
      if (deserializer.HasEntry("LongProperty"u8, BsonConstants.BSON_TYPE_INT64))
        LongProperty = deserializer.ReadLong();
      if (deserializer.HasEntry("ListOfLong"u8) != BsonConstants.BSON_TYPE_EOD)
        ListOfLong = Serializer.SerializeListOfULong(deserializer, "ListOfLong"u8, context);
      if (deserializer.HasEntry("IntProperty"u8, BsonConstants.BSON_TYPE_INT32))
        IntProperty = deserializer.ReadInt();
    }
  }

  [TestMethod]
  public void ReadListULong()
  {
    // 0x5e000000 12 4c6f6e6750726f706572747900 f910000000000000 
    //            04 4c6973744f664c6f6e6700 26000000 
    //               12 3000 0100000000000000
    //               12 3100 0200000000000000
    //               12 3200 0300000000000000
    //            00
    //            10 496e7450726f706572747900 13000000
    //         00

    var expected_tc1 = new TestClass1
    {
      LongProperty = 4345,
      ListOfLong = new List<ulong> { 1, 2, 3 },
      IntProperty = 19
    };
    var expected_tc2 = new TestClass2
    {
      LongProperty = 4345,
      ListOfLong = new List<ulong> { 1, 2, 3 },
      IntProperty = 19
    };

    var bson_doc1 = ApiSerializer.Serialize(expected_tc1, _ser_context);
    Debug.WriteLine($"   {Utils.HexConverter.ByteArrayToHexString(bson_doc1)} ");
    var actual_tc1 = ApiSerializer.Deserialize<TestClass1>(bson_doc1, _deser_context);
    Assert.AreEqual(expected_tc1.LongProperty, actual_tc1.LongProperty);
    Assert.AreEqual(expected_tc1.ListOfLong, actual_tc1.ListOfLong);
    Assert.AreEqual(expected_tc1.IntProperty, actual_tc1.IntProperty);

    var bson_doc2 = ApiSerializer.Serialize(expected_tc2, _ser_context);
    Debug.WriteLine($"   {Utils.HexConverter.ByteArrayToHexString(bson_doc2)} ");
    var actual_tc2 = ApiSerializer.Deserialize<TestClass1>(bson_doc2, _deser_context);
    Assert.AreEqual(expected_tc2.LongProperty, actual_tc2.LongProperty);
    Assert.AreEqual(expected_tc2.ListOfLong, actual_tc2.ListOfLong);
    Assert.AreEqual(expected_tc2.IntProperty, actual_tc2.IntProperty);
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