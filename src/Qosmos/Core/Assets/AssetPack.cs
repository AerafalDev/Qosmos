// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO.Compression;
using OneOf;

namespace Qosmos.Core.Assets;

/// <summary>
/// Represents a source that can either be a <see cref="ZipArchive"/> or a <see cref="DirectoryInfo"/>.
/// </summary>
public sealed class ZipOrDirectory : OneOfBase<ZipArchive, DirectoryInfo>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ZipOrDirectory"/> class.
    /// </summary>
    /// <param name="input">The source, which can be either a <see cref="ZipArchive"/> or a <see cref="DirectoryInfo"/>.</param>
    public ZipOrDirectory(OneOf<ZipArchive, DirectoryInfo> input) : base(input)
    {
    }
}

/// <summary>
/// Represents a pack of assets, which can be sourced from a zip file or a directory.
/// </summary>
public sealed class AssetPack
{
    /// <summary>
    /// Gets the name of the asset pack.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the path to the asset pack.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets a value indicating whether the asset pack is immutable.
    /// </summary>
    public bool IsImmutable { get; }

    /// <summary>
    /// Gets the source of the asset pack, which can be a zip file or a directory.
    /// </summary>
    public ZipOrDirectory Source { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetPack"/> class.
    /// </summary>
    /// <param name="name">The name of the asset pack.</param>
    /// <param name="path">The path to the asset pack.</param>
    /// <param name="isImmutable">A value indicating whether the asset pack is immutable.</param>
    /// <param name="source">The source of the asset pack, which can be a zip file or a directory.</param>
    public AssetPack(string name, string path, bool isImmutable, ZipOrDirectory source)
    {
        Name = name;
        Path = path;
        IsImmutable = isImmutable;
        Source = source;
    }

    /// <summary>
    /// Returns a string representation of the asset pack.
    /// </summary>
    /// <returns>A string that represents the asset pack.</returns>
    public override string ToString()
    {
        return $"AssetPack(Name={Name}, Path={Path}, IsImmutable={IsImmutable}, Source={(Source.IsT0 ? "Zip" : "Directory")})";
    }
}
