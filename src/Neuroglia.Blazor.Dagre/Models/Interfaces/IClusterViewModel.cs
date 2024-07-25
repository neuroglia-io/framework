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
/// Defines the fundamentals of a view model for a cluster, which is a type of node view model containing child nodes and clusters.
/// </summary>
public interface IClusterViewModel
    : INodeViewModel
{
    /// <summary>
    /// Gets the dictionary of child nodes in the cluster.
    /// </summary>
    IReadOnlyDictionary<string, INodeViewModel> Children { get; }

    /// <summary>
    /// Gets the dictionary of all nodes in the cluster, including nested nodes.
    /// </summary>
    IReadOnlyDictionary<string, INodeViewModel> AllNodes { get; }

    /// <summary>
    /// Gets the dictionary of all clusters in the cluster, including nested clusters.
    /// </summary>
    IReadOnlyDictionary<string, IClusterViewModel> AllClusters { get; }

    /// <summary>
    /// Adds a child node to the cluster.
    /// </summary>
    /// <param name="node">The node to be added as a child.</param>
    void AddChild(INodeViewModel node);

    /// <summary>
    /// Occurs when a child node is added to the cluster.
    /// </summary>
    event Action<INodeViewModel>? ChildAdded;
}
