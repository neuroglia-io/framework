using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Hosting;

namespace Neuroglia.UnitTests.Services;

public class ContainerBootstrapper
    : IHostedService
{

    public ContainerBootstrapper(IContainer container) => this.Container = container;

    protected IContainer Container { get; }

    public Task StartAsync(CancellationToken cancellationToken) => Container.StartAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}
