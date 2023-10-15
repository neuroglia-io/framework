using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Plugins;
using Neuroglia.Plugins.Services;

namespace Neuroglia.UnitTests.Cases.Plugins.Sources;

public class AssemblyPluginSourceTests
    : PluginSourceTestsBase
{
    public AssemblyPluginSourceTests() : base(BuildServices()) { }

    static IServiceCollection BuildServices()
    {
        var services = new ServiceCollection();
        services.AddPluginSource(new AssemblyPluginSource("source1", new() { Filter = (PluginTypeFilter)new PluginTypeFilterBuilder().Implements<IGreet>().Build() }, typeof(AssemblyPluginSourceTests).Assembly.Location));
        return services;
    }

}
