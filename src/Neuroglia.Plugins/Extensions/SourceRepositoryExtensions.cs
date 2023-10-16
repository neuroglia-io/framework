// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol.Plugins;
using NuGet.Versioning;
using System.Runtime.Versioning;
using System.Reflection;

namespace Neuroglia.Plugins;

/// <summary>
/// Defines extensions for <see cref="SourceRepository"/> instances
/// </summary>
public static class SourceRepositoryExtensions
{

    static readonly NuGetFramework CurrentFramework = NuGetFramework.Parse(typeof(PluginManager).Assembly.GetCustomAttribute<TargetFrameworkAttribute>()!.FrameworkName!);

    /// <summary>
    /// Downloads and extracts the specified nuget package and all its dependencies
    /// </summary>
    /// <param name="repository">The extended <see cref="SourceRepository"/></param>
    /// <param name="package">The identity of the package to download</param>
    /// <param name="outputDirectory">The directory to download and extract the package to</param>
    /// <param name="cache">The <see cref="SourceCacheContext"/> to use</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    public static async ValueTask DownloadAndExtractPackageAsync(this SourceRepository repository, PackageIdentity package, DirectoryInfo outputDirectory, SourceCacheContext cache, CancellationToken cancellationToken = default)
    {
        var packageFileName = Path.Combine(outputDirectory.FullName, $"{package.Id}.{package.Version.OriginalVersion}.nupkg");
        Stream packageStream;
        if (File.Exists(packageFileName))
        {
            packageStream = File.OpenRead(packageFileName);
        }
        else
        {
            packageStream = File.Open(packageFileName, FileMode.Create);
            var findPackageById = await repository.GetResourceAsync<FindPackageByIdResource>().ConfigureAwait(false);
            await findPackageById.CopyNupkgToStreamAsync(package.Id, package.Version, packageStream, cache, NullLogger.Instance, cancellationToken).ConfigureAwait(false);
            await packageStream.FlushAsync(cancellationToken).ConfigureAwait(false);
            packageStream.Position = 0;
        }
        using var packageReader = new PackageArchiveReader(packageStream);
        var libItems = await packageReader.GetLibItemsAsync(cancellationToken).ConfigureAwait(false);
        var framework = libItems.Where(f => DefaultCompatibilityProvider.Instance.IsCompatible(CurrentFramework, f.TargetFramework)).LastOrDefault();
        if (framework != null)
        {
            foreach (var item in framework.Items)
            {
                var outputFile = Path.Combine(outputDirectory.FullName, item.Split('/').Last());
                packageReader.ExtractFile(item, outputFile, NullLogger.Instance);
            }
        }
        await packageStream.DisposeAsync().ConfigureAwait(false);
        File.Delete(packageFileName);
    }

    /// <summary>
    /// Gets the latest version of the specified Nuget package
    /// </summary>
    /// <param name="repository">The extended <see cref="SourceRepository"/></param>
    /// <param name="id">The id of the package to get the latest version of</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The latest version of the specified Nuget package</returns>
    /// <exception cref="NullReferenceException"></exception>
    public static async ValueTask<NuGetVersion> GetPackageLatestVersionAsync(this SourceRepository repository, string id, CancellationToken cancellationToken = default)
    {
        var search = await repository.GetResourceAsync<PackageSearchResource>().ConfigureAwait(false);
        var searchFilter = new SearchFilter(false);
        var searchResults = await search.SearchAsync(id, searchFilter, 0, 100, NuGet.Common.NullLogger.Instance, cancellationToken).ConfigureAwait(false);
        var searchResult = searchResults.FirstOrDefault() ?? throw new NullReferenceException($"Failed to find nuget package with id '{id}' in source '{repository.PackageSource.SourceUri}'");
        var versions = await searchResult.GetVersionsAsync().ConfigureAwait(false);
        var versionInfo = versions.OrderByDescending(v => v.Version.Version).First();
        return new(versionInfo.Version);
    }

    /// <summary>
    /// Lists all the dependencies of the specified package
    /// </summary>
    /// <param name="repository">The extended <see cref="SourceRepository"/></param>
    /// <param name="package">The identity of the package to list the dependencies of</param>
    /// <param name="cache">The current <see cref="SourceCacheContext"/></param>
    /// <param name="recursive">A boolean indicating whether or not to list the packages recursively</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="List{T}"/> containing the dependencies of the specified package</returns>
    public static async ValueTask<List<PackageDependency>> ListPackageDependenciesAsync(this SourceRepository repository, PackageIdentity package, SourceCacheContext cache, bool recursive = true, CancellationToken cancellationToken = default)
    {
        var dependencyResolver = await repository.GetResourceAsync<DependencyInfoResource>().ConfigureAwait(false);
        var dependencyTree = await dependencyResolver.ResolvePackage(package, CurrentFramework, cache, new NullLogger(), default);
        var dependencies = new List<PackageDependency>(dependencyTree.Dependencies);
        foreach (var dependency in dependencyTree.Dependencies)
        {
            var version = dependency.VersionRange.HasUpperBound ? dependency.VersionRange.MaxVersion : dependency.VersionRange.MinVersion;
            dependencies.AddRange(await repository.ListPackageDependenciesAsync(new(dependency.Id, version), cache, recursive, cancellationToken).ConfigureAwait(false));
        }
        return dependencies.GroupBy(d => d.Id).Where(g => !g.Key.StartsWith("System.")).Select(g => g.OrderByDescending(d => d.VersionRange.HasUpperBound ? d.VersionRange.MaxVersion : d.VersionRange.MinVersion).First()).ToList();
    }

}