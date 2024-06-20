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

public class SvgPath
{
    protected virtual string Path { get; set; } = "";

    public virtual string GetPath() => this.Path;

    public virtual SvgPath MoveTo(Point position)
    {
        ArgumentNullException.ThrowIfNull(position);
        return this.MoveTo(position.X, position.Y);
    }

    public virtual SvgPath MoveTo(double x, double y)
    {
        this.Path += $"M {x.ToInvariantString()} {y.ToInvariantString()} ";
        return this;
    }

    public virtual SvgPath LineTo(Point point)
    {
        ArgumentNullException.ThrowIfNull(point);
        return this.LineTo(point.X, point.Y);
    }

    public virtual SvgPath LineTo(double x, double y)
    {
        this.Path += $"L {x.ToInvariantString()} {y.ToInvariantString()} ";
        return this;
    }

    public virtual SvgPath QuadraticCurveTo(IPositionable origin, IPositionable target)
    {
        ArgumentNullException.ThrowIfNull(origin);
        ArgumentNullException.ThrowIfNull(target);
        return this.QuadraticCurveTo(origin.Position.X, origin.Position.Y, target.Position.X, target.Position.Y);
    }

    public virtual SvgPath QuadraticCurveTo(double control1x, double control1y, double x, double y)
    {
        this.Path += $"Q {control1x.ToInvariantString()} {control1y.ToInvariantString()} {x.ToInvariantString()} {y.ToInvariantString()} ";
        return this;
    }

    public virtual SvgPath BezierCurveTo(IPositionable control1, IPositionable control2, IPositionable position)
    {
        ArgumentNullException.ThrowIfNull(control1);
        ArgumentNullException.ThrowIfNull(control2);
        ArgumentNullException.ThrowIfNull(position);
        return this.BezierCurveTo(control1.Position.X, control1.Position.Y, control2.Position.X, control2.Position.Y, position.Position.X, position.Position.Y);
    }

    public virtual SvgPath BezierCurveTo(double control1x, double control1y, double control2x, double control2y, double x, double y)
    {
        this.Path += $"C {control1x.ToInvariantString()} {control1y.ToInvariantString()} {control2x.ToInvariantString()} {control2y.ToInvariantString()} {x.ToInvariantString()} {y.ToInvariantString()} ";
        return this;
    }

}
