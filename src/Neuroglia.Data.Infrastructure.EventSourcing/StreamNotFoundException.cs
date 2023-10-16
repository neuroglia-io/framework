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
/// Represents an exception thrown when an event stream could not be found
/// </summary>
public class StreamNotFoundException
    : Exception
{

    /// <summary>
    /// Initializes a new <see cref="StreamNotFoundException"/>
    /// </summary>
    public StreamNotFoundException() : base("Failed to find the specified stream") { }

    /// <summary>
    /// Initializes a new <see cref="StreamNotFoundException"/>
    /// </summary>
    /// <param name="streamId">The id of the stream that could not be found</param>
    public StreamNotFoundException(string streamId) : this() { this.StreamId = streamId; }

    /// <summary>
    /// Gets the id of the stream that could not be found
    /// </summary>
    public virtual string? StreamId { get; protected set; }

}
