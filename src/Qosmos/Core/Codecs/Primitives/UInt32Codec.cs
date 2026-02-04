// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Primitives;

/// <summary>
/// Provides encoding and decoding functionality for 32-bit unsigned integers (uint).
/// </summary>
public sealed class UInt32Codec : Codec<uint>
{
    /// <summary>
    /// Decodes a 32-bit unsigned integer (uint) from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded 32-bit unsigned integer.</returns>
    public override uint Decode(ref Utf8JsonReader reader)
    {
        return reader.GetUInt32();
    }

    /// <summary>
    /// Encodes a 32-bit unsigned integer (uint) to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The 32-bit unsigned integer to encode.</param>
    public override void Encode(Utf8JsonWriter writer, uint obj)
    {
        writer.WriteNumberValue(obj);
    }
}
