// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Structs;

/// <summary>
/// Provides encoding and decoding functionality for <see cref="DateTimeOffset"/> values.
/// </summary>
public sealed class DateTimeOffsetCodec : Codec<DateTimeOffset>
{
    /// <summary>
    /// Decodes a <see cref="DateTimeOffset"/> value from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded <see cref="DateTimeOffset"/> value.</returns>
    public override DateTimeOffset Decode(ref Utf8JsonReader reader)
    {
        return reader.GetDateTimeOffset();
    }

    /// <summary>
    /// Encodes a <see cref="DateTimeOffset"/> value to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The <see cref="DateTimeOffset"/> value to encode.</param>
    public override void Encode(Utf8JsonWriter writer, DateTimeOffset obj)
    {
        writer.WriteStringValue(obj);
    }
}
