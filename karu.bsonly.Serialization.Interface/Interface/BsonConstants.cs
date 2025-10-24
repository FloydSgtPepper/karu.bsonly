
namespace karu.bsonly.Serialization.Interface
{

  public static class BsonConstants
  {
    public const byte BSON_TYPE_EOD = 0x00;

    public const byte BSON_TYPE_DOUBLE = 0x01;
    public const byte BSON_TYPE_UTF8 = 0x02;
    public const byte BSON_TYPE_DOCUMENT = 0x03;
    public const byte BSON_TYPE_ARRAY = 0x04;
    public const byte BSON_TYPE_BINARY = 0x05;

    public const byte BSON_TYPE_OBJECT_ID = 0x07;
    public const byte BSON_TYPE_BOOL = 0x08;
    public const byte BSON_TYPE_NULL = 0x0a;

    public const byte BSON_TYPE_INT32 = 0x10;
    public const byte BSON_TYPE_INT64 = 0x12; // BsonType.Int64

    // binary subtypes
    public const byte BSON_BINARY_SUBTYPE_BINARY = 0x00;
    public const byte BSON_BINARY_SUBTYPE_GUID = 0x04;
    public const byte BSON_BINARY_SUBTYPE_GUID_CSHARP_LEGACY = 0x03;

    // user types
    public const byte BSON_USER_TYPE_SEQ_UINT_8 = 0x80;

    public const byte BSON_USER_TYPE_SEQ_UINT_16 = 0x81;
    public const byte BSON_USER_TYPE_SEQ_UINT_32 = 0x82;
    public const byte BSON_USER_TYPE_SEQ_UINT_64 = 0x83; // SEQ_UINT_64

    public const byte BSON_USER_TYPE_SEQ_INT_8 = 0x84;
    public const byte BSON_USER_TYPE_SEQ_INT_16 = 0x85;
    public const byte BSON_USER_TYPE_SEQ_INT_32 = 0x86;
    public const byte BSON_USER_TYPE_SEQ_INT_64 = 0x87; // SEQ_INT_64
    public const byte BSON_USER_TYPE_DOC_ZIPPED = 0xE0;
    public const byte BSON_USER_TYPE_RAW_BINARY = 0xE1;
    public const byte BSON_USER_TYPE_DOC_LZ4 = 0xE2;

    public const int SIZE_OF_GUID = 16;

    public const byte JSON_SPACE_CHARACTER = 0x20;
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