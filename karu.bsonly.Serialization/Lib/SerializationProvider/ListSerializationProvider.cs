using karu.bsonly.Serialization.Interface;

namespace karu.bsonly.Serialization.Provider;

class SerializationOfListOfULong : ISerializationProvider
{
  public void SerializationFunction(IDocumentSerializer serializer, ReadOnlySpan<byte> key, object value, Type type_of_value)
  {
    if (type_of_value == typeof(List<ulong>))
    {
      var list_value = value as List<ulong>;
      Serializer.Serialize(serializer, key, list_value!);
      return;
    }

    throw new ArgumentException($"value must be List<ulong> but was {type_of_value}");
  }

  public void DeserializationFunction(IDocumentDeserializer serializer, ReadOnlySpan<byte> key, ref object? value, Type type_of_value)
  {
    if (type_of_value == typeof(List<ulong>))
    {
      value = Serializer.SerializeListOfULong(serializer, key);
      return;
    }

    throw new ArgumentException($"value must be List<ulong> but was {type_of_value}");
  }
}

class SerializationOfListOfLong : ISerializationProvider
{
  public void SerializationFunction(IDocumentSerializer serializer, ReadOnlySpan<byte> key, object value, Type type_of_value)
  {
    if (type_of_value == typeof(List<long>))
    {
      var list_value = value as List<long>;
      Serializer.Serialize(serializer, key, list_value!);
      return;
    }

    throw new ArgumentException($"value must be List<long> but was {type_of_value}");
  }

  public void DeserializationFunction(IDocumentDeserializer serializer, ReadOnlySpan<byte> key, ref object? value, Type type_of_value)
  {
    if (type_of_value == typeof(List<long>))
    {
      value = Serializer.SerializeListOfLong(serializer, key);
      return;
    }

    throw new ArgumentException($"value must be List<long> but was {type_of_value}");
  }
}

class SerializationOfListOfUInt : ISerializationProvider
{
  public void SerializationFunction(IDocumentSerializer serializer, ReadOnlySpan<byte> key, object value, Type type_of_value)
  {
    if (type_of_value == typeof(List<uint>))
    {
      var list_value = value as List<uint>;
      Serializer.Serialize(serializer, key, list_value!);
      return;
    }

    throw new ArgumentException($"value must be List<uint> but was {type_of_value}");
  }

  public void DeserializationFunction(IDocumentDeserializer serializer, ReadOnlySpan<byte> key, ref object? value, Type type_of_value)
  {
    if (type_of_value == typeof(List<uint>))
    {
      value = Serializer.SerializeListOfUInt(serializer, key);
      return;
    }

    throw new ArgumentException($"value must be List<uint> but was {type_of_value}");
  }
}

class SerializationOfListOfInt : ISerializationProvider
{
  public void SerializationFunction(IDocumentSerializer serializer, ReadOnlySpan<byte> key, object value, Type type_of_value)
  {
    if (type_of_value == typeof(List<int>))
    {
      var list_value = value as List<int>;
      Serializer.Serialize(serializer, key, list_value!);
      return;
    }

    throw new ArgumentException($"value must be List<int> but was {type_of_value}");
  }

  public void DeserializationFunction(IDocumentDeserializer serializer, ReadOnlySpan<byte> key, ref object? value, Type type_of_value)
  {
    if (type_of_value == typeof(List<int>))
    {
      value = Serializer.SerializeListOfInt(serializer, key);
      return;
    }

    throw new ArgumentException($"value must be List<int> but was {type_of_value}");
  }
}