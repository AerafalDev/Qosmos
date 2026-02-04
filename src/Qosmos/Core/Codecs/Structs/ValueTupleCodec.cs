// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Structs;

/// <summary>
/// A codec for encoding and decoding <see cref="ValueTuple{T1, T2}"/> objects to and from JSON.
/// </summary>
/// <typeparam name="T1">The type of the first element in the ValueTuple.</typeparam>
/// <typeparam name="T2">The type of the second element in the ValueTuple.</typeparam>
public sealed class ValueTupleCodec<T1, T2> : Codec<ValueTuple<T1?, T2?>>
{
    private readonly Codec<T1> _codec1;
    private readonly Codec<T2> _codec2;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueTupleCodec{T1, T2}"/> class.
    /// </summary>
    /// <param name="codec1">The codec used to encode and decode the first element of the ValueTuple.</param>
    /// <param name="codec2">The codec used to encode and decode the second element of the ValueTuple.</param>
    public ValueTupleCodec(Codec<T1> codec1, Codec<T2> codec2)
    {
        _codec1 = codec1;
        _codec2 = codec2;
    }

    /// <summary>
    /// Decodes a JSON array into a <see cref="ValueTuple{T1, T2}"/>.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read the JSON data from.</param>
    /// <returns>A <see cref="ValueTuple{T1, T2}"/> instance.</returns>
    /// <exception cref="JsonException">Thrown if the JSON structure is invalid.</exception>
    public override ValueTuple<T1?, T2?> Decode(ref Utf8JsonReader reader)
    {
        if (reader.TokenType is not JsonTokenType.StartArray)
            throw new JsonException("Expected start of array for ValueTuple deserialization.");

        if (!reader.Read())
            throw new JsonException("Unexpected end of JSON while reading ValueTuple item 1.");

        var item1 = _codec1.Decode(ref reader);

        if (!reader.Read())
            throw new JsonException("Unexpected end of JSON while reading ValueTuple item 2.");

        var item2 = _codec2.Decode(ref reader);

        if (!reader.Read())
            throw new JsonException("Unexpected end of JSON while reading end of ValueTuple array.");

        if (reader.TokenType is not JsonTokenType.EndArray)
            throw new JsonException("Expected end of array for ValueTuple deserialization.");

        return ValueTuple.Create(item1, item2);
    }

    /// <summary>
    /// Encodes a <see cref="ValueTuple{T1, T2}"/> into a JSON array.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write the JSON data to.</param>
    /// <param name="obj">The <see cref="ValueTuple{T1, T2}"/> to encode.</param>
    public override void Encode(Utf8JsonWriter writer, ValueTuple<T1?, T2?> obj)
    {
        if (obj.Item1 is null && obj.Item2 is null)
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
