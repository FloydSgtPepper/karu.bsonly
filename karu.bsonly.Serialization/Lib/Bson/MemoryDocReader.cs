using System.Diagnostics;
using karu.bsonly.Serialization.Interface;

namespace karu.bsonly.Serialization;

public class MemoryDocReader : IDocumentDeserializer, IDisposable
{

  private BsonDocument _doc;

  private DeserializationContext _context;

  private bool _disposed = false;

  protected virtual void Dispose(bool disposing)
  {
    if (_disposed) return;

    if (disposing)
    {
      // Dispose managed state (managed objects)
    }
    // Free unmanaged resources

    _disposed = true;
  }

  public void Dispose()
  {
    // Dispose of unmanaged resources
    Dispose(true);

    // Suppress finalization
    GC.SuppressFinalize(this);
  }

  public MemoryDocReader(BsonDocument doc, DeserializationContext context)
  {
    _doc = doc;
    _context = context;
  }


  public MemoryDocReader(byte[] bson_document, DeserializationContext context)
  {
    // value type
    _doc = new BsonDocument(bson_document, context.Configuration.OutOfOrderEvaluation);
    _context = context;
  }

  public BsonDocument Document()
  {
    return _doc;
  }

  public DeserializationContext Context()
  {
    return _context;
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

  public byte BinarySubType()
  {
    return _doc.BinarySubType();
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
  public ReadOnlySpan<byte> ReadBinary()
  {
    var data = _doc.CurrentElement();
    if (data.Length > 1)
      return data.Slice(1);

    return Array.Empty<byte>();
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
  //   var reader = new MemoryDocReader(_doc.DocStream, _max_document_size, _out_of_order_evaluation); // also give the buffer?
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
      var sub_doc = BsonDocument.SubDocument(_doc);
      _doc.ConsumeValue(type);
      return new MemoryDocReader(sub_doc, _context);
    }

    throw new BsonSerializationException("not at a document position");
  }

  public IArrayDeserializer ArrayReader()
  {
    var (key, type) = _doc.NextEntry();
    if (type == BsonConstants.BSON_TYPE_ARRAY)
    {
      var sub_doc = BsonDocument.SubDocument(_doc);
      _doc.ConsumeValue(type);
      return new MemoryArrayReader(sub_doc, _context);
    }

    throw new BsonSerializationException("not at a array position");
  }

  public void Finish()
  {
    _doc.Finish();
  }

  // public void ReadDocument<T>(T value) where T : ISerializable
  // {
  //   // FIXME: I dont think I need a MemoryDocReader and MemoryDocReader since
  //   // I have BsonDocument and BasicReader
  //   // but I will continue with this approach for now to get a running system
  //   //value.Deserialize(FirstEntry(), context);
  //   // doc_serializer.Finish();
  // }
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