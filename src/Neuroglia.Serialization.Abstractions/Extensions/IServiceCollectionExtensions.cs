﻿// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
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
using Neuroglia.Serialization.Json;

namespace Neuroglia.Serialization;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configure Neuroglia Serialization services such as <see cref="ISerializerProvider"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="lifetime">The lifetime of configured services</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddSerialization(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.TryAdd(new ServiceDescriptor(typeof(ISerializerProvider), typeof(SerializerProvider), lifetime));
        services.AddJsonSerializer(lifetime: lifetime);
        return services;
    }

    /// <summary>
    /// Adds and configures the specified <see cref="ISerializer"/>
    /// </summary>
    /// <typeparam name="TSerializer">The type of <see cref="ISerializer"/> to add</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="lifetime">The <see cref="ISerializer"/>'s lifetime</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddSerializer<TSerializer>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TSerializer : class, ISerializer
    {
        services.TryAdd(new ServiceDescriptor(typeof(TSerializer), typeof(TSerializer), lifetime));
        if (!services.Any(s => s.ServiceType == typeof(ISerializer) && s.ImplementationType == typeof(TSerializer))) services.Add(new ServiceDescriptor(typeof(ISerializer), typeof(TSerializer), lifetime));
        return services;
    }

    /// <summary>
    /// Adds and configure a <see cref="JsonSerializer"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to configure the <see cref="System.Text.Json.JsonSerializerOptions"/> to use</param>
    /// <param name="lifetime">The <see cref="JsonSerializer"/>'s lifetime</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddJsonSerializer(this IServiceCollection services, Action<System.Text.Json.JsonSerializerOptions>? setup = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        setup ??= JsonSerializer.DefaultOptionsConfiguration!;
        services.Configure(setup);
        services.AddSerializer<JsonSerializer>(lifetime);
        services.TryAdd(new ServiceDescriptor(typeof(IJsonSerializer), typeof(JsonSerializer), lifetime));
        return services;
    }

}
