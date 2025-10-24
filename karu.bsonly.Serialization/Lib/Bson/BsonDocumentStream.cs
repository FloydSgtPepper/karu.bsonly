using System;
using System.IO;
using System.Diagnostics;

namespace karu.bsonly.Serialization.Interface
{
  public class BsonDocumentStream : IDisposable
  {
    // implementation:
    //  _iterator._position is always < _doc.length so that one byte can be read without length checking

    internal enum IterState : byte
    {
      Invalid,
      AtElement,
      AfterElement
    }

    internal struct Iter
    {
      public long _doc_start;

      // public byte _element_type;

      // public ReadOnlyMemory<byte> _element_key;
      // public IterState _state;
    }

    private Stream _doc;

    private BsonDocumentStream? _parent = null;
    private const int SIZE_OF_SIZE_FIELD = sizeof(int);


    private Iter _iterator;

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
      if (_disposed) return;

      if (disposing)
      {
        // Dispose managed state (managed objects)
        if (_parent == null)
          _doc.Dispose();
        _parent = null;
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

    public BsonDocumentStream()
    {
      _doc = new MemoryStream();
      // _iterator._position = 0;
      // WriteSize???
      _iterator._doc_start = 0;


      // _iterator._state = IterState.Invalid;
    }

    public BsonDocumentStream(Stream bson_doc)
    {
      if (!bson_doc.CanSeek)
        throw new ArgumentException("only stream which can be seeked are supported");
      _doc = bson_doc;
      // _iterator._position = _doc.Position;
      // WriteSize???
      _iterator._doc_start = _doc.Position;
      // _iterator._state = IterState.Invalid;
    }

    public Stream Stream()
    {
      return _doc;
    }

    public long DocStartPosition()
    {
      return _iterator._doc_start;
    }

    public void WriteSubDocSizeAndEod()
    {
      _doc.WriteByte(BsonConstants.BSON_TYPE_EOD);
      var size = _doc.Position - _iterator._doc_start - SIZE_OF_SIZE_FIELD;
      if (size < int.MaxValue)
      {
        _doc.Seek(_iterator._doc_start, SeekOrigin.Begin);
        WriteSize((int)size);
        _doc.Seek(0, SeekOrigin.End);
        return;
      }
      throw new BufferUnderrunException($"the bson document is longer than the maximal size");
    }

    public void WriteDocSizeAndEod()
    {
      _doc.WriteByte(BsonConstants.BSON_TYPE_EOD);
      var size = _doc.Position - _iterator._doc_start;
      if (size < int.MaxValue)
      {
        _doc.Seek(_iterator._doc_start, SeekOrigin.Begin);
        WriteSize((int)size);
        _doc.Seek(0, SeekOrigin.End);
        return;
      }
      throw new BufferUnderrunException($"the bson document is longer than the maximal size");
    }

    public void StartDoc()
    {
      WriteSize(-1);
    }

    private void WriteSize(int size)
    {
      var size_bytes = BitConverter.GetBytes(size);
      _doc.Write(size_bytes);
    }
    /// <summary>
    /// create a child document. The parent must be at a Bson Binary, Bson Array or Bson Document
    /// position.
    /// </summary>
    /// <param name="parent"></param>
    /// <exception cref="ArgumentException"></exception>
    private BsonDocumentStream(BsonDocumentStream parent)
    {
      _parent = parent;
      _doc = parent._doc;
      _iterator._doc_start = parent._doc.Position;
      // _iterator._state = IterState.Invalid;
    }

    public static BsonDocumentStream SubDocument(BsonDocumentStream parent)
    {
      return new BsonDocumentStream(parent);
    }

    public BsonDocumentStream SubDocument()
    {
      return new BsonDocumentStream(this);
    }

    public BsonDocumentStream? ParentDocument()
    {
      return _parent;
    }

    // public /*IBaseSerializer*/void SetIteratorKeyAndType(ReadOnlySpan<byte> key, byte type_id)
    // {
    //   _iterator._state = IterState.AtElement;
    //   _doc.WriteByte(type_id);
    //   WriteString(key);
    // }

    // private void WriteString(ReadOnlySpan<byte> str)
    // {
    //   _doc.Write(str);
    //   _doc.WriteByte(0);
    // }


    // private void WriteEod()
    // {
    //   _doc.WriteByte(BsonConstants.BSON_TYPE_EOD);
    // }

    // private void WriteSize(int size)
    // {
    //   var size_bytes = BitConverter.GetBytes(size);
    //   _doc.Write(size_bytes);
    // }

    // public byte[] Finish()
    // {
    //   WriteEod();
    //   var size = _doc.Position - _iterator._doc_start;

    //   if (size < int.MaxValue)
    //   {
    //     _doc.Seek(_iterator._doc_start, SeekOrigin.Begin);
    //     WriteSize((int)size);
    //     _doc.Seek(_iterator._doc_start, SeekOrigin.Begin);

    //     var bytes = Array.Empty<byte>();
    //     if (_doc is MemoryStream memory_stream)
    //     {
    //       bytes = memory_stream.ToArray();
    //     }
    //     else
    //     {
    //       using (var mem_stream = new MemoryStream())
    //       {
    //         _doc.CopyTo(mem_stream);
    //         bytes = mem_stream.ToArray();
    //       }
    //     }

    //     _doc.Seek(0, SeekOrigin.End);
    //     return bytes;
    //   }

    //   throw new ArgumentException("the bson document is too big");
    // }
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