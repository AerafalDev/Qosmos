// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Primitives;

/// <summary>
/// Provides encoding and decoding functionality for 32-bit signed integers (int).
/// </summary>
public sealed class Int32Codec : Codec<int>
{
    /// <summary>
    /// Decodes a 32-bit signed integer (int) from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded 32-bit signed integer.</returns>
    public override int Decode(ref Utf8JsonReader reader)
    {
        return reader.GetInt32();
    }

    /// <summary>
    /// Encodes a 32-bit signed integer (int) to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The 32-bit signed integer to encode.</param>
    public override void Encode(Utf8JsonWriter writer, int obj)
    {
        writer.WriteNumberValue(obj);
    }
}
