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

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Represents the implementation of the <see cref="IProjectionStatus"/> interface
/// </summary>
public record ProjectionStatus
    : IProjectionStatus
{

    /// <summary>
    /// Initializes a new <see cref="ProjectionStatus"/>
    /// </summary>
    public ProjectionStatus() { }

    /// <summary>
    /// Initializes a new <see cref="ProjectionStatus"/>
    /// </summary>
    /// <param name="phase">The current status phase of the described projection</param>
    /// <param name="reason">The reason, if any, that explains why the projection is in the current status</param>
    /// <param name="offset">The projection's offset</param>
    public ProjectionStatus(string phase, string? reason = null, ulong? offset = null)
    {
        this.Phase = phase;
        this.Reason = reason;
        this.Offset = offset;
    }

    /// <inheritdoc/>
    public virtual string Phase { get; set; } = null!;

    /// <inheritdoc/>
    public virtual string? Reason { get; set; }

    /// <inheritdoc/>
    public virtual ulong? Offset { get; set; }
}
