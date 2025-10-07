using karu.bsonly.Serialization.Interface;

namespace karu.bsonly.Serialization;



// does not really handle a stream but only a completed stream
public class MemoryArrayReader : IArrayDeserializer
{
  private IBsonDocument _bson_doc;

  /// <summary>
  /// create a reader to read sub document of array type
  /// </summary>
  /// <param name="bson_document">the bson document</param>
  public MemoryArrayReader(IBsonDocument bson_document)
  {
    // FIXME: input BsonDoc instead of baseSerializer
    _bson_doc = bson_document;
  }

  public bool HasEntry(ReadOnlySpan<byte> key, byte type_id)
  {
    return _bson_doc.HasEntry(key, type_id);
  }

  public byte HasEntry(ReadOnlySpan<byte> key)
  {
    return _bson_doc.HasEntry(key);
  }

  public IBaseDeserializer NextEntry()
  {
    var (key, type) = _bson_doc.NextEntry();

    // FIXME: test test test
    return new MemoryReader(_bson_doc);
  }

  public byte NextEntryType()
  {
    var (key, type) = _bson_doc.NextEntry();
    return type;
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
    var doc_serializer = new MemoryDocReader(_bson_doc);

    value.Deserialize(doc_serializer.FirstEntry(), context);
    doc_serializer.Finish();
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