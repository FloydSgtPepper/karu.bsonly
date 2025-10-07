using System.Text;
using System.Reflection;
using karu.bsonly.Serialization.Interface;


namespace karu.bsonly.Serialization;

static public partial class Serializer
{
  public static void Serialize(IArraySerializer serializer, object? value, SerializationContext? context)
  {
    SerializeObjectType(serializer, value, context);
  }

  public static void Serialize(IBaseSerializer serializer, ReadOnlySpan<byte> key, object? value, SerializationContext? context)
  {
    SerializeObjectType(serializer, key, value, context);
  }

  public static void Serialize(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, ref object? value, Type object_type, DeserializationContext? context)
  {
    DeserializeObjectType(deserializer, key, ref value, object_type, context);
  }

  public static void SerializeObjectType(IBaseSerializer serializer, ReadOnlySpan<byte> key, object? value, SerializationContext? context)
  {
    if (value == null)
    {
      serializer.WriteNull(key);
      return;
    }

    var object_type = value.GetType();
    var method = SerializationMethod(object_type);
    if (method != null)
    {
      SerializeWithMethod(serializer, value, context, method);
      return;
    }

    if (object_type == typeof(string))
    {
      serializer.WriteString(key, Encoding.UTF8.GetBytes((string)value));
      return;
    }
    if (object_type == typeof(long))
    {
      serializer.WriteLong(key, (long)value);
      return;
    }
    if (object_type == typeof(int))
    {
      serializer.WriteInt(key, (int)value);
      return;
    }
    if (object_type == typeof(double))
    {
      serializer.WriteDouble(key, (double)value);
      return;
    }
    if (object_type == typeof(bool))
    {
      serializer.WriteBool(key, (bool)value);
      return;
    }

    if (context != null && context.SerializationRegistry != null)
    {
      var serialization_fct = context.SerializationRegistry.Serializer(value.GetType());
      if (serialization_fct != null)
      {
        serialization_fct.SerializationFunction(serializer, key, value, value.GetType(), context);
      }
    }

    return;
  }

  public static void SerializeObjectType(IArraySerializer serializer, object? value, SerializationContext? context)
  {
    if (value == null)
    {
      serializer.AddNull();
      return;
    }
    var object_type = value.GetType();
    if (object_type == typeof(string))
    {
      serializer.Add(Encoding.UTF8.GetBytes((string)value));
      return;
    }
    if (object_type == typeof(long))
    {
      serializer.Add((long)value);
      return;
    }
    if (object_type == typeof(int))
    {
      serializer.Add((int)value);
      return;
    }
    if (object_type == typeof(double))
    {
      serializer.Add((double)value);
      return;
    }
    if (object_type == typeof(bool))
    {
      serializer.Add((bool)value);
      return;
    }

    if (context != null && context.SerializationRegistry != null)
    {
      var serialization_fct = context.SerializationRegistry.Serializer(object_type);
      if (serialization_fct != null)
      {
        var (value_serializer, key) = serializer.SerializeValue();
        serialization_fct.SerializationFunction(value_serializer, key, value, object_type, context);
      }
    }

    return;
  }

