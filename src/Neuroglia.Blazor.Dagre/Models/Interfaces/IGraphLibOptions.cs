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
/// Represents the options used to construct a new <see cref="IGraphLib"/>
/// </summary>
public interface IGraphLibOptions
{
    /// <summary>
    /// Set to true to allow a graph to have compound nodes - nodes which can be the parent of other nodes. Default: false.
    /// </summary>
    /// 
    bool? Compound { get; set; }

    /// <summary>
    /// Set to true to get a directed graph and false to get an undirected graph. An undirected graph does not treat the order of nodes in an edge as significant. In other words, g.edge("a", "b") === g.edge("b", "a") for an undirected graph. Default: true.
    /// </summary>
    bool? Directed { get; set; }

    /// <summary>
    /// Set to true to allow a graph to have multiple edges between the same pair of nodes. Default: false.
    /// </summary>
    bool? Multigraph { get; set; }
}
