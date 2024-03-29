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

using Neuroglia.Plugins.Configuration;
using System.Reflection;

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Represents an <see cref="IPluginSource"/> implementation that retrieves plugins from a specific <see cref="Assembly"/>
/// </summary>
public class AssemblyPluginSource
    : IPluginSource
{

    readonly List<IPluginDescriptor> _plugins = new();

    /// <summary>
    /// Initializes a new <see cref="AssemblyPluginSource"/>
    /// </summary>
    /// <param name="name">The name of the source, if any</param>
    /// <param name="options">The source's options</param>
    /// <param name="path">The path to the <see cref="Assembly"/> file used to source <see cref="IPluginDescriptor"/>s</param>
    public AssemblyPluginSource(string? name, PluginSourceOptions options, string path)
    {
        if(string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
        if (!System.IO.Path.IsPathRooted(path)) path = System.IO.Path.Combine(AppContext.BaseDirectory, path);
        if (!File.Exists(path)) throw new FileNotFoundException($"Failed to find the specified file '{path}'", path);
        this.Name = name;
        this.Path = path;
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc/>
    public virtual string? Name { get; }

    /// <summary>
    /// Gets the path to the <see cref="Assembly"/> file used to source <see cref="IPluginDescriptor"/>s
    /// </summary>
    protected virtual string Path { get; }

    /// <summary>
    /// Gets the source's options
    /// </summary>
    protected PluginSourceOptions Options { get; }

    /// <inheritdoc/>
    public virtual bool IsLoaded { get; protected set; }

    /// <inheritdoc/>
    public virtual IReadOnlyList<IPluginDescriptor> Plugins => this.IsLoaded ? this._plugins.AsReadOnly() : throw new NotSupportedException("The plugin source has not yet been loaded");

    /// <inheritdoc/>
    public virtual async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (this.IsLoaded) throw new NotSupportedException("The plugin source is already loaded");

        var assemblyFile = new FileInfo(this.Path);
        if (!assemblyFile.Exists) throw new FileNotFoundException($"Failed to find the specified assembly file '{assemblyFile.FullName}'", assemblyFile.FullName);
        var assemblyLoadContext = new PluginAssemblyLoadContext(assemblyFile.FullName);
        var assembly = assemblyLoadContext.Load();

        foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract && this.Options.Filter.Filters(t)))
        {
            var pluginAttribute = type.GetCustomAttribute<PluginAttribute>();
            var name = pluginAttribute?.Name ?? type.FullName!;
            var version = pluginAttribute?.Version ?? assembly.GetName().Version ?? new(1, 0, 0);
            var tags = pluginAttribute?.Tags;
            this._plugins.Add(new PluginDescriptor(name, version, type, assembly, assemblyLoadContext, this, tags));
        }

        this.IsLoaded = true;

        await Task.CompletedTask;
    }

}