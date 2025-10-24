using karu.bsonly.Serialization;
using karu.bsonly.Serialization.Interface;
using karu.bsonly.Generator;

namespace karu.bsonly.Serialization.Test;
[BsonlyGenerator]
partial class TestClassBinary
{
  [BsonlyBinaryData(BsonConstants.BSON_BINARY_SUBTYPE_BINARY, BsonConstants.BSON_BINARY_SUBTYPE_GUID)]
  public byte[] Bin = Array.Empty<byte>();

  [BsonlyBinaryData("SerializeByteArray")]
  public byte[] ByteArray1 = Array.Empty<byte>();

  [BsonlyBinaryData("SerializeByteArray", BsonConstants.BSON_USER_TYPE_SEQ_INT_8)]
  public byte[] ByteArray2 = Array.Empty<byte>();

  [BsonlyBinaryData("SerializeByteArray", BsonConstants.BSON_BINARY_SUBTYPE_BINARY, BsonConstants.BSON_BINARY_SUBTYPE_BINARY)]
  public byte[] ByteArray3 = Array.Empty<byte>();

  [BsonlyBinaryData(BsonConstants.BSON_BINARY_SUBTYPE_BINARY, BsonConstants.BSON_BINARY_SUBTYPE_BINARY)]
  public byte[] ByteArray4 = Array.Empty<byte>();
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