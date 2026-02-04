// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Primitives;

/// <summary>
/// Provides encoding and decoding functionality for character values (char).
/// </summary>
public sealed class CharCodec : Codec<char>
{
    /// <summary>
    /// Decodes a character (char) from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded character. Returns <see cref="char.MinValue"/> if the string is empty.</returns>
    public override char Decode(ref Utf8JsonReader reader)
    {
        return reader.GetString() is { Length: > 0 } str
            ? str[0]
            : char.MinValue;
    }

    /// <summary>
    /// Encodes a character (char) to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The character to encode.</param>
    public override void Encode(Utf8JsonWriter writer, char obj)
    {
        writer.WriteStringValue(obj.ToString());
    }
}
