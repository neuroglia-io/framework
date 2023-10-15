using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Plugins;
using Neuroglia.Plugins.Services;

namespace Neuroglia.UnitTests.Cases.Plugins.Sources;

public class DirectoryPluginSourceTests
    : PluginSourceTestsBase
{
    public DirectoryPluginSourceTests() : base(BuildServices()) { }

    static IServiceCollection BuildServices()
    {
        var services = new ServiceCollection();
        services.AddPluginSource(new DirectoryPluginSource("source1", new() { Filter = (PluginTypeFilter)new PluginTypeFilterBuilder().Implements<IGreet>().Build() }, AppContext.BaseDirectory));
        return services;
    }

}
