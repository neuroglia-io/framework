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
/// Defines the fundamentals of a node's view model
/// </summary>
public interface INodeViewModel
    : IGraphElement, IPositionable, ISizeable, IRadius
{
    /// <summary>
    /// Gets/sets the id of the node's parent, if any
    /// </summary>
    string? ParentId { get; set; }
    /// <summary>
    /// Gets/sets the node's shape
    /// </summary>
    string? Shape { get; set; }

    /// <summary>
    /// Gets/sets the <see cref="BoundingBox"/> of the node
    /// </summary>
    BoundingBox? Bounds { get; }

    /// <summary>
    /// Sets the <see cref="INodeViewModel"/>'s <see cref="BoundingBox"/> using measurments
    /// </summary>
    /// <param name="width">The width</param>
    /// <param name="height">The height</param>
    /// <param name="x">The horizontal position</param>
    /// <param name="y">The vertical position</param>
    void SetBounds(double width, double height, double x, double y);

    /// <summary>
    /// Moves the node
    /// </summary>
    /// <param name="deltaX">The distance to move the node on the horizontal axis</param>
    /// <param name="deltaY">The distance to move the node on the vertical axis</param>
    void Move(double deltaX, double deltaY);
}
