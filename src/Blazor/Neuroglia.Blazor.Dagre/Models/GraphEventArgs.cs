using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Neuroglia.Blazor.Dagre.Models
{
    /// <summary>
    /// Supplies information about a mouse event that's being raised by the graph
    /// </summary>
    public class GraphEventArgs<T>
        where T : EventArgs
    {
        /// <summary>
        /// The component <see cref="ElementReference"/> that raised the event
        /// </summary>
        public ElementReference Component { get; set; }

        /// <summary>
        /// The graph element, <see cref="IGraphElement"/>, that raised the event, if any.
        /// </summary>
        public IGraphElement? GraphElement { get; set; }

        /// <summary>
        /// The <see cref="EventArgs"/> (<see cref="MouseEventArgs"/> or <see cref="WheelEventArgs"/>)
        /// </summary>
        public T BaseEvent { get; set; }

        /// <summary>
        /// Constructs a new <see cref="GraphArgs"/>
        /// </summary>
        /// <param name="baseEvent"></param>
        /// <param name="component"></param>
        /// <param name="graphElement"></param>
        public GraphEventArgs(T baseEvent, ElementReference component, IGraphElement? graphElement)
        {
            this.Component = component;
            this.GraphElement = graphElement;
            this.BaseEvent = baseEvent;
        }
    }
}
