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

public class EdgeViewModel(string sourceId, string targetId, string? label = null, string? cssClass = null, string? shape = null, IEnumerable<IPositionable>? points = null, Type? componentType = null)
    : GraphElement(label, cssClass, componentType), IEdgeViewModel
{
    /// <inheritdoc/>
    public virtual string SourceId { get; set; } = sourceId;

    /// <inheritdoc/>
    public virtual string TargetId { get; set; } = targetId;

    IEnumerable<IPositionable> _points = points ?? [];
    /// <inheritdoc/>
    public virtual IEnumerable<IPositionable> Points
    {
        get => this._points;
        set
        {
            this._points = [.. value];
            this.UpdateBounds();
        }
    }

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

    double _width = Constants.EdgeLabelWidth;
    /// <inheritdoc/>
    public virtual double Width
    {
        get => this._width;
        set
        {
            this._width = value;
            this.UpdateBounds();
        }
    }

    double _height = Constants.EdgeLabelHeight;
    /// <inheritdoc/>
    public virtual double Height
    {
        get => this._height;
        set
        {
            this._height = value;
            this.UpdateBounds();
        }
    }

    /// <inheritdoc/>
    [System.Text.Json.Serialization.JsonPropertyName("labelpos")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty(PropertyName = "labelpos", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public virtual string? LabelPosition { get; set; }

    /// <inheritdoc/>
    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public virtual double? LabelOffset { get; set; }

    /// <inheritdoc/>
    public virtual string Shape { get; set; } = shape ?? EdgeShape.BSpline;

    /// <inheritdoc/>
    public virtual string? StartMarkerId { get; set; }

    /// <inheritdoc/>
    public virtual string? EndMarkerId { get; set; } = Constants.EdgeEndArrowId;

    BoundingBox _bounds = new BoundingBox(Constants.EdgeLabelWidth, Constants.EdgeLabelHeight);
    /// <inheritdoc/>
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public virtual BoundingBox Bounds => this._bounds;

    /// <inheritdoc/>
    public virtual void SetBounds(IEnumerable<IPositionable> points, double width = 0, double height = 0, double x = 0, double y = 0)
    {
        this._points = points;
        this._x = x;
        this._y = y;
        this._width = width;
        this._height = height;
        this.UpdateBounds();
    }

    /// <summary>
    /// Updates the bounding box
    /// </summary>
    private void UpdateBounds()
    {
        var minX = 0d;
        var maxX = 0d;
        var minY = 0d;
        var maxY = 0d;
        if (this.Points.Any())
        {
            minX = this.Points.Min(p => p.X);
            maxX = this.Points.Max(p => p.X);
            minY = this.Points.Min(p => p.Y);
            maxY = this.Points.Max(p => p.Y);
        }
        var width = maxX - minX;
        var height = maxY - minY;
        var x = minX + width;
        var y = minY + height;
        this._bounds = new BoundingBox(Math.Max(width, 1), Math.Max(height, 1), minX, minY);
        this.OnChange();
    }
}
