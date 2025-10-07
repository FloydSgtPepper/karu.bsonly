using System.Reflection;
using System.Runtime.CompilerServices;
using karu.bsonly.Serialization.Interface;


namespace karu.bsonly.Serialization;

static public class ApiSerializer
{
  public static byte[] Serialize<T>(T value, SerializationContext context) where T : ISerializable
  {
    StreamWriter writer = new StreamWriter(BsonSettings.BSON_API.MaxSize);

    try
    {
      value.Serialize(writer, context);
      return writer.Finish();
    }
    catch (Exception ex)
    {
      throw new BsonSerializationException($"Serialization of {typeof(T)} failed", ex);
    }
  }

  public static byte[] Serialize<T>(T value, SerializationContext context, IBaseSerializer writer) where T : ISerializable
  {
    try
    {
      value.Serialize(writer, context);
      return writer.Finish();
    }
    catch (Exception ex)
    {
      throw new BsonSerializationException($"Serialization of {typeof(T)} failed", ex);
    }
  }

  public static T Deserialize<T>(byte[] bson_doc, DeserializationContext context) where T : ISerializable, new()
  {
    var reader = new MemoryDocReader(bson_doc, BsonSettings.BSON_API.OutOfOrderEvaluation);

    try
    {
      var value = new T();
      value.Deserialize(reader.FirstEntry(), context);
      reader.Finish();
      return value;
    }
    catch (Exception ex)
    {
      throw new BsonSerializationException($"Serialization of {typeof(T)} failed", ex);
    }
  }

  public static byte[] Serialize(object obj, SerializationContext context)
  {
    StreamWriter writer = new StreamWriter(BsonSettings.BSON_API.MaxSize);
    // member function
    var parameter_types = new Type[] { typeof(IBaseSerializer), typeof(SerializationContext) };
    var method_deserialize = obj.GetType().GetMethod("Serialize", BindingFlags.Instance | BindingFlags.Public, parameter_types);
    if (method_deserialize != null)
    {
      try
      {
        var parameters = new object[] { writer, context };
        method_deserialize.Invoke(obj, parameters);
        return writer.Finish();
      }
      catch (Exception ex)
      {
        throw new BsonSerializationException($"Serialization of {obj.GetType()} failed", ex);
      }
    }

    throw new NotImplementedException($"{obj.GetType()} does not provide a 'Serialize' method");
  }


  public static void Deserialize(byte[] bson_doc, object value, DeserializationContext context)
  {
    var reader = new MemoryDocReader(bson_doc, context.Configuration.OutOfOrderEvaluation);
    // member function
    var parameter_types = new Type[] { typeof(IBaseDeserializer), typeof(DeserializationContext) };
    var method_deserialize = value.GetType().GetMethod("Deserialize", BindingFlags.Instance | BindingFlags.Public, parameter_types);
    if (method_deserialize != null)
    {
      try
      {
        var parameters = new object[] { reader.FirstEntry(), context };
        method_deserialize.Invoke(value, parameters);
        reader.Finish();
        // TODO: Dispose the reader
        return;
      }
      catch (Exception ex)
      {
        throw new BsonSerializationException($"Deserialization of {value.GetType()} failed", ex);
      }
    }

    throw new NotImplementedException($"{value.GetType()} does not provide a 'Deserialize' method");
  }

  public static void Deserialize<T>(byte[] bson_doc, T value, DeserializationContext context) where T : ISerializable
  {
    var reader = new MemoryDocReader(bson_doc, context.Configuration.OutOfOrderEvaluation);
    value.Deserialize(reader.FirstEntry(), context);
    reader.Finish();
  }

  public static object? Deserialize(byte[] bson_doc, Type object_type, DeserializationContext context)
  {
    var reader = new MemoryDocReader(bson_doc, BsonSettings.BSON_API.OutOfOrderEvaluation);

    object? instance = null;
    var ctor_info = object_type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);
    if (ctor_info != null)
      instance = ctor_info.Invoke(Array.Empty<object>());

    if (instance == null)
      throw new ArgumentException($"failed to create instance of type {object_type}");

    // member function
    var parameter_types = new Type[] { typeof(IBaseDeserializer), typeof(DeserializationContext) };
    var method_deserialize = object_type.GetMethod("Deserialize", BindingFlags.Instance | BindingFlags.Public, parameter_types);
    if (method_deserialize != null)
    {
      try
      {
        var parameters = new object[] { reader.FirstEntry(), context };
        method_deserialize.Invoke(instance, parameters);
        reader.Finish();
        // TODO: Dispose the reader
        return instance;
      }
      catch (Exception ex)
      {
        throw new BsonSerializationException($"Deserialization of {object_type} failed", ex);
      }
    }
    else
    {
      if (context.SerializationRegistry != null)
      {
        var serialization_provider = context.SerializationRegistry.Serializer(object_type);

        if (serialization_provider != null)
        {
          serialization_provider.DeserializationFunction(reader.FirstEntry(), ref instance, object_type, context);
          reader.Finish();
          return instance;
        }
      }
    }

    throw new NotImplementedException($"{object_type} does not provide a 'Deserialize' method and no serialiization provider is registered");
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