// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Structs;

/// <summary>
/// Provides encoding and decoding functionality for <see cref="DateOnly"/> values.
/// </summary>
public sealed class DateOnlyCodec : Codec<DateOnly>
{
    /// <summary>
    /// Decodes a <see cref="DateOnly"/> value from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded <see cref="DateOnly"/> value. Returns <see cref="DateOnly.MinValue"/> if the string is null or empty.</returns>
    public override DateOnly Decode(ref Utf8JsonReader reader)
    {
        return reader.GetString() is { } str
            ? DateOnly.Parse(str)
            : DateOnly.MinValue;
    }

    /// <summary>
    /// Encodes a <see cref="DateOnly"/> value to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The <see cref="DateOnly"/> value to encode.</param>
    public override void Encode(Utf8JsonWriter writer, DateOnly obj)
    {
        writer.WriteStringValue(obj.ToString("O"));
    }
}
