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
using Neuroglia.Blazor.Dagre.Services;
using Neuroglia.Blazor.Dagre.Templates;
using System.Collections.ObjectModel;

namespace Neuroglia.Blazor.Dagre.Models;

/// <summary>
/// Represents the default implementation of the <see cref="IGraphViewModel"/> interface
/// </summary>
/// <param name="graphLayout">The graph's layout</param>
public class GraphViewModel(IGraphLayout graphLayout)
    : GraphElement, IGraphViewModel
{

    Point _position = new(0, 0);
    Size _size = new(0, 0);
    decimal _scale;
    bool _enableProfiling;
    readonly Dictionary<Type, GraphBehavior> _behaviors = [];
    readonly Dictionary<string, INodeViewModel> _nodes = [];
    readonly Dictionary<string, INodeViewModel> _allNodes = [];
    readonly Dictionary<string, IEdgeViewModel> _edges = [];
    readonly Dictionary<string, IClusterViewModel> _clusters = [];
    readonly Dictionary<string, IClusterViewModel> _allClusters = [];
    readonly Dictionary<Type, Type> _components = [];
    readonly Collection<Type> _svgDefinitionComponents = [];
    readonly Type _defaultNodeComponentType = typeof(NodeTemplate);
    readonly Type _defaultClusterComponentType = typeof(ClusterTemplate);
    readonly Type _defaultEdgeComponentType = typeof(EdgeTemplate);

    /// <inheritdoc/>
    public virtual IGraphLayout GraphLayout { get; } = graphLayout;

    /// <inheritdoc/>
    public virtual Point Position
    {
        get => this._position;
        set
        {
            this._position = value;
            this.OnChange();
        }
    }

    /// <inheritdoc/>
    public virtual Size Size
    {
        get => this._size;
        set
        {
            this._size = value;
            this.OnChange();
        }
    }

    /// <inheritdoc/>
    public virtual decimal Scale
    {
        get => this._scale;
        set
        {
            this._scale = value;
            this.OnChange();
        }
    }

    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, INodeViewModel> Nodes => this._nodes;

    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, INodeViewModel> AllNodes => this._allNodes;

    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, IEdgeViewModel> Edges => this._edges;

    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, IClusterViewModel> Clusters => this._clusters;

    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, IClusterViewModel> AllClusters => this._allClusters;

    /// <inheritdoc/>
    public virtual IReadOnlyCollection<Type> SvgComponentTypes => this._svgDefinitionComponents;

    /// <inheritdoc/>
    public virtual bool EnableProfiling
    {
        get => this._enableProfiling;
        set
        {
            this._enableProfiling = value;
            this.OnChange();
        }
    }

    /// <summary>
    /// Gets the default component type for nodes
    /// </summary>
    protected virtual Type DefaultNodeComponentType => this._defaultNodeComponentType;

    /// <summary>
    /// Gets the default component type for clusters
    /// </summary>
    protected virtual Type DefaultClusterComponentType => this._defaultClusterComponentType;

    /// <summary>
    /// Gets the default component type for edges
    /// </summary>
    protected virtual Type DefaultEdgeComponentType => this._defaultEdgeComponentType;

    /// <inheritdoc/>
    public virtual IClusterViewModel AddCluster(IClusterViewModel cluster)
    {
        ArgumentNullException.ThrowIfNull(cluster);
        this._clusters.Add(cluster.Id, cluster);
        this._allClusters.Add(cluster.Id, cluster);
        cluster.ChildAdded += this.OnChildAdded;
        this.Flatten(cluster);
        this.OnChange();
        return cluster;
    }

    /// <inheritdoc/>
    public virtual INodeViewModel AddNode(INodeViewModel node)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (node is IClusterViewModel cluster) return this.AddCluster(cluster);
        this._nodes.Add(node.Id, node);
        this._allNodes.Add(node.Id, node);
        this.GraphLayout.AddNode(node);
        this.OnChange();
        return node;
    }

    /// <inheritdoc/>
    public virtual IEdgeViewModel AddEdge(INodeViewModel source, INodeViewModel target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);
        var link = new EdgeViewModel(source, target);
        this._edges.Add($"{source.Id}:{target.Id}", link);
        this.GraphLayout.AddEdge(source, target);
        this.OnChange();
        return link;
    }

    /// <inheritdoc/>
    public virtual void RegisterComponentType<TElement, TComponent>()
        where TElement : IGraphElement
        where TComponent : ComponentBase
    {
        var elementType = typeof(TElement);
        var componentType = typeof(TComponent);
        if (elementType == null) throw new ArgumentNullException(nameof(TElement));
        if (componentType == null) throw new ArgumentNullException(nameof(TComponent));
        if (this._components.ContainsKey(elementType)) throw new ArgumentException("An element with the same key already exists in the dictionary.");
        this._components.Add(elementType, componentType);
    }

    /// <inheritdoc/>
    public virtual Type GetComponentType<TElement>(TElement element)
        where TElement : IGraphElement
    {
        ArgumentNullException.ThrowIfNull(element);
        if (element.ComponentType != null) return element.ComponentType;
        var elementType = element.GetType();
        if (_components.TryGetValue(elementType, out Type? value))  return value;
        if (element is IClusterViewModel) return this.DefaultClusterComponentType;
        if (element is IEdgeViewModel) return this.DefaultEdgeComponentType;
        return this.DefaultNodeComponentType;
    }

    /// <inheritdoc/>
    public virtual void Layout()
    {
        this.GraphLayout.Compute();
        foreach (var node in this._allNodes.Values)
        {
            var nodeLayout = this.GraphLayout.GetNode(node);
            node.Position = nodeLayout.Position;
        }
        foreach (var edge in this._edges.Values)
        {
            var edgeLayout = this.GraphLayout.GetEdge(edge);
            edge.Points = edgeLayout.Points;
        }
    }

    /// <summary>
    /// Adds nested nodes/clusters to allNodes/Clusters
    /// </summary>
    /// <param name="cluster">The <see cref="IClusterViewModel"/> to flatten</param>
    public virtual void Flatten(IClusterViewModel cluster)
    {
        foreach (var subClusters in cluster.AllClusters.Values)
        {
            if (subClusters == null) continue;
            this._allClusters.Add(subClusters.Id, subClusters);
        }
        foreach (var subNode in cluster.AllNodes.Values)
        {
            if (subNode == null) continue;
            this._allNodes.Add(subNode.Id, subNode);
        }
    }

    /// <summary>
    /// Adds the provided <see cref="GraphBehavior"/>
    /// </summary>
    /// <param name="graphBehavior">The <see cref="GraphBehavior"/> to add</param>
    public virtual void RegisterBehavior(GraphBehavior graphBehavior)
    {
        var behaviorType = graphBehavior.GetType();
        if (this._behaviors.ContainsKey(behaviorType)) throw new ArgumentException($"A behavior of type '{behaviorType}' already exists", nameof(graphBehavior));
        this._behaviors.Add(behaviorType, graphBehavior);
    }

    /// <summary>
    /// Removes the provided <see cref="GraphBehavior"/>
    /// </summary>
    /// <param name="graphBehavior">The <see cref="GraphBehavior"/> to add</param>
    public virtual void UnregisterBehavior(GraphBehavior graphBehavior)
    {
        var behaviorType = graphBehavior.GetType();
        if (!_behaviors.TryGetValue(behaviorType, out GraphBehavior? value)) return;
        value.Dispose();
        this._behaviors.Remove(behaviorType);
    }

    /// <summary>
    /// Handles the addition of the specified child <see cref="INodeViewModel"/>
    /// </summary>
    /// <param name="child">The added <see cref="INodeViewModel"/></param>
    public virtual void OnChildAdded(INodeViewModel child)
    {
        if (child is IClusterViewModel cluster)
        {
            this._allClusters.Add(cluster.Id, cluster);
            this.Flatten(cluster);
        }
        else if (child is INodeViewModel node)
        {
            this._allNodes.Add(node.Id, node);
        }
        this.OnChange();
    }

}
