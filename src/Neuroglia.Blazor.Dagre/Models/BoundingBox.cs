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
/// Represents the bounding box of a 2D shape
/// </summary>
/// <param name="Position">The position</param>
/// <param name="Size">The size</param>
/// <remarks>
/// Initializes a <see cref="BoundingBox"/> using the given parameters
/// </remarks>
/// <param name="x">The horizontal position of the <see cref="BoundingBox"/></param>
/// <param name="y">The vertical position of the <see cref="BoundingBox"/></param>
/// <param name="width">The width of the <see cref="BoundingBox"/></param>
/// <param name="height">The height of the <see cref="BoundingBox"/></param>
public class BoundingBox(double? x = null, double? y = null, double? width = null, double? height = null)
        : IPositionable, ISizeable
{
    /// <inheritdoc/>
    public double X { get; set; } = x ?? 0;

    /// <inheritdoc/>
    public double Y { get; set; } = y ?? 0;

    /// <inheritdoc/>
    public double Width { get; set; } = width ?? 0;

    /// <inheritdoc/>
    public double Height { get; set; } = height ?? 0;
}