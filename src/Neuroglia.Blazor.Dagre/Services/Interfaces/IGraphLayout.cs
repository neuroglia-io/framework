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

using Neuroglia.Blazor.Dagre.Models;

namespace Neuroglia.Blazor.Dagre.Services;

/// <summary>
/// Defines the fundamentals of a graph layout
/// </summary>
public interface IGraphLayout
{

    /// <summary>
    /// Adds the specified <see cref="INodeViewModel"/> to the layout
    /// </summary>
    /// <param name="node">The <see cref="INodeViewModel"/> to add</param>
    void AddNode(INodeViewModel node);

    /// <summary>
    /// Adds a new link from the specified source to the specified target
    /// </summary>
    /// <param name="source">The source of the link to add</param>
    /// <param name="target">The target of the link to add</param>
    void AddEdge(INodeViewModel source, INodeViewModel target);

    /// <summary>
    /// Gets the layout for the specified node
    /// </summary>
    /// <param name="node">The node to get the layout for</param>
    /// <returns>The layout for the specified node</returns>
    INodeLayout GetNode(INodeViewModel node);

    /// <summary>
    /// Gets the layout for the specified edge
    /// </summary>
    /// <param name="edge">The edge to get the layout for</param>
    /// <returns>The layout for the specified edge</returns>
    IEdgeLayout GetEdge(IEdgeViewModel edge);

    /// <summary>
    /// Computes the positions of the layout's elements
    /// </summary>
    void Compute();

}
