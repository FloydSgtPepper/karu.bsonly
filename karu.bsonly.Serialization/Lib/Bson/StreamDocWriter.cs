using System.Diagnostics;
using System.Reflection.Metadata;
using karu.bsonly.Serialization.Interface;

namespace karu.bsonly.Serialization;

public class StreamDocWriter : IDocumentSerializer
{
  private BsonDocumentStream _bson_doc;

  private StreamBasicWriter _base_writer;

  private readonly SerializationContext _context;

  public StreamDocWriter(SerializationContext context)
  {
    _context = context;
    var stream = new MemoryStream();
    _bson_doc = new BsonDocumentStream(stream);


    _base_writer = new StreamBasicWriter(stream);

    _bson_doc.StartDoc(); // dummy write
  }

  public StreamDocWriter(Stream stream, SerializationContext context)
  {
    _context = context;
    _bson_doc = new BsonDocumentStream(stream);
    _base_writer = new StreamBasicWriter(stream);
    _bson_doc.StartDoc(); // dummy write
  }

  public void FinishSubDocument()
  {
    _bson_doc.WriteSubDocSizeAndEod();
    _bson_doc = _bson_doc.ParentDocument()!;
  }

  public byte[] Finish()
  {
    _bson_doc.WriteDocSizeAndEod();

    var bytes = Array.Empty<byte>();
    if (_bson_doc.Stream() is MemoryStream memory_stream)
    {
      bytes = memory_stream.ToArray();
    }
    else
    {
      using (var mem_stream = new MemoryStream())
      {
        _bson_doc.Stream().CopyTo(mem_stream);
        bytes = mem_stream.ToArray();
      }
    }

    _bson_doc.Stream().Seek(0, SeekOrigin.End);
    return bytes;
  }

  public SerializationContext Context()
  {
    return _context;
  }

  private IBaseSerializer Write(ReadOnlySpan<byte> key, byte type)
  {
    _base_writer.WriteKeyAndType(key, type);
    return _base_writer;
  }

  public IBaseSerializer WriteLong(ReadOnlySpan<byte> key)
  {
    return Write(key, BsonConstants.BSON_TYPE_INT64);
  }
  public IBaseSerializer WriteInt(ReadOnlySpan<byte> key)
  {
    return Write(key, BsonConstants.BSON_TYPE_INT32);
  }

  public IBaseSerializer WriteDouble(ReadOnlySpan<byte> key)
  {
    return Write(key, BsonConstants.BSON_TYPE_DOUBLE);
  }

  public IBaseSerializer WriteString(ReadOnlySpan<byte> key)
  {
    return Write(key, BsonConstants.BSON_TYPE_UTF8);
  }

  public IBaseSerializer WriteBool(ReadOnlySpan<byte> key)
  {
    return Write(key, BsonConstants.BSON_TYPE_BOOL);
  }

  public IBaseSerializer WriteNull(ReadOnlySpan<byte> key)
  {
    return Write(key, BsonConstants.BSON_TYPE_NULL);
  }

  public IDocumentSerializer WriteDocument(ReadOnlySpan<byte> key)
  {
    _base_writer.WriteKeyAndType(key, BsonConstants.BSON_TYPE_DOCUMENT);
    var sub_doc = _bson_doc.SubDocument();
    _bson_doc = sub_doc;
    _bson_doc.StartDoc();
    return this;
  }

  public IBaseSerializer WriteBinary(ReadOnlySpan<byte> key)
  {
    // _base_writer.WriteKeyAndType(key, BsonConstants.BSON_TYPE_BINARY);
    // var sub_doc = _bson_doc.SubDocument();
    // _bson_doc = sub_doc;
    // _bson_doc.StartDoc();
    return Write(key, BsonConstants.BSON_TYPE_BINARY);
  }



  public IArraySerializer WriteArray(ReadOnlySpan<byte> key)
  {
    _base_writer.WriteKeyAndType(key, BsonConstants.BSON_TYPE_ARRAY);
    return new StreamArrayWriter(_bson_doc, _context.Configuration.Arrays == Arrays.EMPTY_KEYS);
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