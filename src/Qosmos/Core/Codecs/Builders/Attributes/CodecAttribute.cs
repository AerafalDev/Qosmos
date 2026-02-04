// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qosmos.Core.Codecs.Builders.Attributes;

/// <summary>
/// An attribute used to mark a class as a codec.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class CodecAttribute : Attribute;
