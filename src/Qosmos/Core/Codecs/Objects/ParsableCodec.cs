// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Text.Json;

namespace Qosmos.Core.Codecs.Objects;

/// <summary>
/// A codec for encoding and decoding objects of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// The type of the object to be encoded or decoded. Must be a class and implement <see cref="IParsable{T}"/>.
/// </typeparam>
public sealed class ParsableCodec<T> : Codec<T>
    where T : class, IParsable<T>
{
    /// <summary>
    /// Decodes an object of type <typeparamref name="T"/> from a JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read the JSON data from.</param>
    /// <returns>
    /// The decoded object of type <typeparamref name="T"/>, or <c>null</c> if the JSON value is not a valid string.
    /// </returns>
    public override T? Decode(ref Utf8JsonReader reader)
    {
        return reader.GetString() is { } str
            ? T.Parse(str, CultureInfo.InvariantCulture)
            : null;
    }

    /// <summary>
    /// Encodes an object of type <typeparamref name="T"/> to a JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write the JSON data to.</param>
    /// <param name="obj">The object of type <typeparamref name="T"/> to encode. Can be <c>null</c>.</param>
    public override void Encode(Utf8JsonWriter writer, T? obj)
    {
        writer.WriteStringValue(obj?.ToString());
    }
}
