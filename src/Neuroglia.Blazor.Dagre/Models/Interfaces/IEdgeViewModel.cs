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
    : IGraphElement, ISizeable, IPositionable
{
    /// <summary>
    /// Gets/sets the id of the node at the beginning of the edge
    /// </summary>
    string SourceId { get; set; }

    /// <summary>
    /// Gets/sets the id of the node at the end of the edge 
    /// </summary>
    string TargetId { get; set; }

    /// <summary>
    /// Gets a list containing the link's points
    /// </summary>
    IEnumerable<IPositionable> Points { get; set; }

    /// <summary>
    /// Gets the edge's bounding box
    /// </summary>
    BoundingBox Bounds { get; }

    /// <summary>
    /// Gets/sets the <see cref="EdgeLabelPosition"/>, where the label should be place relative to the <see cref="IEdgeViewModel"/>
    /// </summary>
    string? LabelPosition { get; set; }

    /// <summary>
    /// Gets/sets the offset between the <see cref="IEdgeViewModel"/> and its label
    /// </summary>
    double? LabelOffset { get; set; }

    /// <summary>
    /// Gets/sets the <see cref="EdgeShape"/> of the <see cref="IEdgeViewModel"/>
    /// </summary>
    string Shape { get; set; }

    /// <summary>
    /// Gets/sets the id of the marker used at the source end of the <see cref="IEdgeViewModel"/>
    /// </summary>
    string? StartMarkerId { get; set; }

    /// <summary>
    /// Gets/sets the id of the marker used at the target end of the <see cref="IEdgeViewModel"/>
    /// </summary>
    string? EndMarkerId { get; set; }

    /// <summary>
    /// Sets the <see cref="IEdgeViewModel"/>'s measurments
    /// </summary>
    /// <param name="points">The edge's points</param>
    /// <param name="width">The width</param>
    /// <param name="height">The height</param>
    /// <param name="x">The label center horizontal position</param>
    /// <param name="y">The label center vertical position</param>
    void SetBounds(IEnumerable<IPositionable> points, double width, double height, double x, double y);
}
