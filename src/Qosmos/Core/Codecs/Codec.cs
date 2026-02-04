// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs;

/// <summary>
/// Represents an abstract base class for encoding and decoding objects of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of object to be encoded and decoded.</typeparam>
public abstract class Codec<T>
{
    /// <summary>
    /// Decodes an object of type <typeparamref name="T"/> from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded object of type <typeparamref name="T"/>, or null if decoding fails.</returns>
    public abstract T? Decode(ref Utf8JsonReader reader);

    /// <summary>
    /// Encodes an object of type <typeparamref name="T"/> to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The object of type <typeparamref name="T"/> to encode. Can be null.</param>
    public abstract void Encode(Utf8JsonWriter writer, T? obj);
}
