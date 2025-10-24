
namespace karu.bsonly.Serialization.Interface
{
  public enum Sequences : byte
  {
    STRUCTURED,
    BINARY
  }

  public enum Arrays : byte
  {
    COMPLIANT,
    EMPTY_KEYS
  };

  public enum Format : byte
  {
    BSON,
    JSON
  };

  /// Schould serialization employ compression? (Memmory mapping use-case etc)
  public enum Compaction : byte
  {
    COMPRESSED,
    EXPANDED
  };

  public enum GuidRepresentation : byte
  {
    STANDARD,
    CSHARP_LEGACY
  }

  public class BsonSettings
  {
    public static BsonSettings BSONLY_API = new BsonSettings
    {
      OutOfOrderEvaluation = false,
      Sequences = Sequences.BINARY,
      Arrays = Arrays.EMPTY_KEYS,
      Format = Format.BSON,
      Compaction = Compaction.COMPRESSED,
      GuidRepresentation = GuidRepresentation.STANDARD,
    };

    public static BsonSettings BSON_API = new BsonSettings
    {
      OutOfOrderEvaluation = false,
      Sequences = Sequences.STRUCTURED,
      Arrays = Arrays.COMPLIANT,
      Format = Format.BSON,
      Compaction = Compaction.COMPRESSED,
      GuidRepresentation = GuidRepresentation.STANDARD,
    };

    public BsonSettings()
    {
      OutOfOrderEvaluation = false;
      Sequences = Sequences.BINARY;
      Arrays = Arrays.COMPLIANT;
      Format = Format.BSON;
      Compaction = Compaction.COMPRESSED;
      GuidRepresentation = GuidRepresentation.STANDARD;
    }

    public bool OutOfOrderEvaluation { get; set; } = false;

    public Sequences Sequences = Sequences.STRUCTURED;

    public Arrays Arrays = Arrays.COMPLIANT;
    public Format Format = Format.BSON;
    public Compaction Compaction = Compaction.COMPRESSED;
    public GuidRepresentation GuidRepresentation = GuidRepresentation.STANDARD;
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