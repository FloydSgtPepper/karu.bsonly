using System.Collections.Specialized;
using System.Diagnostics;
using karu.bsonly.Serialization;
using karu.bsonly.Serialization.Interface;
using karu.hexutil;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.ObjectModel;
using System.IO.Pipelines;

namespace karu.bsonly.Serialization.Test;


[TestClass]
public class TestBsonValueDocument
{
  [TestInitialize]
  public void Initialize()
  {
  }

  [TestMethod]
  public void InitializationTest()
  {
    var bson_str = "02 726573756c7400 56000000 50726f6a65637453746f726167655773536572766963652076657273696f6e3a20323032372d52312e3020436f6d6d69743a203362633137333631623464634275696c6420446174653a20323032352d30392d313100".Replace(" ", "");
    var bson_doc = HexConverter.HexStringToByteArray(bson_str);


    // just testing that no excpetion is thrown, i.e. bson_doc is a valid bson document
    var document = new BsonDocument(bson_doc, 0, bson_doc.Length, BsonSettings.BSON_API.OutOfOrderEvaluation);
  }

  [TestMethod]
  public void NextEntryAndConsumeValue()
  {
    var bson_doc = HexConverter.HexStringToByteArray(Regex.Replace(@"
      02 726573756c7400 56000000 
          50726f6a65637453746f726167655773536572766963652076657273696f6e3a20323032372d5231
          2e3020436f6d6d69743a203362633137333631623464634275696c6420446174653a20323032352d
          30392d3131
      00", "\\s+", ""));

    // ctor throw excpetion when bson_doc is not valid
    var document = new BsonDocument(bson_doc, 0, bson_doc.Length, BsonSettings.BSON_API.OutOfOrderEvaluation);
    var (key, type) = document.NextEntry();
    Assert.AreEqual(BsonConstants.BSON_TYPE_UTF8, type);
    MemoryExtensions.SequenceEqual(key.Span, "result"u8);

    document.ConsumeValue(BsonConstants.BSON_TYPE_UTF8);
    Assert.IsFalse(document.HasNextEntry());
    document.Finish();
  }

  [TestMethod]
  public void BsonDocumentAsSubDocument()
  {
    // 7061796c6f616400 ->  p a y l o a d     e1 -> raw_binary
    // 636865636b73756d00 -> checksum
    // children -> 6368696c6472656e00
    // use_counter -> 7573655f636f756e74657200
    // 7265665f636f756e7400 -> ref_count
    //  h o l d e r _ c o u n t -> 686f6c6465725f636f756e7400
    var bson_doc = HexConverter.HexStringToByteArray(Regex.Replace(@"
      0x32010000 12 726573706f6e73655f696400 9c02000000000000
        03 726573756c7400 10010000 
          05 7061796c6f616400 90000000 e1 
                 900000000274797065000900000045646d496e64657800056c7a340060000000e2f04f5e00000003
                 62736f6e005300000005757569640010000000048247444ed8b54029a4f5aa91b033ea3503737061
                 6365000e0000001069647800000000000005696e6469636573001000000082000000000000000002
                 0000000000000000001273697a65005e0000000000000000
          05 636865636b73756d00 14000000 84 73f3190fbd335f1553a0e371f867fe452844829c
          05 6368696c6472656e00 08000000 83 5100000000000080 
          03 7573655f636f756e74657200 26000000 
            10 7265665f636f756e7400 08000000
            10 686f6c6465725f636f756e7400 00000000 
          00
        00
      00", "\\s+", ""));

    var document = new BsonDocument(bson_doc, out_of_order_evaluation: false);
    var (key, type) = document.NextEntry();

    MemoryExtensions.SequenceEqual(key.Span, "result"u8);
    Assert.AreEqual(BsonConstants.BSON_TYPE_INT64, type);
    document.ConsumeValue(type);
    var value_doc = document.NextElement();

    (key, type) = value_doc.NextEntry();
    MemoryExtensions.SequenceEqual(key.Span, "payload"u8);
    Assert.AreEqual(BsonConstants.BSON_TYPE_DOCUMENT, type);
    value_doc.Finish();

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