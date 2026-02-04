// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Structs;

/// <summary>
/// A codec for encoding and decoding tuples of two elements <see cref="Tuple{T1, T2}"/> to and from JSON.
/// </summary>
/// <typeparam name="T1">The type of the first element in the tuple.</typeparam>
/// <typeparam name="T2">The type of the second element in the tuple.</typeparam>
public sealed class TupleCodec<T1, T2> : Codec<Tuple<T1?, T2?>>
{
    private readonly Codec<T1> _codec1;
    private readonly Codec<T2> _codec2;

    /// <summary>
    /// Initializes a new instance of the <see cref="TupleCodec{T1, T2}"/> class.
    /// </summary>
    /// <param name="codec1">The codec used to encode and decode the first element of the tuple.</param>
    /// <param name="codec2">The codec used to encode and decode the second element of the tuple.</param>
    public TupleCodec(Codec<T1> codec1, Codec<T2> codec2)
    {
        _codec1 = codec1;
        _codec2 = codec2;
    }

    /// <summary>
    /// Decodes a JSON array into a tuple of two elements <see cref="Tuple{T1, T2}"/>.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read the JSON data from.</param>
    /// <returns>A tuple of two elements <see cref="Tuple{T1, T2}"/>.</returns>
    /// <exception cref="JsonException">Thrown if the JSON structure is invalid.</exception>
    public override Tuple<T1?, T2?> Decode(ref Utf8JsonReader reader)
    {
        if (reader.TokenType is not JsonTokenType.StartArray)
            throw new JsonException("Expected start of array for tuple deserialization.");

        if (!reader.Read())
            throw new JsonException("Unexpected end of JSON while reading tuple item 1.");

        var item1 = _codec1.Decode(ref reader);

        if (!reader.Read())
            throw new JsonException("Unexpected end of JSON while reading tuple item 2.");

        var item2 = _codec2.Decode(ref reader);

        if (!reader.Read())
            throw new JsonException("Unexpected end of JSON while reading end of tuple array.");

        if (reader.TokenType is not JsonTokenType.EndArray)
            throw new JsonException("Expected end of array for tuple deserialization.");

        return Tuple.Create(item1, item2);
    }

    /// <summary>
    /// Encodes a tuple of two elements <see cref="Tuple{T1, T2}"/> into a JSON array.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write the JSON data to.</param>
    /// <param name="obj">The tuple of two elements <see cref="Tuple{T1, T2}"/> to encode. Can be null.</param>
    public override void Encode(Utf8JsonWriter writer, Tuple<T1?, T2?>? obj)
    {
        if (obj is null || (obj.Item1 is null && obj.Item2 is null))
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();

        _codec1.Encode(writer, obj.Item1);
        _codec2.Encode(writer, obj.Item2);

        writer.WriteEndArray();
    }
}
