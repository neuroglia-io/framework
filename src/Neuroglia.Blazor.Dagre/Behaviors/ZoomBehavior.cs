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

internal class ZoomBehavior
    : GraphBehavior
{
    public ZoomBehavior(IGraphViewModel graph)
        : base(graph)
    {
        this.Graph.Wheel += this.OnWheelAsync;
    }

    protected virtual async Task OnWheelAsync(GraphEventArgs<WheelEventArgs> e)
    {
        if (e.GraphElement != null)
            return;
        this.Graph.Scale += (decimal)(e.BaseEvent.DeltaY / Math.Abs(e.BaseEvent.DeltaY)) * -0.1M; ;
        this.Graph.Scale = Math.Clamp(this.Graph.Scale, Constants.MinScale, Constants.MaxScale);
        await Task.CompletedTask;
    }

    public override void Dispose()
    {
        this.Graph.Wheel -= this.OnWheelAsync;
    }
}
