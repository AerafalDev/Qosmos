// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Text.Json;

namespace Qosmos.Core.Codecs.Collections.Immutable;

/// <summary>
/// A codec for encoding and decoding <see cref="ImmutableHashSet{T}"/> objects to and from JSON.
/// </summary>
/// <typeparam name="T">The type of the elements in the hash set.</typeparam>
public sealed class ImmutableHashSetCodec<T> : Codec<ImmutableHashSet<T>>
{
    private readonly Codec<T> _codec;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableHashSetCodec{T}"/> class.
    /// </summary>
    /// <param name="codec">The codec used to encode and decode the elements of the hash set.</param>
    public ImmutableHashSetCodec(Codec<T> codec)
    {
        _codec = codec;
    }

    /// <summary>
    /// Decodes a JSON array into an <see cref="ImmutableHashSet{T}"/>.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read the JSON data from.</param>
    /// <returns>An <see cref="ImmutableHashSet{T}"/> instance, or null if the JSON token is null.</returns>
    /// <exception cref="JsonException">Thrown if the JSON structure is invalid.</exception>
    public override ImmutableHashSet<T>? Decode(ref Utf8JsonReader reader)
    {
        if (reader.TokenType is JsonTokenType.Null)
            return null;

        if (reader.TokenType is not JsonTokenType.StartArray)
            throw new JsonException($"Expected start of array token but found {reader.TokenType}.");

        var values = ImmutableHashSet.CreateBuilder<T>();

        while (reader.Read())
        {
            if (reader.TokenType is JsonTokenType.EndArray)
                break;

            var value = _codec.Decode(ref reader);

            if (value is not null)
                values.Add(value);
        }

        return values.ToImmutable();
    }

    /// <summary>
    /// Encodes an <see cref="ImmutableHashSet{T}"/> into a JSON array.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write the JSON data to.</param>
    /// <param name="obj">The <see cref="ImmutableHashSet{T}"/> to encode. Can be null.</param>
    public override void Encode(Utf8JsonWriter writer, ImmutableHashSet<T>? obj)
    {
        if (obj is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();

        foreach (var value in obj)
            _codec.Encode(writer, value);

        writer.WriteEndArray();
    }
}
