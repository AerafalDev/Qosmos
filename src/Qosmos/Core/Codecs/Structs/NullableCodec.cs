// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Structs;

/// <summary>
/// Provides encoding and decoding functionality for nullable value types.
/// </summary>
/// <typeparam name="T">The underlying value type that is nullable.</typeparam>
public sealed class NullableCodec<T> : Codec<T?>
    where T : struct
{
    private readonly Codec<T> _codec;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableCodec{T}"/> class.
    /// </summary>
    /// <param name="codec">The codec used to encode and decode the underlying value type.</param>
    public NullableCodec(Codec<T> codec)
    {
        _codec = codec;
    }

    /// <summary>
    /// Decodes a nullable value type from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded nullable value type. Returns <c>null</c> if the JSON token is null.</returns>
    public override T? Decode(ref Utf8JsonReader reader)
    {
        return reader.TokenType is JsonTokenType.Null
            ? null
            : _codec.Decode(ref reader);
    }

    /// <summary>
    /// Encodes a nullable value type to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The nullable value type to encode. Writes a null value if the object is null.</param>
    public override void Encode(Utf8JsonWriter writer, T? obj)
    {
        if (obj.HasValue)
            _codec.Encode(writer, obj.Value);
        else
            writer.WriteNullValue();
    }
}
