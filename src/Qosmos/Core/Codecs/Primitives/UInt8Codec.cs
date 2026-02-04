// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Primitives;

/// <summary>
/// Provides encoding and decoding functionality for 8-bit unsigned integers (byte).
/// </summary>
public sealed class UInt8Codec : Codec<byte>
{
    /// <summary>
    /// Decodes an 8-bit unsigned integer (byte) from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded 8-bit unsigned integer.</returns>
    public override byte Decode(ref Utf8JsonReader reader)
    {
        return reader.GetByte();
    }

    /// <summary>
    /// Encodes an 8-bit unsigned integer (byte) to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The 8-bit unsigned integer to encode.</param>
    public override void Encode(Utf8JsonWriter writer, byte obj)
    {
        writer.WriteNumberValue(obj);
    }
}
