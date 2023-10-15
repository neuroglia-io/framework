using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Neuroglia.Plugins;
using Neuroglia.Plugins.Services;

namespace Neuroglia.UnitTests.Cases.Plugins.Sources;

public class NugetPackagePluginSourceTests
    : PluginSourceTestsBase
{

    public NugetPackagePluginSourceTests() : base(BuildServices()) { }

    static IServiceCollection BuildServices()
    {
        var services = new ServiceCollection();
        services.AddPluginSource(new NugetPackagePluginSource(new NullLoggerFactory(), "source1", new() { Filter = (PluginTypeFilter)new PluginTypeFilterBuilder().Implements<ILogger>().Build() }, "Microsoft.Extensions.Logging", "6.0.0"));
        return services;
    }

}
