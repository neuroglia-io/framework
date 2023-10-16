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
using Neuroglia.Data.PatchModel.Services;

namespace Neuroglia.Data.PatchModel;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures a new <see cref="IPatchHandler"/>
    /// </summary>
    /// <typeparam name="THandler">The type of the <see cref="IPatchHandler"/> to add</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddPatchHandler<THandler>(this IServiceCollection services)
        where THandler : class, IPatchHandler
    {
        services.TryAddSingleton<THandler>();
        services.AddSingleton<IPatchHandler>(services => services.GetRequiredService<THandler>());
        return services;
    }

    /// <summary>
    /// Adds and configures a new <see cref="IPatchHandler"/> used to handle JSON Patches
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJsonPatchHandler(this IServiceCollection services)
    {
        services.AddPatchHandler<JsonPatchHandler>();
        return services;
    }

    /// <summary>
    /// Adds and configures a new <see cref="IPatchHandler"/> used to handle JSON Merge Patches
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJsonMergePatchHandler(this IServiceCollection services)
    {
        services.AddPatchHandler<JsonMergePatchHandler>();
        return services;
    }

    /// <summary>
    /// Adds and configures a new <see cref="IPatchHandler"/> used to handle JSON Strategic Merge Patches
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJsonStrategicMergePatchHandler(this IServiceCollection services)
    {
        services.AddPatchHandler<JsonStrategicMergePatchHandler>();
        return services;
    }

}
