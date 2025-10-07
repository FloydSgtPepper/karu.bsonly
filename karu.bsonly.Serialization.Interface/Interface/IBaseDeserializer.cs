using System;
using System.IO;

namespace karu.bsonly.Serialization.Interface
{

  public interface IBaseDeserializer
  {
    bool HasNextEntry();

    bool HasEntry(ReadOnlySpan<byte> key_string, byte type_id);


    bool SkipEntry(ReadOnlySpan<byte> key_string);
    byte HasEntry(ReadOnlySpan<byte> key_string);
    public (ReadOnlyMemory<byte> key_string, byte type) NextEntry();

    IDocumentDeserializer DocumentReader();

    IArrayDeserializer ArrayReader();

    long ReadLong();

    int ReadInt();

    bool ReadBool();

    double ReadDouble();

    void ReadNull();

    ReadOnlySpan<byte> ReadString();

    Guid ReadGuid();

    ReadOnlySpan<byte> ReadBinary(byte user_type);
    ReadOnlySpan<byte> ReadRawBinary();

    ReadOnlySpan<byte> ReadRawDocument();

    int ReadSize();

    IBsonDocument GetBsonDoc();
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