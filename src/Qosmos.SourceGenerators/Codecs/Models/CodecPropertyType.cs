// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;

namespace Qosmos.SourceGenerators.Codecs.Models;

/// <summary>
/// Represents the type of a property in a codec, including its name, nullability,
/// whether it is an enumeration, and its type arguments.
/// </summary>
/// <param name="Name">The name of the property type.</param>
/// <param name="IsNullable">Indicates whether the property type is nullable.</param>
/// <param name="IsEnum">Indicates whether the property type is an enumeration.</param>
/// <param name="TypeArguments">The type arguments of the property type, if any.</param>
public sealed record CodecPropertyType(
    string Name,
    bool IsNullable,
    bool IsEnum,
    ImmutableArray<CodecPropertyType> TypeArguments
);
