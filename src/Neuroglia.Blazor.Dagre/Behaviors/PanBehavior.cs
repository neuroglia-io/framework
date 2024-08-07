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

using Microsoft.AspNetCore.Components.Web;
using Neuroglia.Blazor.Dagre.Models;

namespace Neuroglia.Blazor.Dagre.Behaviors;

internal class PanBehavior
    : GraphBehavior
{

    bool _enableMovement = false;
    double _previousX = 0;
    double _previousY = 0;
    double _movementX = 0;
    double _movementY = 0;

    public PanBehavior(IGraphViewModel graph)
        : base(graph)
    {
        this.Graph.MouseMove += this.OnMouseMoveAsync;
        this.Graph.MouseDown += this.OnMouseDownAsync;
        this.Graph.MouseUp += this.OnMouseUpAsync;
    }

    protected virtual async Task OnMouseMoveAsync(GraphEventArgs<MouseEventArgs> e)
    {
        if (!this._enableMovement) return;
        this._movementX = e.BaseEvent.ClientX - this._previousX;
        this._movementY = e.BaseEvent.ClientY - this._previousY;
        this.UpdatedPosition();
        await Task.CompletedTask;
    }

    protected virtual async Task OnMouseDownAsync(GraphEventArgs<MouseEventArgs> e)
    {
        if (e.GraphElement != null) return;
        this._enableMovement = true;
        this._previousX = e.BaseEvent.ClientX;
        this._previousY = e.BaseEvent.ClientY;
        await Task.CompletedTask;
    }

    protected virtual async Task OnMouseUpAsync(GraphEventArgs<MouseEventArgs> e)
    {
        this._enableMovement = false;
        if (e.GraphElement != null) return;
        this.UpdatedPosition();
        await Task.CompletedTask;
    }

    protected virtual void UpdatedPosition()
    {
        if (this._movementX != 0 || this._movementY != 0) { 
            this.Graph.X += this._movementX / (double)this.Graph.Scale;
            this.Graph.Y += this._movementY / (double)this.Graph.Scale;
            this._previousX += this._movementX;
            this._previousY += this._movementY;
            this._movementX = 0;
            this._movementY = 0;
        }
    }

    public override void Dispose()
    {
        this.Graph.MouseMove -= this.OnMouseMoveAsync;
        this.Graph.MouseDown -= this.OnMouseDownAsync;
        this.Graph.MouseUp -= this.OnMouseUpAsync;
    }
}
