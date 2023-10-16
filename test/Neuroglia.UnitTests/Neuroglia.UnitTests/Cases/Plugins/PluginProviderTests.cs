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
using Neuroglia.Data.Infrastructure.Services;
using Neuroglia.Plugins;
using Neuroglia.Plugins.Services;

namespace Neuroglia.UnitTests.Cases.Plugins;

public class PluginProviderTests
    : IAsyncLifetime
{

    public PluginProviderTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddPluginProvider();
        services.AddPluginSource(source => source.FromDirectory("source1", plugin => plugin.Implements<IGreet>(), AppContext.BaseDirectory, "*.dll", SearchOption.TopDirectoryOnly));
        services.AddPluginSource(source => source.FromDirectory("source2", plugin => plugin.Implements(typeof(IRepository<,>)), AppContext.BaseDirectory, "*.dll", SearchOption.TopDirectoryOnly));
        services.AddPlugin<IGreet>();
        services.AddPlugin(serviceType: typeof(IRepository<User, string>));
        this.ServiceProvider = services.BuildServiceProvider();
    }

    protected ServiceProvider ServiceProvider { get; }

    protected CancellationTokenSource CancellationTokenSource { get; private set; } = null!;

    protected IPluginProvider PluginProvider { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        this.CancellationTokenSource = new CancellationTokenSource();
        foreach(var hostedService in this.ServiceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(this.CancellationTokenSource.Token);
        }
    }

    public async Task DisposeAsync()
    {
        this.CancellationTokenSource.Dispose();
        await this.ServiceProvider.DisposeAsync();
    }

    [Fact]
    public void Get_PluginService_Should_Work()
    {
        //assert
        this.ServiceProvider.GetService<IGreet>().Should().NotBeNull();
        this.ServiceProvider.GetServices<IGreet>().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Get_GenericPluginService_Should_Work()
    {
        //assert
        this.ServiceProvider.GetService<IRepository<User, string>>().Should().NotBeNull();
    }

}
