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
/// Represents a position in a 2D space.
/// </summary>
public class Position 
    : IPositionable
{
    /// <summary>
    /// Gets or sets the X coordinate.
    /// </summary>
    public virtual double X { get; set; } = 0;

    /// <summary>
    /// Gets or sets the Y coordinate.
    /// </summary>
    public virtual double Y { get; set; } = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="Position"/> class with default values (0, 0).
    /// </summary>
    public Position() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Position"/> class with specified X and Y coordinates.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    public Position(double x, double y)
    {
        this.X = x;
        this.Y = y;
    }
}