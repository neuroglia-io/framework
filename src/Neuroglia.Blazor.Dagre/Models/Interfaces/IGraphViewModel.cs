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

namespace Neuroglia.Blazor.Dagre.Models;

/// <summary>
/// Defines the fundamentals of a graph view model
/// </summary>
public interface IGraphViewModel
    : IIdentifiable, ILabeled, ISizeable, IPositionable, ICssStyleable, IMetadata
{

    /// <summary>
    /// The event fired whenever the graph view model changes
    /// </summary>
    event Action? Changed;

    /// <summary>
    /// Gets/sets the graph's scale
    /// </summary>
    decimal Scale { get; set; }

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
    /// Gets SVG component types
    /// </summary>
    IReadOnlyCollection<Type> SvgComponentTypes { get; }

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
    /// <param name="source">The source of the link to add</param>
    /// <param name="target">The target of the link to add</param>
    IEdgeViewModel AddEdge(INodeViewModel source, INodeViewModel target);

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
    /// Renders the layout of the <see cref="IGraphElement"/>s the <see cref="IGraphViewModel"/> is made out of
    /// </summary>
    void Layout();

}
