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

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Represents the service used to build <see cref="PluginTypeFilter"/>s
/// </summary>
public class PluginTypeFilterBuilder
    : IPluginTypeFilterBuilder
{

    /// <summary>
    /// Gets the <see cref="PluginTypeFilter"/> to build
    /// </summary>
    protected PluginTypeFilter Filter { get; } = new();

    /// <inheritdoc/>
    public IPluginTypeFilterBuilder AssignableTo(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (!type.IsInterface) throw new ArgumentException($"The specified type must be an interface", nameof(type));
        this.Filter.Criteria.Add(new() { AssignableTo = type.AssemblyQualifiedName });
        return this;
    }

    /// <inheritdoc/>
    public virtual IPluginTypeFilterBuilder Implements(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (!type.IsInterface) throw new ArgumentException($"The specified type must be an interface", nameof(type));
        this.Filter.Criteria.Add(new() { Implements = type.AssemblyQualifiedName });
        return this;
    }

    /// <inheritdoc/>
    public virtual IPluginTypeFilterBuilder InheritsFrom(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (!type.IsInterface) throw new ArgumentException($"The specified type must be an interface", nameof(type));
        this.Filter.Criteria.Add(new() { InheritsFrom = type.AssemblyQualifiedName });
        return this;
    }

    /// <inheritdoc/>
    public virtual IPluginTypeFilterBuilder AssignableTo<T>() where T : class => this.AssignableTo(typeof(T));

    /// <inheritdoc/>
    public virtual IPluginTypeFilterBuilder Implements<T>() where T : class => this.Implements(typeof(T));

    /// <inheritdoc/>
    public virtual IPluginTypeFilterBuilder Inherits<T>() where T : class => this.InheritsFrom(typeof(T));

    /// <inheritdoc/>
    public virtual PluginTypeFilter Build() => this.Filter;

    IPluginTypeFilter IPluginTypeFilterBuilder.Build() => this.Build();

}
