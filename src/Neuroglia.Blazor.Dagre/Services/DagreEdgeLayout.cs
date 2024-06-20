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
/// Represents the Dagre implementation of the <see cref="IEdgeLayout"/> interface
/// </summary>
/// <param name="edge">The underlying <see cref="DagreInputEdge"/></param>
public class DagreEdgeLayout(DagreInputEdge edge)
    : IEdgeLayout
{

    /// <inheritdoc/>
    public virtual IReadOnlyCollection<Point> Points { get; } = edge.Points.Select(p => new Point(p.X, p.Y)).ToList().AsReadOnly();

}