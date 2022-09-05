using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Neuroglia.Blazor.Dagre.Models;

namespace Neuroglia.Blazor.Dagre.Behaviors
{
    internal class ZoomBahavior
        : GraphBehavior
    {
        public ZoomBahavior(IGraphViewModel graph)
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
}
