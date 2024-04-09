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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class EsdbServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures a <see cref="EsdbEventStore"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to configure the <see cref="EsdbEventStore"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddEsdbEventStore(this IServiceCollection services, Action<IEventStoreOptionsBuilder>? setup = null)
    {
        services.AddEventStore<EsdbEventStore>(setup);
        return services;
    }

    /// <summary>
    /// Adds and configures a <see cref="EsdbProjectionManager"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddEsdbProjectionManager(this IServiceCollection services)
    {
        services.TryAddSingleton<EsdbProjectionManager>();
        services.TryAddSingleton<IProjectionManager>(provider => provider.GetRequiredService<EsdbProjectionManager>());
        return services;
    }

}
