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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Infrastructure;
using Neuroglia.Data.Infrastructure.Redis.Services;
using Neuroglia.Mediation;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Containers;
using StackExchange.Redis;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.Repositories;

[TestCaseOrderer("Neuroglia.UnitTests.Services.PriorityTestCaseOrderer", "Neuroglia.UnitTests")]
public class RedisRepositoryTests
    : RepositoryTestsBase
{

    public RedisRepositoryTests() : base(BuildServices()) { }

    protected new RedisRepository<User, string> Repository => (RedisRepository<User, string>)base.Repository;

    static IServiceCollection BuildServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddSingleton(provider => RedisContainerBuilder.Build());
        services.AddHostedService(provider => new ContainerBootstrapper(provider.GetRequiredService<IContainer>()));
        services.AddMediator();
        services.AddJsonSerializer();
        services.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect($"localhost:{provider.GetRequiredService<IContainer>().GetMappedPublicPort(RedisContainerBuilder.PublicPort)}"));
        services.AddRedisRepository<User, string>();
        return services;
    }

}
