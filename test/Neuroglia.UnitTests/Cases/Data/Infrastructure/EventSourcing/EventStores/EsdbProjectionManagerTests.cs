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

using DotNet.Testcontainers.Containers;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neuroglia.Collections;
using Neuroglia.Data.Infrastructure.EventSourcing;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Containers;
using Pipelines.Sockets.Unofficial.Arenas;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.EventSourcing.EventStores;

[TestCaseOrderer("Neuroglia.UnitTests.Services.PriorityTestCaseOrderer", "Neuroglia.UnitTests")]
public class EsdbProjectionManagerTests
    : IAsyncLifetime
{

    public EsdbProjectionManagerTests()
    {
        var services = BuildServices();
        this.ServiceProvider = services.BuildServiceProvider();
    }

    protected CancellationTokenSource CancellationTokenSource { get; } = new();

    protected ServiceProvider ServiceProvider { get; }

    protected IEventStore EventStore => this.ServiceProvider.GetRequiredService<IEventStore>();

    protected IProjectionManager ProjectionManager => this.ServiceProvider.GetRequiredService<IProjectionManager>();

    public virtual async Task InitializeAsync()
    {
        foreach (var hostedService in this.ServiceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(this.CancellationTokenSource.Token).ConfigureAwait(false);
        }
    }

    public virtual async Task DisposeAsync() => await this.ServiceProvider.DisposeAsync().ConfigureAwait(false);

    public static IServiceCollection BuildServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSerialization();
        services.AddSingleton(EventStoreContainerBuilder.Build());
        services.AddHostedService(provider => new ContainerBootstrapper(provider.GetRequiredService<IContainer>()));
        services.AddSingleton(provider => EventStoreClientSettings.Create($"esdb://{provider.GetRequiredService<IContainer>().Hostname}:{provider.GetRequiredService<IContainer>().GetMappedPublicPort(EventStoreContainerBuilder.PublicPort2)}?tls=false"));
        services.AddSingleton(provider => new EventStoreClient(provider.GetRequiredService<EventStoreClientSettings>()));
        services.AddSingleton(provider => new EventStorePersistentSubscriptionsClient(provider.GetRequiredService<EventStoreClientSettings>()));
        services.AddSingleton(provider => new EventStoreProjectionManagementClient(provider.GetRequiredService<EventStoreClientSettings>()));
        services.AddEsdbEventStore();
        services.AddEsdbProjectionManager();
        return services;
    }

    [Fact, Priority(1)]
    public async Task Create_Projection_FromStream_Should_Work()
    {
        //arrange
        var name = "fake-name";
        var streamName = "fake-stream";
        var initialState = new List<int>() { 1, 2, 3 };
        var index = 4;
        Action<IProjectionSourceBuilder<List<int>>> setup = projection => projection
            .FromStream(streamName)
            .Given(() => new List<int>() { 1, 2, 3 })
            .When((state, e) => e.Data != null && e.Data!.GetProperty("index") != null && !state.Contains(e.Data!.GetProperty("index")))
            .Then((state, e) => state.Add(e.Data!.GetProperty("index")));
        initialState.Add(index);

        //act
        await this.ProjectionManager.CreateAsync(name, setup);
        await this.EventStore.AppendAsync(streamName, [new EventDescriptor("fake-type", new { index = index })]);
        await Task.Delay(250);
        var state = await this.ProjectionManager.GetStateAsync<List<int>>(name);

        //assert
        state.Should().NotBeNull();
        state.Should().BeEquivalentTo(initialState);
    }

    [Fact, Priority(2)]
    public async Task Create_Projection_FromStream_LinkTo_Should_Work()
    {
        //arrange
        var name = "fake-name";
        var streamName = "fake-stream";
        var linkToStreamName = "fake-link-to-stream";
        var initialState = new List<int>() { 1, 2, 3 };
        var index = 4;
        Action<IProjectionSourceBuilder<List<int>>> setup = projection => projection
            .FromStream(streamName)
            .Given(() => new List<int>() { 1, 2, 3 })
            .When((state, e) => e.Data != null && e.Data!.GetProperty("index") != null && !state.Contains(e.Data!.GetProperty("index")))
            .Then((state, e) => Projection.LinkEventTo(linkToStreamName, e));
        initialState.Add(index);

        //act
        await this.ProjectionManager.CreateAsync(name, setup);
        await this.EventStore.AppendAsync(streamName, [new EventDescriptor("fake-type", new { index })]);
        await Task.Delay(250);

        //assert
        IEventStreamDescriptor result = null!;
        var action = async () => result = await this.EventStore.GetAsync(linkToStreamName);
        await action.Should().NotThrowAsync();
        result.Should().NotBeNull();
        result.Length.Should().Be(1);
    }

}