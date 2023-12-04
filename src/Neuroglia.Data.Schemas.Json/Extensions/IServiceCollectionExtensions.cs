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
using Microsoft.Extensions.Hosting;
using Neuroglia.Data.Schemas.Json.Configuration;

namespace Neuroglia.Data.Schemas.Json;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures a new <see cref="JsonSchemaResolver"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJsonSchemaResolver(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.TryAddSingleton<IJsonSchemaResolver, JsonSchemaResolver>();
        return services;
    }

    /// <summary>
    /// Adds and configures a new <see cref="JsonSchemaRegistry"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="configuration">An <see cref="Action{T}"/> used to configure the <see cref="JsonSchemaRegistryOptions"/> to use</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJsonSchemaRegistry(this IServiceCollection services, Action<JsonSchemaRegistryOptions>? configuration = null)
    {
        services.AddJsonSchemaResolver();
        if (configuration != null) services.Configure(configuration);
        services.TryAddSingleton<JsonSchemaRegistry>();
        services.TryAddSingleton<IJsonSchemaRegistry>(provider => provider.GetRequiredService<JsonSchemaRegistry>());
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<JsonSchemaRegistry>());
        return services;
    }

}
