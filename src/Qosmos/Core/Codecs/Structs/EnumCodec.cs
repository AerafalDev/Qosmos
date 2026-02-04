// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace Qosmos.Core.Codecs.Structs;

/// <summary>
/// Provides encoding and decoding functionality for enumerations of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The enumeration type to encode and decode. Must be a struct and an enumeration.</typeparam>
public sealed class EnumCodec<T> : Codec<T>
    where T : struct, Enum
{
    private static readonly Type s_enumType = typeof(T);
    private static readonly Type s_enumUnderlyingType = Enum.GetUnderlyingType(s_enumType);

    private static readonly ReadDelegate s_cachedReadMethod;
    private static readonly WriteDelegate s_cachedWriteMethod;

    /// <summary>
    /// Static constructor to initialize the cached read and write methods based on the underlying type of the enumeration.
    /// </summary>
    static EnumCodec()
    {
        s_cachedReadMethod = s_enumUnderlyingType switch
        {
            var type when type == typeof(byte) => (ref reader) => reader.GetByte(),
            var type when type == typeof(sbyte) => (ref reader) => reader.GetSByte(),
            var type when type == typeof(short) => (ref reader) => reader.GetInt16(),
            var type when type == typeof(ushort) => (ref reader) => reader.GetUInt16(),
            var type when type == typeof(int) => (ref reader) => reader.GetInt32(),
            var type when type == typeof(uint) => (ref reader) => reader.GetUInt32(),
            var type when type == typeof(long) => (ref reader) => reader.GetInt64(),
            var type when type == typeof(ulong) => (ref reader) => reader.GetUInt64(),
            _ => throw new NotSupportedException($"Unsupported enum underlying type: {s_enumUnderlyingType}.")
        };

        s_cachedWriteMethod = s_enumUnderlyingType switch
        {
            var type when type == typeof(byte) => (writer, value) => writer.WriteNumberValue(Convert.ToByte(value)),
            var type when type == typeof(sbyte) => (writer, value) => writer.WriteNumberValue(Convert.ToSByte(value)),
            var type when type == typeof(short) => (writer, value) => writer.WriteNumberValue(Convert.ToInt16(value)),
            var type when type == typeof(ushort) => (writer, value) => writer.WriteNumberValue(Convert.ToUInt16(value)),
            var type when type == typeof(int) => (writer, value) => writer.WriteNumberValue(Convert.ToInt32(value)),
            var type when type == typeof(uint) => (writer, value) => writer.WriteNumberValue(Convert.ToUInt32(value)),
            var type when type == typeof(long) => (writer, value) => writer.WriteNumberValue(Convert.ToInt64(value)),
            var type when type == typeof(ulong) => (writer, value) => writer.WriteNumberValue(Convert.ToUInt64(value)),
            _ => throw new NotSupportedException($"Unsupported enum underlying type: {s_enumUnderlyingType}.")
        };
    }

    /// <summary>
    /// Decodes an enumeration value of type <typeparamref name="T"/> from the provided JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The decoded enumeration value of type <typeparamref name="T"/>.</returns>
    public override T Decode(ref Utf8JsonReader reader)
    {
        return (T)Enum.ToObject(s_enumType, s_cachedReadMethod(ref reader));
    }

    /// <summary>
    /// Encodes an enumeration value of type <typeparamref name="T"/> to the provided JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The enumeration value of type <typeparamref name="T"/> to encode.</param>
    public override void Encode(Utf8JsonWriter writer, T obj)
    {
        s_cachedWriteMethod(writer, obj);
    }

    /// <summary>
    /// Delegate for reading an enumeration value from a JSON reader.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read JSON data from.</param>
    /// <returns>The read enumeration value as an object.</returns>
    private delegate object ReadDelegate(ref Utf8JsonReader reader);

    /// <summary>
    /// Delegate for writing an enumeration value to a JSON writer.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write JSON data to.</param>
    /// <param name="obj">The enumeration value to write.</param>
    private delegate void WriteDelegate(Utf8JsonWriter writer, T obj);
}
