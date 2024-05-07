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

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neuroglia.Data.Infrastructure.ResourceOriented;
using Neuroglia.Data.Infrastructure.ResourceOriented.Services;
using Neuroglia.Data.PatchModel.Services;
using Neuroglia.Security.Services;

namespace Neuroglia.UnitTests.Services;

internal sealed class TestRepositoryBuilder(Action<IConfiguration, IServiceCollection> setup)
    : IDisposable
{

    readonly Action<IConfiguration, IServiceCollection> _setup = setup;
    readonly List<IResourceDefinition> _definitions = [];
    readonly List<IResource> _resources = [];
    ServiceProvider _serviceProvider = null!;

    internal TestRepositoryBuilder WithDefinition<TDefinition>()
        where TDefinition : class, IResourceDefinition, new()
    {
        this._definitions.Add(new TDefinition());
        return this;
    }

    internal TestRepositoryBuilder WithResource<TResource>(TResource resource)
        where TResource : class, IResource, new()
    {
        this._resources.Add(resource);
        return this;
    }

    internal async Task<IResourceRepository> BuildAsync()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddHttpClient();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IUserAccessor, HttpContextUserAccessor>();
        services.AddSingleton<IUserInfoProvider, UserInfoProvider>();
        services.AddSingleton<IAdmissionControl, AdmissionControl>();
        services.AddSingleton<IVersionControl, VersionControl>();
        services.AddSingleton<IPatchHandler, JsonMergePatchHandler>();
        services.AddSingleton<IPatchHandler, JsonPatchHandler>();
        services.AddSingleton<IPatchHandler, JsonStrategicMergePatchHandler>();
        this._setup(configuration, services);
        services.AddHostedService<DatabaseInitializer>();

        this._serviceProvider = services.BuildServiceProvider();

        foreach (var hostedService in _serviceProvider.GetServices<IHostedService>()) 
            await hostedService.StartAsync(default).ConfigureAwait(false);

        var repository = this._serviceProvider.GetRequiredService<IResourceRepository>();

        foreach (var definition in this._definitions) await repository.AddAsync(definition.ConvertTo<ResourceDefinition>()!, false).ConfigureAwait(false);

        foreach (var resource in this._resources) await repository.AddAsync(resource, resource.GetGroup(), resource.GetVersion(), resource.Definition.Plural, false).ConfigureAwait(false);

        return repository;
    }

#pragma warning disable CA2012 // Use ValueTasks correctly
    public void Dispose() => this._serviceProvider.DisposeAsync().GetAwaiter().GetResult();
#pragma warning restore CA2012 // Use ValueTasks correctly

}
