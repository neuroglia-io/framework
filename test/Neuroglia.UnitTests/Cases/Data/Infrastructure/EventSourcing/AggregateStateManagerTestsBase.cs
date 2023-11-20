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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.EventSourcing;

public abstract class AggregateStateManagerTestsBase
    : IAsyncLifetime
{

    protected AggregateStateManagerTestsBase(IServiceCollection services)
    {
        this.ServiceProvider = services.BuildServiceProvider();
    }

    protected ServiceProvider ServiceProvider { get; }

    protected CancellationTokenSource CancellationTokenSource { get; } = new();

    protected IAggregateStateManager<User, string> AggregateStateManager { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        foreach (var service in this.ServiceProvider.GetServices<IHostedService>()) await service.StartAsync(this.CancellationTokenSource.Token);
        this.AggregateStateManager = this.ServiceProvider.GetRequiredService<IAggregateStateManager<User, string>>();
    }

    public async Task DisposeAsync()
    {
        this.CancellationTokenSource.Cancel();
        this.CancellationTokenSource.Dispose();
        await this.ServiceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Snapshot_Should_Work()
    {
        //arrange
        var user = User.Create();

        //act
        await this.AggregateStateManager.TakeSnapshotAsync(user);

        //assert

    }

    [Fact]
    public async Task RestoreState_Should_Work()
    {
        //arrange
        var user = User.Create();
        await this.AggregateStateManager.TakeSnapshotAsync(user);

        //act
        var result = await this.AggregateStateManager.RestoreStateAsync(user.Id);

        //assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.State.Should().BeEquivalentTo(user.State);
    }

}