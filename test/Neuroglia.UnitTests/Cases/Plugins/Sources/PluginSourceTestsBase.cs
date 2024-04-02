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
    public virtual async Task Load_Should_Work()
    {
        //act
        await this.PluginSource.LoadAsync();

        //assert
        this.PluginSource.IsLoaded.Should().BeTrue();
        this.PluginSource.Plugins.Should().ContainSingle();
    }

}
