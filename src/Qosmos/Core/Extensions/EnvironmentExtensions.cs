// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qosmos.Core.Extensions;

/// <summary>
/// Provides extension methods for working with environment variables
/// specific to .NET and Qosmos.
/// </summary>
public static class EnvironmentExtensions
{
    extension(Environment)
    {
        /// <summary>
        /// Gets the prefix used for .NET environment variables.
        /// </summary>
        public static string DotnetPrefix =>
            "DOTNET_";

        /// <summary>
        /// Gets the prefix used for Qosmos environment variables.
        /// </summary>
        public static string QosmosPrefix =>
            "QOSMOS_";

        /// <summary>
        /// Retrieves the value of a .NET-specific environment variable.
        /// </summary>
        /// <param name="variable">The name of the environment variable (without the prefix).</param>
        /// <returns>The value of the environment variable, or null if it does not exist.</returns>
        public static string? GetDotnetEnvironmentVariable(string variable)
        {
            return Environment.GetEnvironmentVariable(string.Concat(Environment.DotnetPrefix, variable));
        }

        /// <summary>
        /// Retrieves the value of a Qosmos-specific environment variable.
        /// </summary>
        /// <param name="variable">The name of the environment variable (without the prefix).</param>
        /// <returns>The value of the environment variable, or null if it does not exist.</returns>
        public static string? GetQosmosEnvironmentVariable(string variable)
        {
            return Environment.GetEnvironmentVariable(string.Concat(Environment.QosmosPrefix, variable));
        }
    }
}
