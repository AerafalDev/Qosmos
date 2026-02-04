// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Structs;

/// <summary>
/// A codec for encoding and decoding <see cref="Guid"/> values to and from JSON.
/// </summary>
public sealed class GuidCodec : Codec<Guid>
{
    /// <summary>
    /// Decodes a JSON value into a <see cref="Guid"/>.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read the JSON data from.</param>
    /// <returns>A <see cref="Guid"/> instance.</returns>
    public override Guid Decode(ref Utf8JsonReader reader)
    {
        return reader.GetGuid();
    }

    /// <summary>
    /// Encodes a <see cref="Guid"/> into a JSON value.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write the JSON data to.</param>
    /// <param name="obj">The <see cref="Guid"/> to encode.</param>
    public override void Encode(Utf8JsonWriter writer, Guid obj)
    {
        writer.WriteStringValue(obj);
    }
}
