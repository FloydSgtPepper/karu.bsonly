using System.Diagnostics;
using System.Reflection;
using karu.bsonly.Serialization.Interface;

namespace karu.bsonly.Serialization;

/*
 IBaseReader contains a BsonDocument
  - the BsonDocument contains the doc, an iterator + buffer + settings
  - ArrayReader / DocReader are created containing (or using) the IBaseReader
 

  

*/


// does not really handle a stream but only a completed stream
public class MemoryDocReader : IDocumentDeserializer
{
  private IBsonDocument _bson_doc;


  public MemoryDocReader(IBsonDocument bson_document)
  {
    _bson_doc = bson_document;
  }

  /// <summary>
  /// create a reader to read a top level bson document
  /// </summary>
  /// <param name="bson_doc"></param>
  /// <param name="out_of_order_evaluation"></param>
  public MemoryDocReader(byte[] bson_doc, bool out_of_order_evaluation)
  {
    _bson_doc = new BsonDocument(bson_doc, out_of_order_evaluation);
  }

  // uint Version() // optional version
  // ReadOnlySpan<byte> Type(); // optional type, i.e. $type entry

  public bool HasEntry(ReadOnlySpan<byte> key, byte type_id)
  {
    return _bson_doc.HasEntry(key, type_id);
  }

  public byte HasEntry(ReadOnlySpan<byte> key)
  {
    return _bson_doc.HasEntry(key);
  }

  public (ReadOnlyMemory<byte> key_string, byte type) NextEntry()
  {
    return _bson_doc.NextEntry();
  }

  public bool HasNextEntry()
  {
    return _bson_doc.HasNextEntry();
  }

  public bool SkipEntry(ReadOnlySpan<byte> key)
  {
    return _bson_doc.SkipEntry(key);
  }

  public IBaseDeserializer FirstEntry()
  {
    // FIXME: test test test
    var base_reader = new MemoryReader(_bson_doc);
    return base_reader;
  }

  public void Finish()
  {
    _bson_doc.Finish();
  }

  public void ReadNull()
  {
    BasicReader.ReadNull(_bson_doc);
  }

  public long ReadLong()
  {
    return BasicReader.ReadLong(_bson_doc);
  }
  public int ReadInt()
  {
    return BasicReader.ReadInt(_bson_doc);
  }

  public double ReadDouble()
  {
    return BasicReader.ReadDouble(_bson_doc);
  }

  public bool ReadBool()
  {
    return BasicReader.ReadBool(_bson_doc);
  }

  public ReadOnlySpan<byte> ReadString()
  {
    return BasicReader.ReadString(_bson_doc);
  }

  public Guid ReadGuid()
  {
    return BasicReader.ReadGuid(_bson_doc);
  }

  public ReadOnlySpan<byte> ReadBinary(byte user_type)
  {
    return BasicReader.ReadBinary(_bson_doc, user_type);
  }

  public ReadOnlySpan<byte> ReadRawBinary()
  {
    return BasicReader.ReadRawBinary(_bson_doc);
  }

  public ReadOnlySpan<byte> ReadRawDocument()
  {
    return BasicReader.ReadRawDocument(_bson_doc);
  }

  public void ReadDocument<T>(T value, DeserializationContext context) where T : ISerializable
  {
    // FIXME: I dont think I need a MemoryDocReader and MemoryReader since
    // I have BsonDocument and BasicReader
    // but I will continue with this approach for now to get a running system
    value.Deserialize(FirstEntry(), context);
    // doc_serializer.Finish();
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