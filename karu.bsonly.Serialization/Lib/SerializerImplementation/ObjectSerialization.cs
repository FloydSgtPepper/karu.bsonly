using System.Text;
using System.Reflection;
using karu.bsonly.Serialization.Interface;
using System.Diagnostics;
using System.Collections;


namespace karu.bsonly.Serialization;

static public partial class Serializer
{
  public static void Serialize(IDocumentSerializer serializer, ReadOnlySpan<byte> key, object? value)
  {
    SerializeObjectType(serializer, key, value);
  }

  public static void Serialize(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, ref object? value, Type object_type)
  {
    DeserializeObjectType(deserializer, key, ref value, object_type);
  }

  public static void SerializeObjectType(IDocumentSerializer serializer, ReadOnlySpan<byte> key, object? value)
  {
    if (value == null)
    {
      serializer.WriteNull(key).WriteNull();
      return;
    }

    var type_of_value = value.GetType();
    var method = SerializationMethod(type_of_value);
    if (method != null)
    {
      SerializeWithMethod(serializer, value, method);
      return;
    }

    if (type_of_value == typeof(string))
    {
      serializer.WriteString(key).WriteString(Encoding.UTF8.GetBytes((string)value));
      return;
    }
    if (type_of_value == typeof(long))
    {
      serializer.WriteLong(key).WriteLong((long)value);
      return;
    }
    if (type_of_value == typeof(int))
    {
      serializer.WriteInt(key).WriteInt((int)value);
      return;
    }
    if (type_of_value == typeof(double))
    {
      serializer.WriteDouble(key).WriteDouble((double)value);
      return;
    }
    if (type_of_value == typeof(bool))
    {
      serializer.WriteBool(key).WriteBool((bool)value);
      return;
    }
    if (type_of_value == typeof(byte[]))
    {
      var bytes = value as byte[];
      // SEQ_INT_8 because std::vector<char> in c++
      serializer.WriteBinary(key).WriteBinary(bytes!, BsonConstants.BSON_USER_TYPE_SEQ_INT_8);
      return;
    }

    var context = serializer.Context();
    if (context != null && context.SerializationRegistry != null)
    {
      var serialization_fct = context.SerializationRegistry.Serializer(value.GetType());
      if (serialization_fct != null)
      {
        serialization_fct.SerializationFunction(serializer, key, value, value.GetType());
        return;
      }
    }

    if (type_of_value.IsGenericType)
    {
      Debug.WriteLine($"SerializeObjectType type is {type_of_value} -- {typeof(List<>)} -- {typeof(Dictionary<,>)}");
      if (type_of_value.GetGenericTypeDefinition() == typeof(List<>))
      {
        Debug.WriteLine($"SerializeObjectType list type {type_of_value}");
        var enumerable = (IEnumerable)value;

        var array_serializer = serializer.WriteArray(key);
        foreach (var element in enumerable)
        {
          SerializeObjectType(serializer, array_serializer.NextKey(), element);
        }
        array_serializer.Finish();
        return;
      }
      else if (type_of_value.GetGenericTypeDefinition() == typeof(Dictionary<,>))
      {
        // CONTINUE HERE: check guid serialization, then List<Guid> and then dictionary
        // CONTINUE HERE: first step: support Dictionary<K,T> where T is either primitive or ISerializable
        Debug.WriteLine($"SerializeObjectType dictiionary type {type_of_value}");
        return;
      }
    }

    var serializer_fct = SerializationMethodFromSerializer(type_of_value);
    if (serializer_fct != null)
      serializer_fct.Invoke(null, new object[] { serializer, key.ToArray(), value });

    return;
  }

  public static void DeserializeObjectType(IDocumentDeserializer deserializer, ReadOnlySpan<byte> key, ref object? value, Type object_type)
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
      case BsonConstants.BSON_TYPE_BINARY:
        if (object_type == typeof(byte[]) && deserializer.BinarySubType() == BsonConstants.BSON_BINARY_SUBTYPE_BINARY)
        {
          value = deserializer.ReadBinary().ToArray();
        }
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
        // if (method != null && instance != null)
        // {
        //   DeserializeWithMethod(doc_reader.FirstEntry(), instance, object_type, context, method);
        //   doc_reader.Finish();
        //   value = instance;
        //   return;
        // }

        // if (context.SerializationRegistry != null)
        // {
        //   var serialization_provider = context.SerializationRegistry.Serializer(object_type);
        //   if (serialization_provider != null)
        //   {
        //     serialization_provider.DeserializationFunction(doc_reader.FirstEntry(), ref value, object_type, context);
        //     doc_reader.Finish();
        //     return;
        //   }
        // }
        break;
    }

    throw new TypeException($"bson type {type_id} for key {System.Text.Encoding.UTF8.GetString(key)} not supported for object serialization");
  }

  private static MethodInfo? SerializationMethod(Type value_type)
  {
    var parameter_types = new Type[] { typeof(IDocumentSerializer), typeof(SerializationContext) };
    var method_serialize = value_type.GetMethod("Serialize", BindingFlags.Instance | BindingFlags.Public, parameter_types);
    return method_serialize;
  }

  private static MethodInfo? DeserializationMethod(Type value_type)
  {
    var parameter_types = new Type[] { typeof(IDocumentDeserializer), typeof(DeserializationContext) };
    var method_deserialize = value_type.GetMethod("Deserialize", BindingFlags.Instance | BindingFlags.Public, parameter_types);
    return method_deserialize;
  }

  private static MethodInfo? SerializationMethodFromSerializer(Type value_type)
  {
    const string METHOD_NAME = "Serialize";

    var parameter_types = new Type[] { typeof(IDocumentSerializer), typeof(byte[]), value_type };
    var method_deserialize = typeof(Serializer).GetMethod(METHOD_NAME, BindingFlags.Static | BindingFlags.Public, parameter_types);
    return method_deserialize;
  }

  private static MethodInfo? DeserializationMethodFromSerializer(Type value_type)
  {
    const string METHOD_NAME = "Serialize";

    var parameter_types = new Type[] { typeof(IDocumentDeserializer), typeof(byte[]), value_type.MakeByRefType() };
    var method_deserialize = typeof(Serializer).GetMethod(METHOD_NAME, BindingFlags.Static | BindingFlags.Public, parameter_types);
    return method_deserialize;
  }

  private static void SerializeWithMethod(IDocumentSerializer serializer, object value, MethodInfo serialize_method)
  {
    try
    {
      var parameters = new object[] { serializer };
      serialize_method.Invoke(value, parameters);
      return;
    }
    catch (Exception ex)
    {
      throw new BsonSerializationException($"Serialization of {value.GetType()} failed", ex);
    }
  }

  private static void DeserializeWithMethod(IDocumentDeserializer serializer, object value, Type object_type, MethodInfo serialize_method)
  {
    try
    {
      var parameters = new object[] { serializer };
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