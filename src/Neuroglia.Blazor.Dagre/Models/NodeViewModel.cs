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

public class NodeViewModel
    : GraphElement, INodeViewModel
{

    double _x = 0;
    /// <inheritdoc/>
    public virtual double X
    {
        get => this._x;
        set
        {
            this._x = value;
            this.OnChange();
        }
    }

    double _y = 0;
    /// <inheritdoc/>
    public virtual double Y
    {
        get => this._y;
        set
        {
            this._y = value;
            this.OnChange();
        }
    }

    double _width = 0;
    /// <inheritdoc/>
    public virtual double Width
    {
        get => this._width;
        set
        {
            this._width = value;
            this.OnChange();
        }
    }

    double _height = 0;
    /// <inheritdoc/>
    public virtual double Height
    {
        get => this._height;
        set
        {
            this._height = value;
            this.OnChange();
        }
    }

    double _radiusX = 0;
    /// <inheritdoc/>
    public virtual double RadiusX
    {
        get => this._radiusX;
        set
        {
            this._radiusX = value;
            this.OnChange();
        }
    }

    double _radiusY = 0;
    /// <inheritdoc/>
    public virtual double RadiusY
    {
        get => this._radiusY;
        set
        {
            this._radiusY = value;
            this.OnChange();
        }
    }

    string? _parentId = null;
    /// <inheritdoc/>
    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public virtual string? ParentId
    {
        get => this._parentId;
        set
        {
            this._parentId = value;
            this.OnChange();
        }
    }

    string? _shape = null;
    /// <inheritdoc/>
    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public virtual string? Shape
    {
        get => this._shape;
        set
        {
            this._shape = value;
            this.OnChange();
        }
    }

    BoundingBox _bounds = new();
    /// <inheritdoc/>
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public virtual BoundingBox Bounds => this._bounds;

    public NodeViewModel()
        : this("", null, null, Constants.NodeWidth, Constants.NodeHeight, Constants.NodeRadius, Constants.NodeRadius, 0, 0, null, null)
    { }

    public NodeViewModel(
        string? label = "",
        string? cssClass = null,
        string? shape = null,
        double width = Constants.NodeWidth, 
        double height = Constants.NodeHeight,
        double radiusX = Constants.NodeRadius,
        double radiusY = Constants.NodeRadius,
        double x = 0,
        double y = 0,
        Type? componentType = null,
        string? parentId = null
    )
        : base(label, cssClass, componentType)
    {
        this.Label = label;
        this.CssClass = cssClass;
        this.Shape = shape;
        this._width = width;
        this._height = height;
        this._x = x;
        this._y = y;
        this.RadiusX = radiusX;
        this.RadiusY = radiusY;
        this._bounds = new BoundingBox();
        this.ParentId = parentId;
        this.UpdateBounds();
    }

    /// <inheritdoc/>
    public virtual void SetBounds(double width = 0, double height = 0, double x = 0, double y = 0)
    {
        bool changed = false;
        if (this._x != x)
        {
            this._x = x;
            changed = true;
        }
        if (this._y != y)
        {
            this._y = y;
            changed = true;
        }
        if (this._width != width)
        {
            this._width = width;
            changed = true;
        }
        if (this._height != height)
        {
            this._height = height;
            changed = true;
        }
        if (changed)
        {
            this.UpdateBounds();
        }
    }

    /// <inheritdoc/>
    public virtual void Move(double deltaX, double deltaY)
    {
        if (deltaX == 0 && deltaY == 0) return;
        this._x += deltaX;
        this._y += deltaY;
        this.OnChange();
    }

    /// <summary>
    /// Updates the bounding box
    /// </summary>
    private void UpdateBounds()
    {
        this._bounds = this.Shape switch
        {
            NodeShape.Circle or NodeShape.Ellipse => new BoundingBox(this.Width / 2, this.Height / 2, 0, 0),
            _ => new BoundingBox(this.Width, this.Height, 0 - this.Width / 2, 0 - this.Height / 2),
        };
        this.OnChange();
    }

}
