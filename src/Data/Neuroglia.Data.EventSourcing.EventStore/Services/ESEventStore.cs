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
using EventStore.ClientAPI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neuroglia.Data.EventSourcing.EventStore.Configuration;
using Neuroglia.Data.EventSourcing.EventStore;
using Neuroglia.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using EData = EventStore.ClientAPI.EventData;

namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Represents the default <see href="https://www.eventstore.com/">Event Store</see> implementation of the <see cref="IEventStore"/> interface
    /// </summary>
    public class ESEventStore
        : IEventStore
    {

        /// <summary>
        /// Gets the options used to configure the <see cref="ESEventStore"/>
        /// </summary>
        protected virtual EventStoreOptions Options { get; }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        protected virtual IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected virtual Microsoft.Extensions.Logging.ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to interact with the remove <see href="https://www.eventstore.com/">Event Store</see> service
        /// </summary>
        protected virtual IEventStoreConnection Connection { get; }

        /// <summary>
        /// Gets the service used to serialize and deserialize <see cref="ISourcedEvent"/>s
        /// </summary>
        protected virtual ISerializer Serializer { get; }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all pending <see cref="EventStoreTransaction"/>s
        /// </summary>
        protected virtual ConcurrentDictionary<string, EventStore.EventStoreTransaction> PendingTransactions { get; } = new();

        /// <summary>
        /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> containing all active <see cref="EventStoreSubscription"/>s
        /// </summary>
        protected virtual ConcurrentDictionary<string, EventStore.EventStoreSubscription> Subscriptions { get; } = new();

        /// <inheritdoc/>
        public virtual async Task<ITransaction> StartTransactionAsync(string streamId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentNullException(nameof(streamId));
            EventStore.EventStoreTransaction transaction = new(await this.Connection.StartTransactionAsync(streamId, ExpectedVersion.Any));
            transaction.Disposed += (sender, e) =>
            {
                this.PendingTransactions.TryRemove(streamId, out _);
            };
            this.PendingTransactions.AddOrUpdate(streamId, transaction, (key, existing) =>
            {
                existing.Dispose();
                return transaction;
            });
            return transaction;
        }

        /// <inheritdoc/>
        public virtual async Task<IEventStream> GetStreamAsync(string streamId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentNullException(nameof(streamId));
            EventReadResult readResult;
            readResult = await this.Connection.ReadEventAsync(streamId, StreamPosition.Start, false);
            if (readResult.Status == EventReadStatus.NoStream)
                return null;
            RecordedEvent firstEvent = readResult.Event.Value.Event;
            readResult = await this.Connection.ReadEventAsync(streamId, StreamPosition.End, false);
            RecordedEvent lastEvent = readResult.Event.Value.Event;
            return new EventStream(streamId, lastEvent.EventNumber, firstEvent.Created, lastEvent.Created);
        }

        /// <inheritdoc/>
        public virtual async Task AppendToStreamAsync(string streamId, IEnumerable<IEventMetadata> events, long expectedVersion, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentNullException(nameof(streamId));
            if (events == null || !events.Any())
                throw new ArgumentNullException(nameof(events));
            IEnumerable<EData> eventDataCollection = await this.GenerateEventsDataAsync(events, cancellationToken);
            if (this.PendingTransactions.TryGetValue(streamId, out EventStore.EventStoreTransaction transaction))
                await transaction.UnderlyingTransaction.WriteAsync(eventDataCollection);
            else
                await this.Connection.AppendToStreamAsync(streamId, expectedVersion, eventDataCollection);
        }

        /// <inheritdoc/>
        public virtual async Task AppendToStreamAsync(string streamId, IEnumerable<IEventMetadata> events, CancellationToken cancellationToken = default)
        {
            await this.AppendToStreamAsync(streamId, events, ExpectedVersion.Any, cancellationToken);
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
            return await this.ReadEventsForwardAsync(streamId, start, StreamPosition.End, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<ISourcedEvent>> ReadAllEventsForwardAsync(string streamId, CancellationToken cancellationToken = default)
        {
            return await this.ReadEventsForwardAsync(streamId, StreamPosition.Start, StreamPosition.End, cancellationToken);
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
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentNullException(nameof(streamId));
            if (end < start)
                throw new ArgumentOutOfRangeException(nameof(end));
            List<ResolvedEvent> events = new((int)(end - start));
            StreamEventsSlice slice;
            long position = start;
            do
            {
                int remainingEvents = (int)(end - position);
                int sliceLength = this.Options.MaxSliceLength;
                if (sliceLength > remainingEvents)
                    sliceLength = remainingEvents;
                slice = await this.Connection.ReadStreamEventsForwardAsync(streamId, position, sliceLength, true);
                events.AddRange(events);
                position = slice.NextEventNumber;
            }
            while (!cancellationToken.IsCancellationRequested
                && !slice.IsEndOfStream);
            return events;
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
            return await this.ReadEventsBackwardAsync(streamId, StreamPosition.Start, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<ISourcedEvent>> ReadAllEventsBackwardAsync(string streamId, CancellationToken cancellationToken = default)
        {
            return await this.ReadEventsBackwardAsync(streamId, StreamPosition.Start, StreamPosition.End, cancellationToken);
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
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentNullException(nameof(streamId));
            if (start < end)
                throw new ArgumentOutOfRangeException(nameof(start));
            List<ResolvedEvent> events = new((int)(end - start));
            StreamEventsSlice slice;
            long position = start;
            do
            {
                int remainingEvents = (int)(end - position);
                int sliceLength = this.Options.MaxSliceLength;
                if (sliceLength > remainingEvents)
                    sliceLength = remainingEvents;
                slice = await this.Connection.ReadStreamEventsBackwardAsync(streamId, position, sliceLength, true);
                events.AddRange(slice.Events);
                position = slice.NextEventNumber;
            }
            while (!cancellationToken.IsCancellationRequested
                && !slice.IsEndOfStream);
            return events;
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
                    await this.Connection.CreatePersistentSubscriptionAsync(streamId, options.DurableName, settings, null);
                }
                catch (InvalidOperationException) { }
                subscriptionSource = await this.Connection.ConnectToPersistentSubscriptionAsync(streamId, options.DurableName,
                    this.CreatePersistentSubscriptionHandler(handler),
                    this.CreatePersistentSubscriptionDropHandler(subscriptionId, streamId, options.DurableName, handler, options.AckMode),
                    autoAck: options.AckMode == EventAckMode.Automatic);
            }
            else
            {
                if (options.StartFrom.HasValue)
                    subscriptionSource = this.Connection.SubscribeToStreamFrom(streamId, options.StartFrom, options.ToCatchUpSubscriptionSettings(),
                        this.CreateCatchUpSubscriptionHandler(handler),
                        subscriptionDropped: this.CreateCatchUpSubscriptionDropHandler(subscriptionId, streamId, options.StartFrom, options.ToCatchUpSubscriptionSettings(), handler));
                else
                    subscriptionSource = await this.Connection.SubscribeToStreamAsync(streamId, options.ResolveLinks,
                        this.CreateStandardSubscriptionHandler(handler),
                        this.CreateStandardSubscriptionDropHandler(subscriptionId, streamId, options.ResolveLinks, handler));
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
            if (this.Subscriptions.TryGetValue(subscriptionId, out EventStore.EventStoreSubscription subscription))
                subscription.Dispose();
        }

        /// <inheritdoc/>
        public virtual async Task DeleteStreamAsync(string streamId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentNullException(nameof(streamId));
            await this.Connection.DeleteStreamAsync(streamId, ExpectedVersion.Any);
        }

        /// <summary>
        /// Generates <see cref="EData"/> for the specified <see cref="IEventMetadata"/>s
        /// </summary>
        /// <param name="events">An <see cref="IEnumerable{T}"/> containing the <see cref="IEventMetadata"/>s to process</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing the processed <see cref="EData"/></returns>
        protected virtual async Task<IEnumerable<EData>> GenerateEventsDataAsync(IEnumerable<IEventMetadata> events, CancellationToken cancellationToken = default)
        {
            List<EData> eventDataList = new(events.Count());
            foreach (IEventMetadata e in events)
            {
                byte[] rawData = e.Data == null ? Array.Empty<byte>() : await this.Serializer.SerializeAsync(e.Data, cancellationToken);
                byte[] rawMetadata = e.Metadata == null ? Array.Empty<byte>() : await this.Serializer.SerializeAsync(e.Metadata, cancellationToken);
                eventDataList.Add(new EData(e.Id, e.Type, this.Serializer is IJsonSerializer, rawData, rawMetadata));
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
            JObject metadata = await this.Serializer.DeserializeAsync<JObject>(resolvedEvent.Event.Metadata, cancellationToken);
            object data = await this.Serializer.DeserializeAsync(resolvedEvent.Event.Data, typeof(JObject), cancellationToken);
            return new SourcedEvent(resolvedEvent.Event.EventId, resolvedEvent.Event.EventNumber, resolvedEvent.Event.Created, resolvedEvent.Event.EventType, data, metadata);
        }

        /// <summary>
        /// Adds or updates a new <see cref="IEventStoreSubscription"/>
        /// </summary>
        /// <param name="subscriptionId">The <see cref="IEventStoreSubscription"/>'s id</param>
        /// <param name="subscriptionSource">The <see cref="IEventStoreSubscription"/>'s source</param>
        protected virtual void AddOrUpdateSubscription(string subscriptionId, object subscriptionSource)
        {
            if (this.Subscriptions.TryGetValue(subscriptionId.ToString(), out EventStore.EventStoreSubscription subscription))
            {
                subscription.SetSource(subscriptionSource);
            }
            else
            {
                subscription = EventStore.EventStoreSubscription.CreateFor(subscriptionId, (dynamic)subscriptionSource);
                subscription.Disposed += this.OnSubscriptionDisposed;
                this.Subscriptions.AddOrUpdate(subscription.Id, subscription, (id, sub) => sub);
            }
        }

        /// <summary>
        /// Creates a new persistent subscription handler
        /// </summary>
        /// <param name="handler">A <see cref="Func{T1, T2, TResult}"/> used to handle the subscription</param>
        /// <returns>A new <see cref="Func{T1, T2, TResult}"/> used to handle the persistent subscription</returns>
        protected virtual Func<EventStorePersistentSubscriptionBase, ResolvedEvent, Task> CreatePersistentSubscriptionHandler(Func<IServiceProvider, ISourcedEvent, Task> handler)
        {
            return async (subscription, e) =>
            {
                using IServiceScope scope = this.ServiceProvider.CreateScope();
                try
                {
                    await handler(scope.ServiceProvider, await this.UnwrapsStoredEventAsync(e));
                    subscription.Acknowledge(e);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError($"An error occured while processing a stored event received from EventStore.{Environment.NewLine}Details:{{ex}}", ex.Message);
                    subscription.Fail(e, PersistentSubscriptionNakEventAction.Park, ex.Message);
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
        protected virtual Action<EventStorePersistentSubscriptionBase, SubscriptionDropReason, Exception> CreatePersistentSubscriptionDropHandler(string subscriptionId, string streamId,
            string subscriptionName, Func<IServiceProvider, ISourcedEvent, Task> handler, EventAckMode eventAckMode)
        {
            return (sub, reason, ex) =>
            {
                switch (reason)
                {
                    case SubscriptionDropReason.UserInitiated:
                    case SubscriptionDropReason.NotFound:
                    case SubscriptionDropReason.NotAuthenticated:
                    case SubscriptionDropReason.AccessDenied:
                    case SubscriptionDropReason.PersistentSubscriptionDeleted:
                        this.Logger.LogInformation("The persistent subscription to stream with id '{streamId}' and with group name '{subscriptionName}' has been dropped for the following reason: '{dropReason}'.",
                            streamId, subscriptionName, reason);
                        this.UnsubscribeFrom(subscriptionId);
                        break;
                    default:
                        this.Logger.LogInformation("The persistent subscription to stream with id '{streamId}' and with group name '{subscriptionName}' has been dropped for the following reason: '{dropReason}'. Resubscribing...",
                            streamId, subscriptionName, reason);
                        object subscriptionSource = this.Connection.ConnectToPersistentSubscriptionAsync(streamId, subscriptionName,
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
        protected virtual Func<EventStoreCatchUpSubscription, ResolvedEvent, Task> CreateCatchUpSubscriptionHandler(Func<IServiceProvider, ISourcedEvent, Task> handler)
        {
            return async (subscription, e) =>
            {
                using IServiceScope scope = this.ServiceProvider.CreateScope();
                try
                {
                    await handler(scope.ServiceProvider, await this.UnwrapsStoredEventAsync(e));
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
        /// <param name="settings">The <see cref="CatchUpSubscriptionSettings"/> to use</param>
        /// <param name="handler">A <see cref="Func{T1, T2, TResult}"/> used to handle the subscription</param>
        /// <returns>A new <see cref="Action{T1, T2, T3}"/> used to handle subscription drops</returns>
        protected virtual Action<EventStoreCatchUpSubscription, SubscriptionDropReason, Exception> CreateCatchUpSubscriptionDropHandler(string subscriptionId, string streamId, long? startFrom, CatchUpSubscriptionSettings settings, Func<IServiceProvider, ISourcedEvent, Task> handler)
        {
            return (sub, reason, ex) =>
            {
                switch (reason)
                {
                    case SubscriptionDropReason.UserInitiated:
                    case SubscriptionDropReason.NotFound:
                    case SubscriptionDropReason.NotAuthenticated:
                    case SubscriptionDropReason.AccessDenied:
                        this.Logger.LogInformation("A catch-up subscription to stream with id '{streamId}' has been dropped for the following reason: '{dropReason}'.", streamId, reason);
                        this.UnsubscribeFrom(subscriptionId);
                        break;
                    default:
                        this.Logger.LogInformation("A catch-up subscription to stream with id '{streamId}' has been dropped for the following reason: '{dropReason}'. Resubscribing...", streamId, reason);
                        object subscriptionSource = this.Connection.SubscribeToStreamFrom(streamId, startFrom, settings,
                            this.CreateCatchUpSubscriptionHandler(handler),
                            subscriptionDropped: this.CreateCatchUpSubscriptionDropHandler(subscriptionId, streamId, startFrom, settings, handler));
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
        protected virtual Func<global::EventStore.ClientAPI.EventStoreSubscription, ResolvedEvent, Task> CreateStandardSubscriptionHandler(Func<IServiceProvider, ISourcedEvent, Task> handler)
        {
            return async (subscription, e) =>
            {
                using IServiceScope scope = this.ServiceProvider.CreateScope();
                try
                {
                    await handler(scope.ServiceProvider, await this.UnwrapsStoredEventAsync(e));
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
        protected virtual Action<global::EventStore.ClientAPI.EventStoreSubscription, SubscriptionDropReason, Exception> CreateStandardSubscriptionDropHandler(string subscriptionId, string streamId, bool resolveLinks, Func<IServiceProvider, ISourcedEvent, Task> handler)
        {
            return (sub, reason, ex) =>
            {
                switch (reason)
                {
                    case SubscriptionDropReason.UserInitiated:
                    case SubscriptionDropReason.NotFound:
                    case SubscriptionDropReason.NotAuthenticated:
                    case SubscriptionDropReason.AccessDenied:
                        this.Logger.LogInformation("A standard subscription to stream with id '{streamId}' has been dropped for the following reason: '{dropReason}'.", streamId, reason);
                        this.UnsubscribeFrom(subscriptionId);
                        break;
                    default:
                        this.Logger.LogInformation("A standard subscription to stream with id '{streamId}' has been dropped for the following reason: '{dropReason}'. Resubscribing...", streamId, reason);
                        object subscriptionSource = this.Connection.SubscribeToStreamAsync(streamId, resolveLinks,
                            this.CreateStandardSubscriptionHandler(handler),
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
            EventStore.EventStoreSubscription subscription = (EventStore.EventStoreSubscription)sender;
            this.Subscriptions.Remove(subscription.Id, out _);
        }

    }

}
