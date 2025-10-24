using System.Collections.Generic;
using karu.hexutil;
using System.Diagnostics;
using System.Text.RegularExpressions;
using karu.bsonly.Serialization.Interface;
using System.Net.Sockets;


namespace karu.bsonly.Serialization.Test;


[TestClass]
public class TestGuidSerialization
{
  [TestInitialize]
  public void Initialize()
  {
  }


  internal class TestClass : ISerializable
  {
    public string Foo = "bar";

    public Guid AGuid = Guid.Empty;

    public void Serialize(IDocumentSerializer serializer)
    {
      Serializer.Serialize(serializer, "AGuid"u8, AGuid);
    }

    public void Deserialize(IDocumentDeserializer deserializer)
    {
      AGuid = Serializer.SerializeGuid(deserializer, "AGuid"u8);
    }
  }

  internal class TcListOfGuid : ISerializable
  {
    public List<Guid> GuidList = new();

    public void Serialize(IDocumentSerializer serializer)
    {
      Serializer.Serialize(serializer, "GuidList"u8, GuidList);
    }

    public void Deserialize(IDocumentDeserializer deserializer)
    {
      // CONTINUE HERE
      // List<Guid>? tmp = new();
      // Serializer.DeserializeObjectType(deserializer, "GuidList"u8, ref tmp, typeof(List<Guid>));
    }
  }