  public static void DeserializeObjectType(IBaseDeserializer deserializer, ReadOnlySpan<byte> key, ref object? value, Type object_type, DeserializationContext? context)
  {
    var type_id = deserializer.HasEntry(key);
    switch (type_id)
    {
      case BsonConstants.BSON_TYPE_BOOL:
        if (object_type == typeof(bool))
        {
          var bool_value = deserializer.ReadBool();
          value = bool_value;
          return;
        }
        throw new BsonTypeException($"requested {object_type} but got 'bool'");

      case BsonConstants.BSON_TYPE_UTF8:
        if (object_type == typeof(string))
        {
          var str_value = deserializer.ReadString();
          value = Encoding.UTF8.GetString(str_value);
          return;
        }
        else if (object_type == typeof(byte[]))
        {
          var str_value = deserializer.ReadString();
          value = str_value.ToArray();
          return;

        }
        throw new BsonTypeException($"requested {object_type} but got 'utf8'");

      case BsonConstants.BSON_TYPE_INT32:
        if (object_type == typeof(int))
        {
          var int_value = deserializer.ReadInt();
          value = int_value;
          return;
        }
        throw new BsonTypeException($"requested {object_type} but got 'int'");

      case BsonConstants.BSON_TYPE_INT64:
        if (object_type == typeof(long))
        {
          var long_value = deserializer.ReadLong();
          value = long_value;
          return;
        }
        throw new BsonTypeException($"requested {object_type} but got 'long'");

      case BsonConstants.BSON_TYPE_DOUBLE:
        if (object_type == typeof(double))
        {
          var double_value = deserializer.ReadDouble();
          value = double_value;
          return;
        }
        throw new BsonTypeException($"requested {object_type} but got 'double'");

      case BsonConstants.BSON_TYPE_NULL:
        deserializer.ReadNull();
        value = null;
        return;

      case BsonConstants.BSON_TYPE_EOD:
        throw new KeyNotAvailableException($"no entry {System.Text.Encoding.UTF8.GetString(key)} found");
      case BsonConstants.BSON_TYPE_ARRAY:
        // if (value != null && context != null && context.SerializationRegistry != null)
        // {
        //   var (deserialize_fct, is_array_type) = context.SerializationRegistry.Deserializer(value.GetType());
        //   if (deserialize_fct != null && is_array_type)
        //   {
        //     deserialize_fct(deserializer, ref value, value.GetType(), context);
        //     return;
        //   }
        // }
        break;
      case BsonConstants.BSON_TYPE_DOCUMENT:
        var doc_reader = deserializer.DocumentReader();
        if (context != null)
        {
          var method = DeserializationMethod(object_type);
          object? instance = value;
          if (method != null)
          {
            if (instance == null)
            {
              var ctor_info = object_type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);
              if (ctor_info != null)
                instance = ctor_info.Invoke(Array.Empty<object>());
            }
          }
          if (method != null && instance != null)
          {
            DeserializeWithMethod(doc_reader.FirstEntry(), instance, object_type, context, method);
            doc_reader.Finish();
            value = instance;
            return;
          }

          if (context.SerializationRegistry != null)
          {
            var serialization_provider = context.SerializationRegistry.Serializer(object_type);
            if (serialization_provider != null)
            {
              serialization_provider.DeserializationFunction(doc_reader.FirstEntry(), ref value, object_type, context);
              doc_reader.Finish();
              return;
            }
          }
        }
        break;
    }

    throw new TypeException($"bson type {type_id} for key {System.Text.Encoding.UTF8.GetString(key)} not supported for object serialization");
  }

  private static MethodInfo? SerializationMethod(Type value_type)
  {
    var parameter_types = new Type[] { typeof(IBaseSerializer), typeof(SerializationContext) };
    var method_serialize = value_type.GetMethod("Serialize", BindingFlags.Instance | BindingFlags.Public, parameter_types);
    return method_serialize;
  }

  private static MethodInfo? DeserializationMethod(Type value_type)
  {
    var parameter_types = new Type[] { typeof(IBaseDeserializer), typeof(DeserializationContext) };
    var method_deserialize = value_type.GetMethod("Deserialize", BindingFlags.Instance | BindingFlags.Public, parameter_types);
    return method_deserialize;
  }

  private static void SerializeWithMethod(IBaseSerializer serializer, object value, SerializationContext? context, MethodInfo serialize_method)
  {
    try
    {
      var parameters = new object[] { serializer, context };
      serialize_method.Invoke(value, parameters);
      return;
    }
    catch (Exception ex)
    {
      throw new BsonSerializationException($"Serialization of {value.GetType()} failed", ex);
    }
  }

  private static void DeserializeWithMethod(IBaseDeserializer serializer, object value, Type object_type, DeserializationContext? context, MethodInfo serialize_method)
  {
    try
    {
      var parameters = new object[] { serializer, context };
      serialize_method.Invoke(value, parameters);
      return;
    }
    catch (Exception ex)
    {
      throw new BsonSerializationException($"Deserialization of {object_type} failed", ex);
    }
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