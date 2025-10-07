using karu.bsonly.Serialization.Interface;
namespace karu.bsonly.Serialization;

public class BsonValue
{
  public byte _bson_type;

  private byte _binary_subtype;

  private byte[]? _integral_value;

  private object? _object_value;

  public byte GetBsonType()
  {
    return _bson_type;
  }

  public byte GetBinarySubType()
  {
    return _binary_subtype;
  }

  public byte[] GetRawBytes()
  {
    if (_bson_type != BsonConstants.BSON_TYPE_DOCUMENT)
      return _integral_value!;

    return Array.Empty<byte>();
  }

  static public BsonValue fromBool(bool value)
  {
    byte[] buffer = BitConverter.GetBytes(value);
    return new BsonValue(buffer, BsonConstants.BSON_TYPE_BOOL);
  }

  public bool TryAsBool(out bool value)
  {
    if (_bson_type == BsonConstants.BSON_TYPE_BOOL)
    {
      value = _integral_value![0] != 0;
      return true;
    }

    value = false;
    return false;
  }

  static public BsonValue fromInt(int value)
  {
    var buffer = BitConverter.GetBytes(value);
    return new BsonValue(buffer, BsonConstants.BSON_TYPE_INT32);
  }

  public bool TryAsInt(out int value)
  {
    if (_bson_type == BsonConstants.BSON_TYPE_INT32)
    {
      value = BitConverter.ToInt32(_integral_value!, sizeof(int));
      return true;
    }

    value = 0;
    return false;
  }

  static public BsonValue fromLong(long value)
  {
    var buffer = BitConverter.GetBytes(value);
    return new BsonValue(buffer, BsonConstants.BSON_TYPE_INT64);
  }
  public bool TryAsLong(out long value)
  {
    if (_bson_type == BsonConstants.BSON_TYPE_INT64)
    {
      value = BitConverter.ToInt64(_integral_value!, sizeof(long));
      return true;
    }

    value = 0;
    return false;
  }

  static public BsonValue fromDouble(double value)
  {
    var buffer = BitConverter.GetBytes(value);
    return new BsonValue(buffer, BsonConstants.BSON_TYPE_DOUBLE);
  }
  public bool TryAsDouble(out double value)
  {
    if (_bson_type == BsonConstants.BSON_TYPE_DOUBLE)
    {
      value = BitConverter.ToDouble(_integral_value!, sizeof(double));
      return true;
    }

    value = 0.0;
    return false;
  }
  static public BsonValue fromNull()
  {
    var buffer = Array.Empty<byte>();
    return new BsonValue(buffer, BsonConstants.BSON_TYPE_NULL);
  }
  public bool TryAsNull()
  {
    return _bson_type == BsonConstants.BSON_TYPE_NULL;
  }

  static public BsonValue fromString(string value)
  {
    byte[] buffer = System.Text.UTF8Encoding.UTF8.GetBytes(value);
    return new BsonValue(buffer, BsonConstants.BSON_TYPE_UTF8);
  }
  public bool TryAsString(out string value)
  {
    if (_bson_type == BsonConstants.BSON_TYPE_UTF8)
    {
      value = System.Text.Encoding.UTF8.GetString(_integral_value!);
      return true;
    }

    value = string.Empty;
    return false;
  }

  static public BsonValue fromBinary(byte[] value, byte binary_subtype = BsonConstants.BSON_BINARY_SUBTYPE_BINARY)
  {
    return new BsonValue(value, BsonConstants.BSON_TYPE_BINARY, binary_subtype);
  }
  public bool TryAsBinary(out byte[] value, out byte binary_subtype)
  {
    if (_bson_type == BsonConstants.BSON_TYPE_BINARY)
    {
      value = _integral_value!;
      binary_subtype = _binary_subtype;
      return true;
    }

    value = Array.Empty<byte>();
    binary_subtype = BsonConstants.BSON_BINARY_SUBTYPE_BINARY;
    return false;
  }

  static public BsonValue fromObject(object value)
  {
    return new BsonValue(value, BsonConstants.BSON_TYPE_DOCUMENT);
  }
  public object? TryAsObject()
  {
    return _object_value;
  }


  static public BsonValue fromSerializedBytes(ReadOnlySpan<byte> bytes, byte bson_type)
  {
    return new BsonValue(bytes, bson_type);
  }


  private BsonValue(byte[] value, byte type, byte binary_subtype = BsonConstants.BSON_BINARY_SUBTYPE_BINARY)
  {
    _integral_value = value;
    _bson_type = type;
    _object_value = null;
    _binary_subtype = binary_subtype;
  }

  private BsonValue(ReadOnlySpan<byte> value, byte type, byte binary_subtype = BsonConstants.BSON_BINARY_SUBTYPE_BINARY)
  {
    _integral_value = value.ToArray();
    _bson_type = type;
    _object_value = null;
    _binary_subtype = BsonConstants.BSON_BINARY_SUBTYPE_BINARY;
  }

  private BsonValue(object value, byte type)
  {
    _object_value = value;
    _bson_type = type;
    _integral_value = null;
    _binary_subtype = 0;
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