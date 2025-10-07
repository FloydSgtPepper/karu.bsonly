using System.Collections.Specialized;
using System.Diagnostics;
using karu.bsonly.Serialization;
using karu.bsonly.Serialization.Interface;
using karu.bsonly.Serialization.Test.Utils;
using System.Text.RegularExpressions;
using System.Text;

namespace karu.bsonly.Serialization.Test;


[TestClass]
public class TestBsonDocument
{
  [TestInitialize]
  public void Initialize()
  {
  }

  [TestMethod]
  public void InitializationTest()
  {
    var bson_str = "0x12000000 02 62617200 04000000 666f6f00 00".Replace(" ", "");
    var bson_doc = Utils.HexConverter.HexStringToByteArray(bson_str);


    var document = new BsonDocument(bson_doc, BsonSettings.BSON_API.OutOfOrderEvaluation);
    Assert.IsTrue(document.HasNextEntry());
  }

  [TestMethod]
  public void CatchTooSmallDocumentSize()
  {
    var bson_str = "0x10000000 02 62617200 04000000 666f6f00 00".Replace(" ", "");
    var bson_doc = Utils.HexConverter.HexStringToByteArray(bson_str);
    // wrong doc size - must throw exception
    Assert.ThrowsExactly<ArgumentException>(() => new BsonDocument(bson_doc, BsonSettings.BSON_API.OutOfOrderEvaluation));
  }

  [TestMethod]
  public void MinimalDocumentSize()
  {
    // minimal size - empty doc
    var bson_str = "0x05000000 00".Replace(" ", "");
    var bson_doc = Utils.HexConverter.HexStringToByteArray(bson_str);
    var serializer = new MemoryReader(bson_doc, 64 * 1024 * 1024, false);
    Assert.IsFalse(serializer.HasNextEntry());
  }

  [TestMethod]
  public void CatchDocumentTooSmall()
  {
    var bson_str = "0x04000000";
    var bson_doc = Utils.HexConverter.HexStringToByteArray(bson_str);
    // doc too small -- must throw
    Assert.ThrowsExactly<ArgumentException>(() => new BsonDocument(bson_doc, out_of_order_evaluation: false));
  }

  [TestMethod]
  public void SearchInOrder()
  {
    // 127
    // pos 4 - pos 5
    //  18     LongProperty 
    // pos 26 - pos 27
    //  3      "InnerClass 71
    //    2 StrProperty 8 foo_bar
    //   18
    //  16 InIntProperty
    // 0

    string bson_str = Regex.Replace(@"0x7f000000 
                                       12 4c6f6e6750726f706572747900 6712000000000000
                                       03 496e6e6572436c61737300 47000000 
                                         02 53747250726f706572747900 08000000 666f6f5f62617200
                                         12 4c6f6e6750726f706572747900 8681470e01000000
                                         10 496e496e7450726f706572747900 aa55aa55
                                       00
                                       10 496e7450726f706572747900 ffffffff
                                      00", "\\s+", "");

    var bson_doc = Utils.HexConverter.HexStringToByteArray(bson_str);
    var document = new BsonDocument(bson_doc, out_of_order_evaluation: false);

    Assert.IsTrue(document.HasEntry("LongProperty"u8, BsonConstants.BSON_TYPE_INT64));
    document.ConsumeValue(BsonConstants.BSON_TYPE_INT64);
    Assert.IsTrue(document.HasEntry("InnerClass"u8, BsonConstants.BSON_TYPE_DOCUMENT));
    document.ConsumeValue(BsonConstants.BSON_TYPE_DOCUMENT);
    Assert.IsTrue(document.HasEntry("IntProperty"u8, BsonConstants.BSON_TYPE_INT32));
    document.ConsumeValue(BsonConstants.BSON_TYPE_INT32);
    Assert.IsFalse(document.HasNextEntry());

    document = new BsonDocument(bson_doc, out_of_order_evaluation: false);
    Assert.IsTrue(document.HasEntry("LongProperty"u8, BsonConstants.BSON_TYPE_INT64));
    document.ConsumeValue(BsonConstants.BSON_TYPE_INT64);
    Assert.IsFalse(document.HasEntry("IntProperty"u8, BsonConstants.BSON_TYPE_INT32));
    Assert.IsTrue(document.HasEntry("InnerClass"u8, BsonConstants.BSON_TYPE_DOCUMENT));
    document.ConsumeValue(BsonConstants.BSON_TYPE_DOCUMENT);
    Assert.IsTrue(document.HasEntry("IntProperty"u8, BsonConstants.BSON_TYPE_INT32));
    document.ConsumeValue(BsonConstants.BSON_TYPE_INT32);
    Assert.IsFalse(document.HasNextEntry());
  }


  [TestMethod]
  public void SearchStaysInOneLevel()
  {
    // 127
    // pos 4 - pos 5         pos 18
    //  18     LongProperty  <value>
    // pos 26 - pos 27    pos 38
    //  3      "InnerClass 71     ..
    //    2 StrProperty 8 foo_bar
    //   18
    //  16 InIntProperty
    // 0

    string bson_str = Regex.Replace(@"0x7f000000
                                       12 4c6f6e6750726f706572747900 6712000000000000
                                       03 496e6e6572436c61737300 47000000
                                         02 53747250726f706572747900 08000000 666f6f5f62617200
                                         12 4c6f6e6750726f706572747900 8681470e01000000
                                         10 496e496e7450726f706572747900 aa55aa55
                                       00
                                       10 496e7450726f706572747900 ffffffff
                                      00", "\\s+", "");

    var bson_doc = Utils.HexConverter.HexStringToByteArray(bson_str);
    var document = new BsonDocument(bson_doc, out_of_order_evaluation: true);

    Assert.IsTrue(document.HasEntry("LongProperty"u8, BsonConstants.BSON_TYPE_INT64));
    Assert.IsTrue(document.HasEntry("IntProperty"u8, BsonConstants.BSON_TYPE_INT32));
    Assert.IsTrue(document.HasEntry("InnerClass"u8, BsonConstants.BSON_TYPE_DOCUMENT));
    Assert.IsFalse(document.HasEntry("StrProperty"u8, BsonConstants.BSON_TYPE_UTF8));
    Assert.IsFalse(document.HasEntry("InIntProperty"u8, BsonConstants.BSON_TYPE_INT32));
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