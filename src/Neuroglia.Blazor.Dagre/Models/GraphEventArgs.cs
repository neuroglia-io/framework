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

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Neuroglia.Blazor.Dagre.Models;

/// <summary>
/// Supplies information about a mouse event that's being raised by the graph
/// </summary>
/// <remarks>
/// Constructs a new <see cref="GraphArgs"/>
/// </remarks>
/// <param name="baseEvent"></param>
/// <param name="component"></param>
/// <param name="graphElement"></param>
public class GraphEventArgs<T>(T baseEvent, ElementReference component, IGraphElement? graphElement)
    where T : EventArgs
{
    /// <summary>
    /// The component <see cref="ElementReference"/> that raised the event
    /// </summary>
    public ElementReference Component { get; set; } = component;

    /// <summary>
    /// The graph element, <see cref="IGraphElement"/>, that raised the event, if any.
    /// </summary>
    public IGraphElement? GraphElement { get; set; } = graphElement;

    /// <summary>
    /// The <see cref="EventArgs"/> (<see cref="MouseEventArgs"/> or <see cref="WheelEventArgs"/>)
    /// </summary>
    public T BaseEvent { get; set; } = baseEvent;
}
