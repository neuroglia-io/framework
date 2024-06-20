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
/// Represents the default implementation of the <see cref="INodeViewModel"/> interface
/// </summary>
public class NodeViewModel
    : GraphElement, INodeViewModel
{

    string? _parentId;
    string? _shape;

    /// <inheritdoc/>
    public virtual Point Position { get; set; } = new(0, 0);

    /// <inheritdoc/>
    public virtual Size Size { get; set; } = new(100, 30);

    /// <inheritdoc/>
    public virtual Point? Radius { get; set; }

    /// <inheritdoc/>
    public virtual string? ParentId
    {
        get => this._parentId;
        set
        {
            this._parentId = value;
            this.OnChange();
        }
    }

    /// <inheritdoc/>
    public virtual string? Shape
    {
        get => this._shape;
        set
        {
            this._shape = value;
            this.OnChange();
        }
    }

    /// <inheritdoc/>
    public virtual void Move(double deltaX, double deltaY)
    {
        if (deltaX == 0 && deltaY == 0) return;
        this.Position = new(this.Position.X + deltaX, this.Position.Y + deltaY);
        this.OnChange();
    }

}
