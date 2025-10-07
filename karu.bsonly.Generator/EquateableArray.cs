using System;
using System.Collections;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace karu.bsonly.Generator
{

  public class EquatableArray<T> : System.IEquatable<EquatableArray<T>>
  {
    private ImmutableArray<T> _array;

    static public EquatableArray<T> Empty()
    {
      var array = Array.Empty<T>();
      return new EquatableArray<T>(array);
    }

    public EquatableArray(T[] array)
    {
      _array = ImmutableArray.Create(array);
    }

    public T GetValue(int index)
    {
      return _array[index];
    }

    public T this[int index]
    {
      get => GetValue(index);
    }

    public int Length()
    {
      return _array.Length;
    }

    public bool Equals(EquatableArray<T>? other)
    {
      // If the other list is null or a different size, they're not equal
      if (other is null || _array.Length != other._array.Length)
      {
        return false;
      }

      // Compare each pair of elements for equality
      for (int i = 0; i < _array.Length; i++)
      {
        if (!System.Collections.Generic.EqualityComparer<T>.Default.Equals(this[i], other[i]))
        {
          return false;
        }
      }

      // If we got this far, the lists are equal
      return true;
    }
    public override bool Equals(object obj)
    {
      return Equals(obj as EquatableArray<T>);
    }
    public override int GetHashCode()
    {
      int hash = 17; // start with prime number
      foreach (var item in _array)
      {
        hash = hash * 31 + (item?.GetHashCode() ?? 0);
      }

      return hash;
    }

    public static bool operator ==(EquatableArray<T> list1, EquatableArray<T> list2)
    {
      return ReferenceEquals(list1, list2)
          || list1 is not null && list2 is not null && list1.Equals(list2);
    }
    public static bool operator !=(EquatableArray<T> list1, EquatableArray<T> list2)
    {
      return !(list1 == list2);
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