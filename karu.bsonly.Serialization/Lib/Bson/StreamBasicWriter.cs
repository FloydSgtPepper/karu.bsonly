using System.Diagnostics;
using System.Text;
using karu.bsonly.Serialization.Interface;

namespace karu.bsonly.Serialization;

public class StreamBasicWriter : IBaseSerializer
{
  internal enum IterState : byte
  {
    Invalid,
    AtElement,
    AfterElement
  }

  internal struct Iter
  {
    // public int _position;
    // public int _doc_end;

    public long _doc_start;

    // public byte _element_type;

    // public ReadOnlyMemory<byte> _element_key;
    // public IterState _state;
  }

  private Iter _iterator;

  private Stream _stream;


  public StreamBasicWriter(Stream stream)
  {
    _stream = stream;

    _iterator._doc_start = _stream.Position;
  }

  public void WriteKeyAndType(ReadOnlySpan<byte> key, byte type_id)
  {
    _stream.WriteByte(type_id);
    _stream.Write(key);
    _stream.WriteByte(0);
  }

  public void WriteEod()
  {
    _stream.WriteByte(BsonConstants.BSON_TYPE_EOD);
  }

  public void WriteLong(long value)
  {
    var buffer = BitConverter.GetBytes(value);
    _stream.Write(buffer);
  }

  public void WriteInt(int value)
  {
    var buffer = BitConverter.GetBytes(value);
    _stream.Write(buffer);
  }

  public void WriteDouble(double value)
  {
    var buffer = BitConverter.GetBytes(value);
    _stream.Write(buffer);
  }

  public void WriteBool(bool value)
  {
    if (value)
      _stream.WriteByte(1);
    else
      _stream.WriteByte(0);
  }

  public void WriteNull()
  {
    // _stream.WriteByte(0);
  }

  public void WriteString(string value)
  {
    var utf8 = Encoding.UTF8.GetBytes(value);
    WriteString(utf8);
  }

  public void WriteString(ReadOnlySpan<byte> value)
  {
    WriteSize(value.Length + 1);
    _stream.Write(value);
    _stream.WriteByte(0);
  }

  public void WriteBinary(ReadOnlySpan<byte> binary_data, byte binary_subtype)
  {
    var start_position = _stream.Position;
    WriteSize(0); // dummy size
    _stream.WriteByte(binary_subtype);
    _stream.Write(binary_data);
    WriteSizeAt(start_position);
  }

  // public void WriteDocument<T>(T document) where T : ISerializable
  // {
  //   var pos = _stream.Position;
  //   WriteSize(0);
  //   document.Serialize(this, new SerializationContext());
  //   FinishDocument(pos);
  // }

  public void WriteRawDocument(ReadOnlySpan<byte> document)
  {
    WriteSize(document.Length);
    _stream.Write(document);
    WriteEod();
  }

  private void WriteSizeAt(long start_position)
  {
    var size = _stream.Position - start_position - 1 - sizeof(int);
    if (size < int.MaxValue)
    {
      _stream.Seek(start_position, SeekOrigin.Begin);
      WriteSize((int)size);
      _stream.Seek(0, SeekOrigin.End);
      return;
    }

    throw new ArgumentException("the bson document is too big");
  }

  private void WriteSize(int size)
  {
    var size_bytes = BitConverter.GetBytes(size);
    _stream.Write(size_bytes);
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