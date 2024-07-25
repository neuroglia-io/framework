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
    /// <inheritdoc />
    public virtual string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Stores the element's label
    /// </summary>
    protected string? _label;
    /// <inheritdoc />
    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public virtual string? Label
    {
        get => this._label;
        set
        {
            this._label = value;
            this.OnChange();
        }
    }

    /// <summary>
    /// Stores the element's component type
    /// </summary>
    protected Type? _componentType;
    /// <inheritdoc />
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public virtual Type? ComponentType
    {
        get => this._componentType;
        set
        {
            this._componentType = value;
            this.OnChange();
        }
    }

    /// <summary>
    /// Stores the element's css class
    /// </summary>
    protected string? _cssClass;
    /// <inheritdoc />
    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public virtual string? CssClass
    {
        get => this._cssClass;
        set
        {
            this._cssClass = value;
            this.OnChange();
        }
    }


    /// <summary>
    /// Stores the element's metadata
    /// </summary>
    protected IDictionary<string, object>? _metadata = null;
    /// <inheritdoc />
    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    [System.Text.Json.Serialization.JsonExtensionData]
    [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    [Newtonsoft.Json.JsonExtensionData]
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
    /// The action tiggered when a property changes
    /// </summary>
    public event Action? Changed;

    /// <summary>
    /// Constructs a new <see cref="GraphElement"/>
    /// </summary>
    protected GraphElement() 
    { }

    /// <summary>
    /// Constructs a new <see cref="GraphElement"/>
    /// </summary>
    /// <param name="label">The element's label</param>
    /// <param name="cssClass">The element's css class(es)</param>
    /// <param name="componentType">The element's component(template) type</param>
    protected GraphElement(string? label = "", string? cssClass = null, Type? componentType = null) {
        this.Label = label;
        this.CssClass = cssClass;
        this.ComponentType = componentType;
    }

    /// <summary>
    /// Invokes the change action
    /// </summary>
    protected virtual void OnChange() => this.Changed?.Invoke();

}
