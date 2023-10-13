using NuGet.Common;
using NuGet.Configuration;

namespace Neuroglia.Plugins;

/// <summary>
/// Represents an implementation of the <see cref="IMachineWideSettings"/> interface
/// </summary>
internal class MachineWideSettings 
    : IMachineWideSettings
{

    /// <inheritdoc/>
    public ISettings Settings => NuGet.Configuration.Settings.LoadMachineWideSettings(NuGetEnvironment.GetFolderPath(NuGetFolderPath.MachineWideConfigDirectory));

}