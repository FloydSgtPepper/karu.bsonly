// using System.Collections.Specialized;
// using System.Diagnostics;
// using System.Security.Cryptography.X509Certificates;
// using karu.bsonly.Serialization;
// using karu.bsonly.Serialization.Interface;

// namespace karu.bsonly.Serialization.Test;


// [TestClass]
// public class BasicSerialization
// {
//   internal class TestClass
//   {
//     public long value { get; set; } = 0;
//     public string str { get; set; } = "unchanged";

//     public static void Serialize(TestClass obj, BsonDocument bson_doc, Context context, IMPtrEncoder? encoder = null)
//     { }

//     public static TestClass Deserialize(BsonDocument bson_doc, Context context, IMPtrEncoder? encoder = null)
//     {


//       var obj = new TestClass { str = "static" };
//       return obj;
//     }
//   }

//   internal class TestClassWithMember
//   {
//     public string str { get; set; } = "member";

//     public static void Serialize(TestClass obj, BsonDocument bson_doc, Context context, IMPtrEncoder? encoder = null)
//     { }

//     public void Deserialize(BsonDocument bson_doc, Context context, IMPtrEncoder? encoder = null)
//     {
//       str = "member";
//     }
//   }


//   [TestInitialize]
//   public void Initialize()
//   {
//   }

//   [TestMethod]
//   public void BasicDeserialization()
//   {
//     var bson_doc = new BsonDocument();
//     var test_class = Serializer.Deserialize<TestClass>(bson_doc, new Context { }, null);
//     Assert.AreEqual(test_class.str, "static");

//     var tc_member = Serializer.Deserialize<TestClassWithMember>(bson_doc, new Context { }, null);
//     Assert.AreEqual(tc_member.str, "member");

//   }

//   internal record Property
//   {
//     public string Name { get; init; }
//     public int Order { get; init; }

//     public Property(string name, int order)
//     {
//       Name = name;
//       Order = order;
//     }
//   }

//   [TestMethod]
//   public void TestSortOrder()
//   {
//     var l = new List<Property>();
//     l.Add(new("zero", -1));
//     l.Add(new("one", 4));
//     l.Add(new("two", -1));
//     l.Add(new("three", 2));
//     l.Add(new("four", -1));
//     l.Add(new("five", -1));

//     var prop_array = SortByOrder(l);
//     Assert.AreEqual(prop_array.Length, 5);
//     Assert.AreEqual(prop_array[0].Name, "zero");
//     Assert.AreEqual(prop_array[1].Name, "two");
//     Assert.AreEqual(prop_array[2].Name, "three");
//     Assert.AreEqual(prop_array[3].Name, "four");
//     Assert.AreEqual(prop_array[4].Name, "one");
//     Assert.AreEqual(prop_array[5].Name, "five");
//   }

//   private static Property[] SortByOrder(System.Collections.Generic.List<Property> properties)
//   {
//     // order 2, 4 are set
//     //     0, 1->4, 2, 3 -> 2, 4, 5
//     //     0, 2, 3, 4, 1, 5
//     // idx 0, 1, 2, 3, 4, 5

//     var num_of_props = properties.Count;
//     var prop_array = new Property[num_of_props];
//     for (var idx = 0; idx < num_of_props; ++idx)
//     {
//       var found_idx = properties.FindIndex(p => p.Order == idx);
//       if (found_idx == -1)
//       {
//         found_idx = properties.FindIndex(p => p.Order == -1);
//         if (found_idx != -1)
//         {
//           prop_array[idx] = properties[found_idx];
//           properties.RemoveAt(found_idx);
//         }
//         else
//         {
//           prop_array[idx] = properties[0];
//           properties.RemoveAt(0);
//         }
//       }
//       else
//       {
//         prop_array[idx] = properties[found_idx];
//         properties.RemoveAt(found_idx);
//       }
//     }
//     return prop_array;

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