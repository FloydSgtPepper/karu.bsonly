using System.Text;
using System.Text.RegularExpressions;

namespace karu.bsonly.Serialization.Benchmarks.Utils;

public static class HexConverter
{
  public static string RemoveSpaces(string input)
  {
    return Regex.Replace(input, "\\s+", "");
  }

  public static byte[] HexStringToByteArray(string hexString)
  {
    // Remove the "0x" prefix if it exists
    if (hexString.StartsWith("0x"))
      hexString = hexString.Substring(2);

    // Convert the hex string to a byte array
    int numBytes = hexString.Length / 2;
    byte[] byteArray = new byte[numBytes];
    for (int i = 0; i < numBytes; i++)
    {
      string byteString = hexString.Substring(i * 2, 2);
      byteArray[i] = Convert.ToByte(byteString, 16);
    }

    return byteArray;
  }

  public static string ByteArrayToHexString(byte[] byteArray)
  {
    return "0x" + BitConverter.ToString(byteArray).Replace("-", string.Empty).ToLower();
  }

  public static string ByteArrayToHexString(byte[] byteArray, int limit)
  {
    if (byteArray.Length < limit)
      return "0x" + BitConverter.ToString(byteArray).Replace("-", string.Empty).ToLower();
    else
      return "0x" + BitConverter.ToString(byteArray[0..limit]).Replace("-", string.Empty).ToLower() + $" .. ({byteArray.Length - limit}bytes ommitted)";
  }

  public static string ByteArrayWhackyString(byte[] byteArray, int limit)
  {
    if (byteArray.Length < limit)
      return Encoding.UTF8.GetString(byteArray);
    else
      return Encoding.UTF8.GetString(byteArray[0..limit]) + $" .. ({byteArray.Length - limit}bytes ommitted)";
  }

  public static byte[] ToByteArray(char[] input)
  {
    var output = new byte[input.Length];
    // var input_cnv = new ReadOnlySpan<byte>(&input);
    // Array.Copy(input_cnv, output, input.Length,
    for (int idx = 0; idx < input.Length; ++idx)
    {
      output[idx] = (byte)input[idx];
    }
    return output;
  }

  public static char[] ToCharArray(byte[] input)
  {
    var output = new char[input.Length];
    // var input_cnv = new ReadOnlySpan<byte>(&input);
    // Array.Copy(input_cnv, output, input.Length,
    for (int idx = 0; idx < input.Length; ++idx)
    {
      output[idx] = (char)input[idx];
    }
    return output;
  }


  // public static string ByteArrayToHexString(Span<byte> byteArray)
  // {
  //   return "0x" + BitConverter.ToString(byteArray).Replace("-", string.Empty).ToLower();
  // }

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