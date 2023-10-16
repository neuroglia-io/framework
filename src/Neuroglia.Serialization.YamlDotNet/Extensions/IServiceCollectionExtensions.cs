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
using Neuroglia.Serialization.Yaml;
using YamlDotNet.Serialization;

namespace Neuroglia.Serialization;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configure a <see cref="YamlSerializer"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to configure the <see cref="YamlSerializer"/> to use</param>
    /// <param name="lifetime">The <see cref="YamlSerializer"/>'s lifetime</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddYamlDotNetSerializer(this IServiceCollection services, Action<IYamlSerializerBuilder>? setup = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        var builder = new YamlSerializerBuilder();
        setup?.Invoke(builder);
        services.TryAdd(new ServiceDescriptor(typeof(YamlDotNet.Serialization.ISerializer), builder.Serializer.Build()));
        services.TryAdd(new ServiceDescriptor(typeof(IDeserializer), builder.Deserializer.Build()));
        services.AddSerializer<YamlSerializer>(lifetime);
        return services;
    }

}