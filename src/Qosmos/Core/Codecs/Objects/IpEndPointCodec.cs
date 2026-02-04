// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Text.Json;

namespace Qosmos.Core.Codecs.Objects;

/// <summary>
/// A codec for encoding and decoding <see cref="IPEndPoint"/> objects to and from JSON.
/// </summary>
public sealed class IpEndPointCodec : Codec<IPEndPoint>
{
    /// <summary>
    /// Decodes a JSON string into an <see cref="IPEndPoint"/> object.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read the JSON data from.</param>
    /// <returns>An <see cref="IPEndPoint"/> object, or null if the JSON string is null.</returns>
    public override IPEndPoint? Decode(ref Utf8JsonReader reader)
    {
        return reader.GetString() is { } str
            ? IPEndPoint.Parse(str)
            : null;
    }

    /// <summary>
    /// Encodes an <see cref="IPEndPoint"/> object into a JSON string.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write the JSON data to.</param>
    /// <param name="obj">The <see cref="IPEndPoint"/> object to encode. Can be null.</param>
    public override void Encode(Utf8JsonWriter writer, IPEndPoint? obj)
    {
        writer.WriteStringValue(obj?.ToString());
    }
}