  [TestMethod]
  public void WriteStandardGuid()
  {
    const string guid_str = "10dc8deb-e358-4a93-8dfe-aa35c5c2bfa8";
    // standard guid
    var expected_bson = Regex.Replace(
         @"0x21000000 
                05 414775696400 10000000 04 10dc8deb e358 4a93 8dfe aa35c5c2bfa8
           00", "\\s+", "");

    var tc = new TestClass();
    Guid.TryParse(guid_str, out var guid);
    tc.AGuid = guid;

    // Debug.WriteLine($"guid: {tc.AGuid}");
    // var guid_bytes = tc.AGuid.ToByteArray(true);
    // Debug.WriteLine($"guid {HexConverter.ByteArrayToHexString(guid_bytes)}");

    var actual = BsonlySerializer.Serialize(tc, SerializationContext.Default);
    Debug.WriteLine($"actual: {HexConverter.ByteArrayToHexString(actual)}");
    Console.WriteLine($"actual: {HexConverter.ByteArrayToString(actual)}");
    Assert.AreEqual(expected_bson, HexConverter.ByteArrayToHexString(actual));
  }

  [TestMethod]
  public void ReadStandardGuid()
  {
    Guid.TryParse("10dc8deb-e358-4a93-8dfe-aa35c5c2bfa8", out var expected_guid);
    // standard guid
    var bson_str = Regex.Replace(
         @"0x21000000 
                05 414775696400 10000000 04 10dc8deb e358 4a93 8dfe aa35c5c2bfa8
           00", "\\s+", "");
    var bson_doc = HexConverter.HexStringToByteArray(bson_str);

    var actual = BsonlySerializer.Deserialize<TestClass>(bson_doc, DeserializationContext.Default);
    Debug.WriteLine($"actual guid: {actual.AGuid}");
    Assert.AreEqual(expected_guid, actual.AGuid);
  }

  [TestMethod]
  public void WriteListOfGuid()
  {
    const string guid_str1 = "10dc8deb-e358-4a93-8dfe-aa35c5c2bfa8";
    const string guid_str2 = "652e6ab1-9ce4-456b-a4da-d7e2ea9d72b5";
    const string guid_str3 = "eb270cff-2bab-4826-bd84-08e1ca3ca97a";

    // standard guid
    var expected_bson = Regex.Replace(
         @"0x5c000000
               04 477569644c69737400 4d000000
                    05 3000 10000000 04 10dc8debe3584a938dfeaa35c5c2bfa8
                    05 3100 10000000 04 652e6ab19ce4456ba4dad7e2ea9d72b5
                    05 3200 10000000 04 eb270cff2bab4826bd8408e1ca3ca97a
                00
             00", "\\s+", "");

    // 0x14000000 04 477569644c69737400 01000000 00 00

    var tc = new TcListOfGuid();
    Guid.TryParse(guid_str1, out var guid1);
    tc.GuidList.Add(guid1);
    Guid.TryParse(guid_str2, out var guid2);
    tc.GuidList.Add(guid2);
    Guid.TryParse(guid_str3, out var guid3);
    tc.GuidList.Add(guid3);

    var actual = BsonlySerializer.Serialize(tc, SerializationContext.Default);
    // Debug.WriteLine($"actual: {HexConverter.ByteArrayToHexString(actual)}");
    // Console.WriteLine($"actual: {HexConverter.ByteArrayToString(actual)}");
    Assert.AreEqual(expected_bson, HexConverter.ByteArrayToHexString(actual));
  }

  [TestMethod]
  public void WriteGuid_CSharpLegacy()
  {
    const string guid_str = "10dc8deb-e358-4a93-8dfe-aa35c5c2bfa8";
    // standard guid
    var expected_bson = Regex.Replace(
         @"0x21000000 
                05 414775696400 10000000 03 eb8ddc10 58e3 934a 8dfe aa35c5c2bfa8
           00", "\\s+", "");

    var tc = new TestClass();
    Guid.TryParse(guid_str, out var guid);
    tc.AGuid = guid;
    // Debug.WriteLine($"guid: {tc.AGuid}");

    // var guid_bytes = tc.AGuid.ToByteArray(true);
    // Debug.WriteLine($"guid {HexConverter.ByteArrayToHexString(guid_bytes)}");

    var context = new SerializationContext();
    context.Configuration.GuidRepresentation = Interface.GuidRepresentation.CSHARP_LEGACY;
    var actual = BsonlySerializer.Serialize(tc, context);

    Debug.WriteLine($"actual: {HexConverter.ByteArrayToHexString(actual)}");
    Assert.AreEqual(expected_bson, HexConverter.ByteArrayToHexString(actual));
  }

  [TestMethod]
  public void WriteListOfGuid_CSharpLegacy()
  {
    const string guid_str1 = "10dc8deb-e358-4a93-8dfe-aa35c5c2bfa8";
    const string guid_str2 = "652e6ab1-9ce4-456b-a4da-d7e2ea9d72b5";
    const string guid_str3 = "eb270cff-2bab-4826-bd84-08e1ca3ca97a";

    // standard guid
    var expected_bson = Regex.Replace(
         @"0x5c000000
               04 477569644c69737400 4d000000
                    05 3000 10000000 03 eb8ddc10 58e3 934a 8dfe aa35c5c2bfa8
                    05 3100 10000000 03 b16a2e65 e49c 6b45 a4da d7e2ea9d72b5
                    05 3200 10000000 03 ff0c27eb ab2b 2648 bd84 08e1ca3ca97a
                00
             00", "\\s+", "");

    var tc = new TcListOfGuid();
    Guid.TryParse(guid_str1, out var guid1);
    tc.GuidList.Add(guid1);
    Guid.TryParse(guid_str2, out var guid2);
    tc.GuidList.Add(guid2);
    Guid.TryParse(guid_str3, out var guid3);
    tc.GuidList.Add(guid3);

    var context = new SerializationContext();
    context.Configuration.GuidRepresentation = Interface.GuidRepresentation.CSHARP_LEGACY;
    var actual = BsonlySerializer.Serialize(tc, context);
    Debug.WriteLine($"actual: {HexConverter.ByteArrayToHexString(actual)}");
    Console.WriteLine($"actual: {HexConverter.ByteArrayToString(actual)}");
    Assert.AreEqual(expected_bson, HexConverter.ByteArrayToHexString(actual));
  }

  [TestMethod]
  public void ReadGuid_CSharpLegacy()
  {
    Guid.TryParse("10dc8deb-e358-4a93-8dfe-aa35c5c2bfa8", out var expected_guid);
    // standard guid
    var bson_str = Regex.Replace(
         @"0x21000000 
                05 414775696400 10000000 03 eb8ddc10 58e3 934a 8dfe aa35c5c2bfa8
           00", "\\s+", "");
    var bson_doc = HexConverter.HexStringToByteArray(bson_str);

    var actual = BsonlySerializer.Deserialize<TestClass>(bson_doc, DeserializationContext.Default);
    Debug.WriteLine($"actual guid: {actual.AGuid}");
    Assert.AreEqual(expected_guid, actual.AGuid);
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