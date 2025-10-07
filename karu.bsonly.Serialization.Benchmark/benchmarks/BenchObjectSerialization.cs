// using System;
// using BenchmarkDotNet.Attributes;

// using karu.bsonly.Serialization.Interface;

// namespace karu.bsonly.Serialization.Benchmarks
// {

//   [MemoryDiagnoser]
//   [ThreadingDiagnoser]
//   [ExceptionDiagnoser]
//   public class BenchObjectSerialization
//   {
//     private const int N = 10000;
//     private readonly TestClassSimple _tc_simple;
//     private readonly TestClassISerializable _tc_serializable;

//     private readonly WrappedClass _tc_wrapper;

//     private readonly SerializationContext _context;

//     public BenchObjectSerialization()
//     {
//       _tc_simple = new TestClassSimple
//       {
//         LongProperty = 4711,
//         InnerClass = new TestClassInnerSimple { StrProperty = "foo_bar", IntProperty = 1437226410/*55AA55AA*/, LongProperty = 4534534534 },
//         IntProperty = -1
//       };

//       _tc_serializable = new TestClassISerializable
//       {
//         LongProperty = 4711,
//         InnerClass = new TestClassInnerISerializable { StrProperty = "foo_bar", IntProperty = 1437226410/*55AA55AA*/, LongProperty = 4534534534 },
//         IntProperty = -1
//       };

//       _tc_wrapper = new WrappedClass
//       {
//         LongProperty = 4711,
//         InnerClass = new WrappedClassInner
//         {
//           StrProperty = "foo_bar",
//           IntProperty = 1437226410/*55AA55AA*/,
//           LongProperty = 4534534534
//         },
//         IntProperty = -1
//       };


//       var registry = new WrapperRegistry();
//       registry.Register(typeof(WrappedClassInner), (value) => { return new WrapperClassInner((WrappedClassInner)value); });

//       _context = new SerializationContext
//       {
//         Foo = string.Empty,
//         WrapperRegistry = () => { return registry; }
//       };
//     }

//     [Benchmark]
//     public byte[] Serialize_Simple()
//     {
//       return ApiSerializer.Serialize(_tc_simple, _context);
//     }

//     [Benchmark]
//     public byte[] Serialize_ISerializable()
//     {
//       return ApiSerializer.Serialize(_tc_serializable, _context);
//     }

//     [Benchmark]
//     public byte[] Serialize_Wrapper()
//     {
//       return ApiSerializer.Serialize(_tc_wrapper, _context);
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