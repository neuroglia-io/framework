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
/// Represents the default implementation of the <see cref="IAckableEventRecord"/> interface
/// </summary>
public record AckableEventRecord
    : EventRecord, IAckableEventRecord
{

    /// <inheritdoc/>
    public AckableEventRecord() { }

    /// <summary>
    /// Initializes a new <see cref="EventRecord"/>
    /// </summary>
    /// <param name="streamId">The id of the stream the recorded event belongs to</param>
    /// <param name="id">The id of the recorded event</param>
    /// <param name="offset">The offset of the recorded event</param>
    /// <param name="position">The global position of the recorded event</param>
    /// <param name="timestamp">The date and time at which the event has been recorded</param>
    /// <param name="type">The type of the recorded event. Should be a non-versioned reverse uri made out alphanumeric, '-' and '.' characters</param>
    /// <param name="data">The data of the recorded event</param>
    /// <param name="metadata">The metadata of the recorded event</param>
    /// <param name="ackDelegate">The <see cref="Func{TResult}"/>, if any, used to ack the record</param>
    /// <param name="nackDelegate">The <see cref="Func{TResult}"/>, if any, used to nack the record</param>
    /// <param name="replayed">A boolean indicating whether or not the recorded event is being replayed to its consumer(s)</param>
    public AckableEventRecord(string streamId, string id, ulong offset, ulong position, DateTimeOffset timestamp, string type, object? data = null, IDictionary<string, object>? metadata = null, bool? replayed = false, Func<Task>? ackDelegate = null, Func<string?, Task>? nackDelegate = null) 
        : base(streamId, id, offset, position, timestamp, type, data, metadata, replayed)
    {
        this.AckDelegate = ackDelegate;
        this.NackDelegate = nackDelegate;
    }

    /// <summary>
    /// Gets the <see cref="Func{TResult}"/>, if any, used to ack the record
    /// </summary>
    [IgnoreDataMember]
    protected Func<Task>? AckDelegate { get; }

    /// <summary>
    /// Gets the <see cref="Func{TResult}"/>, if any, used to ack the record
    /// </summary>
    [IgnoreDataMember]
    protected Func<string?, Task>? NackDelegate { get; }

    /// <inheritdoc/>
    public virtual Task AckAsync(CancellationToken cancellationToken = default) => this.AckDelegate?.Invoke() ?? Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task NackAsync(string? reason = null, CancellationToken cancellationToken = default) => this.NackDelegate?.Invoke(reason) ?? Task.CompletedTask;

}