// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Primitives;

/// <summary>
/// Provides encoding and decoding functionality for boolean values.
/// </summary>
public sealed class BooleanCodec : Codec<bool>
{
    /// <summary>
    /// Decodes a boolean value from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded boolean value.</returns>
    public override bool Decode(ref Utf8JsonReader reader)
    {
        return reader.GetBoolean();
    }

    /// <summary>
    /// Encodes a boolean value to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The boolean value to encode.</param>
    public override void Encode(Utf8JsonWriter writer, bool obj)
    {
        writer.WriteBooleanValue(obj);
    }
}
