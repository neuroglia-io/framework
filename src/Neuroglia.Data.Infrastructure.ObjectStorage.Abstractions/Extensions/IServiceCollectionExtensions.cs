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
using Neuroglia.Data.Infrastructure.ObjectStorage.Services;

namespace Neuroglia.Data.Infrastructure.ObjectStorage;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Registers and configures an <see cref="IObjectStorage"/> implementation
    /// </summary>
    /// <typeparam name="TStorage">The type of the <see cref="IObjectStorage"/> implementation to use</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="lifetime">The service's lifetime</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddObjectStorage<TStorage>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TStorage : class, IObjectStorage
    {
        services.TryAdd(new ServiceDescriptor(typeof(TStorage), typeof(TStorage), lifetime));
        services.Add(new ServiceDescriptor(typeof(IObjectStorage), typeof(TStorage), lifetime));
        return services;
    }

}
