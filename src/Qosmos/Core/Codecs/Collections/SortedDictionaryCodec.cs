// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Collections;

/// <summary>
/// Provides encoding and decoding functionality for sorted dictionaries with keys of type <typeparamref name="TKey"/>
/// and values of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TKey">The type of the sorted dictionary keys. Must be non-nullable.</typeparam>
/// <typeparam name="TValue">The type of the sorted dictionary values.</typeparam>
public sealed class SortedDictionaryCodec<TKey, TValue> : Codec<SortedDictionary<TKey, TValue>>
    where TKey : notnull
{
    private readonly Codec<TValue> _codec;
    private readonly Func<TKey, string> _keyToStringMethod;
    private readonly Func<string, TKey> _stringToKeyMethod;

    /// <summary>
    /// Initializes a new instance of the <see cref="SortedDictionaryCodec{TKey, TValue}"/> class with the specified codec
    /// and key conversion methods.
    /// </summary>
    /// <param name="codec">The codec used to encode and decode sorted dictionary values.</param>
    /// <param name="keyToStringMethod">The method to convert sorted dictionary keys to strings.</param>
    /// <param name="stringToKeyMethod">The method to convert strings to sorted dictionary keys.</param>
    public SortedDictionaryCodec(Codec<TValue> codec, Func<TKey, string> keyToStringMethod, Func<string, TKey> stringToKeyMethod)
    {
        _codec = codec;
        _keyToStringMethod = keyToStringMethod;
        _stringToKeyMethod = stringToKeyMethod;
    }

    /// <summary>
    /// Decodes a sorted dictionary of type <typeparamref name="TKey"/> and <typeparamref name="TValue"/>
    /// from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded sorted dictionary. Returns <c>null</c> if the JSON token is null.</returns>
    /// <exception cref="JsonException">Thrown if the JSON token is not an object or if decoding fails.</exception>
    public override SortedDictionary<TKey, TValue>? Decode(ref Utf8JsonReader reader)
    {
        if (reader.TokenType is JsonTokenType.Null)
            return null;

        if (reader.TokenType is not JsonTokenType.StartObject)
            throw new JsonException($"Expected start of object token but found {reader.TokenType}.");

        var values = new SortedDictionary<TKey, TValue>();

        while (reader.Read())
        {
            if (reader.TokenType is JsonTokenType.EndObject)
                break;

            if (reader.TokenType is not JsonTokenType.PropertyName)
                throw new JsonException($"Expected property name token but found {reader.TokenType}.");

            var keyStr = reader.GetString();

            if (string.IsNullOrEmpty(keyStr))
                throw new JsonException("SortedDictionary key cannot be null or empty.");

            var key = _stringToKeyMethod(keyStr);

            if (!reader.Read())
                throw new JsonException("Unexpected end of JSON while reading SortedDictionary value.");

            var value = _codec.Decode(ref reader);

            if (value is not null)
                values[key] = value;
        }

        return values;
    }

    /// <summary>
    /// Encodes a sorted dictionary of type <typeparamref name="TKey"/> and <typeparamref name="TValue"/>
    /// to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The sorted dictionary to encode. Writes a null value if the sorted dictionary is null.</param>
    public override void Encode(Utf8JsonWriter writer, SortedDictionary<TKey, TValue>? obj)
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
