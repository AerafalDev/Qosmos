// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using Semver;

namespace Qosmos.Core.Codecs.Objects;

/// <summary>
/// A codec for encoding and decoding <see cref="SemVersionRange"/> objects to and from JSON.
/// </summary>
public sealed class SemVersionRangeCodec : Codec<SemVersionRange>
{
    /// <summary>
    /// Decodes a JSON string into a <see cref="SemVersionRange"/> object.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read the JSON data from.</param>
    /// <returns>A <see cref="SemVersionRange"/> object, or null if the JSON string is null.</returns>
    public override SemVersionRange? Decode(ref Utf8JsonReader reader)
    {
        return reader.GetString() is { } str
            ? SemVersionRange.Parse(str)
            : null;
    }

    /// <summary>
    /// Encodes a <see cref="SemVersionRange"/> object into a JSON string.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write the JSON data to.</param>
    /// <param name="obj">The <see cref="SemVersionRange"/> object to encode. Can be null.</param>
    public override void Encode(Utf8JsonWriter writer, SemVersionRange? obj)
    {
        writer.WriteStringValue(obj?.ToString());
    }
}
