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
using KurrentDB.Client;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Infrastructure.EventSourcing;
using Neuroglia.Mediation;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Containers;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.Repositories;

public class EventSourcingRepositoryTests
    : RepositoryTestsBase
{

    public EventSourcingRepositoryTests() : base(BuildServices()) { }

    static IServiceCollection BuildServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSerialization();
        services.AddMediator();
        services.AddSingleton(_ => KurrentDBContainerBuilder.Build()); // created by the provider, which disposes it, and with it the container, after each test
        services.AddHostedService(provider => new ContainerBootstrapper(provider.GetRequiredService<IContainer>()));
        services.AddSingleton(provider => KurrentDBClientSettings.Create($"kurrentdb://{provider.GetRequiredService<IContainer>().Hostname}:{provider.GetRequiredService<IContainer>().GetMappedPublicPort(KurrentDBContainerBuilder.PublicPort)}?tls=false"));
        services.AddSingleton(provider => new KurrentDBClient(provider.GetRequiredService<KurrentDBClientSettings>()));
        services.AddSingleton(provider => new KurrentDBPersistentSubscriptionsClient(provider.GetRequiredService<KurrentDBClientSettings>()));
        services.AddKurrentDBEventStore();
        services.AddEventSourcingRepository<User, string>();
        return services;
    }

}
