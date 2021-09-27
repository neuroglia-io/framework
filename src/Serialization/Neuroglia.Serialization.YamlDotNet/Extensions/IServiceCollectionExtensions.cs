/*
 * Copyright © 2021 Neuroglia SPRL. All rights reserved.
 * <p>
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * <p>
 * http://www.apache.org/licenses/LICENSE-2.0
 * <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;
using Yaml = YamlDotNet.Serialization;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds and configures an YamlDotNet <see cref="ISerializer"/> and <see cref="IDeserializer"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="serializerConfiguration">The <see cref="Action{T}"/> used to configure the <see cref="ISerializer"/> to add</param>
        /// <param name="deserializerConfiguration">The <see cref="Action{T}"/> used to configure the <see cref="IDeserializer"/> to add</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddYamlDotNet(this IServiceCollection services, Action<SerializerBuilder> serializerConfiguration = null, Action<DeserializerBuilder> deserializerConfiguration = null)
        {
            services.TryAddSingleton(provider =>
            {
                SerializerBuilder builder = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .WithTypeConverter(new Yaml.UriTypeConverter())
                    .WithTypeConverter(new JTokenSerializer())
                    .WithTypeConverter(new JSchemaTypeConverter());
                serializerConfiguration?.Invoke(builder);
                return builder.Build();
            });
            services.TryAddSingleton(provider =>
            {
                DeserializerBuilder builder = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .WithTypeConverter(new Yaml.UriTypeConverter())
                    .WithNodeDeserializer(
                        inner => new AbstractTypeDeserializer(inner),
                        syntax => syntax.InsteadOf<ObjectNodeDeserializer>())
                    .WithNodeDeserializer(
                        inner => new JTokenDeserializer(inner),
                        syntax => syntax.InsteadOf<DictionaryNodeDeserializer>())
                    .WithNodeDeserializer(
                        inner => new JSchemaDeserializer(inner),
                        syntax => syntax.InsteadOf<JTokenDeserializer>())
                    .WithObjectFactory(new NonPublicConstructorObjectFactory())
                    .IncludeNonPublicProperties();
                deserializerConfiguration?.Invoke(builder);
                return builder.Build();
            });
            return services;
        }

        /// <summary>
        /// Adds and configures a <see cref="YamlDotNetSerializer"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="serializerConfiguration">The <see cref="Action{T}"/> used to configure the <see cref="ISerializer"/> to add</param>
        /// <param name="deserializerConfiguration">The <see cref="Action{T}"/> used to configure the <see cref="IDeserializer"/> to add</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddYamlDotNetSerializer(this IServiceCollection services, Action<SerializerBuilder> serializerConfiguration = null, Action<DeserializerBuilder> deserializerConfiguration = null)
        {
            services.AddYamlDotNet(serializerConfiguration, deserializerConfiguration);
            return services.AddSerializer<YamlDotNetSerializer>();
        }

    }                   

}
