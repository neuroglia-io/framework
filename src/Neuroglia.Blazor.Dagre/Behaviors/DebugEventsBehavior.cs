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

public class DebugEventsBehavior
    : GraphBehavior
{
    public DebugEventsBehavior(IGraphViewModel graph)
        : base(graph)
    {
        //this.Graph.MouseMove += this.OnMouseMoveAsync;
        this.Graph.MouseEnter += this.OnMouseEnterAsync;
        this.Graph.MouseLeave += this.OnMouseLeaveAsync;
        this.Graph.MouseDown += this.OnMouseDownAsync;
        this.Graph.MouseUp += this.OnMouseUpAsync;
        this.Graph.Wheel += this.OnWheelAsync;
    }

    protected virtual async Task OnMouseEnterAsync(GraphEventArgs<MouseEventArgs> e)
    {
        this.Print("Mouse enter", e);
        await Task.CompletedTask;
    }

    protected virtual async Task OnMouseLeaveAsync(GraphEventArgs<MouseEventArgs> e)
    {
        this.Print("Mouse leave", e);
        await Task.CompletedTask;
    }

    protected virtual async Task OnMouseMoveAsync(GraphEventArgs<MouseEventArgs> e)
    {
        this.Print("Mouse move", e);
        await Task.CompletedTask;
    }

    protected virtual async Task OnMouseDownAsync(GraphEventArgs<MouseEventArgs> e)
    {
        this.Print("Mouse down", e);
        await Task.CompletedTask;
    }

    protected virtual async Task OnMouseUpAsync(GraphEventArgs<MouseEventArgs> e)
    {
        this.Print("Mouse up", e);
        await Task.CompletedTask;
    }

    protected virtual async Task OnWheelAsync(GraphEventArgs<WheelEventArgs> e)
    {
        this.Print("Wheel", e);
        await Task.CompletedTask;
    }

    protected virtual void Print(string type, GraphEventArgs<MouseEventArgs> e)
    {
        Console.WriteLine(type);
        Console.WriteLine(type + " - event - " + Newtonsoft.Json.JsonConvert.SerializeObject(e));
    }

    protected virtual void Print(string type, GraphEventArgs<WheelEventArgs> e)
    {
        Console.WriteLine(type);
        Console.WriteLine(type + " - event - " + Newtonsoft.Json.JsonConvert.SerializeObject(e));
    }

    public override void Dispose()
    {
        //this.Graph.MouseMove -= this.OnMouseMoveAsync;
        this.Graph.MouseEnter -= this.OnMouseEnterAsync;
        this.Graph.MouseLeave -= this.OnMouseLeaveAsync;
        this.Graph.MouseDown -= this.OnMouseDownAsync;
        this.Graph.MouseUp -= this.OnMouseUpAsync;
        this.Graph.Wheel -= this.OnWheelAsync;
        GC.SuppressFinalize(this);
    }
}
