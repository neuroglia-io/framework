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

public delegate Task MouseEventHandler(GraphEventArgs<MouseEventArgs> e);
public delegate Task WheelEventHandler(GraphEventArgs<WheelEventArgs> e);

/// <summary>
/// Defines the fundamentals of a graph view model
/// </summary>
public interface IGraphViewModel
    : IIdentifiable, ILabeled, ISizeable, IPositionable, ICssStylable, IMetadata
{
    /// <summary>
    /// Gets/sets the graph's scale
    /// </summary>
    decimal Scale { get; set; }

    /// <summary>
    /// Gets a boolean indicating the profiling should be enabled or not
    /// </summary>
    bool EnableProfiling { get; set; }

    /// <summary>
    /// Gets the top level graph nodes
    /// </summary>
    IReadOnlyDictionary<string, INodeViewModel> Nodes { get; }

    /// <summary>
    /// Gets all graph nodes, including nested children
    /// </summary>
    IReadOnlyDictionary<string, INodeViewModel> AllNodes { get; }

    /// <summary>
    /// Gets the graph edges
    /// </summary>
    IReadOnlyDictionary<string, IEdgeViewModel> Edges { get; }

    /// <summary>
    /// Gets the top level clusters
    /// </summary>
    IReadOnlyDictionary<string, IClusterViewModel> Clusters { get; }

    /// <summary>
    /// Gets all graph clusters, including nested children
    /// </summary>
    IReadOnlyDictionary<string, IClusterViewModel> AllClusters { get; }

    /// <summary>
    /// Gets components types for defs SVG which are not rendered directly but can be referenced by other objects
    /// </summary>
    IReadOnlyCollection<Type> ReferenceableComponentTypes { get; }

    IGraphLib? DagreGraph {  get; set; }

    /// <summary>
    /// Occurs when the mouse moves over the graph.
    /// </summary>
    event MouseEventHandler? MouseMove;

    /// <summary>
    /// Occurs when a mouse button is pressed over the graph.
    /// </summary>
    event MouseEventHandler? MouseDown;

    /// <summary>
    /// Occurs when a mouse button is released over the graph.
    /// </summary>
    event MouseEventHandler? MouseUp;

    /// <summary>
    /// Occurs when the mouse pointer enters the graph.
    /// </summary>
    event MouseEventHandler? MouseEnter;

    /// <summary>
    /// Occurs when the mouse pointer leaves the graph.
    /// </summary>
    event MouseEventHandler? MouseLeave;

    /// <summary>
    /// Occurs when the mouse wheel is used over the graph.
    /// </summary>
    event WheelEventHandler? Wheel;

    /// <summary>
    /// Occurs when the graph view model changes
    /// </summary>
    event Action? Changed;

    /// <summary>
    /// Registers a component type associated with a node type
    /// </summary>
    /// <typeparam name="TElement">The type of element to register the component type for</typeparam>
    /// <typeparam name="TComponent">The type of component to register</typeparam>
    void RegisterComponentType<TElement, TComponent>()
        where TElement : IGraphElement
        where TComponent : ComponentBase;

    /// <summary>
    /// Gets the component type associated with the specified element type
    /// </summary>
    /// <typeparam name="TElement">The type of <see cref="IGraphElement"/> to get the component type for</typeparam>
    /// <param name="element">The <see cref="IGraphElement"/> to get the component type for</param>
    /// <returns>The component type for the specified <see cref="IGraphElement"/></returns>
    Type GetComponentType<TElement>(TElement node)
        where TElement : IGraphElement;

    /// <summary>
    /// Adds the specified <see cref="IClusterViewModel"/> to the graph
    /// </summary>
    /// <param name="cluster">The <see cref="IClusterViewModel"/> to add</param>
    /// <returns>The added <see cref="IClusterViewModel"/></returns>
    IClusterViewModel AddCluster(IClusterViewModel cluster);

    /// <summary>
    /// Adds the specified <see cref="INodeViewModel"/> to the graph
    /// </summary>
    /// <param name="node">The <see cref="INodeViewModel"/> to add</param>
    INodeViewModel AddNode(INodeViewModel node);

    /// <summary>
    /// Adds a new link from the specified source to the specified target
    /// </summary>
    /// <param name="edge">The <see cref="IEdgeViewModel"> to add</param>
    IEdgeViewModel AddEdge(IEdgeViewModel edge);

    /// <summary>
    /// Handles the mouse move event asynchronously.
    /// </summary>
    /// <param name="sender">The reference to the element that triggered the event.</param>
    /// <param name="e">The mouse event arguments.</param>
    /// <param name="element">The graph element under the mouse cursor, if any.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task OnMouseMoveAsync(ElementReference sender, MouseEventArgs e, IGraphElement? element);

    /// <summary>
    /// Handles the mouse down event asynchronously.
    /// </summary>
    /// <param name="sender">The reference to the element that triggered the event.</param>
    /// <param name="e">The mouse event arguments.</param>
    /// <param name="element">The graph element under the mouse cursor, if any.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task OnMouseDownAsync(ElementReference sender, MouseEventArgs e, IGraphElement? element);

    /// <summary>
    /// Handles the mouse up event asynchronously.
    /// </summary>
    /// <param name="sender">The reference to the element that triggered the event.</param>
    /// <param name="e">The mouse event arguments.</param>
    /// <param name="element">The graph element under the mouse cursor, if any.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task OnMouseUpAsync(ElementReference sender, MouseEventArgs e, IGraphElement? element);

    /// <summary>
    /// Handles the mouse enter event asynchronously.
    /// </summary>
    /// <param name="sender">The reference to the element that triggered the event.</param>
    /// <param name="e">The mouse event arguments.</param>
    /// <param name="element">The graph element under the mouse cursor, if any.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task OnMouseEnterAsync(ElementReference sender, MouseEventArgs e, IGraphElement? element);

    /// <summary>
    /// Handles the mouse leave event asynchronously.
    /// </summary>
    /// <param name="sender">The reference to the element that triggered the event.</param>
    /// <param name="e">The mouse event arguments.</param>
    /// <param name="element">The graph element under the mouse cursor, if any.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task OnMouseLeaveAsync(ElementReference sender, MouseEventArgs e, IGraphElement? element);

    /// <summary>
    /// Handles the mouse wheel event asynchronously.
    /// </summary>
    /// <param name="sender">The reference to the element that triggered the event.</param>
    /// <param name="e">The wheel event arguments.</param>
    /// <param name="element">The graph element under the mouse cursor, if any.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task OnWheelAsync(ElementReference sender, WheelEventArgs e, IGraphElement? element);

}
