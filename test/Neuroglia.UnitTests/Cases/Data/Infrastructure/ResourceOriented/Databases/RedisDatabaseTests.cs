﻿// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
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
using Neuroglia.Data.Infrastructure.ResourceOriented.Services;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Containers;
using StackExchange.Redis;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.ResourceOriented.Databases;

public class RedisDatabaseTests
    : DatabaseTestsBase
{

    public RedisDatabaseTests() : base(ConfigureServices) { }

    static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddSerialization();
        services.AddSingleton(provider => RedisContainerBuilder.Build());
        services.AddHostedService(provider => new ContainerBootstrapper(provider.GetRequiredService<IContainer>()));
        services.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect($"localhost:{provider.GetRequiredService<IContainer>().GetMappedPublicPort(RedisContainerBuilder.PublicPort)}"));
        services.AddSingleton<Neuroglia.Data.Infrastructure.ResourceOriented.Services.IDatabase, RedisDatabase>();
        services.AddSingleton<IResourceRepository, ResourceRepository>();
    }

}
