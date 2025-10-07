using System;

namespace karu.bsonly.Serialization.Interface
{
  public interface IBaseSerializerNoKeys
  {
    void WriteLong(long value);

    void WriteInt(int value);

    void WriteDouble(double value);

    void WriteBool(bool value);

    void WriteNull();

    void WriteString(ReadOnlySpan<byte> value);

    void WriteGuid(Guid value);

    void WriteBinary(ReadOnlySpan<byte> binary_data, byte binary_subtype);
    // void WriteRawBinary(  ReadOnlySpan<byte> binary);

    public void WriteDocument<T>(T document) where T : ISerializable;

    // write an already serialized document
    public void WriteRawDocument(ReadOnlySpan<byte> document);
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