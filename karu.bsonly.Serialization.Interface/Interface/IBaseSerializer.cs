using System;

namespace karu.bsonly.Serialization.Interface
{
  public interface IBaseSerializer
  {
    public void WriteEod();

    public void WriteKeyAndType(ReadOnlySpan<byte> key, byte type_id);

    public void WriteLong(long value);

    public void WriteInt(int value);
    public void WriteDouble(double value);
    public void WriteBool(bool value);

    public void WriteNull();

    public void WriteString(string value);

    public void WriteString(ReadOnlySpan<byte> value);

    public void WriteBinary(ReadOnlySpan<byte> binary_data, byte binary_subtype);

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