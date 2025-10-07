using System;

namespace karu.bsonly.Serialization.Interface
{
  // public delegate object SerializeObj(IBasicDeserializer deserializer);

  // public delegate void SerializeT<T>(ReadOnlySpan<byte> key, T value); // more generic todo

  public interface IArrayDeserializer
  {
    public bool HasEntry(ReadOnlySpan<byte> key_string, byte type_id);

    public byte NextEntryType();

    public bool HasNextEntry();

    public void Finish();

    public IBaseDeserializer NextEntry();
    public void ReadNull();

    public long ReadLong();

    public int ReadInt();

    public double ReadDouble();

    public bool ReadBool();

    public ReadOnlySpan<byte> ReadString();

    public Guid ReadGuid(/* enum type */);

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