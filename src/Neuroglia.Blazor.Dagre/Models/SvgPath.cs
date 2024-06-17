﻿// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
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

    public virtual SvgPath MoveTo(IPosition position)
    {
        ArgumentNullException.ThrowIfNull(position);
        if (position.X == null) throw new ArgumentNullException(nameof(position.X));
        if (position.Y == null) throw new ArgumentNullException(nameof(position.Y));
        return this.MoveTo(position.X.Value, position.Y.Value);
    }

    public virtual SvgPath MoveTo(double x, double y)
    {
        this.Path += $"M {x.ToInvariantString()} {y.ToInvariantString()} ";
        return this;
    }

    public virtual SvgPath LineTo(IPosition position)
    {
        ArgumentNullException.ThrowIfNull(position);
        if (position.X == null) throw new ArgumentNullException(nameof(position.X));
        if (position.Y == null) throw new ArgumentNullException(nameof(position.Y));
        return this.LineTo(position.X.Value, position.Y.Value);
    }

    public virtual SvgPath LineTo(double x, double y)
    {
        this.Path += $"L {x.ToInvariantString()} {y.ToInvariantString()} ";
        return this;
    }

    public virtual SvgPath QuadraticCurveTo(IPosition control1, IPosition position)
    {
        ArgumentNullException.ThrowIfNull(control1);
        if (control1.X == null) throw new ArgumentNullException(nameof(control1.X));
        if (control1.Y == null) throw new ArgumentNullException(nameof(control1.Y));
        ArgumentNullException.ThrowIfNull(position);
        if (position.X == null) throw new ArgumentNullException(nameof(position.X));
        if (position.Y == null) throw new ArgumentNullException(nameof(position.Y));
        return this.QuadraticCurveTo(control1.X.Value, control1.Y.Value, position.X.Value, position.Y.Value);
    }

    public virtual SvgPath QuadraticCurveTo(double control1x, double control1y, double x, double y)
    {
        this.Path += $"Q {control1x.ToInvariantString()} {control1y.ToInvariantString()} {x.ToInvariantString()} {y.ToInvariantString()} ";
        return this;
    }

    public virtual SvgPath BezierCurveTo(IPosition control1, IPosition control2, IPosition position)
    {
        ArgumentNullException.ThrowIfNull(control1);
        if (control1.X == null) throw new ArgumentNullException(nameof(control1.X));
        if (control1.Y == null) throw new ArgumentNullException(nameof(control1.Y));
        ArgumentNullException.ThrowIfNull(control2);
        if (control2.X == null) throw new ArgumentNullException(nameof(control2.X));
        if (control2.Y == null) throw new ArgumentNullException(nameof(control2.Y));
        ArgumentNullException.ThrowIfNull(position);
        if (position.X == null) throw new ArgumentNullException(nameof(position.X));
        if (position.Y == null) throw new ArgumentNullException(nameof(position.Y));
        return this.BezierCurveTo(control1.X.Value, control1.Y.Value, control2.X.Value, control2.Y.Value, position.X.Value, position.Y.Value);
    }

    public virtual SvgPath BezierCurveTo(double control1x, double control1y, double control2x, double control2y, double x, double y)
    {
        this.Path += $"C {control1x.ToInvariantString()} {control1y.ToInvariantString()} {control2x.ToInvariantString()} {control2y.ToInvariantString()} {x.ToInvariantString()} {y.ToInvariantString()} ";
        return this;
    }
}
