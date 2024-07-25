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
using Neuroglia.Blazor.Dagre.Behaviors;
using Neuroglia.Blazor.Dagre.Templates;
using System.Collections.ObjectModel;

namespace Neuroglia.Blazor.Dagre.Models;

public class GraphViewModel
    : GraphElement, IGraphViewModel
{

    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public virtual IGraphLib? DagreGraph { get; set; }

    double _x = 0;
    /// <inheritdoc/>
    public virtual double X
    {
        get => this._x;
        set
        {
            this._x = value;
            this.OnChange();
        }
    }

    double _y = 0;
    /// <inheritdoc/>
    public virtual double Y
    {
        get => this._y;
        set
        {
            this._y = value;
            this.OnChange();
        }
    }

    double _width = 0;
    /// <inheritdoc/>
    public virtual double Width
    {
        get => this._width;
        set
        {
            this._width = value;
            this.OnChange();
        }
    }

    double _height = 0;
    /// <inheritdoc/>
    public virtual double Height
    {
        get => this._height;
        set
        {
            this._height = value;
            this.OnChange();
        }
    }

    decimal _scale;
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

    /// <summary>
    /// The first level graph nodes (direct children)
    /// </summary>
    protected readonly Dictionary<string, INodeViewModel> nodes;
    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, INodeViewModel> Nodes => this.nodes;

    /// <summary>
    /// The flattened graph nodes (nested children)
    /// </summary>
    protected readonly Dictionary<string, INodeViewModel> allNodes;
    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, INodeViewModel> AllNodes => this.allNodes;

    /// <summary>
    /// The graph edges
    /// </summary>
    protected readonly Dictionary<string, IEdgeViewModel> edges;
    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, IEdgeViewModel> Edges => this.edges;

    /// <summary>
    /// The first level graph clusters (direct children)
    /// </summary>
    protected readonly Dictionary<string, IClusterViewModel> clusters;
    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, IClusterViewModel> Clusters => this.clusters;

    /// <summary>
    /// The flattened graph clusters (nested children)
    /// </summary>
    protected readonly Dictionary<string, IClusterViewModel> allClusters;
    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, IClusterViewModel> AllClusters => this.allClusters;

    protected readonly Collection<Type> referenceableComponentTypes;
    /// <inheritdoc/>
    public virtual IReadOnlyCollection<Type> ReferenceableComponentTypes => this.referenceableComponentTypes;

    protected bool enableProfiling = false;
    /// <inheritdoc/>
    public virtual bool EnableProfiling
    {
        get => this.enableProfiling;
        set
        {
            this.enableProfiling = value;
            this.OnChange();
        }
    }

    /// <summary>
    /// The map of node type and their component type
    /// </summary>
    protected readonly Dictionary<Type, Type> components;

    readonly Type defaultNodeComponentType = typeof(NodeTemplate);
    protected virtual Type DefaultNodeComponentType => this.defaultNodeComponentType;

    readonly Type defaultClusterComponentType = typeof(ClusterTemplate);
    protected virtual Type DefaultClusterComponentType => this.defaultClusterComponentType;

    readonly Type defaultEdgeComponentType = typeof(EdgeTemplate);
    protected virtual Type DefaultEdgeComponentType => this.defaultEdgeComponentType;

    readonly Dictionary<Type, GraphBehavior> _behaviors;

    /// <inheritdoc/>
    public event MouseEventHandler? MouseMove;

    /// <inheritdoc/>
    public event MouseEventHandler? MouseDown;

    /// <inheritdoc/>
    public event MouseEventHandler? MouseUp;

    /// <inheritdoc/>
    public event MouseEventHandler? MouseEnter;

    /// <inheritdoc/>
    public event MouseEventHandler? MouseLeave;

    /// <inheritdoc/>
    public event WheelEventHandler? Wheel;

    public GraphViewModel(
        Dictionary<string, INodeViewModel>? nodes = null,
        Dictionary<string, IEdgeViewModel>? edges = null,
        Dictionary<string, IClusterViewModel>? clusters = null,
        Collection<Type>? referenceableComponentTypes = null,
        string? cssClass = null,
        double width = 0,
        double height = 0,
        string? label = null,
        Type? componentType = null,
        bool enableProfiling = false
    )
        : base(label, cssClass, componentType)
    {
        this.nodes = nodes ?? [];
        this.edges = edges ?? []; ;
        this.clusters = clusters ?? [];
        this.referenceableComponentTypes = referenceableComponentTypes ?? [typeof(ArrowDefinitionTemplate)];
        this.Scale = 1;
        this.X = 0;
        this.Y = 0;
        this.Width = width;
        this.Height = height;
        this.components = [];
        this.allNodes = [];
        this.allClusters = [];
        this._behaviors = [];
        this.EnableProfiling = enableProfiling;
        // this.RegisterBehavior(new DebugEventsBehavior(this));
        this.RegisterBehavior(new ZoomBehavior(this));
        this.RegisterBehavior(new PanBehavior(this));
        // this.RegisterBehavior(new MoveNodeBehavior(this));
        foreach (var node in this.nodes.Values)
        {
            if (node == null) continue;
            this.allNodes.Add(node.Id, node);
        }
        foreach (var cluster in this.clusters.Values)
        {
            if (cluster == null)  continue;
            cluster.ChildAdded += this.OnChildAdded;
            this.allClusters.Add(cluster.Id, cluster);
            this.Flatten(cluster);
        }
    }

    /// <summary>
    /// Adds the provided <see cref="IGraphElement"/> to the graph
    /// </summary>
    /// <param name="element"></param>
    /// <returns>The added element</returns>
    protected virtual IGraphElement AddElement(IGraphElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        if (element is IEdgeViewModel edge)
        {
            return this.AddEdge(edge);
        }
        if (element is IClusterViewModel cluster)
        {
            return this.AddCluster(cluster);
        }
        if (element is INodeViewModel node)
        {
            return this.AddNode(node);
        }
        throw new Exception("Unknown element type");
    }

    /// <summary>
    /// Adds the provided <see cref="IGraphElement"/>s to the graph
    /// </summary>
    /// <param name="elements"></param>
    /// <returns>The added elements</returns>
    protected virtual IEnumerable<IGraphElement> AddElements(IEnumerable<IGraphElement> elements)
    {
        if (elements == null || !elements.Any()) throw new ArgumentNullException(nameof(elements));
        foreach (var element in elements) this.AddElement(element);
        this.OnChange();
        return elements;
    }

    /// <summary>
    /// Adds the provided <see cref="IClusterViewModel"/> to the graph
    /// </summary>
    /// <param name="cluster"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual IClusterViewModel AddCluster(IClusterViewModel cluster)
    {
        ArgumentNullException.ThrowIfNull(cluster);
        this.clusters.Add(cluster.Id, cluster);
        this.allClusters.Add(cluster.Id, cluster);
        cluster.ChildAdded += this.OnChildAdded;
        this.Flatten(cluster);
        this.OnChange();
        return cluster;
    }

    /// <summary>
    /// Adds the provided <see cref="INodeViewModel"/> to the graph
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual INodeViewModel AddNode(INodeViewModel node)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (node is IClusterViewModel cluster)
        {
            return this.AddCluster(cluster);
        }
        this.nodes.Add(node.Id, node);
        this.allNodes.Add(node.Id, node);
        this.OnChange();
        return node;
    }

    /// <summary>
    /// Adds the provided <see cref="IEdgeViewModel"/> to the graph
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual IEdgeViewModel AddEdge(IEdgeViewModel edge)
    {
        ArgumentNullException.ThrowIfNull(edge);
        this.edges.Add(edge.Id, edge);
        this.OnChange();
        return edge;
    }

    /// <summary>
    /// Registers a component type associated with a node type
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TComponent"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public virtual void RegisterComponentType<TElement, TComponent>()
        where TElement : IGraphElement
        where TComponent : ComponentBase
    {
        var elementType = typeof(TElement);
        var componentType = typeof(TComponent);
        if (elementType == null)
        {
            throw new ArgumentNullException(nameof(TElement));
        }
        if (componentType == null)
        {
            throw new ArgumentNullException(nameof(TComponent));
        }
        if (this.components.ContainsKey(elementType))
        {
            throw new ArgumentException("An element with the same key already exists in the dictionary.");
        }
        this.components.Add(elementType, componentType);
    }

    /// <summary>
    /// Gets the component type associated with the specified node type
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public virtual Type GetComponentType<TElement>(TElement element)
        where TElement : IGraphElement
    {
        if (element.ComponentType != null) return element.ComponentType;
        var elementType = element.GetType();
        if (components.TryGetValue(elementType, out Type? value))  return value;
        if (element is IClusterViewModel) return this.DefaultClusterComponentType;
        if (element is IEdgeViewModel) return this.DefaultEdgeComponentType;
        return this.DefaultNodeComponentType;
    }

    /// <summary>
    /// Adds nested nodes/clusters to allNodes/Clusters
    /// </summary>
    /// <param name="cluster"></param>
    public virtual void Flatten(IClusterViewModel cluster)
    {
        foreach (var subClusters in cluster.AllClusters.Values)
        {
            if (subClusters == null) continue;
            this.allClusters.Add(subClusters.Id, subClusters);
        }
        foreach (var subNode in cluster.AllNodes.Values)
        {
            if (subNode == null) continue;
            this.allNodes.Add(subNode.Id, subNode);
        }
    }

    /// <summary>
    /// Adds the provided <see cref="GraphBehavior"/>
    /// </summary>
    /// <param name="graphBehavior"></param>
    /// <exception cref="ArgumentException"></exception>
    public virtual void RegisterBehavior(GraphBehavior graphBehavior)
    {
        var behaviorType = graphBehavior.GetType();
        if (this._behaviors.ContainsKey(behaviorType)) throw new ArgumentException($"A behavior of type '{behaviorType}' already exists", nameof(graphBehavior));
        this._behaviors.Add(behaviorType, graphBehavior);
    }

    /// <summary>
    /// Removes the provided <see cref="GraphBehavior"/>
    /// </summary>
    /// <param name="graphBehavior"></param>
    /// <exception cref="ArgumentException"></exception>
    public virtual void UnregisterBehavior(GraphBehavior graphBehavior)
    {
        var behaviorType = graphBehavior.GetType();
        if (!_behaviors.TryGetValue(behaviorType, out GraphBehavior? value)) return;
        value.Dispose();
        this._behaviors.Remove(behaviorType);
    }

    /// <inheritdoc/>
    public virtual async Task OnMouseMoveAsync(ElementReference sender, MouseEventArgs e, IGraphElement? element)
    {
        if (this.MouseMove != null) await this.MouseMove.Invoke(new (e, sender, element));
    }

    /// <inheritdoc/>
    public virtual async Task OnMouseDownAsync(ElementReference sender, MouseEventArgs e, IGraphElement? element)
    {
        if (this.MouseDown != null) await this.MouseDown.Invoke(new(e, sender, element));
    }

    /// <inheritdoc/>
    public virtual async Task OnMouseUpAsync(ElementReference sender, MouseEventArgs e, IGraphElement? element)
    {
        if (this.MouseUp != null) await this.MouseUp.Invoke(new(e, sender, element));
    }

    /// <inheritdoc/>
    public virtual async Task OnMouseEnterAsync(ElementReference sender, MouseEventArgs e, IGraphElement? element)
    {
        if (this.MouseEnter != null) await this.MouseEnter.Invoke(new(e, sender, element));
    }

    /// <inheritdoc/>
    public virtual async Task OnMouseLeaveAsync(ElementReference sender, MouseEventArgs e, IGraphElement? element)
    {
        if (this.MouseLeave != null) await this.MouseLeave.Invoke(new(e, sender, element));
    }

    /// <inheritdoc/>
    public virtual async Task OnWheelAsync(ElementReference sender, WheelEventArgs e, IGraphElement? element)
    {
        if (this.Wheel != null) await this.Wheel.Invoke(new(e, sender, element));
    }

    /// <inheritdoc/>
    public virtual void OnChildAdded(INodeViewModel child)
    {
        if (child is IClusterViewModel cluster)
        {
            this.allClusters.Add(cluster.Id, cluster);
            this.Flatten(cluster);
        }
        else if (child is INodeViewModel node)
        {
            this.allNodes.Add(node.Id, node);
        }
        this.OnChange();
    }
}
