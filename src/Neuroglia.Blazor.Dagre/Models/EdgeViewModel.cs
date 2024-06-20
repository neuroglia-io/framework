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

public class EdgeViewModel(INodeViewModel source, INodeViewModel target)
    : GraphElement, IEdgeViewModel
{

    List<Point> _points = [];

    /// <inheritdoc/>
    public virtual INodeViewModel Source { get; } = source;

    /// <inheritdoc/>
    public virtual INodeViewModel Target { get; } = target;

    /// <inheritdoc/>
    public virtual IReadOnlyCollection<Point> Points
    {
        get
        {
            return this._points;
        }
        set
        {
            this._points = [.. value];
            var minX = this.Points.Min(p => p.X);
            var maxX = this.Points.Max(p => p.X);
            var minY = this.Points.Min(p => p.Y);
            var maxY = this.Points.Max(p => p.Y);
            var width = maxX - minX;
            var height = maxY - minY;
            var x = minX + width / 2;
            var y = minY + height / 2;
            this.Bounds = new(new(width, height), new(x, y));
            this.OnChange();
        }
    }

    /// <inheritdoc/>
    public virtual BoundingBox Bounds { get; protected set; } = new(new(0, 0), new(0, 0));

    public virtual Point Position { get; protected set; } = new(0, 0);

    public virtual Size Size { get; protected set; } = new(0, 0);

    public virtual string? LabelPosition { get; set; }

    public virtual double? LabelOffset { get; set; }

    public virtual string Shape { get; set; } = EdgeShape.BSpline;

    public virtual string? StartMarkerId { get; set; }

    public virtual string? EndMarkerId { get; set; } = Constants.EdgeEndArrowId;

}
