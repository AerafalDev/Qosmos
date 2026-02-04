// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;

namespace Qosmos.SourceGenerators.Codecs.Models;

/// <summary>
/// Represents a codec, including its namespace, name, self-reference status, and properties.
/// </summary>
/// <param name="Namespace">The namespace of the codec.</param>
/// <param name="Name">The name of the codec.</param>
/// <param name="ContainsSelfReference">Indicates whether the codec contains a self-reference.</param>
/// <param name="Properties">The properties of the codec.</param>
public sealed record Codec(
    string Namespace,
    string Name,
    bool ContainsSelfReference,
    ImmutableArray<CodecProperty> Properties
);
