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
using Minio;
using Neuroglia.Data.Infrastructure.ObjectStorage;
using Neuroglia.UnitTests.Containers;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.ObjectStorage;

public class MinioObjectStorageTests
    : ObjectStorageTestsBase
{

    public MinioObjectStorageTests() : base(BuildServices()) { }

    static IServiceCollection BuildServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddSingleton(provider => MinioContainerBuilder.Build());
        services.AddHostedService(provider => new ContainerBootstrapper(provider.GetRequiredService<IContainer>()));
        services.AddSingleton(provider =>
        {
            var container = provider.GetRequiredService<IContainer>();
            return new MinioClient().WithEndpoint($"{container.Hostname}:{container.GetMappedPublicPort(MinioContainerBuilder.PublicPort1)}").WithCredentials(MinioContainerBuilder.DefaultAccessKey, MinioContainerBuilder.DefaultSecretKey).Build();
        });
        services.AddMinioObjectStorage();
        return services;
    }

}