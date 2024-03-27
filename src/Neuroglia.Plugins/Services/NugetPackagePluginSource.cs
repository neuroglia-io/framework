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

using Microsoft.Extensions.Logging;
using Neuroglia.Plugins.Configuration;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.PackageExtraction;
using NuGet.Packaging.Signing;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol.Plugins;
using NuGet.Resolver;
using NuGet.Versioning;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Represents an <see cref="IPluginSource"/> implementation that relies on a Nuget package to provide plugins
/// </summary>
public class NugetPackagePluginSource
    : IPluginSource
{

    const string DefaultPackageSource = "https://api.nuget.org/v3/index.json";
    readonly List<AssemblyPluginSource> _assemblies = [];

    /// <summary>
    /// Initializes a new <see cref="NugetPackagePluginSource"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="Microsoft.Extensions.Logging.ILogger"/>s</param>
    /// <param name="name">The name of the source, if any</param>
    /// <param name="options">The source's options</param>
    /// <param name="packageId">The id of the Nuget package to use</param>
    /// <param name="packageVersion">The version, if any, of the Nuget package to use</param>
    /// <param name="packageSourceUri">The package source to use</param>
    /// <param name="includePreRelease">A boolean indicating whether or not to include pre-release packages</param>
    public NugetPackagePluginSource(ILoggerFactory loggerFactory, string? name, PluginSourceOptions options, string packageId, string? packageVersion = null, Uri? packageSourceUri = null, bool includePreRelease = false)
    {
        if (string.IsNullOrWhiteSpace(packageId)) throw new ArgumentNullException(nameof(packageId));

        this.LoggerFactory = loggerFactory;
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Name = name;
        this.NugetLogger = new LoggingExtensionLogger(this.Logger);
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
        this.PackageId = packageId;
        this.PackageVersion = packageVersion;
        this.PackageSourceUri = packageSourceUri;
        this.IncludePreRelease = includePreRelease;

        this.PackagesDirectory =  new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins", packageId, packageVersion ?? "latest", ".nuget"));        
        if (!this.PackagesDirectory.Exists) this.PackagesDirectory.Create();

        this.PluginDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins", packageId, packageVersion ?? "latest"));
        if (!this.PluginDirectory.Exists) this.PluginDirectory.Create();

        this.PackageSource = this.PackageSourceUri == null ? null : this.BuildPackageSource(this.PackageSourceUri);
        this.SourceRepository = packageSourceUri == null ? null : new(this.PackageSource, global::Neuroglia.Plugins.Services.NugetPackagePluginSource.Providers);
    }

    /// <inheritdoc/>
    public virtual string? Name { get; }

    /// <summary>
    /// Gets the service used to create <see cref="Microsoft.Extensions.Logging.ILogger"/>s
    /// </summary>
    protected ILoggerFactory LoggerFactory { get; }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected Microsoft.Extensions.Logging.ILogger Logger { get; }

    /// <summary>
    /// Gets the service used by Nuget to perform logging
    /// </summary>
    protected NuGet.Common.ILogger NugetLogger { get; }

    /// <summary>
    /// Gets the source's options
    /// </summary>
    protected PluginSourceOptions Options { get; }

    /// <summary>
    /// Gets the id of the Nuget package to use
    /// </summary>
    public virtual string PackageId { get; protected set; }

    /// <summary>
    /// Gets the version, if any, of the Nuget package to use
    /// </summary>
    public virtual string? PackageVersion { get; protected set; }

    /// <summary>
    /// Gets the uri of the package source to use
    /// </summary>
    public virtual Uri? PackageSourceUri { get; protected set; }

    /// <summary>
    /// Gets the directory to output packages to
    /// </summary>
    public virtual DirectoryInfo PackagesDirectory { get; protected set; }

    /// <summary>
    /// Gets the plugin directory
    /// </summary>
    public virtual DirectoryInfo PluginDirectory { get; protected set; }

    /// <summary>
    /// Gets a boolean indicating whether or not to include pre-release packages
    /// </summary>
    public virtual bool IncludePreRelease { get; protected set; }

    /// <inheritdoc/>
    public virtual bool IsLoaded { get; protected set; }

    /// <summary>
    /// Gets the <see cref="NuGet.Configuration.PackageSource"/> to use
    /// </summary>
    protected PackageSource? PackageSource { get; }

    /// <summary>
    /// Gets the <see cref="NuGet.Protocol.Core.Types.SourceRepository"/> to use
    /// </summary>
    protected SourceRepository? SourceRepository { get; }

    /// <inheritdoc/>
    public virtual IReadOnlyList<IPluginDescriptor> Plugins => this.IsLoaded ? this._assemblies.SelectMany(a => a.Plugins).ToList().AsReadOnly() : throw new NotSupportedException("The plugin source has not yet been loaded");

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing the <see cref="INuGetResourceProvider"/>s to use
    /// </summary>
    protected static IEnumerable<Lazy<INuGetResourceProvider>> Providers => Repository.Provider.GetCoreV3().ToList();

    /// <inheritdoc/>
    public virtual async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (this.IsLoaded) throw new NotSupportedException("The plugin source is already loaded");

        var settings = Settings.LoadDefaultSettings(string.Empty, null, new MachineWideSettings());
        var packageSourceProvider = new PackageSourceProvider(settings);
        var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, Providers);
        var framework = NuGetFramework.Parse(Assembly.GetEntryAssembly()!.GetCustomAttribute<TargetFrameworkAttribute>()!.FrameworkName!);

        var package = await this.GetPackageAsync(this.PackageId, this.PackageVersion, this.IncludePreRelease, false, cancellationToken).ConfigureAwait(false) ?? throw new ArgumentNullException($"Failed to find the specified package with id '{this.PackageId}' and version '{this.PackageVersion}'");
        var project = new PluginNugetProject(this.PackagesDirectory.FullName, this.PluginDirectory.FullName, package.Identity, framework);
        var packageManager = new NuGetPackageManager(sourceRepositoryProvider, settings, this.PackagesDirectory.FullName) { PackagesFolderNuGetProject = project };
        var clientPolicyContext = ClientPolicyContext.GetClientPolicy(settings, this.NugetLogger);
        var projectContext = new PluginNugetProjectContext(this.LoggerFactory) { PackageExtractionContext = new PackageExtractionContext(PackageSaveMode.Defaultv3, PackageExtractionBehavior.XmlDocFileSaveMode, clientPolicyContext, this.NugetLogger) };
        var resolutionContext = new ResolutionContext(DependencyBehavior.Lowest, true, false, VersionConstraints.None, new(), new());
        var downloadContext = new PackageDownloadContext(resolutionContext.SourceCacheContext, this.PackagesDirectory.FullName, false);
        
        await packageManager.InstallPackageAsync(project, package.Identity, resolutionContext, projectContext, downloadContext, package.SourceRepository, [], cancellationToken).ConfigureAwait(false);
        await project.PostProcessAsync(projectContext, cancellationToken).ConfigureAwait(false);
        await project.PreProcessAsync(projectContext, cancellationToken).ConfigureAwait(false);
        await packageManager.RestorePackageAsync(package.Identity, projectContext, downloadContext, [package.SourceRepository], cancellationToken).ConfigureAwait(false);

        foreach (var pluginAssemblyFilePath in project.PluginAssemblies)
        {
            var assemblyCatalog = new AssemblyPluginSource(this.Name, this.Options, Path.Combine(this.PluginDirectory.FullName, pluginAssemblyFilePath));
            await assemblyCatalog.LoadAsync(cancellationToken).ConfigureAwait(false);
            this._assemblies.Add(assemblyCatalog);
        }

        try { this.PackagesDirectory.Delete(true); } catch { }
        this.IsLoaded = true;
    }

    /// <summary>
    /// Gets information about the specified Nuget package
    /// </summary>
    /// <param name="packageId">The id of the package to get</param>
    /// <param name="packageVersion">The version, if any, of the package to get</param>
    /// <param name="includePreRelease">A boolean indicating whether or not to include pre-release packages</param>
    /// <param name="refreshCache">A boolean indicating whether or not to refresh the cache</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="NugetPackageInfo"/> that describes the specified Nuget package</returns>
    protected virtual async Task<NugetPackageInfo?> GetPackageAsync(string packageId, string? packageVersion, bool includePreRelease, bool refreshCache, CancellationToken cancellationToken = default)
    {
        var settings = Settings.LoadDefaultSettings(string.Empty, null, new MachineWideSettings());
        var packageSourceProvider = new PackageSourceProvider(settings);
        var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, Providers);

        if (this.SourceRepository != null) return await this.SearchPackageAsync(packageId, packageVersion, includePreRelease, this.SourceRepository, refreshCache, cancellationToken).ConfigureAwait(false);

        foreach (var repository in sourceRepositoryProvider.GetRepositories())
        {
            var package = await this.SearchPackageAsync(packageId, packageVersion, includePreRelease, repository, refreshCache, cancellationToken).ConfigureAwait(false);
            if (package != null) return package;
        }

        return null;

    }

    /// <summary>
    /// Searches a <see cref="NuGet.Protocol.Core.Types.SourceRepository"/> for the specified Nuget package
    /// </summary>
    /// <param name="packageId">The id of the package to search for</param>
    /// <param name="packageVersion">The version, if any, of the package to search for</param>
    /// <param name="includePreRelease">A boolean indicating whether or not to include pre-release packages</param>
    /// <param name="sourceRepository">The <see cref="NuGet.Protocol.Core.Types.SourceRepository"/> to search</param>
    /// <param name="refreshCache">A boolean indicating whether or not to refresh the cache</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="NugetPackageInfo"/> that describes the specified Nuget package, if found of the specified <see cref="NuGet.Protocol.Core.Types.SourceRepository"/></returns>
    protected virtual async Task<NugetPackageInfo?> SearchPackageAsync(string packageId, string? packageVersion, bool includePreRelease, SourceRepository sourceRepository, bool refreshCache, CancellationToken cancellationToken = default)
    {
        var packageMetadataResource = await sourceRepository.GetResourceAsync<PackageMetadataResource>();
        var sourceCacheContext = new SourceCacheContext();
        if (refreshCache) sourceCacheContext = sourceCacheContext.WithRefreshCacheTrue();

        IPackageSearchMetadata? packageMetaData = null;
        if ((!string.IsNullOrEmpty(packageVersion) && !packageVersion.Contains('*')) || packageVersion == "latest")
        {
            if (!NuGetVersion.TryParse(packageVersion, out var nugetVersion) || nugetVersion == null) throw new Exception($"The specified value '{packageVersion}' is not a valid Nuget Package version");
            var packageIdentity = new PackageIdentity(packageId, nugetVersion);
            packageMetaData = await packageMetadataResource.GetMetadataAsync(packageIdentity, sourceCacheContext, this.NugetLogger, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var searchResults = await packageMetadataResource.GetMetadataAsync(packageId, includePreRelease, false, sourceCacheContext, this.NugetLogger, cancellationToken).ConfigureAwait(false);
            searchResults = searchResults.OrderByDescending(p => p.Identity.Version);

            if (!string.IsNullOrEmpty(packageVersion))
            {
                var searchPattern = packageVersion.Replace("*", ".*");
                searchResults = searchResults.Where(p => Regex.IsMatch(p.Identity.Version.ToString(), searchPattern));
            }

            packageMetaData = searchResults.FirstOrDefault();
        }

        return packageMetaData == null ? null : new NugetPackageInfo(sourceRepository, packageMetaData.Identity);
    }

    /// <summary>
    /// Builds the default <see cref="NuGet.Configuration.PackageSource"/>
    /// </summary>
    /// <param name="sourceUri">The package source uri, if any</param>
    /// <returns>A new <see cref="NuGet.Configuration.PackageSource"/></returns>
    protected virtual PackageSource BuildPackageSource(Uri sourceUri)
    {
        ArgumentNullException.ThrowIfNull(sourceUri);

        var source = sourceUri.OriginalString;
        var userInfo = sourceUri.UserInfo;
        if (string.IsNullOrWhiteSpace(source)) source = DefaultPackageSource;
        var packageSource = new PackageSource(source);

        if (!string.IsNullOrWhiteSpace(userInfo))
        {
            var components = userInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var username = components.Length > 0 ? components[0] : null;
            var password = components.Length > 1 ? components[1] : null;
            packageSource.Credentials = new PackageSourceCredential(source, username, password, true, null);
        }

        return packageSource;
    }

    /// <summary>
    /// Describes a Nuget package
    /// </summary>
    /// <remarks>
    /// Initializes a new <see cref="NugetPackageInfo"/>
    /// </remarks>
    /// <param name="sourceRepository">The <see cref="NuGet.Protocol.Core.Types.SourceRepository"/> the package is sourced by</param>
    /// <param name="identity">The package's identity</param>
    protected class NugetPackageInfo(SourceRepository sourceRepository, PackageIdentity identity)
    {

        /// <summary>
        /// Gets the <see cref="NuGet.Protocol.Core.Types.SourceRepository"/> the package is sourced by
        /// </summary>
        public SourceRepository SourceRepository { get; } = sourceRepository;

        /// <summary>
        /// Gets the package's identity
        /// </summary>
        public PackageIdentity Identity { get; } = identity;

    }

}
