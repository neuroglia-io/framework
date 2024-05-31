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
using Neuroglia.Scripting.Services;

namespace Neuroglia.Scripting;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures an <see cref="IScriptExecutor"/> of the specified type
    /// </summary>
    /// <typeparam name="TExecutor">The type of <see cref="IScriptExecutor"/> to register</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/>. Defaults to <see cref="ServiceLifetime.Transient"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddScriptExecutor<TExecutor>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TExecutor : class, IScriptExecutor
    {
        services.TryAddSingleton<IScriptExecutorProvider, ScriptExecutorProvider>();
        services.TryAdd(new ServiceDescriptor(typeof(TExecutor), typeof(TExecutor), lifetime));
        services.Add(new ServiceDescriptor(typeof(IScriptExecutor), provider => provider.GetRequiredService<TExecutor>(), lifetime));
        return services;
    }

}
