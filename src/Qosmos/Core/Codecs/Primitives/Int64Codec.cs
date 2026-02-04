// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Primitives;

/// <summary>
/// Provides encoding and decoding functionality for 64-bit signed integers (long).
/// </summary>
public sealed class Int64Codec : Codec<long>
{
    /// <summary>
    /// Decodes a 64-bit signed integer (long) from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded 64-bit signed integer.</returns>
    public override long Decode(ref Utf8JsonReader reader)
    {
        return reader.GetInt64();
    }

    /// <summary>
    /// Encodes a 64-bit signed integer (long) to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The 64-bit signed integer to encode.</param>
    public override void Encode(Utf8JsonWriter writer, long obj)
    {
        writer.WriteNumberValue(obj);
    }
}
