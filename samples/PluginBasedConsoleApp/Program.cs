using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neuroglia.Plugins;
using PluginBasedConsoleApp;

var host = new HostBuilder();

host.ConfigureAppConfiguration(configuration => configuration.AddJsonFile("appsettings.json", true));
host.ConfigureServices((context, services) =>
{
    services.AddLogging();
    services.AddPluginProvider(context.Configuration);
    services.AddHostedService<App>();
});

using var app = host.Build();
await app.RunAsync();