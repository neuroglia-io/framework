/*
 * Copyright © 2021 Neuroglia SPRL. All rights reserved.
 * <p>
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * <p>
 * http://www.apache.org/licenses/LICENSE-2.0
 * <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.Data.EventSourcing.EventStore;
using Neuroglia.Data.EventSourcing.EventStore.Configuration;
using Neuroglia.Serialization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static EventStore.Client.EventStoreClient;

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Represents the default <see href="https://www.eventstore.com/">Event Store</see> implementation of the <see cref="IEventStore"/> interface
    /// </summary>
    public class ESEventStore
        : IEventStore
    {

        /// <summary>
        /// Initializes a new <see cref="ESEventStore"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        /// <param name="serializerProvider">The service used to provide <see cref="ISerializer"/>s</param>
        /// <param name="options">The options used to configure the <see cref="ESEventStore"/></param>
        /// <param name="eventStoreClient">The service used to interact with the remove <see href="https://www.eventstore.com/">Event Store</see> service</param>
        public ESEventStore(ILogger<ESEventStore> logger, IServiceProvider serviceProvider, ISerializerProvider serializerProvider, IOptions<EventStoreOptions> options, EventStoreClient eventStoreClient)
        {
            this.Logger = logger;
            this.ServiceProvider = serviceProvider;
            this.Options = options.Value;
            this.Serializer = serializerProvider.GetSerializer(this.Options.SerializerType);
            this.EventStoreClient = eventStoreClient;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        protected virtual IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the options used to configure the <see cref="ESEventStore"/>
        /// </summary>
        protected virtual EventStoreOptions Options { get; }

        /// <summary>
        /// Gets the service used to interact with the remove <see href="https://www.eventstore.com/">Event Store</see> service
        /// </summary>
        protected virtual EventStoreClient EventStoreClient { get; }

        /// <summary>
        /// Gets the service used to interact with the remove <see href="https://www.eventstore.com/">Event Store</see> service, exclusively for persistent subscriptions
        /// </summary>
        protected virtual EventStorePersistentSubscriptionsClient EventStorePersistentSubscriptionsClient { get; }

        /// <summary>
        /// Gets the service used to serialize and deserialize <see cref="ISourcedEvent"/>s
        /// </summary>
        protected virtual ISerializer Serializer { get; }

        /// <summary>
        /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> containing all active <see cref="EventStoreSubscription"/>s
        /// </summary>
        protected virtual ConcurrentDictionary<string, EventStoreSubscription> Subscriptions { get; } = new();

        /// <inheritdoc/>
        public virtual async Task<IEventStream> GetStreamAsync(string streamId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentNullException(nameof(streamId));
            ReadStreamResult readResult;
            readResult = this.EventStoreClient.ReadStreamAsync(Direction.Forwards, streamId, StreamPosition.Start, cancellationToken: cancellationToken);
            if (await readResult.ReadState == ReadState.StreamNotFound)
                return null;
            ResolvedEvent firstEvent = await readResult.FirstAsync(cancellationToken);
            readResult = this.EventStoreClient.ReadStreamAsync(Direction.Forwards, streamId, StreamPosition.Start, cancellationToken: cancellationToken);
            ResolvedEvent lastEvent = await readResult.FirstAsync(cancellationToken);
            return new EventStream(streamId, lastEvent.Event.EventNumber.ToInt64() + 1, firstEvent.Event.Created, lastEvent.Event.Created);
        }

        /// <inheritdoc/>
        public virtual async Task AppendToStreamAsync(string streamId, IEnumerable<IEventMetadata> events, long expectedVersion, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentNullException(nameof(streamId));
            if (events == null || !events.Any())
                throw new ArgumentNullException(nameof(events));
            IEnumerable<EventData> eventDataCollection = await this.GenerateEventsDataAsync(events, cancellationToken);
            await EventStoreClient.AppendToStreamAsync(streamId, StreamRevision.FromInt64(expectedVersion), eventDataCollection, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task AppendToStreamAsync(string streamId, IEnumerable<IEventMetadata> events, CancellationToken cancellationToken = default)
        {
            await this.AppendToStreamAsync(streamId, events, StreamState.NoStream.ToInt64(), cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<ISourcedEvent>> ReadEventsForwardAsync(string streamId, long start, long end, CancellationToken cancellationToken = default)
        { 
            IEnumerable<ResolvedEvent> resolvedEvents = await this.ReadResolvedEventsForwardAsync(streamId, start, end, cancellationToken);
            List<ISourcedEvent> events = new(resolvedEvents.Count());
            foreach(ResolvedEvent e in resolvedEvents)
            {
                events.Add(await this.UnwrapsStoredEventAsync(e, cancellationToken));
            }
            return events;
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<ISourcedEvent>> ReadEventsForwardAsync(string streamId, long start, CancellationToken cancellationToken = default)
        {
            return await this.ReadEventsForwardAsync(streamId, start, StreamPosition.End.ToInt64(), cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<ISourcedEvent>> ReadAllEventsForwardAsync(string streamId, CancellationToken cancellationToken = default)
        {
            return await this.ReadEventsForwardAsync(streamId, StreamPosition.Start.ToInt64(), StreamPosition.End.ToInt64(), cancellationToken);
        }

        /// <summary>
        /// Reads the <see cref="ResolvedEvent"/>s of the specified <see cref="IEventStream"/> in a forward fashion
        /// </summary>
        /// <param name="streamId">The id of the <see cref="IEventStream"/> to get the <see cref="ResolvedEvent"/>s of</param>
        /// <param name="start">The number of the <see cref="ResolvedEvent"/> to start reading the <see cref="IEventStream"/> from</param>
        /// <param name="end">The number of the <see cref="ResolvedEvent"/> to read the <see cref="IEventStream"/> to</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing the <see cref="ResolvedEvent"/>s the <see cref="IEventStream"/> is made out of</returns>
        protected virtual async Task<IEnumerable<ResolvedEvent>> ReadResolvedEventsForwardAsync(string streamId, long start, long end, CancellationToken cancellationToken = default)
        {
            return await this.ReadResolvedEventsAsync(Direction.Forwards, streamId, start, end, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<ISourcedEvent>> ReadEventsBackwardAsync(string streamId, long start, long end, CancellationToken cancellationToken = default)
        {
            IEnumerable<ResolvedEvent> resolvedEvents = await this.ReadResolvedEventsBackwardAsync(streamId, start, end, cancellationToken);
            List<ISourcedEvent> events = new(resolvedEvents.Count());
            foreach (ResolvedEvent e in resolvedEvents)
            {
                events.Add(await this.UnwrapsStoredEventAsync(e, cancellationToken));
            }
            return events;
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<ISourcedEvent>> ReadEventsBackwardAsync(string streamId, long start, CancellationToken cancellationToken = default)
        {
            return await this.ReadEventsBackwardAsync(streamId, StreamPosition.Start.ToInt64(), cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<ISourcedEvent>> ReadAllEventsBackwardAsync(string streamId, CancellationToken cancellationToken = default)
        {
            return await this.ReadEventsBackwardAsync(streamId, StreamPosition.Start.ToInt64(), StreamPosition.End.ToInt64(), cancellationToken);
        }

        /// <summary>
        /// Reads the <see cref="ResolvedEvent"/>s of the specified <see cref="IEventStream"/> in a backward fashion
        /// </summary>
        /// <param name="streamId">The id of the <see cref="IEventStream"/> to get the <see cref="ResolvedEvent"/>s of</param>
        /// <param name="start">The number of the <see cref="ResolvedEvent"/> to start reading the <see cref="IEventStream"/> from</param>
        /// <param name="end">The number of the <see cref="ResolvedEvent"/> to read the <see cref="IEventStream"/> to</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing the <see cref="ResolvedEvent"/>s the <see cref="IEventStream"/> is made out of</returns>
        protected virtual async Task<IEnumerable<ResolvedEvent>> ReadResolvedEventsBackwardAsync(string streamId, long start, long end, CancellationToken cancellationToken = default)
        {
            return await this.ReadResolvedEventsAsync(Direction.Backwards, streamId, start, end, cancellationToken);
        }

        /// <summary>
        /// Reads <see cref="ResolvedEvent"/>s from the specified stream, in the specified direction
        /// </summary>
        /// <param name="direction">The direction to read the stream from</param>
        /// <param name="streamId">The id of the stream to read</param>
        /// <param name="start">The position from which to start reading the stream</param>
        /// <param name="end">The position until which to read the stream</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing the <see cref="ResolvedEvent"/>s the <see cref="IEventStream"/> is made out of</returns>
        protected virtual async Task<IEnumerable<ResolvedEvent>> ReadResolvedEventsAsync(Direction direction, string streamId, long start, long end, CancellationToken cancellationToken = default)
        {
            long eventsCount = long.MaxValue;
            if (direction == Direction.Forwards)
            {
                if(end >= 0)
                {
                    if (start > end)
                        throw new ArgumentOutOfRangeException(nameof(end));
                    eventsCount = end - start;
                }
            }
            else
            {
                if(end > start)
                    throw new ArgumentOutOfRangeException(nameof(end));
                eventsCount = start - end;
            }
            ReadStreamResult readResult = this.EventStoreClient.ReadStreamAsync(direction, streamId, StreamPosition.FromInt64(start), eventsCount, resolveLinkTos: true, cancellationToken: cancellationToken);
            return await readResult.ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<string> SubscribeToStreamAsync(string streamId, Action<ISubscriptionOptionsBuilder> setup, Func<IServiceProvider, ISourcedEvent, Task> handler, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentNullException(nameof(streamId));
            if (setup == null)
                throw new ArgumentNullException(nameof(setup));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            ISubscriptionOptionsBuilder optionsBuilder = new SubscriptionOptionsBuilder();
            setup(optionsBuilder);
            ISubscriptionOptions options = optionsBuilder.Build();
            object subscriptionSource = null;
            string subscriptionId = Guid.NewGuid().ToString();
            if (options.IsDurable)
            {
                PersistentSubscriptionSettings settings = options.ToPersistentSubscriptionSettings();
                try
                {
                    await EventStorePersistentSubscriptionsClient.CreateAsync(streamId, options.DurableName, settings, null, cancellationToken);
                }
                catch (InvalidOperationException) { }
                subscriptionSource = await EventStorePersistentSubscriptionsClient.SubscribeAsync(
                    streamId, 
                    options.DurableName, 
                    this.CreatePersistentSubscriptionHandler(handler),
                    this.CreatePersistentSubscriptionDropHandler(subscriptionId, streamId, options.DurableName, handler, options.AckMode), 
                    autoAck: options.AckMode == EventAckMode.Automatic, 
                    cancellationToken: cancellationToken);
            }
            else
            {
                if (options.StartFrom.HasValue)
                    subscriptionSource = await this.EventStoreClient.SubscribeToStreamAsync(
                        streamId, 
                        options.StartFrom.HasValue ? StreamPosition.FromInt64(options.StartFrom.Value) : StreamPosition.Start,
                        this.CreateCatchUpSubscriptionHandler(handler),
                        options.ResolveLinks,
                        subscriptionDropped: this.CreateCatchUpSubscriptionDropHandler(subscriptionId, streamId, options.StartFrom, options, handler),
                        cancellationToken: cancellationToken);
                else
                    subscriptionSource = await this.EventStoreClient.SubscribeToStreamAsync(
                        streamId,
                        this.CreateStandardSubscriptionHandler(handler), 
                        options.ResolveLinks,
                        this.CreateStandardSubscriptionDropHandler(subscriptionId, streamId, options.ResolveLinks, handler),
                        cancellationToken: cancellationToken);
            }
            this.AddOrUpdateSubscription(subscriptionId, subscriptionSource);
            return subscriptionId;
        }

        /// <inheritdoc/>
        public virtual async Task<string> SubscribeToStreamAsync(string streamId, Func<IServiceProvider, ISourcedEvent, Task> handler, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentNullException(nameof(streamId));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            return await this.SubscribeToStreamAsync(streamId, _ => { }, handler, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual void UnsubscribeFrom(string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
                throw new ArgumentNullException(nameof(subscriptionId));
            if (this.Subscriptions.TryGetValue(subscriptionId, out EventStoreSubscription subscription))
                subscription.Dispose();
        }

        /// <inheritdoc/>
        public virtual async Task DeleteStreamAsync(string streamId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentNullException(nameof(streamId));
            await this.EventStoreClient.SoftDeleteAsync(streamId, StreamRevision.FromStreamPosition(StreamPosition.End), cancellationToken: cancellationToken);
            //todo: think about version locking
        }

        /// <summary>
        /// Generates <see cref="EventData"/> for the specified <see cref="IEventMetadata"/>s
        /// </summary>
        /// <param name="events">An <see cref="IEnumerable{T}"/> containing the <see cref="IEventMetadata"/>s to process</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing the processed <see cref="EventData"/></returns>
        protected virtual async Task<IEnumerable<EventData>> GenerateEventsDataAsync(IEnumerable<IEventMetadata> events, CancellationToken cancellationToken = default)
        {
            List<EventData> eventDataList = new(events.Count());
            foreach (IEventMetadata e in events)
            {
                byte[] rawData = e.Data == null ? Array.Empty<byte>() : await this.Serializer.SerializeAsync(e.Data, cancellationToken);
                byte[] rawMetadata = e.Metadata == null ? Array.Empty<byte>() : await this.Serializer.SerializeAsync(e.Metadata, cancellationToken);
                eventDataList.Add(new EventData(Uuid.FromGuid(e.Id), e.Type, rawData, rawMetadata));
            }
            return eventDataList;
        }

        /// <summary>
        /// Unwraps an <see cref="ISourcedEvent"/> from its <see cref="ResolvedEvent"/> envelope
        /// </summary>
        /// <param name="resolvedEvent">The <see cref="ResolvedEvent"/> to resolve</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The unwrapped <see cref="ISourcedEvent"/></returns>
        protected virtual async Task<ISourcedEvent> UnwrapsStoredEventAsync(ResolvedEvent resolvedEvent, CancellationToken cancellationToken = default)
        {
            JObject metadata = await this.Serializer.DeserializeAsync<JObject>(resolvedEvent.Event.Metadata.ToArray(), cancellationToken);
            object data = await this.Serializer.DeserializeAsync(resolvedEvent.Event.Data.ToArray(), typeof(JObject), cancellationToken);
            return new SourcedEvent(resolvedEvent.Event.EventId.ToGuid(), resolvedEvent.Event.EventNumber.ToInt64(), resolvedEvent.Event.Created, resolvedEvent.Event.EventType, data, metadata);
        }

        /// <summary>
        /// Adds or updates a new <see cref="IEventStoreSubscription"/>
        /// </summary>
        /// <param name="subscriptionId">The <see cref="IEventStoreSubscription"/>'s id</param>
        /// <param name="subscriptionSource">The <see cref="IEventStoreSubscription"/>'s source</param>
        protected virtual void AddOrUpdateSubscription(string subscriptionId, object subscriptionSource)
        {
            if (this.Subscriptions.TryGetValue(subscriptionId.ToString(), out EventStoreSubscription subscription))
            {
                subscription.SetSource(subscriptionSource);
            }
            else
            {
                subscription = EventStoreSubscription.CreateFor(subscriptionId, (dynamic)subscriptionSource);
                subscription.Disposed += this.OnSubscriptionDisposed;
                this.Subscriptions.AddOrUpdate(subscription.Id, subscription, (id, sub) => sub);
            }
        }

        /// <summary>
        /// Creates a new persistent subscription handler
        /// </summary>
        /// <param name="handler">A <see cref="Func{T1, T2, TResult}"/> used to handle the subscription</param>
        /// <returns>A new <see cref="Func{T1, T2, TResult}"/> used to handle the persistent subscription</returns>
        protected virtual Func<PersistentSubscription, ResolvedEvent, int?, CancellationToken, Task> CreatePersistentSubscriptionHandler(Func<IServiceProvider, ISourcedEvent, Task> handler)
        {
            return async (subscription, e, retryCount, cancellationToken) =>
            {
                using IServiceScope scope = this.ServiceProvider.CreateScope();
                try
                {
                    await handler(scope.ServiceProvider, await this.UnwrapsStoredEventAsync(e, cancellationToken));
                    await subscription.Ack(e);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError($"An error occured while processing a stored event received from EventStore.{Environment.NewLine}Details:{{ex}}", ex.Message);
                    await subscription.Nack(PersistentSubscriptionNakEventAction.Park, ex.Message, e);
                }
            };
        }

        /// <summary>
        /// Creates a new <see cref="Action{T1, T2, T3}"/> used to handle subscription drops
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription that has been dropped</param>
        /// <param name="streamId">The id of the subscribed stream</param>
        /// <param name="subscriptionName">The persistent subscription's name</param>
        /// <param name="handler">A <see cref="Func{T1, T2, TResult}"/> used to handle the subscription</param>
        /// <param name="eventAckMode">The way subscribed events should be acknowledged</param>
        /// <returns>A new <see cref="Action{T1, T2, T3}"/> used to handle subscription drops</returns>
        protected virtual Action<PersistentSubscription, SubscriptionDroppedReason, Exception> CreatePersistentSubscriptionDropHandler(string subscriptionId, string streamId,
            string subscriptionName, Func<IServiceProvider, ISourcedEvent, Task> handler, EventAckMode eventAckMode)
        {
            return (sub, reason, ex) =>
            {
                switch (reason)
                {
                    case SubscriptionDroppedReason.Disposed:
                        this.Logger.LogInformation("The persistent subscription to stream with id '{streamId}' and with group name '{subscriptionName}' has been dropped for the following reason: '{dropReason}'.",
                            streamId, subscriptionName, reason);
                        this.UnsubscribeFrom(subscriptionId);
                        break;
                    default:
                        this.Logger.LogInformation("The persistent subscription to stream with id '{streamId}' and with group name '{subscriptionName}' has been dropped for the following reason: '{dropReason}'. Resubscribing...",
                            streamId, subscriptionName, reason);
                        object subscriptionSource = this.EventStorePersistentSubscriptionsClient.SubscribeAsync(streamId, subscriptionName,
                            this.CreatePersistentSubscriptionHandler(handler),
                            this.CreatePersistentSubscriptionDropHandler(subscriptionId, streamId, subscriptionName, handler, eventAckMode),
                            autoAck: eventAckMode == EventAckMode.Automatic);
                        this.AddOrUpdateSubscription(subscriptionId, subscriptionSource);
                        break;
                }
            };
        }

        /// <summary>
        /// Creates a new catch-up subscription handler
        /// </summary>
        /// <param name="handler">A <see cref="Func{T1, T2, TResult}"/> used to handle the subscription</param>
        /// <returns>A new <see cref="Func{T1, T2, TResult}"/> used to handle the catch-up subscription</returns>
        protected virtual Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> CreateCatchUpSubscriptionHandler(Func<IServiceProvider, ISourcedEvent, Task> handler)
        {
            return async (subscription, e, cancellationToken) =>
            {
                using IServiceScope scope = this.ServiceProvider.CreateScope();
                try
                {
                    await handler(scope.ServiceProvider, await this.UnwrapsStoredEventAsync(e, cancellationToken));
                }
                catch (Exception ex)
                {
                    this.Logger.LogError($"An error occured while processing a stored event received from EventStore.{Environment.NewLine}Details:{{ex}}", ex.Message);
                }
            };
        }

        /// <summary>
        /// Creates a new <see cref="Action{T1, T2, T3}"/> used to handle subscription drops
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription that has been dropped</param>
        /// <param name="streamId">The id of the catch-up stream</param>
        /// <param name="startFrom">The event number from which to start</param>
        /// <param name="options">The <see cref="ISubscriptionOptions"/> to use</param>
        /// <param name="handler">A <see cref="Func{T1, T2, TResult}"/> used to handle the subscription</param>
        /// <returns>A new <see cref="Action{T1, T2, T3}"/> used to handle subscription drops</returns>
        protected virtual Action<StreamSubscription, SubscriptionDroppedReason, Exception> CreateCatchUpSubscriptionDropHandler(string subscriptionId, string streamId, long? startFrom, ISubscriptionOptions options, Func<IServiceProvider, ISourcedEvent, Task> handler)
        {
            return async (sub, reason, ex) =>
            {
                switch (reason)
                {
                    case SubscriptionDroppedReason.Disposed:
                        this.Logger.LogInformation("A catch-up subscription to stream with id '{streamId}' has been dropped for the following reason: '{dropReason}'.", streamId, reason);
                        this.UnsubscribeFrom(subscriptionId);
                        break;
                    default:
                        this.Logger.LogInformation("A catch-up subscription to stream with id '{streamId}' has been dropped for the following reason: '{dropReason}'. Resubscribing...", streamId, reason);
                        object subscriptionSource = await this.EventStoreClient.SubscribeToStreamAsync(
                            streamId,
                            options.StartFrom.HasValue ? StreamPosition.FromInt64(options.StartFrom.Value) : StreamPosition.Start,
                            this.CreateCatchUpSubscriptionHandler(handler),
                            options.ResolveLinks,
                            subscriptionDropped: this.CreateCatchUpSubscriptionDropHandler(subscriptionId, streamId, startFrom, options, handler));
                        this.AddOrUpdateSubscription(subscriptionId, subscriptionSource);
                        break;
                }
            };
        }

        /// <summary>
        /// Creates a new standard subscription handler
        /// </summary>
        /// <param name="handler">A <see cref="Func{T1, T2, TResult}"/> used to handle the subscription</param>
        /// <returns>A new <see cref="Func{T1, T2, TResult}"/> used to handle the standard subscription</returns>
        protected virtual Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> CreateStandardSubscriptionHandler(Func<IServiceProvider, ISourcedEvent, Task> handler)
        {
            return async (subscription, e, cancellationToken) =>
            {
                using IServiceScope scope = this.ServiceProvider.CreateScope();
                try
                {
                    await handler(scope.ServiceProvider, await this.UnwrapsStoredEventAsync(e, cancellationToken));
                }
                catch (Exception ex)
                {
                    this.Logger.LogError($"An error occured while processing a stored event received from EventStore.{Environment.NewLine}Details:{{ex}}", ex.Message);
                }
            };
        }

        /// <summary>
        /// Creates a new <see cref="Action{T1, T2, T3}"/> used to handle subscription drops
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription that has been dropped</param>
        /// <param name="streamId">The id of the catch-up stream</param>
        /// <param name="resolveLinks">A boolean indicating whether or not to resolve event links</param>
        /// <param name="handler">A <see cref="Func{T1, T2, TResult}"/> used to handle the subscription</param>
        /// <returns>A new <see cref="Action{T1, T2, T3}"/> used to handle subscription drops</returns>
        protected virtual Action<StreamSubscription, SubscriptionDroppedReason, Exception> CreateStandardSubscriptionDropHandler(string subscriptionId, string streamId, bool resolveLinks, Func<IServiceProvider, ISourcedEvent, Task> handler)
        {
            return (sub, reason, ex) =>
            {
                switch (reason)
                {
                    case SubscriptionDroppedReason.Disposed:
                        this.Logger.LogInformation("A standard subscription to stream with id '{streamId}' has been dropped for the following reason: '{dropReason}'.", streamId, reason);
                        this.UnsubscribeFrom(subscriptionId);
                        break;
                    default:
                        this.Logger.LogInformation("A standard subscription to stream with id '{streamId}' has been dropped for the following reason: '{dropReason}'. Resubscribing...", streamId, reason);
                        object subscriptionSource = this.EventStoreClient.SubscribeToStreamAsync(
                            streamId,
                            this.CreateStandardSubscriptionHandler(handler),
                            resolveLinks,
                            this.CreateStandardSubscriptionDropHandler(subscriptionId, streamId, resolveLinks, handler))
                            .GetAwaiter().GetResult();
                        this.AddOrUpdateSubscription(subscriptionId, subscriptionSource);
                        break;
                }
            };
        }

        /// <summary>
        /// Represents the handler fired whenever an <see cref="IEventStoreSubscription"/> has been disposed
        /// </summary>
        /// <param name="sender">The disposed <see cref="IEventStoreSubscription"/></param>
        /// <param name="e">The event's arguments</param>
        protected virtual void OnSubscriptionDisposed(object sender, EventArgs e)
        {
            EventStoreSubscription subscription = (EventStoreSubscription)sender;
            this.Subscriptions.Remove(subscription.Id, out _);
        }

    }

}
