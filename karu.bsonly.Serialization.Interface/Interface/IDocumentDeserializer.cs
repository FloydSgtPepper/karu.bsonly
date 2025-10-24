using System;

namespace karu.bsonly.Serialization.Interface
{

  public interface IDocumentDeserializer
  {
    public bool HasNextEntry();

    public bool HasEntry(ReadOnlySpan<byte> key_string, byte type_id);

    public bool SkipEntry(ReadOnlySpan<byte> key_string);
    public byte HasEntry(ReadOnlySpan<byte> key_string);
    public (ReadOnlyMemory<byte> key_string, byte type) NextEntry();

    /// <summary>
    /// returns the binary subtype.
    /// If the current entry is not a binary document, then the result is undefined.
    /// </summary>
    /// <returns>binary subtype</returns>
    public byte BinarySubType();

    public long ReadLong();

    public int ReadInt();

    public bool ReadBool();

    public double ReadDouble();

    public void ReadNull();

    public ReadOnlySpan<byte> ReadString();

    /// <summary>
    /// Return the binary data.
    /// The subtype is removed. It should be checked before with BinarySubType()
    /// </summary>
    /// <returns>binary data</returns>
    public ReadOnlySpan<byte> ReadBinary();

    /// <summary>
    /// return the binary data.
    /// This includes the binary subtype as the first byte.
    /// </summary>
    /// <returns>binary data</returns>
    public ReadOnlySpan<byte> ReadRawBinary();

    public ReadOnlySpan<byte> ReadRawDocument();

    public int ReadSize();

    public IDocumentDeserializer DocumentReader();

    public IArrayDeserializer ArrayReader();

    public void Finish();

    public DeserializationContext Context();

    public BsonDocument Document();
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