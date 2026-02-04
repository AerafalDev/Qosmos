// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Structs;

/// <summary>
/// Provides encoding and decoding functionality for <see cref="TimeOnly"/> values.
/// </summary>
public sealed class TimeOnlyCodec : Codec<TimeOnly>
{
    /// <summary>
    /// Decodes a <see cref="TimeOnly"/> value from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded <see cref="TimeOnly"/> value. Returns <see cref="TimeOnly.MinValue"/> if the string is null or empty.</returns>
    public override TimeOnly Decode(ref Utf8JsonReader reader)
    {
        return reader.GetString() is { } str
            ? TimeOnly.Parse(str)
            : TimeOnly.MinValue;
    }

    /// <summary>
    /// Encodes a <see cref="TimeOnly"/> value to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The <see cref="TimeOnly"/> value to encode.</param>
    public override void Encode(Utf8JsonWriter writer, TimeOnly obj)
    {
        writer.WriteStringValue(obj.ToString("O"));
    }
}
