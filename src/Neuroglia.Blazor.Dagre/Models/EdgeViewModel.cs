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

    private IEnumerable<IPositionable> _points = points ?? [];
    /// <inheritdoc/>
    public virtual IEnumerable<IPositionable> Points { 
        get => this._points; 
        set
        {
            this._points = [..value];
            var minX = this.Points.Min(p => p.X);
            var maxX = this.Points.Max(p => p.X);
            var minY = this.Points.Min(p => p.Y);
            var maxY = this.Points.Max(p => p.Y);
            var width = maxX - minX;
            var height = maxY - minY;
            var x = minX + width / 2;
            var y = minY + height / 2;
            this.Bounds = new BoundingBox(width, height, x, y);
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

    /// <inheritdoc/>
    public virtual BoundingBox Bounds { get; set; } = new BoundingBox();
}
