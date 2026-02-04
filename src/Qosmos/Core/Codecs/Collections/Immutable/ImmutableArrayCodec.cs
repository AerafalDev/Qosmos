// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Text.Json;

namespace Qosmos.Core.Codecs.Collections.Immutable;

/// <summary>
/// Provides encoding and decoding functionality for immutable arrays of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of elements in the immutable array.</typeparam>
public sealed class ImmutableArrayCodec<T> : Codec<ImmutableArray<T>>
{
    private readonly Codec<T> _codec;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableArrayCodec{T}"/> class with the specified element codec.
    /// </summary>
    /// <param name="codec">The codec used to encode and decode individual elements of the immutable array.</param>
    public ImmutableArrayCodec(Codec<T> codec)
    {
        _codec = codec;
    }

    /// <summary>
    /// Decodes an immutable array of type <typeparamref name="T"/> from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded immutable array of type <typeparamref name="T"/>. Returns an empty immutable array if the JSON token is null.</returns>
    /// <exception cref="JsonException">Thrown if the JSON token is not an array or if decoding fails.</exception>
    public override ImmutableArray<T> Decode(ref Utf8JsonReader reader)
    {
        if (reader.TokenType is JsonTokenType.Null)
            return ImmutableArray<T>.Empty;

        if (reader.TokenType is not JsonTokenType.StartArray)
            throw new JsonException($"Expected start of array token but found {reader.TokenType}.");

        var values = ImmutableArray.CreateBuilder<T>();

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
    /// Encodes an immutable array of type <typeparamref name="T"/> to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The immutable array of type <typeparamref name="T"/> to encode.</param>
    public override void Encode(Utf8JsonWriter writer, ImmutableArray<T> obj)
    {
        writer.WriteStartArray();

        foreach (var value in obj)
            _codec.Encode(writer, value);

        writer.WriteEndArray();
    }
}
