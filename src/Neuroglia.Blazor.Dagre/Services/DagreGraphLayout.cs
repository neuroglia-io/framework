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

using Dagre;
using Neuroglia.Blazor.Dagre.Models;

namespace Neuroglia.Blazor.Dagre.Services;

/// <summary>
/// Represents the Dagre implementation of the <see cref="IGraphLayout"/> interface
/// </summary>
public class DagreGraphLayout
    : IGraphLayout
{

    /// <summary>
    /// Gets the underlying <see cref="DagreInputGraph"/>
    /// </summary>
    protected DagreInputGraph Graph { get; } = new();

    /// <inheritdoc/>
    public virtual void AddNode(INodeViewModel node)
    {
        ArgumentNullException.ThrowIfNull(node);
        Graph.AddNode(node.Id);
    }

    /// <inheritdoc/>
    public virtual void AddEdge(INodeViewModel source, INodeViewModel target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);
        var sourceNode = Graph.GetNode(source.Id);
        var targetNode = Graph.GetNode(target.Id);
        this.Graph.AddEdge(sourceNode, targetNode);
    }

    /// <inheritdoc/>
    public virtual INodeLayout GetNode(INodeViewModel node)
    {
        ArgumentNullException.ThrowIfNull(node);
        var layout = this.Graph.GetNode(node.Id);
        return new DagreNodeLayout(layout);
    }

    /// <inheritdoc/>
    public virtual IEdgeLayout GetEdge(IEdgeViewModel edge)
    {
        ArgumentNullException.ThrowIfNull(edge);
        var source = this.Graph.GetNode(edge.Source.Id);
        var target = this.Graph.GetNode(edge.Target.Id);
        var layout = this.Graph.GetEdge(source, target);
        return new DagreEdgeLayout(layout);
    }

    /// <inheritdoc/>
    public void Compute() => Graph.Layout();

}
