// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Primitives;

/// <summary>
/// Provides encoding and decoding functionality for 8-bit signed integers (sbyte).
/// </summary>
public sealed class Int8Codec : Codec<sbyte>
{
    /// <summary>
    /// Decodes an 8-bit signed integer (sbyte) from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded 8-bit signed integer.</returns>
    public override sbyte Decode(ref Utf8JsonReader reader)
    {
        return reader.GetSByte();
    }

    /// <summary>
    /// Encodes an 8-bit signed integer (sbyte) to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The 8-bit signed integer to encode.</param>
    public override void Encode(Utf8JsonWriter writer, sbyte obj)
    {
        writer.WriteNumberValue(obj);
    }
}
