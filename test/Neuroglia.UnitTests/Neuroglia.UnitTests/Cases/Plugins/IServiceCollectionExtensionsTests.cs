using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neuroglia.Plugins;
using Neuroglia.Plugins.Services;

namespace Neuroglia.UnitTests.Cases.Plugins;

public class IServiceCollectionExtensionsTests
{

    [Fact]
    public async Task ConfigurePlugins_Should_Work()
    {
        //arrange
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddPluginProvider(configuration);
        var tokenSource = new CancellationTokenSource();

        //act
        var serviceProvider = services.BuildServiceProvider();
        foreach (var hostedService in serviceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(tokenSource.Token);
        }

        //assert
        serviceProvider.GetServices<IPluginSource>().Should().HaveCount(3);
        serviceProvider.GetServices<IPluginSource>().Should().AllSatisfy(s => s.Plugins.Should().NotBeNullOrEmpty());

        //clean
        tokenSource.Cancel();
        tokenSource.Dispose();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public void ConfigurePlugins_Twice_Should_Have_No_Effect()
    {
        //arrange
        var configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        
        //act
        var countBefore = services.ConfigurePlugins(configuration).Count;
        var countAfter = services.ConfigurePlugins(configuration).Count;

        //assert
        countAfter.Should().Be(countBefore);
    }


}
