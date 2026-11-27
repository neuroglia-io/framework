// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Neuroglia.Data.Infrastructure.ResourceOriented;
using Neuroglia.Data.Infrastructure.ResourceOriented.Configuration;
using Neuroglia.Data.Infrastructure.ResourceOriented.Services;
using Neuroglia.Serialization.Json;
using Neuroglia.Data;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Covers how a resource controller survives failed reconciliations and unreadable resources (see #139)
/// </summary>
public class ResourceControllerTests
{

    static FakeNamespacedResource Resource(string name, string resourceVersion) => new(new(name, "fake-namespace") { ResourceVersion = resourceVersion }, new(), new());

    static IResource Unreadable(string name) => JsonSerializer.Default.Deserialize<Resource>($$"""
        { "apiVersion": "{{FakeNamespacedResource.ResourceDefinition.Group}}/{{FakeNamespacedResource.ResourceDefinition.Version}}", "kind": "{{FakeNamespacedResource.ResourceDefinition.Kind}}",
          "metadata": { "name": "{{name}}", "namespace": "fake-namespace", "resourceVersion": "2" }, "spec": "not an object" }
        """)!;

    static TestController CreateController(FakeResourceRepository repository) => new(repository, Options.Create(new ResourceControllerOptions<FakeNamespacedResource>()
    {
        Reconciliation = new() { Interval = TimeSpan.FromMilliseconds(100) }
    }));

    [Fact]
    public async Task A_Failed_Reconciliation_Should_Neither_Stop_The_Host_Nor_The_Controller()
    {
        //arrange
        var repository = new FakeResourceRepository { Pages = [() => throw new TimeoutException("The store timed out"), () => [Resource("a", "1")]] };
        await using var controller = CreateController(repository);

        //act
        await controller.StartAsync(CancellationToken.None);
        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (controller.Cached.Count < 1 && DateTime.UtcNow < deadline) await Task.Delay(50);

        //assert
        controller.Cached.Should().ContainKey("fake-namespace.a");
        await controller.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task An_Unreadable_Resource_Should_Be_Skipped_But_Not_Deleted()
    {
        //arrange
        var repository = new FakeResourceRepository { Pages = [() => [Resource("a", "1"), Resource("b", "1")], () => [Resource("a", "1"), Unreadable("b")]] };
        await using var controller = CreateController(repository);

        //act
        (await controller.ReconcileOnceAsync()).Should().BeTrue();
        (await controller.ReconcileOnceAsync()).Should().BeTrue();

        //assert
        controller.Cached.Keys.Should().BeEquivalentTo(["fake-namespace.a", "fake-namespace.b"]);
        controller.Deleted.Should().BeEmpty();
    }

    [Fact]
    public async Task An_Update_Should_Be_Handled_With_The_New_State()
    {
        //arrange
        var repository = new FakeResourceRepository { Pages = [() => [Resource("a", "1")], () => [Resource("a", "2")]] };
        await using var controller = CreateController(repository);

        //act
        await controller.ReconcileOnceAsync();
        await controller.ReconcileOnceAsync();

        //assert
        controller.Updated.Should().ContainSingle().Which.Metadata.ResourceVersion.Should().Be("2");
        controller.Cached["fake-namespace.a"].Metadata.ResourceVersion.Should().Be("2");
    }

    [Fact]
    public async Task A_Failed_Reconciliation_Should_Be_Reported_Rather_Than_Thrown()
    {
        var repository = new FakeResourceRepository { Pages = [() => throw new InvalidOperationException("The store failed")] };
        await using var controller = CreateController(repository);
        (await controller.ReconcileOnceAsync()).Should().BeFalse();
    }

    sealed class TestController(IResourceRepository repository, IOptions<ResourceControllerOptions<FakeNamespacedResource>> options)
        : ResourceController<FakeNamespacedResource>(NullLoggerFactory.Instance, options, repository)
    {

        public IReadOnlyDictionary<string, FakeNamespacedResource> Cached => this.Resources;

        public List<FakeNamespacedResource> Updated { get; } = [];

        public List<FakeNamespacedResource> Deleted { get; } = [];

        public Task<bool> ReconcileOnceAsync() => this.TryReconcileAsync(CancellationToken.None);

        protected override Task OnResourceUpdatedAsync(FakeNamespacedResource resource, CancellationToken cancellationToken = default)
        {
            this.Updated.Add(resource);
            return base.OnResourceUpdatedAsync(resource, cancellationToken);
        }

        protected override Task OnResourceDeletedAsync(FakeNamespacedResource resource, CancellationToken cancellationToken = default)
        {
            this.Deleted.Add(resource);
            return base.OnResourceDeletedAsync(resource, cancellationToken);
        }

    }

    /// <summary>
    /// Stands for a store whose successive listings are scripted
    /// </summary>
    sealed class FakeResourceRepository
        : IResourceRepository
    {

        int _page;

        public List<Func<IEnumerable<IResource>>> Pages { get; init; } = [];

        public async IAsyncEnumerable<IResource> GetAllAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var page = this.Pages[Math.Min(this._page++, this.Pages.Count - 1)];
            await Task.Yield();
            foreach (var resource in page()) yield return resource;
        }

        public Task<IResourceWatch> WatchAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IResourceWatch>(new ResourceWatch(new Subject<IResourceWatchEvent>(), true));

        public Task<IResource> AddAsync(IResource resource, string group, string version, string plural, bool dryRun = false, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IResource?> GetAsync(string group, string version, string plural, string name, string? @namespace = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Neuroglia.Data.Infrastructure.ResourceOriented.ICollection> ListAsync(string group, string version, string plural, string? @namespace = null, IEnumerable<LabelSelector>? labelSelectors = null, ulong? maxResults = null, string? continuationToken = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IResource> ReplaceAsync(IResource resource, string group, string version, string plural, bool dryRun = false, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IResource> PatchAsync(Patch patch, string group, string version, string plural, string name, string? @namespace = null, string? resourceVersion = null, bool dryRun = false, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IResource> ReplaceSubResourceAsync(IResource resource, string group, string version, string plural, string subResource, bool dryRun = false, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IResource> PatchSubResourceAsync(Patch patch, string group, string version, string plural, string name, string subResource, string? @namespace = null, string? resourceVersion = null, bool dryRun = false, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IResource> RemoveAsync(string group, string version, string plural, string name, string? @namespace = null, bool dryRun = false, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public void Dispose() { }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    }

}
