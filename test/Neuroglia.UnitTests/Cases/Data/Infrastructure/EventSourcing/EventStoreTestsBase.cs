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

using Esprima;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neuroglia.Data.Infrastructure.EventSourcing;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;
using Neuroglia.Reactive;
using Org.BouncyCastle.Bcpg;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.EventSourcing;

public abstract class EventStoreTestsBase(IServiceCollection services)
    : IAsyncLifetime
{
    protected ServiceProvider ServiceProvider { get; } = services.BuildServiceProvider();

    protected CancellationTokenSource CancellationTokenSource { get; } = new();

    protected IEventStore EventStore { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        foreach (var hostedService in ServiceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(CancellationTokenSource.Token).ConfigureAwait(false);
        }
        EventStore = ServiceProvider.GetRequiredService<IEventStore>();
    }

    public async Task DisposeAsync() => await ServiceProvider.DisposeAsync().ConfigureAwait(false);

    [Fact, Priority(1)]
    public async Task Append_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();

        //act
        var offset = await EventStore.AppendAsync(streamId, events);
        var storedEvents = await EventStore.ReadAsync(streamId, StreamReadDirection.Forwards, 0).ToListAsync();

        //assert
        offset.Should().Be((ulong)storedEvents.Count - 1);
        storedEvents.Should().HaveSameCount(events);
        for (var i = 0; i < storedEvents.Count; i++)
        {
            var originalEvent = events[i];
            var storedEvent = storedEvents[i];
            storedEvent.Id.Should().NotBeNullOrWhiteSpace();
            storedEvent.Timestamp.Should().NotBe(default);
            storedEvent.Type.Should().Be(originalEvent.Type);
            storedEvent.Data.Should().BeEquivalentTo(originalEvent.Data);
            storedEvent.Metadata.Should().BeEquivalentTo(originalEvent.Metadata);
        }
    }

    [Fact, Priority(2)]
    public async Task Append_ToNonExistingStream_Should_Work_StreamNotFoundException()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();

        //act
        await EventStore.AppendAsync(streamId, events, -1);
    }

    [Fact, Priority(3)]
    public async Task Append_WithValidExpectedVersion_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var eventsToAppend = EventStreamFactory.Create().ToList();
        await EventStore.AppendAsync(streamId, events);

        //act
        await EventStore.AppendAsync(streamId, eventsToAppend, events.Count - 1);
    }

    [Fact, Priority(4)]
    public async Task Append_WithInvalidExpectedVersion_Should_Throw_OptimisticConcurrencyException()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();

        //assert
        var action = async () => await EventStore.AppendAsync(streamId, events, events.Count + 10);
        await action.Should().ThrowAsync<OptimisticConcurrencyException>();
    }

    [Fact, Priority(5)]
    public async Task Get_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await EventStore.AppendAsync(streamId, events);

        //act
        var stream = await EventStore.GetAsync(streamId);
        var storedEvents = await EventStore.ReadAsync(streamId, StreamReadDirection.Forwards, 0).ToListAsync();

        //assert
        stream.Should().NotBeNull();
        stream.Id.Should().Be(streamId);
        stream.Length.Should().Be(events.Count);
        stream.FirstEventAt.Should().Be(storedEvents.First().Timestamp);
        stream.LastEventAt.Should().Be(storedEvents.Last().Timestamp);
    }

    [Fact, Priority(6)]
    public async Task Get_NonExisting_Should_Throw_StreamNotFoundException()
    {
        //assert
        var action = async () => await EventStore.GetAsync("non-existing");
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

    [Fact, Priority(7)]
    public async Task Read_FromStream_Forwards_FromStart_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await EventStore.ReadAsync(streamId, StreamReadDirection.Forwards, StreamPosition.StartOfStream).ToListAsync();

        //assert
        storedEvents.Should().HaveSameCount(events);

        storedEvents.First().Type.Should().Be(events.First().Type);
        storedEvents.First().Data.Should().BeEquivalentTo(events.First().Data);

        storedEvents.Last().Type.Should().Be(events.Last().Type);
        storedEvents.Last().Data.Should().BeEquivalentTo(events.Last().Data);
        storedEvents.Should().BeInAscendingOrder((e1, e2) => e1.Timestamp.CompareTo(e2.Timestamp));
    }

    [Fact, Priority(8)]
    public async Task Read_FromStream_Forwards_FromEnd_Should_BeEmpty()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await EventStore.ReadAsync(streamId, StreamReadDirection.Forwards, StreamPosition.EndOfStream).ToListAsync();

        //assert
        storedEvents.Should().BeEmpty();
    }

    [Fact, Priority(9)]
    public async Task Read_FromStream_Forwards_FromOffset_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var offset = (long)events.Count / 2;
        await EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await EventStore.ReadAsync(streamId, StreamReadDirection.Forwards, offset).ToListAsync();

        //assert
        storedEvents.Count.Should().Be((int)(events.Count - offset));
        storedEvents.First().Offset.Should().Be((ulong)offset);

        storedEvents.Last().Type.Should().Be(events.Last().Type);
        storedEvents.Last().Data.Should().BeEquivalentTo(events.Last().Data);
        storedEvents.Should().BeInAscendingOrder((e1, e2) => e1.Timestamp.CompareTo(e2.Timestamp));
    }

    [Fact, Priority(10)]
    public async Task Read_FromStream_Backwards_FromEnd_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await EventStore.ReadAsync(streamId, StreamReadDirection.Backwards, StreamPosition.EndOfStream).ToListAsync();

        //assert
        storedEvents.Should().HaveSameCount(events);

        storedEvents.First().Type.Should().Be(events.Last().Type);
        storedEvents.First().Data.Should().BeEquivalentTo(events.Last().Data);

        storedEvents.Last().Type.Should().Be(events.First().Type);
        storedEvents.Last().Data.Should().BeEquivalentTo(events.First().Data);
        storedEvents.Should().BeInDescendingOrder((e1, e2) => e1.Timestamp.CompareTo(e2.Timestamp));
    }

    [Fact, Priority(11)]
    public async Task Read_FromStream_Backwards_FromOffset_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var offset = 2;
        await EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await EventStore.ReadAsync(streamId, StreamReadDirection.Backwards, offset).ToListAsync();

        //assert
        storedEvents.Count.Should().Be(events.Count - offset + 1);
        storedEvents.First().Offset.Should().Be((ulong)offset);

        storedEvents.Last().Type.Should().Be(events.First().Type);
        storedEvents.Last().Data.Should().BeEquivalentTo(events.First().Data);
        storedEvents.Should().BeInDescendingOrder((e1, e2) => e1.Timestamp.CompareTo(e2.Timestamp));
    }

    [Fact, Priority(12)]
    public async Task Read_FromStream_Backwards_FromStart_Should_BeEmpty()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await EventStore.ReadAsync(streamId, StreamReadDirection.Backwards, StreamPosition.StartOfStream).ToListAsync();

        //assert
        storedEvents.Should().BeEmpty();
    }

    [Fact, Priority(13)]
    public async Task Read_FromStream_NonExisting_Should_Throw_StreamNotFoundException()
    {
        //assert
        var action = async () => await EventStore.ReadAsync("non-existing", StreamReadDirection.Forwards, 0).ToListAsync();
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

    [Fact, Priority(14)]
    public async Task Read_FromAll_Forwards_FromStart_Should_Work()
    {
        //arrange
        var length = 3;
        var allEvents = new List<IEventDescriptor>();
        for(int i = 0; i< length; i++)
        {
            var streamId = $"fake-stream-{i}";
            var events = EventStreamFactory.Create().ToList();
            await EventStore.AppendAsync(streamId, events);
            allEvents.AddRange(events);
        }

        //act
        var storedEvents = await EventStore.ReadAsync(null, StreamReadDirection.Forwards, StreamPosition.StartOfStream).ToListAsync();

        //assert
        storedEvents.Should().HaveSameCount(allEvents);
    }

    [Fact, Priority(15)]
    public async Task Read_FromAll_Forwards_FromEnd_Should_BeEmpty()
    {
        //arrange
        var length = 3;
        for (int i = 0; i < length; i++)
        {
            var streamId = $"fake-stream-{i}";
            var events = EventStreamFactory.Create().ToList();
            await EventStore.AppendAsync(streamId, events);
        }

        //act
        var storedEvents = await EventStore.ReadAsync(null, StreamReadDirection.Forwards, StreamPosition.EndOfStream).ToListAsync();

        //assert
        storedEvents.Should().BeEmpty();
    }

    [Fact, Priority(16)]
    public async Task Read_FromAll_Forwards_FromOffset_Should_Work()
    {
        //arrange
        var length = 10;
        var allEvents = new List<IEventDescriptor>();
        for (int i = 0; i < length; i++)
        {
            var streamId = $"fake-stream-{i}";
            var events = EventStreamFactory.Create().ToList();
            await EventStore.AppendAsync(streamId, events);
            allEvents.AddRange(events);
        }
        var offset = allEvents.Count / 2;

        //act
        var storedEvents = await EventStore.ReadAsync(null, StreamReadDirection.Forwards, offset).ToListAsync();

        //assert
        storedEvents.Count.Should().Be(allEvents.Count - offset);
    }

    [Fact, Priority(17)]
    public async Task Read_FromAll_Backwards_FromEnd_Should_Work()
    {
        //arrange
        var length = 3;
        var allEvents = new List<IEventDescriptor>();
        for (int i = 0; i < length; i++)
        {
            var streamId = $"fake-stream-{i}";
            var events = EventStreamFactory.Create().ToList();
            await EventStore.AppendAsync(streamId, events);
            allEvents.AddRange(events);
        }

        //act
        var storedEvents = await EventStore.ReadAsync(null, StreamReadDirection.Backwards, StreamPosition.EndOfStream).ToListAsync();

        //assert
        storedEvents.Should().HaveSameCount(allEvents);
        storedEvents.Should().BeInDescendingOrder((e1, e2) => e1.Timestamp.CompareTo(e2.Timestamp));
    }

    [Fact, Priority(18)]
    public async Task Read_FromAll_Backwards_FromOffset_Should_Work()
    {
        //arrange
        var length = 10;
        var allEvents = new List<IEventDescriptor>();
        for (int i = 0; i < length; i++)
        {
            var streamId = $"fake-stream-{i}";
            var events = EventStreamFactory.Create().ToList();
            await EventStore.AppendAsync(streamId, events);
            allEvents.AddRange(events);
        }
        var offset = allEvents.Count / 2;

        //act
        var storedEvents = await EventStore.ReadAsync(null, StreamReadDirection.Backwards, offset).ToListAsync();

        //assert
        storedEvents.Count.Should().Be(allEvents.Count - offset + 1);
        storedEvents.Should().BeInDescendingOrder((e1, e2) => e1.Timestamp.CompareTo(e2.Timestamp));
    }

    [Fact, Priority(19)]
    public async Task Read_FromAll_Backwards_FromStart_Should_BeEmpty()
    {
        //arrange
        var length = 3;
        for (int i = 0; i < length; i++)
        {
            var streamId = $"fake-stream-{i}";
            var events = EventStreamFactory.Create().ToList();
            await EventStore.AppendAsync(streamId, events);
        }

        //act
        var storedEvents = await EventStore.ReadAsync(null, StreamReadDirection.Backwards, StreamPosition.StartOfStream).ToListAsync();

        //assert
        storedEvents.Should().BeEmpty();
    }

    [Fact, Priority(20)]
    public async Task Observe_Stream_FromStart_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var eventsToAppend = EventStreamFactory.Create().ToList();
        var handledEvents = new List<IEventRecord>();
        await EventStore.AppendAsync(streamId, events);

        //act
        var observable = (await EventStore.ObserveAsync(streamId, StreamPosition.StartOfStream)).Subscribe(handledEvents.Add);
        await EventStore.AppendAsync(streamId, eventsToAppend);
        await Task.Delay(100);

        //assert
        handledEvents.Should().HaveCount(events.Count + eventsToAppend.Count);
    }

    [Fact, Priority(21)]
    public async Task Observe_Stream_FromOffset_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var eventsToAppend = EventStreamFactory.Create().ToList();
        var offset = (long)events.Count;
        var handledEvents = new List<IEventRecord>();
        await EventStore.AppendAsync(streamId, events);

        //act
        var observable = (await EventStore.ObserveAsync(streamId, offset))
            .Subscribe(handledEvents.Add);
        await EventStore.AppendAsync(streamId, eventsToAppend);
        await Task.Delay(100);

        //assert
        handledEvents.Should().HaveSameCount(eventsToAppend);
    }

    [Fact, Priority(22)]
    public async Task Observe_Stream_FromEnd_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var eventsToAppend = EventStreamFactory.Create().ToList();
        var handledEvents = new List<IEventRecord>();
        await EventStore.AppendAsync(streamId, events);

        //act
        var observable = (await EventStore.ObserveAsync(streamId))
            .Subscribe(handledEvents.Add);
        await EventStore.AppendAsync(streamId, eventsToAppend);
        await Task.Delay(100);

        //assert
        handledEvents.Should().HaveSameCount(eventsToAppend);
    }

    [Fact, Priority(23)]
    public async Task Observe_Stream_NonExisting_Should_Throw_StreamNotFoundException()
    {
        //assert
        var action = async () => await EventStore.ObserveAsync("non-existing");
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

    [Fact, Priority(24)]
    public async Task Observe_All_FromStart_Should_Work()
    {
        //arrange
        var length = 10;
        var allEvents = new List<IEventDescriptor>();
        for (int i = 0; i < length; i++)
        {
            var streamId = $"fake-stream-{i}";
            var events = EventStreamFactory.Create().ToList();
            await EventStore.AppendAsync(streamId, events);
            allEvents.AddRange(events);
        }
        var eventsToAppend = EventStreamFactory.Create().ToList();
        var handledEvents = new List<IEventRecord>();

        //act
        var observable = (await EventStore.ObserveAsync(offset: StreamPosition.StartOfStream)).Subscribe(handledEvents.Add);
        await EventStore.AppendAsync(Guid.NewGuid().ToString("N")[..15], eventsToAppend);
        await Task.Delay(100);

        //assert
        handledEvents.Should().HaveCount(allEvents.Count + eventsToAppend.Count);
        handledEvents.Should().BeInAscendingOrder((e1, e2) => e1.Timestamp.CompareTo(e2.Timestamp));
    }

    [Fact, Priority(25)]
    public async Task Observe_All_FromOffset_Should_Work()
    {
        //arrange
        var length = 10;
        var allEvents = new List<IEventDescriptor>();
        for (int i = 0; i < length; i++)
        {
            var streamId = $"fake-stream-{i}";
            var events = EventStreamFactory.Create().ToList();
            await EventStore.AppendAsync(streamId, events);
            allEvents.AddRange(events);
        }
        var offset = allEvents.Count;
        var eventsToAppend = EventStreamFactory.Create().ToList();
        var handledEvents = new List<IEventRecord>();

        //act
        var observable = (await EventStore.ObserveAsync(offset: offset)).Subscribe(handledEvents.Add);
        await EventStore.AppendAsync(Guid.NewGuid().ToString("N")[..15], eventsToAppend);
        await Task.Delay(100);

        //assert
        handledEvents.Should().HaveSameCount(eventsToAppend);
    }

    [Fact, Priority(26)]
    public async Task Observe_All_FromEnd_Should_Work()
    {
        //arrange
        var length = 10;
        var allEvents = new List<IEventDescriptor>();
        for (int i = 0; i < length; i++)
        {
            var streamId = $"fake-stream-{i}";
            var events = EventStreamFactory.Create().ToList();
            await EventStore.AppendAsync(streamId, events);
            allEvents.AddRange(events);
        }
        var offset = allEvents.Count;
        var eventsToAppend = EventStreamFactory.Create().ToList();
        var handledEvents = new List<IEventRecord>();

        //act
        var observable = (await EventStore.ObserveAsync()).Subscribe(handledEvents.Add);
        await EventStore.AppendAsync(Guid.NewGuid().ToString("N")[..15], eventsToAppend);
        await Task.Delay(100);

        //assert
        handledEvents.Should().HaveSameCount(eventsToAppend);
    }

    [Fact, Priority(27)]
    public async Task Observe_All_WithConsumerGroup_Should_Work()
    {
        //arrange
        var length = 10;
        var allEvents = new List<IEventDescriptor>();
        for (int i = 0; i < length; i++)
        {
            var streamId = $"fake-stream-{i}";
            var events = EventStreamFactory.Create().ToList();
            await EventStore.AppendAsync(streamId, events);
            allEvents.AddRange(events);
        }
        var eventsToAppend = EventStreamFactory.Create().ToList();
        var handledEvents = new List<IEventRecord>();
        var consumerGroup = "test";

        //act
        using var subscription = (await EventStore.ObserveAsync(offset: StreamPosition.StartOfStream, consumerGroup: consumerGroup))
            .SubscribeAsync(async e => 
            {
                var ack = (IAckableEventRecord)e;
                handledEvents.Add(e);
                if (ack != null) await ack.AckAsync();
            });
        await EventStore.AppendAsync(Guid.NewGuid().ToString("N")[..15], eventsToAppend);
        await Task.Delay(100);

        //assert
        handledEvents.Should().HaveCount(allEvents.Count + eventsToAppend.Count);
        handledEvents.Should().BeInAscendingOrder((e1, e2) => e1.Timestamp.CompareTo(e2.Timestamp));
    }

    [Fact(Skip = "Skip because of ES-related bugs/inconsistencies"), Priority(28)]
    public async Task ObserveAndReplay_All_WithConsumerGroup_Should_Work()
    {
        //arrange
        var length = 10;
        var allEvents = new List<IEventDescriptor>();
        for (int i = 0; i < length; i++)
        {
            var streamId = $"fake-stream-{i}";
            var events = EventStreamFactory.Create().ToList();
            await EventStore.AppendAsync(streamId, events);
            allEvents.AddRange(events);
        }
        var handledEvents = new List<IEventRecord>();
        var replayedEvents = new List<IEventRecord>();
        var consumerGroup = "test";
        var acked = 0;
        var doWork = new TaskCompletionSource();

        //act
        var subscription = (await EventStore.ObserveAsync(offset: StreamPosition.StartOfStream, consumerGroup: consumerGroup))
            .SubscribeAsync(async e =>
            {
                var ack = (IAckableEventRecord)e;
                handledEvents.Add(e);
                if (ack != null) await ack.AckAsync();
                acked++;
                if (acked == allEvents.Count) doWork.SetResult();
            });
        await doWork.Task;
        doWork = new();
        acked = 0;
        await EventStore.SetOffsetAsync(consumerGroup, offset: StreamPosition.StartOfStream);
        subscription = (await EventStore.ObserveAsync(offset: StreamPosition.StartOfStream, consumerGroup: consumerGroup))
            .SubscribeAsync(async e =>
            {
                var ack = (IAckableEventRecord)e;
                replayedEvents.Add(e);
                if (ack != null) await ack.AckAsync();
                acked++;
                if (acked == allEvents.Count) doWork.SetResult();
            });
        await doWork.Task;

        //assert
        handledEvents.Should().HaveSameCount(replayedEvents);
        handledEvents.Should().AllSatisfy(e => e.Replayed.Should().BeFalse());
        replayedEvents.Should().AllSatisfy(e => e.Replayed.Should().BeTrue());
    }

    [Fact, Priority(29)]
    public async Task Truncate_ToZero_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await EventStore.AppendAsync(streamId, events);

        //act
        await EventStore.TruncateAsync(streamId);
        IEventStreamDescriptor? stream;
        try
        {
            stream = await EventStore.GetAsync(streamId);
        }
        catch (StreamNotFoundException)
        {
            stream = new EventStreamDescriptor(streamId, 0, null, null);
        }

        //assert
        stream.Length.Should().Be(0);
        stream.FirstEventAt.Should().BeNull();
        stream.LastEventAt.Should().BeNull();
    }

    [Fact, Priority(30)]
    public async Task Truncate_Partially_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var truncateBefore = events.Count / 2;
        await EventStore.AppendAsync(streamId, events);

        //act
        await EventStore.TruncateAsync(streamId, (ulong)truncateBefore);
        var stream = await EventStore.GetAsync(streamId);

        //assert
        stream.Length.Should().Be(truncateBefore);
        stream.FirstEventAt.Should().NotBeNull();
        stream.LastEventAt.Should().NotBeNull();
    }

    [Fact, Priority(31)]
    public async Task Truncate_NonExisting_Should_Throw_StreamNotFoundException()
    {
        //assert
        var action = async () => await EventStore.TruncateAsync("non-existing");
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

    [Fact, Priority(32)]
    public async Task Delete_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await EventStore.AppendAsync(streamId, events);

        //act
        await EventStore.DeleteAsync(streamId);

        //assert
        var action = async () => await EventStore.GetAsync(streamId);
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

    [Fact, Priority(33)]
    public async Task Delete_NonExisting_Should_Throw_StreamNotFoundException()
    {
        //assert
        var action = async () => await EventStore.DeleteAsync("non-existing");
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

}
