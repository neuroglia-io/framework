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

namespace Neuroglia.Blazor.Dagre.Models;

/// <summary>
/// Represents the default implementation of the <see cref="IClusterViewModel"/>
/// </summary>
public class ClusterViewModel
    : NodeViewModel, IClusterViewModel
{
    readonly Dictionary<string, INodeViewModel> _children;
    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, INodeViewModel> Children => this._children;

    readonly Dictionary<string, INodeViewModel> _allNodes;
    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, INodeViewModel> AllNodes => this._allNodes;

    readonly Dictionary<string, IClusterViewModel> _allClusters;
    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, IClusterViewModel> AllClusters => this._allClusters;

    /// <inheritdoc/>
    public event EventHandler<INodeViewModel>? ChildAdded;

    /// <summary>
    /// Initializes a new <see cref="ClusterViewModel"/>
    /// </summary>
    public ClusterViewModel(
        Dictionary<string, INodeViewModel>? children = null,
        string? label = "",
        string? cssClass = null,
        string? shape = null,
        double width = Constants.ClusterWidth,
        double height = Constants.ClusterHeight,
        double radiusX = Constants.ClusterRadius,
        double radiusY = Constants.ClusterRadius,
        double x = 0,
        double y = 0,
        Type? componentType = null,
        string? parentId = null
    )
        : base(label, cssClass, shape, width, height, radiusX, radiusY, x, y, componentType, parentId)
    {
        this._children = children ?? [];
        this._allNodes = [];
        this._allClusters = [];
        foreach(var child in this._children.Values)
        {
            if (child == null)
            {
                continue;
            }
            child.ParentId = this.Id;
            child.Changed += OnChildChanged;
            if (child is IClusterViewModel cluster)
            {
                cluster.ChildAdded += this.OnChildAdded;
                this._allClusters.Add(cluster.Id, cluster);
                this.Flatten(cluster);
            }
            else if (child is INodeViewModel node)
            {
                this._allNodes.Add(node.Id, node);
            }
        }
    }

    /// <summary>
    /// Moves the <see cref="ClusterViewModel"/> 
    /// </summary>
    /// <param name="deltaX">The horizontal distance</param>
    /// <param name="deltaY">The veritcal distance</param>
    public override void Move(double deltaX, double deltaY)
    {
        if (deltaX == 0 && deltaY == 0)
        {
            return;
        }
        base.Move(deltaX, deltaY);
        foreach(var child in this._children.Values)
        {
            child.Move(deltaX, deltaY);
        }
    }

    /// <summary>
    /// Adds the provided <see cref="INodeViewModel"/> to the cluster
    /// </summary>
    public virtual void AddChild(INodeViewModel node)
    {
        ArgumentNullException.ThrowIfNull(node);
        node.ParentId = this.Id;
        node.Changed += OnChildChanged;
        this._children.Add(node.Id, node);
        this.ChildAdded?.Invoke(this, node);
        if (node is IClusterViewModel cluster)
        {
            cluster.ChildAdded += this.OnChildAdded;
            this._allClusters.Add(cluster.Id, cluster);
            this.Flatten(cluster);
            return;
        }
        this._allNodes.Add(node.Id, node);
        this.OnChange();
    }

    /// <inheritdoc/>
    public virtual void OnChildAdded(object? sender, INodeViewModel child)
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
        this.ChildAdded?.Invoke(this, child);
    }

    /// <summary>
    /// Adds nested nodes/clusters to allNodes/Clusters
    /// </summary>
    /// <param name="cluster"></param>
    protected virtual void Flatten(IClusterViewModel cluster)
    {
        foreach (var subClusters in cluster.AllClusters.Values)
        {
            if (subClusters == null)
            {
                continue;
            }
            this._allClusters.Add(subClusters.Id, subClusters);
        }
        foreach (var subNode in cluster.AllNodes.Values)
        {
            if (subNode == null)
            {
                continue;
            }
            this._allNodes.Add(subNode.Id, subNode);
        }
    }

    /// <summary>
    /// Handles changes to the cluster's children
    /// </summary>
    protected virtual void OnChildChanged(object? sender, EventArgs e)
    {
        var minX = this.Children.Values.Select(node => node.X - node.Width / 2).Min();
        var maxX = this.Children.Values.Select(node => node.X + node.Width / 2).Max();
        var minY = this.Children.Values.Select(node => node.Y - node.Height / 2).Min();
        var maxY = this.Children.Values.Select(node => node.Y + node.Height / 2).Max();
        var x = (minX + maxX) / 2;
        var y = (minY + maxY) / 2;
        var width = maxX - minX + Constants.ClusterPaddingX;
        var height = maxY - minY + Constants.ClusterPaddingY;
        this.SetBounds(width, height, x, y);
    }
}
