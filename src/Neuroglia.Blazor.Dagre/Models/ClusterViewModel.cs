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

public class ClusterViewModel
    : NodeViewModel, IClusterViewModel
{
    protected readonly Dictionary<Guid, INodeViewModel> _children;
    public virtual IReadOnlyDictionary<Guid, INodeViewModel> Children => this._children;

    protected readonly Dictionary<Guid, INodeViewModel> _allNodes;
    public virtual IReadOnlyDictionary<Guid, INodeViewModel> AllNodes => this._allNodes;

    protected readonly Dictionary<Guid, IClusterViewModel> _allClusters;
    public virtual IReadOnlyDictionary<Guid, IClusterViewModel> AllClusters => this._allClusters;

    public event Action<INodeViewModel>? ChildAdded;

    public ClusterViewModel(
        Dictionary<Guid, INodeViewModel>? children = null,
        string? label = "",
        string? cssClass = null,
        string? shape = null,
        double? width = Constants.ClusterWidth,
        double? height = Constants.ClusterHeight,
        double? radiusX = Constants.ClusterRadius,
        double? radiusY = Constants.ClusterRadius,
        double? x = 0,
        double? y = 0,
        Type? componentType = null,
        Guid? parentId = null
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
                this._allClusters.Add(cluster.Id, cluster);
                this.Flatten(cluster);
            }
            else if (child is INodeViewModel node)
            {
                this._allNodes.Add(node.Id, node);
            }
        }
    }

    public override void Move(double deltaX, double deltaY)
    {
        if (deltaX == 0 && deltaY == 0)
            return;
        base.Move(deltaX, deltaY);
        foreach(var child in this._children.Values)
        {
            child.Move(deltaX, deltaY);
        }
    }

    /// <summary>
    /// Adds the provided <see cref="INodeViewModel"/> to the cluster
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task AddChildAsync(INodeViewModel node)
    {
        ArgumentNullException.ThrowIfNull(node);
        node.ParentId = this.Id;
        node.Changed += OnChildChanged;
        this._children.Add(node.Id, node);
        this.ChildAdded?.Invoke(node);
        if (node is IClusterViewModel cluster)
        {
            this._allClusters.Add(cluster.Id, cluster);
            this.Flatten(cluster);
            return;
        }
        this._allNodes.Add(node.Id, node);
        this.OnChange();
        await Task.CompletedTask;
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

    protected virtual void OnChildChanged()
    {
        var minX = this.Children.Values.Select(node => node.X - (node.Width ?? 0) / 2).Min();
        var maxX = this.Children.Values.Select(node => node.X + (node.Width ?? 0) / 2).Max();
        var minY = this.Children.Values.Select(node => node.Y - (node.Height ?? 0) / 2).Min();
        var maxY = this.Children.Values.Select(node => node.Y + (node.Height ?? 0) / 2).Max();
        var x = (minX + maxX) / 2;
        var y = (minY + maxY) / 2;
        var width = maxX - minX + Constants.ClusterPaddingX;
        var height = maxY - minY + Constants.ClusterPaddingY;
        this.SetGeometry(x, y, width, height);
    }
}
