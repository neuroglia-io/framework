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

using System.Runtime.Serialization;

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Represents the default implementation of the <see cref="IEventStreamDescriptor"/> interface
/// </summary>
[DataContract]
public record EventStreamDescriptor
    : IEventStreamDescriptor
{

    /// <summary>
    /// Initializes a new <see cref="EventStreamDescriptor"/>
    /// </summary>
    /// <param name="id">The id of the described stream</param>
    /// <param name="length">The length of the stream</param>
    /// <param name="firstEventAt">The date and time at which the first event of the stream has been created</param>
    /// <param name="lastEventAt">The date and time at which the last event of the stream has been created</param>
    public EventStreamDescriptor(object id, long length, DateTimeOffset? firstEventAt, DateTimeOffset? lastEventAt)
    {
        this.Id = id;
        this.Length = length;
        this.FirstEventAt = firstEventAt;
        this.LastEventAt = lastEventAt;
    }

    /// <inheritdoc/>
    [DataMember]
    public virtual object Id { get; }

    /// <inheritdoc/>
    [DataMember]
    public virtual long Length { get; }

    /// <inheritdoc/>
    [DataMember]
    public virtual DateTimeOffset? FirstEventAt { get; }

    /// <inheritdoc/>
    [DataMember]
    public virtual DateTimeOffset? LastEventAt { get; }

}