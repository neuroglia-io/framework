using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;
using System.IO.Compression;

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Represents a plugin specific implementation of the <see cref="FolderNuGetProject"/>
/// </summary>
public class PluginNugetProject 
    : FolderNuGetProject
{

    readonly List<string> _pluginAssemblies = new();

    /// <summary>
    /// Initializes a new <see cref="PluginNugetProject"/>
    /// </summary>
    /// <param name="root">The <see cref="PluginNugetProject"/>'s root path</param>
    /// <param name="pluginPackage">The identity of the plugin package</param>
    /// <param name="targetFramework">The <see cref="PluginNugetProject"/>'s target <see cref="NuGetFramework"/></param>
    public PluginNugetProject(string root, string pluginDirectory, PackageIdentity pluginPackage, NuGetFramework targetFramework) 
        : base(root, new PackagePathResolver(root), targetFramework) 
    {
        this.PluginDirectory = pluginDirectory;
        this.PluginPackage = pluginPackage;
        this.TargetFramework = targetFramework;
        this.CompatibilityProvider = new CompatibilityProvider(new DefaultFrameworkNameProvider());
        this.FrameworkReducer = new FrameworkReducer(new DefaultFrameworkNameProvider(), this.CompatibilityProvider);
    }

    protected string PluginDirectory { get; }

    protected PackageIdentity PluginPackage { get; }

    /// <summary>
    /// Gets the <see cref="PluginNugetProject"/>'s target <see cref="NuGetFramework"/>
    /// </summary>
    protected NuGetFramework TargetFramework { get; }

    protected CompatibilityProvider CompatibilityProvider { get; }

    protected FrameworkReducer FrameworkReducer { get; }

    /// <summary>
    /// Gets a list containing the paths to the plugin's assemblies, if any
    /// </summary>
    public IReadOnlyList<string> PluginAssemblies => this._pluginAssemblies.AsReadOnly();

    /// <inheritdoc/>
    public override Task<IEnumerable<PackageReference>> GetInstalledPackagesAsync(CancellationToken token) => base.GetInstalledPackagesAsync(token);

    /// <inheritdoc/>
    public override async Task<bool> InstallPackageAsync(PackageIdentity packageIdentity, DownloadResourceResult downloadResourceResult, INuGetProjectContext nuGetProjectContext, CancellationToken cancellationToken)
    {
        var succeeded = await base.InstallPackageAsync(packageIdentity, downloadResourceResult, nuGetProjectContext, cancellationToken).ConfigureAwait(false);
        using var zipArchive = new ZipArchive(downloadResourceResult.PackageStream);
        var zipArchiveEntries = zipArchive.Entries.Where(e => e.Name.EndsWith(".dll") || e.Name.EndsWith(".exe")).ToList();

        var entriesWithTargetFramework = zipArchiveEntries.Select(e => new { TargetFramework = NuGetFramework.Parse(e.FullName.Split('/')[1]), Entry = e }).ToList();
        entriesWithTargetFramework = entriesWithTargetFramework.Where(e => !string.Equals(e.Entry.FullName.Split('/')[0], "ref")).ToList();
        await this.ExtractAssembliesAsync(packageIdentity, zipArchiveEntries, cancellationToken).ConfigureAwait(false);

        return succeeded;
    }

    /// <inheritdoc/>
    public override Task<bool> UninstallPackageAsync(PackageIdentity packageIdentity, INuGetProjectContext nuGetProjectContext, CancellationToken cancellationToken) => base.UninstallPackageAsync(packageIdentity, nuGetProjectContext, cancellationToken);

    protected virtual async Task ExtractAssembliesAsync(PackageIdentity packageIdentity, List<ZipArchiveEntry> zipArchiveEntries, CancellationToken cancellationToken)
    {
        var entriesWithTargetFramework = zipArchiveEntries.Select(e => new { TargetFramework = NuGetFramework.Parse(e.FullName.Split('/')[1]), Entry = e }).ToList();
        entriesWithTargetFramework = entriesWithTargetFramework.Where(e => !string.Equals(e.Entry.FullName.Split('/')[0], "ref")).ToList();

        var compatibleEntries = entriesWithTargetFramework.Where(e => this.CompatibilityProvider.IsCompatible(this.TargetFramework, e.TargetFramework)).ToList();
        var mostCompatibleFramework = this.FrameworkReducer.GetNearest(this.TargetFramework, compatibleEntries.Select(x => x.TargetFramework));
        if (mostCompatibleFramework == null) return;

        var matchingEntries = entriesWithTargetFramework.Where(e => e.TargetFramework == mostCompatibleFramework).ToList();
        if (!matchingEntries.Any()) return;

        foreach (var entry in matchingEntries)
        {
            ZipFileExtensions.ExtractToFile(entry.Entry, Path.Combine(this.PluginDirectory, entry.Entry.Name), overwrite: true);
            if (packageIdentity.Id == this.PluginPackage.Id) this._pluginAssemblies.Add(entry.Entry.Name);
        }

        await Task.CompletedTask;
    }

}
