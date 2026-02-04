// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qosmos.Core.Codecs.Builders;

/// <summary>
/// Represents a codec builder for creating object codecs with property mappings.
/// </summary>
/// <typeparam name="T">The type of the object being encoded or decoded.</typeparam>
public sealed class BuilderCodec<T>
    where T : class, new()
{
    private readonly List<PropertyCodec> _properties;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuilderCodec{T}"/> class.
    /// </summary>
    public BuilderCodec()
    {
        _properties = [];
    }

    /// <summary>
    /// Adds a property codec to the builder.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="codec">The codec used for encoding and decoding the property value.</param>
    /// <param name="getter">The function used to retrieve the property value.</param>
    /// <param name="setter">The action used to assign the property value.</param>
    /// <returns>The current <see cref="BuilderCodec{T}"/> instance.</returns>
    public BuilderCodec<T> Add<TProperty>(string propertyName, Codec<TProperty> codec, Func<T, TProperty> getter, Action<T, TProperty> setter)
    {
        _properties.Add(new PropertyCodec<T, TProperty>(propertyName, codec, getter, setter));

        return this;
    }

    /// <summary>
    /// Builds and returns a codec for the object type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>A <see cref="Codec{T}"/> instance for the object type.</returns>
    public Codec<T> Build()
    {
        return new ObjectCodec<T>(_properties);
    }
}
