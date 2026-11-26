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

using Neuroglia.Data.Infrastructure.ResourceOriented.Configuration;

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

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
    /// <remarks>The <see cref="IKubernetes"/> client used by the <see cref="KubernetesDatabase"/> must be registered separately</remarks>
    public static IServiceCollection AddKubernetesDatabase(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton) => services.AddKubernetesDatabase(_ => { }, lifetime);

    /// <summary>
    /// Adds and configures a <see cref="KubernetesDatabase"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to configure the <see cref="KubernetesDatabaseOptions"/> to use</param>
    /// <param name="lifetime">The <see cref="KubernetesDatabase"/>'s <see cref="ServiceLifetime"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    /// <remarks>The <see cref="IKubernetes"/> client used by the <see cref="KubernetesDatabase"/> must be registered separately</remarks>
    public static IServiceCollection AddKubernetesDatabase(this IServiceCollection services, Action<KubernetesDatabaseOptions> setup, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        ArgumentNullException.ThrowIfNull(setup);
        services.AddOptions<KubernetesDatabaseOptions>().Configure(setup);
        services.Add(new(typeof(KubernetesDatabase), typeof(KubernetesDatabase), lifetime));
        services.Add(new(typeof(IDatabase), provider => provider.GetRequiredService<KubernetesDatabase>(), lifetime));
        return services;
    }

}
