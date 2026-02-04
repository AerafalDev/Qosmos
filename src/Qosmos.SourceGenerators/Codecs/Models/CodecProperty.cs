// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qosmos.SourceGenerators.Codecs.Models;

/// <summary>
/// Represents a property in a codec, including its type and name.
/// </summary>
/// <param name="Type">The type of the codec property.</param>
/// <param name="Name">The name of the codec property.</param>
public sealed record CodecProperty(CodecPropertyType Type, string Name);
