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

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Defines the fundamentals of a service used to provide <see cref="IPluginDescriptor"/>s
/// </summary>
public interface IPluginProvider
{

    /// <summary>
    /// Gets all sourced <see cref="IPluginDescriptor"/>s
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing all sourced <see cref="IPluginDescriptor"/>s</returns>
    IEnumerable<IPluginDescriptor> GetPlugins();

    /// <summary>
    /// Gets the specified plugin
    /// </summary>
    /// <typeparam name="TService">The type of the contract implemented by the plugin to get. Must be an interface</typeparam>
    /// <param name="name">The name of the plugin to get</param>
    /// <param name="version">The version of the plugin to get</param>
    /// <param name="sourceName">The name of the source, if any, to get the specified plugin from</param>
    /// <returns>The specified plugin</returns>
    TService GetPlugin<TService>(string name, Version version, string? sourceName = null)
        where TService : class;

    /// <summary>
    /// Gets all sourced plugins that implement the specified contract
    /// </summary>
    /// <param name="serviceType">The type of the contract implemented by sourced plugins. Must be an interface</param>
    /// <param name="sourceName">The name of the plugin source, if any, to get plugin implementations from</param>
    /// <param name="tags">An <see cref="IEnumerable{T}"/> containing the tags, if any, the plugins to get must define</param>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing the plugins that implement the specified contract</returns>
    IEnumerable<object> GetPlugins(Type serviceType, string? sourceName = null, IEnumerable<string>? tags = null);

    /// <summary>
    /// Gets all sourced plugins that implement the specified contract
    /// </summary>
    /// <typeparam name="TService">The type of the contract implemented by sourced plugins. Must be an interface</typeparam>
    /// <param name="sourceName">The name of the plugin source, if any, to get plugin implementations from</param>
    /// <param name="tags">An <see cref="IEnumerable{T}"/> containing the tags, if any, the plugins to get must define</param>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing the plugins that implement the specified contract</returns>
    IEnumerable<TService> GetPlugins<TService>(string? sourceName = null, IEnumerable<string>? tags = null)
        where TService : class;

}