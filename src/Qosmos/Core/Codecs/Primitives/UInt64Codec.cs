// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Primitives;

/// <summary>
/// Provides encoding and decoding functionality for 64-bit unsigned integers (ulong).
/// </summary>
public sealed class UInt64Codec : Codec<ulong>
{
    /// <summary>
    /// Decodes a 64-bit unsigned integer (ulong) from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded 64-bit unsigned integer.</returns>
    public override ulong Decode(ref Utf8JsonReader reader)
    {
        return reader.GetUInt64();
    }

    /// <summary>
    /// Encodes a 64-bit unsigned integer (ulong) to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The 64-bit unsigned integer to encode.</param>
    public override void Encode(Utf8JsonWriter writer, ulong obj)
    {
        writer.WriteNumberValue(obj);
    }
}
