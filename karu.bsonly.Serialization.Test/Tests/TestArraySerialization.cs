using System.Diagnostics;
using karu.bsonly.Serialization;
using System.Text.RegularExpressions;
using karu.bsonly.Serialization.Interface;


namespace karu.bsonly.Serialization.Test;


[TestClass]
public class TestArraySerialization
{
  private SerializationContext _context = new();

  internal class TcInnerClass
  {
    public string Str = string.Empty;

    public long LongValue;
  }

  internal class ListOfIntsSerialization : ISerializationProvider
  {
    public void SerializationFunction(IBaseSerializer serializer, ReadOnlySpan<byte> key, object value, Type type, SerializationContext context)
    {
      var list_of_int = value as List<int>;
      var array_serializer = serializer.SerializeArray(key);
      if (list_of_int != null)
      {
        for (int idx = 0; idx < list_of_int.Count; ++idx)
        {
          array_serializer.Add(list_of_int[idx]);
        }
        array_serializer.Finish();

        return;
      }

      throw new ArgumentException($"value must be List<int> but was {value.GetType()}");
    }

    public void DeserializationFunction(IBaseDeserializer serializer, ref object? value, Type type, DeserializationContext context)
    {
      throw new NotImplementedException();
    }
  }

  internal class TcSerialization : ISerializationProvider
  {
    public void SerializationFunction(IBaseSerializer serializer, ReadOnlySpan<byte> key, object object_value, Type type, SerializationContext context)
    {
      var value = object_value as TcInnerClass;
      if (value != null)
      {
        Serializer.Serialize(serializer, "Str"u8, value.Str);
        Serializer.Serialize(serializer, "LongValue"u8, value.LongValue);
        return;
      }

      throw new ArgumentException($"value must be {typeof(TcInnerClass)} but was {object_value.GetType()}");
    }

    public void DeserializationFunction(IBaseDeserializer serializer, ref object? value, Type type, DeserializationContext context)
    {
      throw new NotImplementedException();
    }
  }


  internal class TestClass : ISerializable
  {
    public long LongProperty = 0;
    public List<object> ListOfObjects = new();
    public int IntProperty = 0;

    public void Serialize(IBaseSerializer serializer, SerializationContext context)
    {
      Serializer.Serialize(serializer, "LongProperty"u8, LongProperty);
      var array_serializer = serializer.SerializeArray("ListOfObjects"u8);
      for (int idx = 0; idx < ListOfObjects.Count; ++idx)
      {
        Serializer.Serialize(array_serializer, ListOfObjects[idx], context);
      }
      array_serializer.Finish();
      Serializer.Serialize(serializer, "IntProperty"u8, IntProperty);
    }

    public void Deserialize(IBaseDeserializer serializer, DeserializationContext context)
    {
      throw new NotImplementedException();
    }
  }

  [TestInitialize]
  public void Initialize()
  {
    _context.SerializationRegistry = new SerializationRegistry();
    _context.Configuration = new BsonSettings
    {
      OutOfOrderEvaluation = false,
      MaxSize = 640 * 1024,
      Format = Format.BSON,
      Arrays = Arrays.COMPLIANT
    };

    _context.SerializationRegistry.Register(typeof(List<int>), new ListOfIntsSerialization());
    _context.SerializationRegistry.Register(typeof(TcInnerClass), new TcSerialization());
  }

  [TestMethod]
  public void WriteListOfObjectsListOfLong()
  {
    var expected_bson = Regex.Replace(@"
    0x8e000000
      12 4c6f6e6750726f706572747900 1100000000000000
      04 4c6973744f664f626a6563747300 53000000
        04 3000 1a000000 
          10 3000 05000000
          10 3100 03000000
          10 3200 04000000
        00 
        08 3100 01
        02 53747200 11000000 74686973206973206120737472696e6700124c6f6e6756616c756500
        12 0000000000000000
      10 496e7450726f706572747900 01110000
    00", "\\s+", "");

    var ListOfInts = new List<int> { 5, 3, 4 };
    var BoolValue = true;
    var AClass = new TcInnerClass
    {
      Str = "this is a string",
      LongValue = 18
    };
    var tc = new TestClass
    {
      LongProperty = 17,
      ListOfObjects = new(),
      IntProperty = 4353
    };

    tc.ListOfObjects.Add(ListOfInts);
    tc.ListOfObjects.Add(BoolValue);
    tc.ListOfObjects.Add(AClass);

    var bson_doc = ApiSerializer.Serialize(tc, _context);

    Assert.AreEqual(expected_bson, Utils.HexConverter.ByteArrayToHexString(bson_doc));
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