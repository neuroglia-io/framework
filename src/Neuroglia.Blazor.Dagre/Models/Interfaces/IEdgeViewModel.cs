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
/// Defines the fundamentals of a graph link view model
/// </summary>
public interface IEdgeViewModel
    : IGraphElement
{

    /// <summary>
    /// Gets the source <see cref="INodeViewModel"/>
    /// </summary>
    INodeViewModel Source { get; }

    /// <summary>
    /// Gets the target <see cref="INodeViewModel"/>
    /// </summary>
    INodeViewModel Target { get;  }

    /// <summary>
    /// Gets a list containing the link's points
    /// </summary>
    IReadOnlyCollection<Point> Points { get; set; }

    /// <summary>
    /// Gets the edge's bounding box
    /// </summary>
    BoundingBox Bounds { get; }

    string? LabelPosition { get; }

    double? LabelOffset { get; }

    string Shape { get; }

    string? StartMarkerId { get; }

    string? EndMarkerId { get; }

}
