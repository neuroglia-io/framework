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

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using KurrentDB.Client;

namespace Neuroglia.UnitTests.Containers;

public static class KurrentDBContainerBuilder
{

    public const int PublicPort = 2113;

    public static IContainer Build()
    {
        return new ContainerBuilder()
            .WithName($"kurrentdb-{Guid.NewGuid():N}")
            .WithImage("kurrentplatform/kurrentdb:26.0")
            .WithPortBinding(PublicPort, true)
            .WithEnvironment("KURRENTDB_RUN_PROJECTIONS", "All")
            .WithEnvironment("KURRENTDB_START_STANDARD_PROJECTIONS", "true")
            .WithEnvironment("KURRENTDB_INSECURE", "true")
            .WithEnvironment("KURRENTDB_ENABLE_ATOM_PUB_OVER_HTTP", "true")
            // the port opens before the node can accept writes, which are then refused with 'Server Is Not Ready': the node logs that it leads once it can.
            // Even then, its first gRPC calls may still be refused, with 'HTTP_1_1_REQUIRED': the container is only ready once one succeeds
            .WithWaitStrategy(Wait
                .ForUnixContainer()
                .UntilPortIsAvailable(PublicPort)
                .UntilMessageIsLogged("IS LEADER")
                .AddCustomWaitStrategy(new UntilGrpcCallSucceeds()))
            .Build();
    }

    /// <summary>
    /// Waits until the KurrentDB node answers a read over gRPC
    /// </summary>
    class UntilGrpcCallSucceeds
        : IWaitUntil
    {

        public async Task<bool> UntilAsync(IContainer container)
        {
            try
            {
                await using var client = new KurrentDBClient(KurrentDBClientSettings.Create($"kurrentdb://{container.Hostname}:{container.GetMappedPublicPort(PublicPort)}?tls=false"));
                await client.ReadAllAsync(Direction.Forwards, Position.Start, 1).ToListAsync().ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }

}
