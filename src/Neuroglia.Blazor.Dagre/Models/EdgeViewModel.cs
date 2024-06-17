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

using System.Collections.ObjectModel;

namespace Neuroglia.Blazor.Dagre.Models;

public class EdgeViewModel(Guid sourceId, Guid targetId, string? label = null, string? cssClass = null, string? shape = null, ICollection<IPosition>? points = null, Type? componentType = null)
    : GraphElement(label, cssClass, componentType), IEdgeViewModel
{

    public virtual Guid SourceId { get; set; } = sourceId;
    public virtual Guid TargetId { get; set; } = targetId;

    private ICollection<IPosition> _points = points ?? [];
    public virtual ICollection<IPosition> Points { 
        get => this._points; 
        set
        {
            this._points = value;
            var minX = this.Points.Min(p => p.X);
            var maxX = this.Points.Max(p => p.X);
            var minY = this.Points.Min(p => p.Y);
            var maxY = this.Points.Max(p => p.Y);
            var width = maxX - minX;
            var height = maxY - minY;
            var x = minX + width / 2;
            var y = minY + height / 2;
            this.BBox = new BoundingBox(width, height, x, y);
        }
    }

    [System.Text.Json.Serialization.JsonPropertyName("labelpos")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty(PropertyName = "labelpos", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public virtual string? LabelPosition { get; set; }

    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public virtual double? LabelOffset { get; set; }

    public virtual string Shape { get; set; } = shape ?? EdgeShape.BSpline;
    public virtual string? StartMarkerId { get; set; }
    public virtual string? EndMarkerId { get; set; } = Constants.EdgeEndArrowId;
    public virtual IBoundingBox BBox { get; set; } = new BoundingBox();
}
