// using System.Collections.Specialized;
// using System.Diagnostics;
// using karu.bsonly.Serialization;
// using karu.bsonly.Serialization.Interface;
// using karu.bsonly.Serialization.Test.Utils;

// namespace karu.bsonly.Serialization.Test;


// [TestClass]
// public class TestArrayReader
// {
//   // internal class TestClass
//   // {
//   //   public string StrProperty = string.Empty;
//   //   public long LongProperty = 0;
//   //   public int IntProperty = 0;

//   //   public void Deserialize(karu.bsonly.Serialization.Reader reader)
//   //   {
//   //     reader.serialize("StrProperty", ref StrProperty);
//   //     reader.serialize("LongProperty", ref LongProperty);
//   //     reader.serialize("IntProperty", ref IntProperty);
//   //   }
//   // }

//   internal class TestClassInts
//   {
//     public List<int> list1 = new();
//   }

//   internal class TestClassBool
//   {
//     public List<bool> list1 = new();
//   }


//   [TestInitialize]
//   public void Initialize()
//   {
//   }

//   [TestMethod]
//   public void IntListDeserialization()
//   {
//     // var tc = new TestClassInts { list1 = { 1, 2, 3, 5 } };
//     // var bson_str = "0x2d000000046c697374310021000000103000010000001031000200000010320003000000103300050000000000";
//     // "0x2d000000 04 6c6973743100 210000001030000100000010310002000000103200030000001033000500000000 00";
//     var bson_str = "0x21000000 10 3000 01000000 10 3100 02000000 10 3200 03000000 10 3300 05000000 00".Replace(" ", "");
//     var bson_doc = HexConverter.HexStringToByteArray(bson_str);

//     var array_reader = new MemoryArrayReader(bson_doc, new karu.bsonly.Interface.BsonSettings { MaxSize = 4 * 1024 * 1024 });

//     Assert.IsTrue(array_reader.HasNextEntry());
//     Assert.IsTrue(array_reader.HasEntry("0", BsonSerialization.BSON_TYPE_INT32));
//     Assert.AreEqual(1, array_reader.ReadInt());

//     Assert.IsTrue(array_reader.HasNextEntry());
//     Assert.IsTrue(array_reader.HasEntry("1", BsonSerialization.BSON_TYPE_INT32));
//     Assert.AreEqual(2, array_reader.ReadInt());

//     Assert.IsTrue(array_reader.HasNextEntry());
//     Assert.IsTrue(array_reader.HasEntry("2", BsonSerialization.BSON_TYPE_INT32));
//     Assert.AreEqual(3, array_reader.ReadInt());

//     Assert.IsTrue(array_reader.HasNextEntry());
//     Assert.IsTrue(array_reader.HasEntry("3", BsonSerialization.BSON_TYPE_INT32));
//     Assert.AreEqual(5, array_reader.ReadInt());

//     Assert.IsFalse(array_reader.HasNextEntry());
//     Assert.IsFalse(array_reader.HasEntry("4", BsonSerialization.BSON_TYPE_INT32));
//   }
// }

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