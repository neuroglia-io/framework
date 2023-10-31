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
/// Represents the default implementation of the <see cref="IEventRecord"/> interface
/// </summary>
[DataContract]
public record EventRecord
    : IEventRecord
{

    /// <summary>
    /// Initializes a new <see cref="EventRecord"/>
    /// </summary>
    public EventRecord() { }

    /// <summary>
    /// Initializes a new <see cref="EventRecord"/>
    /// </summary>
    /// <param name="streamId">The id of the stream the recorded event belongs to</param>
    /// <param name="id">The id of the recorded event</param>
    /// <param name="offset">The offset of the recorded event</param>
    /// <param name="timestamp">The date and time at which the event has been recorded</param>
    /// <param name="type">The type of the recorded event. Should be a non-versioned reverse uri made out alphanumeric, '-' and '.' characters</param>
    /// <param name="data">The data of the recorded event</param>
    /// <param name="metadata">The metadata of the recorded event</param>
    public EventRecord(string streamId, string id, ulong offset, DateTimeOffset timestamp, string type, object? data = null, IDictionary<string, object>? metadata = null)
    {
        if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentNullException(nameof(streamId));
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
        this.StreamId = streamId;
        this.Id = id;
        this.Offset = offset;
        this.Timestamp = timestamp;
        this.Type = type;
        this.Data = data;
        this.Metadata = metadata;
    }

    /// <inheritdoc/>
    [DataMember]
    public virtual string StreamId { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember]
    public virtual string Id { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember]
    public virtual ulong Offset { get; set; }

    /// <inheritdoc/>
    [DataMember]
    public virtual DateTimeOffset Timestamp { get; set; }

    /// <inheritdoc/>
    [DataMember]
    public virtual string Type { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember]
    public virtual object? Data { get; set; }

    /// <inheritdoc/>
    [DataMember]
    public virtual IDictionary<string, object>? Metadata { get; set; }

}