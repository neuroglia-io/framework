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

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Defines the fundamentals of a service used to store events
/// </summary>
public interface IEventStore
{

    /// <summary>
    /// Appends a list of events to the specified stream
    /// </summary>
    /// <param name="streamId">The id of the stream to append the specified events to</param>
    /// <param name="events">The events to append to the specified stream</param>
    /// <param name="expectedVersion">The expected version of the stream to append the events to. Used for optimistic concurrency</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task AppendAsync(string streamId, IEnumerable<IEventDescriptor> events, long? expectedVersion = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets information about the specified stream
    /// </summary>
    /// <param name="streamId">The id of the stream to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IEventStreamDescriptor"/> used to describe and enumerate the stream</returns>
    Task<IEventStreamDescriptor> GetAsync(string streamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads recorded events
    /// </summary>
    /// <param name="streamId">The id of the stream, if any, to read events from. If not set, reads all events</param>
    /// <param name="readDirection">The direction in which to read the stream</param>
    /// <param name="offset">The offset starting from which to read events</param>
    /// <param name="length">The amount of events to read</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> containing the events read from the store</returns>
    IAsyncEnumerable<IEventRecord> ReadAsync(string? streamId, StreamReadDirection readDirection, long offset, ulong? length = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to events
    /// </summary>
    /// <param name="streamId">The id of the stream, if any, to subscribe to. If not set, subscribes to all events</param>
    /// <param name="offset">The offset starting from which to receive events. Defaults to <see cref="StreamPosition.EndOfStream"/></param>
    /// <param name="consumerGroup">The name of the consumer group, if any, in case the subscription is persistent</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IObservable{T}"/> used to observe events</returns>
    Task<IObservable<IEventRecord>> SubscribeAsync(string? streamId = null, long offset = StreamPosition.EndOfStream, string? consumerGroup = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the specified consumer group's offset
    /// </summary>
    /// <param name="consumerGroup">The name of the consumer group to set the offset of</param>
    /// <param name="streamId">The id of the stream, if any, to set the consumer group offset for</param>
    /// <param name="offset">The offset to set</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task SetOffsetAsync(string consumerGroup, long offset, string? streamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Truncates the specified stream
    /// </summary>
    /// <param name="streamId">The id of the stream to truncate</param>
    /// <param name="beforeVersion">The version before which to truncate the stream. If not set, will truncate before the last event</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task TruncateAsync(string streamId, ulong? beforeVersion = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified stream
    /// </summary>
    /// <param name="streamId">The id of the stream to delete</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task DeleteAsync(string streamId, CancellationToken cancellationToken = default);

}