using System;

namespace karu.bsonly.Serialization.Interface
{
  public interface IArraySerializer
  {
    // IBaseSerializer Append();

    // void Finish();


    void Add(long value);

    void Add(int value);

    void Add(double value);

    void Add(bool value);

    void AddNull();

    void Add(ReadOnlySpan<byte> value);

    void Add(Guid value);

    void AddBinary(ReadOnlySpan<byte> binary, byte binary_subtype);

    // void AddArray(); TODO:

    /// <summary>
    /// add a document which is already serialized
    /// </summary>
    /// <param name="binary">serialized document</param>
    // void AddDocument(ReadOnlySpan<byte> binary);

    void AddDocument<T>(T value) where T : ISerializable;

    // void AddDocument(object value);

    (IBaseSerializer value_serializer, byte[] key_string) SerializeValue();

    void Finish();
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