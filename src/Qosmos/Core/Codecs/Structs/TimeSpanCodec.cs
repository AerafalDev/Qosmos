// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Structs;

/// <summary>
/// Provides encoding and decoding functionality for <see cref="TimeSpan"/> values.
/// </summary>
public sealed class TimeSpanCodec : Codec<TimeSpan>
{
    /// <summary>
    /// Decodes a <see cref="TimeSpan"/> value from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded <see cref="TimeSpan"/> value. Returns <see cref="TimeSpan.Zero"/> if the string is null or empty.</returns>
    public override TimeSpan Decode(ref Utf8JsonReader reader)
    {
        return reader.GetString() is { } str
            ? TimeSpan.Parse(str)
            : TimeSpan.Zero;
    }

    /// <summary>
    /// Encodes a <see cref="TimeSpan"/> value to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The <see cref="TimeSpan"/> value to encode.</param>
    public override void Encode(Utf8JsonWriter writer, TimeSpan obj)
    {
        writer.WriteStringValue(obj.ToString("c"));
    }
}
