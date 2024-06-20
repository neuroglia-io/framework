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

namespace Neuroglia.Blazor.Dagre.Models;

/// <summary>
/// Represents a basic element of <see cref="IGraphViewModel"/>
/// </summary>
public abstract class GraphElement
    : IGraphElement
{

    /// <inheritdoc/>
    public event Action? Changed;

    string? _label;
    Type? _componentType;
    string? _cssClass;
    IDictionary<string, object>? _metadata = null;

    /// <inheritdoc />
    public virtual string Id { get; set; } = Guid.NewGuid().ToShortString();

    /// <inheritdoc />
    public virtual string? Label
    {
        get => this._label;
        set
        {
            this._label = value;
            this.OnChange();
        }
    }

    /// <inheritdoc />
    public virtual Type? ComponentType
    {
        get => this._componentType;
        set
        {
            this._componentType = value;
            this.OnChange();
        }
    }

    /// <inheritdoc />
    public virtual string? CssClass
    {
        get => this._cssClass;
        set
        {
            this._cssClass = value;
            this.OnChange();
        }
    }

    /// <inheritdoc />
    public virtual IDictionary<string, object>? Metadata
    {
        get => this._metadata;
        set
        {
            this._metadata = value;
            this.OnChange();
        }
    }

    /// <summary>
    /// Invokes the change action
    /// </summary>
    protected virtual void OnChange() => this.Changed?.Invoke();

}
