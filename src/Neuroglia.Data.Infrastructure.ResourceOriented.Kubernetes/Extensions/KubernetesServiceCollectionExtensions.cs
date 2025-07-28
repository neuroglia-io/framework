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

namespace Neuroglia.Data.Infrastructure.ResourceOriented.Redis;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class KubernetesServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures a <see cref="KubernetesDatabase"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="lifetime">The <see cref="KubernetesDatabase"/>'s <see cref="ServiceLifetime"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddRedisDatabase(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.Add(new(typeof(KubernetesDatabase), typeof(KubernetesDatabase), lifetime));
        services.Add(new(typeof(IDatabase), provider => provider.GetRequiredService<KubernetesDatabase>(), lifetime));
        return services;
    }

}
