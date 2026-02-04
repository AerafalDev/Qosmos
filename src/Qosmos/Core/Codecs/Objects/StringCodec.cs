// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Objects;

/// <summary>
/// A codec for encoding and decoding string values to and from JSON.
/// </summary>
public sealed class StringCodec : Codec<string>
{
    /// <summary>
    /// Decodes a string value from the specified JSON reader.
    /// </summary>
    /// <param name="reader">The JSON reader to decode the string value from.</param>
    /// <returns>The decoded string value, or null if the value is null.</returns>
    public override string? Decode(ref Utf8JsonReader reader)
    {
        return reader.GetString();
    }

    /// <summary>
    /// Encodes a string value to the specified JSON writer.
    /// </summary>
    /// <param name="writer">The JSON writer to encode the string value to.</param>
    /// <param name="obj">The string value to encode. If null, a JSON null value is written.</param>
    public override void Encode(Utf8JsonWriter writer, string? obj)
    {
        writer.WriteStringValue(obj);
    }
}
