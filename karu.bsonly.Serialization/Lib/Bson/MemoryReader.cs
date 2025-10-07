using System.Diagnostics;
using karu.bsonly.Serialization.Interface;

namespace karu.bsonly.Serialization;

// does not really handle a stream but only a completed stream
public class MemoryReader : IBaseDeserializer
{

  private IBsonDocument _doc;


  public MemoryReader(IBsonDocument doc)
  {
    _doc = doc;
  }

  public IBsonDocument GetBsonDoc() { return _doc; }

  public MemoryReader(byte[] bson_document, int max_doc_size, bool out_of_order_evaluation)
  {
    // value type
    _doc = new BsonDocument(bson_document, out_of_order_evaluation);
  }

  public bool HasEntry(ReadOnlySpan<byte> key, byte type_id)
  {
    return _doc.HasEntry(key, type_id);
  }

  public byte HasEntry(ReadOnlySpan<byte> key)
  {
    return _doc.HasEntry(key);
  }

  public (ReadOnlyMemory<byte> key_string, byte type) NextEntry()
  {
    return _doc.NextEntry();
  }

  public bool HasNextEntry()
  {
    return _doc.HasNextEntry();
  }

  public bool SkipEntry(ReadOnlySpan<byte> key)
  {
    return _doc.SkipEntry(key);
  }

  public void ReadNull()
  {
    var data = _doc.CurrentElement();
    Debug.Assert(data.Length == 1 && data[0] == 0);
  }

  public long ReadLong()
  {
    var data = _doc.CurrentElement();
    if (data.Length == sizeof(long))
      return BitConverter.ToInt64(data); // can throw ArgumentOutOfRangeException

    throw new BufferUnderrunException(); // FIXME: or type exception or InvalidDoc expection maybe??
  }

  public int ReadInt()
  {
    var data = _doc.CurrentElement();
    if (data.Length == sizeof(int))
      return BitConverter.ToInt32(data); // can throw ArgumentOutOfRangeException

    throw new BufferUnderrunException(); // FIXME: or type exception or InvalidDoc expection maybe??
  }

  public double ReadDouble()
  {
    var data = _doc.CurrentElement();
    if (data.Length == sizeof(double))
      return BitConverter.ToDouble(data); // can throw ArgumentOutOfRangeException

    throw new BufferUnderrunException(); // FIXME: or type exception or InvalidDoc expection maybe??
  }

  public bool ReadBool()
  {
    var data = _doc.CurrentElement();
    if (data.Length == sizeof(bool))
      return BitConverter.ToBoolean(data); // can throw ArgumentOutOfRangeException

    throw new BufferUnderrunException(); // FIXME: or type exception or InvalidDoc expection maybe??
  }

  public ReadOnlySpan<byte> ReadString()
  {
    return _doc.CurrentElement();
  }

  public Guid ReadGuid()
  {
    var data = _doc.CurrentElement();

    if (data.Length == BsonConstants.SIZE_OF_GUID + 1 && data[0] == BsonConstants.BSON_BINARY_SUBTYPE_GUID)
    {
      // Standard Bson Guid representation
      // Array.Reverse(value_as_array, 0, 4);
      // Array.Reverse(value_as_array, 4, 2);
      // Array.Reverse(value_as_array, 6, 2);
      return new Guid(data.Slice(1)); // FIXME: need to test
    }
    return Guid.Empty;

    // json
    // }
    // else
    // {
    //   var guid_as_string = ReadString(BsonConstants.SIZE_OF_GUID);
    //   if (Guid.TryParseExact(guid_as_string, "D", out var guid_value))
    //     return guid_value;
    //   throw new TypeException("ReadOnlySpan<byte> could not be parsed as guid");
    // }
  }

  public ReadOnlySpan<byte> ReadBinary(byte user_type)
  {
    var data = _doc.CurrentElement();
    if (data.Length > 1)
      if (user_type != data[0])
        return Array.Empty<byte>();

    return data.Slice(1);
  }

  public ReadOnlySpan<byte> ReadRawBinary()
  {
    return _doc.CurrentElement();
  }

  public ReadOnlySpan<byte> ReadRawDocument()
  {
    var data = _doc.CurrentElement();
    return data.Slice(0, data.Length - 1); // remove trailing EOD
  }

  // public void ReadDocument<T>(T value) where T : ISerializable
  // {
  //   var size = ReadSize();
  //   var reader = new MemoryReader(_doc.DocStream, _max_document_size, _out_of_order_evaluation); // also give the buffer?
  //   value.Deserialize(reader, new DeserializationContext());
  //   ConsumeEndByte();
  // }

  public int ReadSize()
  {
    return _doc.ReadSize();
  }

  public IDocumentDeserializer DocumentReader()
  {
    var (key, type) = _doc.NextEntry();
    if (type == BsonConstants.BSON_TYPE_DOCUMENT)
    {
      var doc = _doc as BsonDocument;// FIXME: do I need IBsonDocument???
      var sub_doc = new BsonDocument(doc!);
      return new MemoryDocReader(sub_doc);
    }

    throw new BsonSerializationException("not at a document position");
  }

  public IArrayDeserializer ArrayReader()
  {
    var (key, type) = _doc.NextEntry();
    if (type == BsonConstants.BSON_TYPE_ARRAY)
    {
      var doc = _doc as BsonDocument;// FIXME: do I need IBsonDocument???
      var sub_doc = new BsonDocument(doc!);
      return new MemoryArrayReader(sub_doc);
    }

    throw new BsonSerializationException("not at a array position");
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