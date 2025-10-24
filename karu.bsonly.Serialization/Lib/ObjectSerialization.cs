using System.Reflection;
using karu.bsonly.Serialization.Interface;


namespace karu.bsonly.Serialization;

static class ObjectSerialization
{
  private static MethodInfo? SerializationMethod(Type value_type)
  {
    var parameter_types = new Type[] { typeof(IDocumentSerializer)};
    var method_deserialize = value_type.GetMethod("Serialize", BindingFlags.Instance | BindingFlags.Public, parameter_types);
    return method_deserialize;
  }

  public static void Serialize(IDocumentSerializer serializer, object value)
  {
    var type_of_value = value.GetType();
    var serialize_method = SerializationMethod(type_of_value);
    if (serialize_method != null)
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

    {
      var context = serializer.Context();
      if (context.SerializationRegistry != null)
      {
        var serialization_fct = context.SerializationRegistry.Serializer(value.GetType());
        if (serialization_fct != null)
        {
          try
          {
            //   serialization_fct.SerializationFunction()

            //   var parameters = new object[] { serializer, context };
            //   wrapper_serialize_method.Invoke(wrapper, parameters);
            return;
          }
          catch (Exception ex)
          {
            throw new BsonSerializationException($"Serialization of {value.GetType()} failed", ex);
          }
        }
      }
    }

    throw new NotImplementedException($"{value.GetType()} does not provide a 'Serialize' method");
  }


  public static void Deserialize(IDocumentDeserializer deserializer, object value)
  {
    // member function
    var parameter_types = new Type[] { typeof(IDocumentDeserializer), typeof(DeserializationContext) };
    var method_deserialize = value.GetType().GetMethod("Deserialize", BindingFlags.Instance | BindingFlags.Public, parameter_types);
    if (method_deserialize != null)
    {
      try
      {
        var parameters = new object[] { deserializer, new DeserializationContext() };
        method_deserialize.Invoke(value, parameters);
        return;
      }
      catch (Exception ex)
      {
        throw new BsonSerializationException($"Deserialization of {value.GetType()} failed", ex);
      }
    }
    // else
    // {
    //   // static function
    //   var static_parameter_types = new Type[] { value.GetType(), typeof(IBasicDeserializer), typeof(DeserializationContext) };

    //   var static_deserialize = value.GetType().GetMethod("Deserialize", BindingFlags.Static | BindingFlags.Public, static_parameter_types);
    //   if (static_deserialize != null)
    //   {
    //     try
    //     {
    //       var parameters = new object[] { value, deserializer, };
    //       static_deserialize.Invoke(null, parameters);
    //       return;
    //     }
    //     catch (Exception ex)
    //     {
    //       throw new BsonSerializationException($"Deserialization of {value.GetType()} failed", ex);
    //     }
    //   }
    // }

    throw new NotImplementedException($"{value.GetType()} does not provide a 'Deserialize' method");

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