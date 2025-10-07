using System;

namespace karu.bsonly.Serialization.Interface
{
  public interface IBaseSerializer
  {
    void WriteLong(ReadOnlySpan<byte> key_string, long value);

    void WriteInt(ReadOnlySpan<byte> key_string, int value);

    void WriteDouble(ReadOnlySpan<byte> key_string, double value);

    void WriteBool(ReadOnlySpan<byte> key_string, bool value);

    void WriteNull(ReadOnlySpan<byte> key_string);

    void WriteString(ReadOnlySpan<byte> key_string, ReadOnlySpan<byte> value);

    void WriteGuid(ReadOnlySpan<byte> key_string, Guid value);

    void WriteBinary(ReadOnlySpan<byte> key_string, ReadOnlySpan<byte> binary_data, byte binary_subtype);
    // void WriteRawBinary(ReadOnlySpan<byte> key_string, ReadOnlySpan<byte> binary);

    public void WriteDocument<T>(ReadOnlySpan<byte> key_string, T document) where T : ISerializable;

    // write an already serialized document
    public void WriteRawDocument(ReadOnlySpan<byte> key_string, ReadOnlySpan<byte> document);

    public byte[] Finish();

    public IArraySerializer SerializeArray(ReadOnlySpan<byte> key_string);
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