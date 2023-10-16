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

using Neuroglia.Plugins.Services;
using System.Reflection;
using System.Runtime.Loader;

namespace Neuroglia.Plugins;

/// <summary>
/// Represents the default implementation of the <see cref="IPluginDescriptor"/> interface
/// </summary>
public class PluginDescriptor
    : IPluginDescriptor
{

    /// <summary>
    /// Initializes a new <see cref="PluginDescriptor"/>
    /// </summary>
    protected PluginDescriptor() { }

    /// <summary>
    /// Initializes a new <see cref="PluginDescriptor"/>
    /// </summary>
    /// <param name="name">The name of the <see cref="IPluginDescriptor"/></param>
    /// <param name="version">The version of the <see cref="IPluginDescriptor"/></param>
    /// <param name="type">The type of the <see cref="IPluginDescriptor"/></param>
    /// <param name="assembly">The assembly of the <see cref="IPluginDescriptor"/></param>
    /// <param name="assemblyLoadContext">The <see cref="IPluginDescriptor"/>'s <see cref="System.Runtime.Loader.AssemblyLoadContext"/></param>
    /// <param name="source">The <see cref="IPluginSource"/> the <see cref="IPluginDescriptor"/> is sourced by</param>
    /// <param name="tags">A list containing the <see cref="IPluginDescriptor"/>'s tags, if any</param>
    public PluginDescriptor(string name, Version version, Type type, Assembly assembly, AssemblyLoadContext assemblyLoadContext, IPluginSource source, IEnumerable<string>? tags = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        this.Name = name;
        this.Version = version ?? new(1, 0, 0);
        this.Type = type ?? throw new ArgumentNullException(nameof(type));
        this.Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        this.AssemblyLoadContext = assemblyLoadContext ?? throw new ArgumentNullException(nameof(assemblyLoadContext));
        this.Source = source ?? throw new ArgumentNullException(nameof(source));
        this.Tags = tags;
    }

    /// <inheritdoc/>
    public virtual string Name { get; protected set; } = null!;

    /// <inheritdoc/>
    public virtual Version Version { get; protected set; } = null!;

    /// <inheritdoc/>
    public virtual Type Type { get; protected set; } = null!;

    /// <inheritdoc/>
    public virtual Assembly Assembly { get; protected set; } = null!;

    /// <inheritdoc/>
    public AssemblyLoadContext AssemblyLoadContext { get; protected set; } = null!;

    /// <inheritdoc/>
    public IPluginSource Source { get; protected set; } = null!;

    /// <inheritdoc/>
    public IEnumerable<string>? Tags { get; protected set; }

}
