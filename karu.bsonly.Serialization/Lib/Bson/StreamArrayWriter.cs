using System.Diagnostics;
using karu.bsonly.Serialization.Interface;

namespace karu.bsonly.Serialization;

public class StreamArrayWriter : IArraySerializer
{
  private BsonDocumentStream _bson_doc;

  //private StreamBasicWriter _base_writer;

  private int _current_index;

  private bool _empty_keys;

  private byte[] _buffer = new byte[11]; // int32.max = 2,147,483,647

  public StreamArrayWriter(BsonDocumentStream bson_document, bool empty_keys = false)
  {
    _current_index = 0;
    _bson_doc = bson_document.SubDocument();
    _empty_keys = empty_keys;
    _bson_doc.StartDoc(); // dummy write
  }


  public ReadOnlySpan<byte> NextKey()
  {
    if (_empty_keys)
      return ReadOnlySpan<byte>.Empty;

    var array_key = WriteIndexAsString(_current_index);
    ++_current_index;
    return array_key;
  }

  // public StreamBasicWriter Write(byte type)
  // {
  //   _base_writer.WriteKeyAndType(WriteIndexAsString(_current_index), type);
  //   return _base_writer;
  // }

  public void Finish()
  {
    _bson_doc.WriteDocSizeAndEod();
  }

  private ReadOnlySpan<byte> WriteIndexAsString(int index)
  {
    int bytes_written;
    index.TryFormat(_buffer.AsSpan(), out bytes_written);
    if (bytes_written < _buffer.Length)
      return _buffer.AsSpan(0, bytes_written);

    return ReadOnlySpan<byte>.Empty;
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