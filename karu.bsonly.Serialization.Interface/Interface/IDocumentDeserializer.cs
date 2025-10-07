using System;

namespace karu.bsonly.Serialization.Interface
{
  public interface IDocumentDeserializer
  {
    public bool HasEntry(ReadOnlySpan<byte> key, byte type_id);

    public bool HasNextEntry();

    public bool SkipEntry(ReadOnlySpan<byte> key);
    public byte HasEntry(ReadOnlySpan<byte> key);
    public (ReadOnlyMemory<byte> key_string, byte type) NextEntry();

    public IBaseDeserializer FirstEntry();

    public void Finish();


    public void ReadNull();

    public long ReadLong();

    public int ReadInt();

    public double ReadDouble();

    public bool ReadBool();

    public ReadOnlySpan<byte> ReadString();

    public Guid ReadGuid(/* enum type */);

    void ReadDocument<T>(T value, DeserializationContext context) where T : ISerializable;
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