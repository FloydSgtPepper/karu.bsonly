using System.Diagnostics;
using karu.bsonly.Serialization.Interface;

namespace karu.bsonly.Serialization;

// does not really handle a stream but only a completed stream
static class BasicReader
{
  public static void ReadNull(BsonDocument doc)
  {
    var data = doc.CurrentElement();
    Debug.Assert(data.Length == 1 && data[0] == 0);
  }

  public static long ReadLong(BsonDocument doc)
  {
    var data = doc.CurrentElement();
    if (data.Length == sizeof(long))
      return BitConverter.ToInt64(data); // can throw ArgumentOutOfRangeException

    throw new BufferUnderrunException(); // FIXME: or type exception or InvalidDoc expection maybe??
  }

  public static int ReadInt(BsonDocument doc)
  {
    var data = doc.CurrentElement();
    if (data.Length == sizeof(int))
      return BitConverter.ToInt32(data); // can throw ArgumentOutOfRangeException

    throw new BufferUnderrunException(); // FIXME: or type exception or InvalidDoc expection maybe??
  }

  public static double ReadDouble(BsonDocument doc)
  {
    var data = doc.CurrentElement();
    if (data.Length == sizeof(double))
      return BitConverter.ToDouble(data); // can throw ArgumentOutOfRangeException

    throw new BufferUnderrunException(); // FIXME: or type exception or InvalidDoc expection maybe??
  }

  public static bool ReadBool(BsonDocument doc)
  {
    var data = doc.CurrentElement();
    if (data.Length == sizeof(bool))
      return BitConverter.ToBoolean(data); // can throw ArgumentOutOfRangeException

    throw new BufferUnderrunException(); // FIXME: or type exception or InvalidDoc expection maybe??
  }

  public static ReadOnlySpan<byte> ReadString(BsonDocument doc)
  {
    return doc.CurrentElement();
  }

  public static ReadOnlySpan<byte> ReadBinary(BsonDocument doc)
  {
    var data = doc.CurrentElement();
    if (data.Length > 1)
      return data.Slice(1);

    return Array.Empty<byte>();
  }

  public static ReadOnlySpan<byte> ReadRawBinary(BsonDocument doc)
  {
    return doc.CurrentElement();
  }

  public static ReadOnlySpan<byte> ReadRawDocument(BsonDocument doc)
  {
    var data = doc.CurrentElement();
    return data.Slice(0, data.Length - 1); // remove trailing EOD
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