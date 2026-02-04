// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using CommandLine;

namespace Qosmos.Core.Network.Hosting;

/// <summary>
/// Represents the command-line options for configuring a Qosmos application.
/// </summary>
public sealed class QosmosApplicationOptions
{
    /// <summary>
    /// Gets or init the path to the assets directory or archive file.
    /// </summary>
    /// <remarks>
    /// This option is not required and defaults to "Assets/Assets.zip".
    /// </remarks>
    [Option("assets", Required = false, Default = "Assets/Assets.zip", HelpText = "Path to the assets directory or archive file")]
    public required string AssetsPath { get; init; }
}
