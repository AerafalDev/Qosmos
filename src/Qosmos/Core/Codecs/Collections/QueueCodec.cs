// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Collections;

/// <summary>
/// Provides encoding and decoding functionality for queues of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of elements in the queue.</typeparam>
public sealed class QueueCodec<T> : Codec<Queue<T>>
{
    private readonly Codec<T> _codec;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueCodec{T}"/> class with the specified element codec.
    /// </summary>
    /// <param name="codec">The codec used to encode and decode individual elements of the queue.</param>
    public QueueCodec(Codec<T> codec)
    {
        _codec = codec;
    }

    /// <summary>
    /// Decodes a queue of type <typeparamref name="T"/> from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded queue of type <typeparamref name="T"/>. Returns <c>null</c> if the JSON token is null.</returns>
    /// <exception cref="JsonException">Thrown if the JSON token is not an array or if decoding fails.</exception>
    public override Queue<T>? Decode(ref Utf8JsonReader reader)
    {
        if (reader.TokenType is JsonTokenType.Null)
            return null;

        if (reader.TokenType is not JsonTokenType.StartArray)
            throw new JsonException($"Expected start of array token but found {reader.TokenType}.");

        var values = new Queue<T>();

        while (reader.Read())
        {
            if (reader.TokenType is JsonTokenType.EndArray)
                break;

            var value = _codec.Decode(ref reader);

            if (value is not null)
                values.Enqueue(value);
        }

        return values;
    }

    /// <summary>
    /// Encodes a queue of type <typeparamref name="T"/> to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The queue of type <typeparamref name="T"/> to encode. Writes a null value if the queue is null.</param>
    public override void Encode(Utf8JsonWriter writer, Queue<T>? obj)
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
