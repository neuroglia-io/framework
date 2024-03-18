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
using DotNet.Testcontainers.Containers;

namespace Neuroglia.UnitTests.Containers;

public static class EventStoreContainerBuilder
{

    public const int PublicPort1 = 1113;
    public const int PublicPort2 = 2113;

    public static IContainer Build()
    {
        return new ContainerBuilder()
            .WithName($"event-store-{Guid.NewGuid():N}")
            .WithImage("eventstore/eventstore:latest")
            .WithPortBinding(PublicPort1, true)
            .WithPortBinding(PublicPort2, true)
            .WithEnvironment("EVENTSTORE_RUN_PROJECTIONS", "All")
            .WithEnvironment("EVENTSTORE_START_STANDARD_PROJECTIONS", "true")
            .WithEnvironment("EVENTSTORE_HTTP_PORT", "2113")
            .WithEnvironment("EVENTSTORE_INSECURE", "true")
            .WithEnvironment("EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP", "true")
            .WithWaitStrategy(Wait
                .ForUnixContainer()
                .UntilPortIsAvailable(PublicPort2))
            .Build();
    }

}