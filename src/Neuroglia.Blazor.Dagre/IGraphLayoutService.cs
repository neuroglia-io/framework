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

namespace Neuroglia.Blazor.Dagre;

/// <summary>
/// Represents the service used to compute the graph layout
/// </summary>
public interface IGraphLayoutService
    : IGraphLibJsonConverter
{
    /// <summary>
    /// Computes the nodes and egdes positions
    /// </summary>
    /// <param name="graphViewModel">The graph to compute the layout of</param>
    /// <param name="options">The Drage options</param>
    /// <returns>The computed graph</returns>
    Task<IGraphViewModel> ComputeLayoutAsync(IGraphViewModel graphViewModel, IDagreGraphOptions? options = null);
}
