// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Primitives;

/// <summary>
/// Provides encoding and decoding functionality for single-precision floating-point numbers (float).
/// </summary>
public sealed class SingleCodec : Codec<float>
{
    /// <summary>
    /// Decodes a single-precision floating-point number (float) from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded single-precision floating-point number.</returns>
    public override float Decode(ref Utf8JsonReader reader)
    {
        return reader.GetSingle();
    }

    /// <summary>
    /// Encodes a single-precision floating-point number (float) to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The single-precision floating-point number to encode.</param>
    public override void Encode(Utf8JsonWriter writer, float obj)
    {
        writer.WriteNumberValue(obj);
    }
}
