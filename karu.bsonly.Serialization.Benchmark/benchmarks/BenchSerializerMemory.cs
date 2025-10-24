// using System;
// using System.Text.RegularExpressions;
// using BenchmarkDotNet.Attributes;

// using karu.bsonly.Serialization.Interface;

// namespace karu.bsonly.Serialization.Benchmarks
// {

//   [MemoryDiagnoser]
//   [ThreadingDiagnoser]
//   [ExceptionDiagnoser]
//   public class BenchSerializerMemory
//   {
//     private byte[] _bson_doc_read = Array.Empty<byte>();
//     private byte[] _bson_doc_write = Array.Empty<byte>();
//     private readonly TestClassISerializable _tc_serializable;

//     private TestClassISerializable _tc_deserializable;

//     private readonly SerializationContext _serialization_context;
//     private readonly DeserializationContext _deserialization_context;

//     private readonly karu.bsonly.Serialization.MemoryDocReader _stream_reader;

//     private IBaseSerializer _writer;

//     public BenchSerializerMemory()
//     {
//       _tc_serializable = new TestClassISerializable
//       {
//         LongProperty = 4711,
//         InnerClass = new TestClassInnerISerializable { StrProperty = "foo_bar", IntProperty = 1437226410/*55AA55AA*/, LongProperty = 4534534534 },
//         IntProperty = -1
//       };


//       _serialization_context = new SerializationContext();
//       _deserialization_context = new DeserializationContext();

//       var bson_str = Regex.Replace(@"0x7d000000 12 4c6f6e6750726f706572747900 6712000000000000 
//                                    03 496e6e6572436c61737300 45000000
//                                      02 53747250726f706572747900 08000000 666f6f5f62617200
//                                      12 4c6f6e6750726f706572747900 8681470e01000000
//                                      10 496e7450726f706572747900 aa55aa55
//                                    00
//                                    10 496e7450726f706572747900 ffffffff
//                                    00", "\\s+", "");

//       _bson_doc_read = Utils.HexConverter.HexStringToByteArray(bson_str);
//       _tc_deserializable = new();

//       var stream = new MemoryStream(_bson_doc_read);
//       _stream_reader = new MemoryDocReader(stream, BsonSettings.BSON_API.MaxSize, BsonSettings.BSON_API.OutOfOrderEvaluation);

//       _writer = new StreamWriter(BsonSettings.BSON_API.MaxSize); // 
//     }

//     [Benchmark]
//     public void Serialize_StreamWriterNew()
//     {
//       _writer = new StreamWriter(BsonSettings.BSON_API.MaxSize);
//     }

//     [Benchmark]
//     public void Serialize_StreamWriter()
//     {
//       _bson_doc_write = BsonlySerializer.Serialize(_tc_serializable, _serialization_context);
//     }

//     [Benchmark]
//     public void Serialize_StreamReader()
//     {
//       Deserialize(_stream_reader, _deserialization_context, _tc_deserializable);
//     }

//     private void Deserialize(IDocumentDeserializer deserializer, DeserializationContext context, ISerializable value)
//     {
//       try
//       {
//         value.Deserialize(deserializer, context);
//         return;
//       }
//       catch (Exception ex)
//       {
//         throw new BsonSerializationException($"Serialization of {value.GetType()} failed", ex);
//       }
//     }

//   }
// }

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