using System;

namespace karu.bsonly.Serialization.Interface
{
  public interface IDocumentSerializer
  {
    public void FinishSubDocument();
    public byte[] Finish();

    public SerializationContext Context();

    public IBaseSerializer WriteLong(ReadOnlySpan<byte> key);
    public IBaseSerializer WriteInt(ReadOnlySpan<byte> key);
    public IBaseSerializer WriteDouble(ReadOnlySpan<byte> key);
    public IBaseSerializer WriteString(ReadOnlySpan<byte> key);

    public IBaseSerializer WriteBool(ReadOnlySpan<byte> key);

    public IBaseSerializer WriteNull(ReadOnlySpan<byte> key);

    public IDocumentSerializer WriteDocument(ReadOnlySpan<byte> key);

    public IArraySerializer WriteArray(ReadOnlySpan<byte> key);

    public IBaseSerializer WriteBinary(ReadOnlySpan<byte> key);
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