// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Builders;

/// <summary>
/// Represents an abstract base class for property codecs, which define methods for encoding and decoding properties.
/// </summary>
public abstract class PropertyCodec
{
    /// <summary>
    /// Gets the name of the property associated with the codec.
    /// </summary>
    public abstract string PropertyName { get; }

    /// <summary>
    /// Decodes a property value from the specified JSON reader and assigns it to the given object.
    /// </summary>
    /// <param name="reader">The JSON reader to decode the property value from.</param>
    /// <param name="obj">The object to assign the decoded property value to.</param>
    public abstract void Decode(ref Utf8JsonReader reader, object obj);

    /// <summary>
    /// Encodes a property value from the given object and writes it to the specified JSON writer.
    /// </summary>
    /// <param name="writer">The JSON writer to encode the property value to.</param>
    /// <param name="obj">The object to retrieve the property value from.</param>
    public abstract void Encode(Utf8JsonWriter writer, object obj);
}

/// <summary>
/// Represents a generic implementation of <see cref="PropertyCodec"/> for a specific property type.
/// </summary>
/// <typeparam name="T">The type of the object containing the property.</typeparam>
/// <typeparam name="TProperty">The type of the property.</typeparam>
public sealed class PropertyCodec<T, TProperty> : PropertyCodec
    where T : class, new()
{
    /// <summary>
    /// Gets the name of the property associated with the codec.
    /// </summary>
    public override string PropertyName { get; }

    /// <summary>
    /// Gets the codec used for encoding and decoding the property value.
    /// </summary>
    public Codec<TProperty> Codec { get; }

    /// <summary>
    /// Gets the function used to retrieve the property value from the object.
    /// </summary>
    public Func<T, TProperty> Getter { get; }

    /// <summary>
    /// Gets the action used to assign the property value to the object.
    /// </summary>
    public Action<T, TProperty> Setter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyCodec{T, TProperty}"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="codec">The codec used for encoding and decoding the property value.</param>
    /// <param name="getter">The function used to retrieve the property value.</param>
    /// <param name="setter">The action used to assign the property value.</param>
    public PropertyCodec(string propertyName, Codec<TProperty> codec, Func<T, TProperty> getter, Action<T, TProperty> setter)
    {
        PropertyName = propertyName;
        Codec = codec;
        Getter = getter;
        Setter = setter;
    }

    /// <summary>
    /// Decodes a property value from the specified JSON reader and assigns it to the given object.
    /// </summary>
    /// <param name="reader">The JSON reader to decode the property value from.</param>
    /// <param name="obj">The object to assign the decoded property value to.</param>
    public override void Decode(ref Utf8JsonReader reader, object obj)
    {
        if (Codec.Decode(ref reader) is { } property)
            Setter((T)obj, property);
    }

    /// <summary>
    /// Encodes a property value from the given object and writes it to the specified JSON writer.
    /// </summary>
    /// <param name="writer">The JSON writer to encode the property value to.</param>
    /// <param name="obj">The object to retrieve the property value from.</param>
    public override void Encode(Utf8JsonWriter writer, object obj)
    {
        Codec.Encode(writer, Getter((T)obj));
    }
}
