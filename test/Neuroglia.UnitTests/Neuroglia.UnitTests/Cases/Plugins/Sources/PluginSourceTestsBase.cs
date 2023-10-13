using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Plugins.Services;

namespace Neuroglia.UnitTests.Cases.Plugins.Sources;

public abstract class PluginSourceTestsBase
    : IAsyncLifetime
{

    public PluginSourceTestsBase(IServiceCollection services) { this.ServiceProvider = services.BuildServiceProvider(); }

    protected ServiceProvider ServiceProvider { get; }

    protected IPluginSource PluginSource { get; private set; } = null!;

    public Task InitializeAsync()
    {
        this.PluginSource = this.ServiceProvider.GetRequiredService<IPluginSource>();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync() => await this.ServiceProvider.DisposeAsync().ConfigureAwait(false);

    [Fact]
    public async Task Load_Should_Work()
    {
        //act
        await this.PluginSource.LoadAsync();

        //assert
        this.PluginSource.IsLoaded.Should().BeTrue();
        this.PluginSource.Plugins.Should().ContainSingle();
    }

}
