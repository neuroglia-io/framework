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
using Neuroglia.Data.Infrastructure.EventSourcing.Configuration;
using Neuroglia.Data.Infrastructure.EventSourcing.Memory;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;
using Neuroglia.Serialization;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.EventSourcing.AggregateStateManagers;

public class EventSourcingStateManagerTests
    : AggregateStateManagerTestsBase
{

    public EventSourcingStateManagerTests() : base(BuildServices()) { }

    static IServiceCollection BuildServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddJsonSerializer();
        services.AddMemoryCacheEventStore();
        services.Configure<StateManagementOptions<User, string>>(options => options.SnapshotFrequency = 0);
        services.AddSingleton<IAggregateStateManager<User, string>, EventSourcingStateManager<User, string>>();
        return services;
    }

}
