// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Primitives;

/// <summary>
/// Provides encoding and decoding functionality for 16-bit unsigned integers (ushort).
/// </summary>
public sealed class UInt16Codec : Codec<ushort>
{
    /// <summary>
    /// Decodes a 16-bit unsigned integer (ushort) from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded 16-bit unsigned integer.</returns>
    public override ushort Decode(ref Utf8JsonReader reader)
    {
        return reader.GetUInt16();
    }

    /// <summary>
    /// Encodes a 16-bit unsigned integer (ushort) to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The 16-bit unsigned integer to encode.</param>
    public override void Encode(Utf8JsonWriter writer, ushort obj)
    {
        writer.WriteNumberValue(obj);
    }
}
