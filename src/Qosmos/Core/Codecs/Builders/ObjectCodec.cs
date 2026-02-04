// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Builders;

/// <summary>
/// Represents a codec for encoding and decoding objects of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the object being encoded or decoded.</typeparam>
public sealed class ObjectCodec<T> : Codec<T>
    where T : class, new()
{
    private readonly IReadOnlyList<PropertyCodec> _properties;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectCodec{T}"/> class with the specified property codecs.
    /// </summary>
    /// <param name="properties">The list of property codecs used for encoding and decoding object properties.</param>
    public ObjectCodec(IReadOnlyList<PropertyCodec> properties)
    {
        _properties = properties;
    }

    /// <summary>
    /// Decodes an object of type <typeparamref name="T"/> from the specified JSON reader.
    /// </summary>
    /// <param name="reader">The JSON reader to decode the object from.</param>
    /// <returns>An instance of <typeparamref name="T"/> decoded from the JSON data.</returns>
    /// <exception cref="JsonException">Thrown when the JSON data is invalid or cannot be decoded.</exception>
    public override T Decode(ref Utf8JsonReader reader)
    {
        if (reader.TokenType is JsonTokenType.None && !reader.Read())
            throw new JsonException($"Cannot decode {typeof(T).Name}: no JSON data available to read.");

        var instance = new T();

        if (reader.TokenType is JsonTokenType.Null)
            return instance;

        if (reader.TokenType is not JsonTokenType.StartObject)
            throw new JsonException($"Cannot decode {typeof(T).Name}: expected start of object token but found {reader.TokenType}.");

        while (reader.Read())
        {
            if (reader.TokenType is JsonTokenType.EndObject)
                break;

            if (reader.TokenType is not JsonTokenType.PropertyName)
                throw new JsonException($"Cannot decode {typeof(T).Name}: expected property name token but found {reader.TokenType}.");

            var propertyName = reader.GetString();

            if (!reader.Read())
                throw new JsonException($"Cannot decode {typeof(T).Name}: unexpected end of JSON while reading value for property '{propertyName}'.");

            var property = _properties.FirstOrDefault(x => x.PropertyName == propertyName);

            if (property is not null)
                try
                {
                    property.Decode(ref reader, instance);
                }
                catch (JsonException ex)
                {
                    throw new JsonException($"Cannot decode {typeof(T).Name}: error decoding property '{propertyName}': {ex.Message}.", ex);
                }
            else
                reader.Skip();
        }

        return instance;
    }

    /// <summary>
    /// Encodes an object of type <typeparamref name="T"/> to the specified JSON writer.
    /// </summary>
    /// <param name="writer">The JSON writer to encode the object to.</param>
    /// <param name="obj">The object to encode. If null, a JSON null value is written.</param>
    /// <exception cref="JsonException">Thrown when an error occurs while encoding a property.</exception>
    public override void Encode(Utf8JsonWriter writer, T? obj)
    {
        if (obj is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();

        foreach (var property in _properties)
        {
            writer.WritePropertyName(property.PropertyName);

            try
            {
                property.Encode(writer, obj);
            }
            catch (Exception ex) when (ex is not JsonException)
            {
                throw new JsonException($"Cannot encode {typeof(T).Name}: error encoding property '{property.PropertyName}': {ex.Message}.", ex);
            }
        }

        writer.WriteEndObject();
    }
}
