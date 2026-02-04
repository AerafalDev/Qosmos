// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Structs;

/// <summary>
/// Provides encoding and decoding functionality for <see cref="DateTime"/> values.
/// </summary>
public sealed class DateTimeCodec : Codec<DateTime>
{
    /// <summary>
    /// Decodes a <see cref="DateTime"/> value from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded <see cref="DateTime"/> value.</returns>
    public override DateTime Decode(ref Utf8JsonReader reader)
    {
        return reader.GetDateTime();
    }

    /// <summary>
    /// Encodes a <see cref="DateTime"/> value to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The <see cref="DateTime"/> value to encode.</param>
    public override void Encode(Utf8JsonWriter writer, DateTime obj)
    {
        writer.WriteStringValue(obj);
    }
}
