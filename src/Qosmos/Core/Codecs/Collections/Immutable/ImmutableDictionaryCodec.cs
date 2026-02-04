// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Text.Json;

namespace Qosmos.Core.Codecs.Collections.Immutable;

/// <summary>
/// A codec for encoding and decoding <see cref="ImmutableDictionary{TKey, TValue}"/> objects to and from JSON.
/// </summary>
/// <typeparam name="TKey">The type of the dictionary keys. Must be non-nullable.</typeparam>
/// <typeparam name="TValue">The type of the dictionary values.</typeparam>
public sealed class ImmutableDictionaryCodec<TKey, TValue> : Codec<ImmutableDictionary<TKey, TValue>>
    where TKey : notnull
{
    private readonly Codec<TValue> _codec;
    private readonly Func<TKey, string> _keyToStringMethod;
    private readonly Func<string, TKey> _stringToKeyMethod;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableDictionaryCodec{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="codec">The codec used to encode and decode the dictionary values.</param>
    /// <param name="keyToStringMethod">A function to convert dictionary keys to strings.</param>
    /// <param name="stringToKeyMethod">A function to convert strings to dictionary keys.</param>
    public ImmutableDictionaryCodec(Codec<TValue> codec, Func<TKey, string> keyToStringMethod, Func<string, TKey> stringToKeyMethod)
    {
        _codec = codec;
        _keyToStringMethod = keyToStringMethod;
        _stringToKeyMethod = stringToKeyMethod;
    }

    /// <summary>
    /// Decodes a JSON object into an <see cref="ImmutableDictionary{TKey, TValue}"/>.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read the JSON data from.</param>
    /// <returns>An <see cref="ImmutableDictionary{TKey, TValue}"/> instance, or null if the JSON token is null.</returns>
    /// <exception cref="JsonException">Thrown if the JSON structure is invalid.</exception>
    public override ImmutableDictionary<TKey, TValue>? Decode(ref Utf8JsonReader reader)
    {
        if (reader.TokenType is JsonTokenType.Null)
            return null;

        if (reader.TokenType is not JsonTokenType.StartObject)
            throw new JsonException($"Expected start of object token but found {reader.TokenType}.");

        var values = ImmutableDictionary.CreateBuilder<TKey, TValue>();

        while (reader.Read())
        {
            if (reader.TokenType is JsonTokenType.EndObject)
                break;

            if (reader.TokenType is not JsonTokenType.PropertyName)
                throw new JsonException($"Expected property name token but found {reader.TokenType}.");

            var keyStr = reader.GetString();

            if (string.IsNullOrEmpty(keyStr))
                throw new JsonException("ImmutableDictionary key cannot be null or empty.");

            var key = _stringToKeyMethod(keyStr);

            if (!reader.Read())
                throw new JsonException("Unexpected end of JSON while reading ImmutableDictionary value.");

            var value = _codec.Decode(ref reader);

            if (value is not null)
                values[key] = value;
        }

        return values.ToImmutable();
    }

    /// <summary>
    /// Encodes an <see cref="ImmutableDictionary{TKey, TValue}"/> into a JSON object.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write the JSON data to.</param>
    /// <param name="obj">The <see cref="ImmutableDictionary{TKey, TValue}"/> to encode. Can be null.</param>
    public override void Encode(Utf8JsonWriter writer, ImmutableDictionary<TKey, TValue>? obj)
    {
        if (obj is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();

        foreach (var (key, value) in obj)
        {
            var keyStr = _keyToStringMethod(key);

            writer.WritePropertyName(keyStr);

            _codec.Encode(writer, value);
        }

        writer.WriteEndObject();
    }
}
