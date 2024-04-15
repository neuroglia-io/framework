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

using System.Reflection;

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Represents a <see cref="DispatchProxy"/> used to interact with lazily loaded plugins
/// </summary>
/// <typeparam name="TPlugin">The type of the plugin interface</typeparam>
public class PluginProxy<TPlugin>
    : DispatchProxy
    where TPlugin : class
{

    TPlugin? _plugin;

    /// <summary>
    /// Gets the service used to provide plugins
    /// </summary>
    protected IPluginProvider PluginProvider { get; private set; } = null!;

    /// <summary>
    /// Gets the name of the source, if any, to load the plugin to proxy from
    /// </summary>
    protected string? SourceName { get; private set; }

    /// <summary>
    /// Gets the plugin's default implementation, if any
    /// </summary>
    protected TPlugin? DefaultImplementation { get; private set; }

    /// <summary>
    /// Gets the proxied plugin interface
    /// </summary>
    protected TPlugin Plugin
    {
        get
        {
            this._plugin ??= this.PluginProvider.GetPlugins<TPlugin>(this.SourceName).FirstOrDefault() ?? this.DefaultImplementation ?? throw new NullReferenceException($"No plugin or implementation type registered for service type '{typeof(TPlugin).Name}'");
            return this._plugin;
        }
    }

    /// <summary>
    /// Initializes the <see cref="PluginProxy{TPlugin}"/>
    /// </summary>
    /// <param name="pluginProvider">The current <see cref="IPluginProvider"/></param>
    /// <param name="defaultImplementation">The implementation to use by default, if any</param>
    /// <param name="sourceName">The name of the source, if any, to load the plugin to proxy from</param>
    protected virtual void Initialize(IPluginProvider pluginProvider, TPlugin? defaultImplementation, string? sourceName)
    {
        ArgumentNullException.ThrowIfNull(pluginProvider);
        this.PluginProvider = pluginProvider;
        this.DefaultImplementation = defaultImplementation;
        this.SourceName = sourceName;
    }

    /// <inheritdoc/>
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => targetMethod?.Invoke(this.Plugin, args);

    /// <summary>
    /// Creates a new <see cref="DispatchProxy"/> for the interfaced plugin
    /// </summary>
    /// <param name="pluginProvider">The service used to manage plugins</param>
    /// <param name="defaultImplementation">The implementation to use by default, if any</param>
    /// <param name="sourceName">The name of the source, if any, to load the plugin to proxy from</param>
    /// <returns>A new <see cref="DispatchProxy"/> implementation of the plugin interface</returns>
    public static TPlugin Create(IPluginProvider pluginProvider, TPlugin? defaultImplementation = null, string? sourceName = null)
    {
        var proxy = Create(typeof(TPlugin), typeof(PluginProxy<TPlugin>));
        ((PluginProxy<TPlugin>)proxy).Initialize(pluginProvider, defaultImplementation, sourceName);
        return (TPlugin)proxy;
    }

}

/// <summary>
/// Represents a <see cref="DispatchProxy"/> used to interact with lazily loaded plugins
/// </summary>
public static class PluginProxy
{

    /// <summary>
    /// Creates a new <see cref="DispatchProxy"/> for the interfaced plugin
    /// </summary>
    /// <param name="pluginProvider">The service used to manage plugins</param>
    /// <param name="pluginType">The type of the plugin to proxy</param>
    /// <param name="defaultImplementation">The implementation to use by default, if any</param>
    /// <param name="sourceName">The name of the source, if any, to load the plugin to proxy from</param>
    /// <returns>A new <see cref="DispatchProxy"/> implementation of the plugin interface</returns>
    public static object Create(IPluginProvider pluginProvider, Type pluginType, object? defaultImplementation = null, string? sourceName = null)
    {
        var genericProxyType = typeof(PluginProxy<>).MakeGenericType(pluginType);
        var method = genericProxyType.GetMethod(nameof(Create), BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)!;
        return method.Invoke(null, [pluginProvider, defaultImplementation, sourceName])!;
    }

}