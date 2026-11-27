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

using k8s;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data;
using Neuroglia.Data.Infrastructure.ResourceOriented;
using Neuroglia.Data.Infrastructure.ResourceOriented.Configuration;
using Neuroglia.Data.Infrastructure.ResourceOriented.Services;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Containers;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.ResourceOriented.Databases;

public class KubernetesDatabaseTests
    : DatabaseTestsBase, IClassFixture<KubernetesClusterFixture>, IAsyncLifetime
{

    const string Namespace = KubernetesClusterFixture.NamespacedResourceNamespace;
    static readonly string Group = FakeNamespacedResourceDefinition.ResourceGroup;
    static readonly string Version = FakeNamespacedResourceDefinition.ResourceVersion;
    static readonly string Plural = FakeNamespacedResourceDefinition.ResourcePlural;
    static readonly string ClusterPlural = FakeClusterResourceDefinition.ResourcePlural;
    static readonly TimeSpan EventTimeout = TimeSpan.FromSeconds(20);

    readonly List<ServiceProvider> _serviceProviders = [];

    public KubernetesDatabaseTests(KubernetesClusterFixture cluster)
        : base((configuration, services) => ConfigureServices(cluster, configuration, services))
    {
        this.Cluster = cluster;
    }

    KubernetesClusterFixture Cluster { get; }

    static void ConfigureServices(KubernetesClusterFixture cluster, IConfiguration configuration, IServiceCollection services)
    {
        services.AddSerialization();
        services.AddSingleton(_ => cluster.CreateClient());
        services.AddKubernetesDatabase();
        services.AddSingleton<IResourceRepository, ResourceRepository>();
    }

    public Task InitializeAsync() => this.Cluster.ResetAsync();

    public async Task DisposeAsync()
    {
        foreach (var provider in this._serviceProviders) await provider.DisposeAsync();
    }

    [Fact]
    public async Task Initialize_Should_Create_NamespaceDefinition_And_DefaultNamespace_Once()
    {
        //arrange
        var database = this.BuildDatabase();

        //act
        var firstRun = await database.InitializeAsync();
        var secondRun = await database.InitializeAsync();

        //assert
        firstRun.Should().BeTrue();
        secondRun.Should().BeFalse();
        (await database.GetDefinitionAsync<Namespace>()).Should().NotBeNull();
        (await database.GetResourceAsync<Namespace>(Neuroglia.Data.Infrastructure.ResourceOriented.Namespace.DefaultNamespaceName)).Should().NotBeNull();
    }

    [Fact]
    public async Task Initialize_Should_Fail_When_ApiServer_Is_Unreachable()
    {
        //arrange
        using var kubernetes = new Kubernetes(new KubernetesClientConfiguration { Host = "https://127.0.0.1:1", SkipTlsVerify = true });
        var database = new KubernetesDatabase(new Microsoft.Extensions.Logging.Abstractions.NullLogger<KubernetesDatabase>(), kubernetes, this.BuildServices().GetRequiredService<IJsonSerializer>());

        //act
        var test = () => database.InitializeAsync();

        //assert
        await test.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task Namespaces_Should_Map_To_Kubernetes_Namespaces()
    {
        //arrange
        var database = this.BuildDatabase();

        //act
        var @namespace = await database.GetResourceAsync<Namespace>(Namespace);
        var namespaces = await database.ListResourcesAsync<Namespace>();

        //assert
        @namespace.Should().NotBeNull();
        @namespace!.ApiVersion.Should().Be(Resource.BuildApiVersion(NamespaceDefinition.ResourceGroup, NamespaceDefinition.ResourceVersion));
        @namespace.Kind.Should().Be(NamespaceDefinition.ResourceKind);
        @namespace.Metadata.ResourceVersion.Should().NotBeNullOrWhiteSpace();
        namespaces.Items!.Select(n => n.GetName()).Should().Contain([Namespace, Neuroglia.Data.Infrastructure.ResourceOriented.Namespace.DefaultNamespaceName]);
    }

    [Fact]
    public async Task Create_Should_Persist_Status_And_Generate_Name()
    {
        //arrange
        var database = this.BuildDatabase();
        var resource = new FakeNamespacedResource(new() { Name = "fake-", Namespace = Namespace }, new() { FakeProperty1 = "spec" }, new() { FakeProperty1 = "status", FakeProperty2 = 42 });

        //act
        var created = await database.CreateResourceAsync(resource);
        var fetched = await database.GetResourceAsync<FakeNamespacedResource>(created.GetName(), Namespace);

        //assert
        created.GetName().Should().StartWith("fake-").And.NotBe("fake-");
        created.Metadata.Generation.Should().Be(1);
        created.Metadata.CreationTimestamp.Should().NotBeNull();
        created.Metadata.ResourceVersion.Should().NotBeNullOrWhiteSpace();
        fetched.Should().NotBeNull();
        fetched!.Spec.Should().BeEquivalentTo(resource.Spec);
        fetched.Status.Should().BeEquivalentTo(resource.Status);
        fetched.Metadata.ResourceVersion.Should().Be(created.Metadata.ResourceVersion);
        fetched.Metadata.Generation.Should().Be(created.Metadata.Generation);
        fetched.Metadata.CreationTimestamp.Should().Be(created.Metadata.CreationTimestamp);
    }

    [Fact]
    public async Task Get_Missing_Resource_Should_Return_Null()
    {
        //arrange
        var database = this.BuildDatabase();

        //act
        var missingResource = await database.GetResourceAsync(Group, Version, Plural, "missing", Namespace);
        var resourceOfMissingType = await database.GetResourceAsync(Group, Version, "missing-types", "missing", Namespace);

        //assert
        missingResource.Should().BeNull();
        resourceOfMissingType.Should().BeNull();
    }

    [Fact]
    public async Task PatchSubResource_Other_Than_Status_Should_Fail()
    {
        //arrange
        var database = this.BuildDatabase();
        var resource = await database.CreateResourceAsync(NewResource("fake-scale"));
        var patch = new Patch(PatchType.JsonMergePatch, new JsonObject { ["spec"] = new JsonObject { ["replicas"] = 3 } });

        //act
        var patchTest = () => database.PatchSubResourceAsync(patch, Group, Version, Plural, resource.GetName(), "scale", Namespace);
        var replaceTest = () => database.ReplaceSubResourceAsync(resource, Group, Version, Plural, resource.GetName(), "scale", Namespace);

        //assert
        (await patchTest.Should().ThrowAsync<ProblemDetailsException>()).Which.Problem.Type.Should().Be(ProblemTypes.UnsupportedSubResource);
        (await replaceTest.Should().ThrowAsync<ProblemDetailsException>()).Which.Problem.Type.Should().Be(ProblemTypes.UnsupportedSubResource);
    }

    [Fact]
    public async Task PatchStatus_Should_Only_Update_Status()
    {
        //arrange
        var database = this.BuildDatabase();
        var resource = await database.CreateResourceAsync(NewResource("fake-status"));
        var patch = new Patch(PatchType.JsonMergePatch, new JsonObject { ["spec"] = new JsonObject { ["fakeProperty1"] = "ignored" }, ["status"] = new JsonObject { ["fakeProperty1"] = "patched" } });

        //act
        var patched = (await database.PatchSubResourceAsync(patch, Group, Version, Plural, resource.GetName(), KubernetesDatabase.StatusSubResource, Namespace)).ConvertTo<FakeNamespacedResource>()!;

        //assert
        patched.Status!.FakeProperty1.Should().Be("patched");
        patched.Spec.Should().BeEquivalentTo(resource.Spec);
        patched.Metadata.Generation.Should().Be(resource.Metadata.Generation);
        patched.Metadata.ResourceVersion.Should().NotBe(resource.Metadata.ResourceVersion);
    }

    [Fact]
    public async Task ReplaceStatus_Should_Only_Update_Status()
    {
        //arrange
        var database = this.BuildDatabase();
        var resource = await database.CreateResourceAsync(NewResource("fake-status"));
        var updated = resource with { Spec = new() { FakeProperty1 = "ignored" }, Status = new() { FakeProperty1 = "replaced" } };

        //act
        var replaced = (await database.ReplaceSubResourceAsync(updated, Group, Version, Plural, resource.GetName(), KubernetesDatabase.StatusSubResource, Namespace)).ConvertTo<FakeNamespacedResource>()!;

        //assert
        replaced.Status!.FakeProperty1.Should().Be("replaced");
        replaced.Spec.Should().BeEquivalentTo(resource.Spec);
        replaced.Metadata.Generation.Should().Be(resource.Metadata.Generation);
    }

    [Fact]
    public async Task Patch_With_ResourceVersion_Should_Enforce_Optimistic_Concurrency()
    {
        //arrange
        var database = this.BuildDatabase();
        var resource = await database.CreateResourceAsync(NewResource("fake-concurrency"));
        var staleResourceVersion = resource.Metadata.ResourceVersion!;
        var jsonPatch = new Patch(PatchType.JsonPatch, JsonNode.Parse("""[{ "op": "replace", "path": "/spec/fakeProperty1", "value": "first" }]""")!);
        var current = await database.PatchResourceAsync(jsonPatch, Group, Version, Plural, resource.GetName(), Namespace, staleResourceVersion);
        var mergePatch = new Patch(PatchType.JsonMergePatch, new JsonObject { ["spec"] = new JsonObject { ["fakeProperty1"] = "second" } });

        //act
        var staleJsonPatch = await CaptureProblemAsync(() => database.PatchResourceAsync(jsonPatch, Group, Version, Plural, resource.GetName(), Namespace, staleResourceVersion));
        var staleMergePatch = await CaptureProblemAsync(() => database.PatchResourceAsync(mergePatch, Group, Version, Plural, resource.GetName(), Namespace, staleResourceVersion));
        var unchanged = await database.GetResourceAsync<FakeNamespacedResource>(resource.GetName(), Namespace);
        var patched = await database.PatchResourceAsync(mergePatch, Group, Version, Plural, resource.GetName(), Namespace, current.Metadata.ResourceVersion);

        //assert
        current.ConvertTo<FakeNamespacedResource>()!.Spec.FakeProperty1.Should().Be("first");
        ShouldBeConflict(staleJsonPatch, staleResourceVersion, current.Metadata.ResourceVersion!);
        ShouldBeConflict(staleMergePatch, staleResourceVersion, current.Metadata.ResourceVersion!);
        unchanged!.Metadata.ResourceVersion.Should().Be(current.Metadata.ResourceVersion);
        unchanged.Spec.FakeProperty1.Should().Be("first");
        patched.ConvertTo<FakeNamespacedResource>()!.Spec.FakeProperty1.Should().Be("second");
    }

    [Fact]
    public async Task PatchStatus_With_Stale_ResourceVersion_Should_Fail_With_Conflict()
    {
        //arrange
        var database = this.BuildDatabase();
        var resource = await database.CreateResourceAsync(NewResource("fake-concurrency"));
        var staleResourceVersion = resource.Metadata.ResourceVersion!;
        var patch = new Patch(PatchType.JsonMergePatch, new JsonObject { ["status"] = new JsonObject { ["fakeProperty1"] = "patched" } });
        var current = await database.PatchSubResourceAsync(patch, Group, Version, Plural, resource.GetName(), KubernetesDatabase.StatusSubResource, Namespace, staleResourceVersion);

        //act
        var test = () => database.PatchSubResourceAsync(patch, Group, Version, Plural, resource.GetName(), KubernetesDatabase.StatusSubResource, Namespace, staleResourceVersion);

        //assert
        await ShouldFailWithConflictAsync(test, staleResourceVersion, current.Metadata.ResourceVersion!);
    }

    [Fact]
    public async Task StrategicMergePatch_Should_Work_And_Enforce_Optimistic_Concurrency()
    {
        //arrange
        var database = this.BuildDatabase();
        var resource = await database.CreateResourceAsync(NewResource("fake-strategic", new Dictionary<string, string> { ["existing"] = "label" }));
        var patch = new Patch(PatchType.JsonStrategicMergePatch, new JsonObject { ["metadata"] = new JsonObject { ["labels"] = new JsonObject { ["added"] = "label" } }, ["spec"] = new JsonObject { ["fakeProperty2"] = 7 } });

        //act
        var patched = (await database.PatchResourceAsync(patch, Group, Version, Plural, resource.GetName(), Namespace, resource.Metadata.ResourceVersion)).ConvertTo<FakeNamespacedResource>()!;
        var staleTest = () => database.PatchResourceAsync(patch, Group, Version, Plural, resource.GetName(), Namespace, resource.Metadata.ResourceVersion);

        //assert
        patched.Spec.FakeProperty2.Should().Be(7);
        patched.Spec.FakeProperty1.Should().Be(resource.Spec.FakeProperty1);
        patched.Metadata.Labels.Should().BeEquivalentTo(new Dictionary<string, string> { ["existing"] = "label", ["added"] = "label" });
        await ShouldFailWithConflictAsync(staleTest, resource.Metadata.ResourceVersion!, patched.Metadata.ResourceVersion!);
    }

    [Fact]
    public async Task Replace_With_Stale_Or_Missing_ResourceVersion_Should_Fail_With_Conflict()
    {
        //arrange
        var database = this.BuildDatabase();
        var resource = await database.CreateResourceAsync(NewResource("fake-concurrency"));
        var current = (await database.ReplaceResourceAsync(resource with { Spec = new() { FakeProperty1 = "first" } }, Group, Version, Plural, resource.GetName(), Namespace)).ConvertTo<FakeNamespacedResource>()!;

        //act
        var staleTest = () => database.ReplaceResourceAsync(resource with { Spec = new() { FakeProperty1 = "second" } }, Group, Version, Plural, resource.GetName(), Namespace);
        var staleStatusTest = () => database.ReplaceSubResourceAsync(resource with { Status = new() { FakeProperty1 = "second" } }, Group, Version, Plural, resource.GetName(), KubernetesDatabase.StatusSubResource, Namespace);
        var missingTest = () => database.ReplaceResourceAsync(resource with { Metadata = resource.Metadata with { ResourceVersion = null } }, Group, Version, Plural, resource.GetName(), Namespace);

        //assert
        current.Spec.FakeProperty1.Should().Be("first");
        await ShouldFailWithConflictAsync(staleTest, resource.Metadata.ResourceVersion!, current.Metadata.ResourceVersion!);
        await ShouldFailWithConflictAsync(staleStatusTest, resource.Metadata.ResourceVersion!, current.Metadata.ResourceVersion!);
        await ShouldFailWithConflictAsync(missingTest, string.Empty, current.Metadata.ResourceVersion!);
    }

    [Fact]
    public async Task Errors_Should_Be_Translated_Into_ProblemDetails()
    {
        //arrange
        var database = this.BuildDatabase();
        var resource = await database.CreateResourceAsync(NewResource("fake-errors"));
        var patch = new Patch(PatchType.JsonMergePatch, new JsonObject { ["spec"] = new JsonObject { ["fakeProperty1"] = "patched" } });
        var invalidResource = new Resource
        {
            ApiVersion = Resource.BuildApiVersion(Group, Version),
            Kind = FakeNamespacedResourceDefinition.ResourceKind,
            Metadata = new("fake-invalid", Namespace),
            ExtensionData = new Dictionary<string, object> { ["spec"] = new Dictionary<string, object> { ["fakeProperty2"] = "not-a-number" } }
        };

        //act
        var duplicate = await CaptureProblemAsync(() => database.CreateResourceAsync(NewResource("fake-errors")));
        var invalid = await CaptureProblemAsync(() => database.CreateResourceAsync(invalidResource, Group, Version, Plural, Namespace));
        var patchMissing = await CaptureProblemAsync(() => database.PatchResourceAsync(patch, Group, Version, Plural, "missing", Namespace));
        var patchStatusMissing = await CaptureProblemAsync(() => database.PatchSubResourceAsync(patch, Group, Version, Plural, "missing", KubernetesDatabase.StatusSubResource, Namespace));
        var replaceMissing = await CaptureProblemAsync(() => database.ReplaceResourceAsync(resource with { Metadata = resource.Metadata with { Name = "missing" } }, Group, Version, Plural, "missing", Namespace));
        var deleteMissing = await CaptureProblemAsync(() => database.DeleteResourceAsync(Group, Version, Plural, "missing", Namespace));
        var listMissingType = await CaptureProblemAsync(() => database.ListResourcesAsync(Group, Version, "missing-types"));
        var watchMissingType = await CaptureProblemAsync(() => database.WatchResourcesAsync(Group, Version, "missing-types"));

        //assert
        duplicate.Type.Should().Be(ProblemTypes.Conflict);
        duplicate.Status.Should().Be(ResourceProblemDetails.ResourceAlreadyExists(new ResourceReference(new(Group, Version, Plural), resource.GetName(), Namespace)).Status);
        invalid.Type.Should().Be(ProblemTypes.SchemaValidationFailed);
        invalid.Status.Should().Be((int)HttpStatusCode.BadRequest);
        invalid.Errors.Should().NotBeNullOrEmpty();
        foreach (var notFound in new[] { patchMissing, patchStatusMissing, replaceMissing, deleteMissing })
        {
            notFound.Type.Should().Be(ProblemTypes.NotFound);
            notFound.Status.Should().Be((int)HttpStatusCode.NotFound);
        }
        foreach (var definitionNotFound in new[] { listMissingType, watchMissingType })
        {
            definitionNotFound.Type.Should().Be(ProblemTypes.DefinitionNotFound);
            definitionNotFound.Status.Should().Be((int)HttpStatusCode.NotFound);
        }
    }

    [Fact]
    public async Task GetResources_Should_Page_Through_All_Resources()
    {
        //arrange
        const int pageSize = 3;
        const int resourceCount = 10;
        var handler = new RecordingHandler();
        var database = this.BuildDatabase(options => options.PageSize = pageSize, handler);
        for (var i = 0; i < resourceCount; i++) await database.CreateResourceAsync(NewResource($"fake-page-{i}"));
        handler.Requests.Clear();

        //act
        var resources = await database.GetResourcesAsync(Group, Version, Plural, Namespace).ToListAsync();

        //assert
        resources.Select(r => r.GetName()).Should().BeEquivalentTo(Enumerable.Range(0, resourceCount).Select(i => $"fake-page-{i}"));
        var listRequests = handler.Requests.Where(r => r.Query.Contains($"limit={pageSize}")).ToList();
        listRequests.Should().HaveCount((int)Math.Ceiling(resourceCount / (double)pageSize));
        listRequests.Skip(1).Should().AllSatisfy(r => r.Query.Should().Contain("continue="));
    }

    [Fact]
    public async Task ListResources_Should_Page_Using_MaxResults_And_ContinuationToken()
    {
        //arrange
        var database = this.BuildDatabase();
        for (var i = 0; i < 10; i++) await database.CreateResourceAsync(NewResource($"fake-page-{i}"));

        //act
        var page1 = await database.ListResourcesAsync(Group, Version, Plural, Namespace, maxResults: 4);
        var page2 = await database.ListResourcesAsync(Group, Version, Plural, Namespace, maxResults: 4, continuationToken: page1.Metadata.Continue);
        var page3 = await database.ListResourcesAsync(Group, Version, Plural, Namespace, maxResults: 4, continuationToken: page2.Metadata.Continue);
        var all = await database.ListResourcesAsync(Group, Version, Plural, Namespace);

        //assert
        page1.Items.Should().HaveCount(4);
        page1.Metadata.Continue.Should().NotBeNullOrWhiteSpace();
        page1.Metadata.RemainingItemCount.Should().BeGreaterThan(0);
        page2.Items.Should().HaveCount(4);
        page3.Items.Should().HaveCount(2);
        page3.Metadata.Continue.Should().BeNull();
        page1.Items!.Concat(page2.Items!).Concat(page3.Items!).OfType<IResource>().Select(r => r.GetName()).Should().OnlyHaveUniqueItems().And.HaveCount(10);
        all.Items.Should().HaveCount(10);
        all.Items.Should().AllBeAssignableTo<IResource>();
    }

    [Fact]
    public async Task Watch_Should_Only_Report_Subsequent_Changes()
    {
        //arrange
        var database = this.BuildDatabase();
        var existing = await database.CreateResourceAsync(NewResource("fake-existing"));
        var events = new ConcurrentQueue<IResourceWatchEvent>();

        //act
        await using var watch = await database.WatchResourcesAsync(Group, Version, Plural, Namespace);
        using var subscription = watch.Subscribe(events.Enqueue);
        var created = await database.CreateResourceAsync(NewResource("fake-new"));
        await database.PatchResourceAsync(new Patch(PatchType.JsonMergePatch, new JsonObject { ["spec"] = new JsonObject { ["fakeProperty1"] = "patched" } }), Group, Version, Plural, created.GetName(), Namespace);
        await database.DeleteResourceAsync(Group, Version, Plural, created.GetName(), Namespace);
        await WaitUntilAsync(() => events.Count >= 3);
        await Task.Delay(250);

        //assert
        events.Select(e => (e.Type, e.Resource.GetName())).Should().Equal(
            (ResourceWatchEventType.Created, "fake-new"),
            (ResourceWatchEventType.Updated, "fake-new"),
            (ResourceWatchEventType.Deleted, "fake-new"));
        events.Should().NotContain(e => e.Resource.GetName() == existing.GetName());
    }

    [Fact]
    public async Task Watch_Should_Filter_By_LabelSelectors()
    {
        //arrange
        var database = this.BuildDatabase();
        var events = new ConcurrentQueue<IResourceWatchEvent>();

        //act
        await using var watch = await database.WatchResourcesAsync(Group, Version, Plural, Namespace, [new LabelSelector("fake-app", LabelSelectionOperator.Equals, "foo")]);
        using var subscription = watch.Subscribe(events.Enqueue);
        await database.CreateResourceAsync(NewResource("fake-bar", new Dictionary<string, string> { ["fake-app"] = "bar" }));
        await database.CreateResourceAsync(NewResource("fake-unlabeled"));
        await database.CreateResourceAsync(NewResource("fake-foo", new Dictionary<string, string> { ["fake-app"] = "foo" }));
        await WaitUntilAsync(() => !events.IsEmpty);
        await Task.Delay(500);

        //assert
        events.Should().ContainSingle().Which.Resource.GetName().Should().Be("fake-foo");
    }

    [Fact]
    public async Task Watch_Should_Survive_ServerSide_Close()
    {
        //arrange
        var handler = new RecordingHandler();
        var database = this.BuildDatabase(options =>
        {
            options.WatchTimeout = TimeSpan.FromSeconds(1);
            options.WatchMinReconnectDelay = TimeSpan.FromMilliseconds(100);
        }, handler);
        var events = new ConcurrentQueue<IResourceWatchEvent>();
        var completed = false;

        //act
        await using var watch = await database.WatchResourcesAsync(Group, Version, Plural, Namespace);
        using var subscription = watch.Subscribe(events.Enqueue, () => completed = true);
        await database.CreateResourceAsync(NewResource("fake-before-close"));
        await WaitUntilAsync(() => events.Count >= 1);
        await WaitUntilAsync(() => handler.WatchRequestCount >= 3);
        await database.CreateResourceAsync(NewResource("fake-after-close"));
        await WaitUntilAsync(() => events.Count >= 2);
        await Task.Delay(250);

        //assert
        completed.Should().BeFalse();
        events.Select(e => (e.Type, e.Resource.GetName())).Should().Equal(
            (ResourceWatchEventType.Created, "fake-before-close"),
            (ResourceWatchEventType.Created, "fake-after-close"));
    }

    [Fact]
    public async Task Watch_Should_Resume_After_Transient_Errors()
    {
        //arrange
        var handler = new RecordingHandler();
        var database = this.BuildDatabase(options =>
        {
            options.WatchTimeout = TimeSpan.FromSeconds(1);
            options.WatchMinReconnectDelay = TimeSpan.FromMilliseconds(100);
            options.WatchMaxReconnectDelay = TimeSpan.FromMilliseconds(200);
        }, handler);
        var events = new ConcurrentQueue<IResourceWatchEvent>();

        //act
        await using var watch = await database.WatchResourcesAsync(Group, Version, Plural, Namespace);
        using var subscription = watch.Subscribe(events.Enqueue);
        await database.CreateResourceAsync(NewResource("fake-1"));
        await WaitUntilAsync(() => events.Count >= 1);
        handler.WatchFault = WatchFault.NetworkError;
        await WaitUntilAsync(() => handler.FaultedWatchRequestCount >= 1);
        await database.CreateResourceAsync(NewResource("fake-2"));
        handler.WatchFault = WatchFault.ServerError;
        await WaitUntilAsync(() => handler.FaultedWatchRequestCount >= 3);
        await database.CreateResourceAsync(NewResource("fake-3"));
        handler.WatchFault = WatchFault.None;
        await WaitUntilAsync(() => events.Count >= 3);
        await Task.Delay(250);

        //assert
        events.Select(e => (e.Type, e.Resource.GetName())).Should().Equal(
            (ResourceWatchEventType.Created, "fake-1"),
            (ResourceWatchEventType.Created, "fake-2"),
            (ResourceWatchEventType.Created, "fake-3"));
    }

    [Fact]
    public async Task Watch_Should_Relist_When_ResourceVersion_Is_Gone()
    {
        //arrange
        var handler = new RecordingHandler();
        var database = this.BuildDatabase(options =>
        {
            options.WatchTimeout = TimeSpan.FromSeconds(1);
            options.WatchMinReconnectDelay = TimeSpan.FromMilliseconds(100);
            options.WatchMaxReconnectDelay = TimeSpan.FromMilliseconds(200);
        }, handler);
        var deleted = await database.CreateResourceAsync(NewResource("fake-deleted"));
        var updated = await database.CreateResourceAsync(NewResource("fake-updated"));
        await database.CreateResourceAsync(NewResource("fake-unchanged"));
        var events = new ConcurrentQueue<IResourceWatchEvent>();

        //act
        await using var watch = await database.WatchResourcesAsync(Group, Version, Plural, Namespace);
        using var subscription = watch.Subscribe(events.Enqueue);
        handler.WatchFault = WatchFault.ServerError;
        await WaitUntilAsync(() => handler.FaultedWatchRequestCount >= 1);
        await database.DeleteResourceAsync(Group, Version, Plural, deleted.GetName(), Namespace);
        await database.PatchResourceAsync(new Patch(PatchType.JsonMergePatch, new JsonObject { ["spec"] = new JsonObject { ["fakeProperty1"] = "patched" } }), Group, Version, Plural, updated.GetName(), Namespace);
        await database.CreateResourceAsync(NewResource("fake-created"));
        handler.WatchFault = WatchFault.Gone;
        await WaitUntilAsync(() => events.Count >= 3);
        await database.CreateResourceAsync(NewResource("fake-after-relist"));
        await WaitUntilAsync(() => events.Count >= 4);
        await Task.Delay(250);

        //assert
        handler.GoneResponseCount.Should().Be(1);
        events.Take(3).Select(e => (e.Type, e.Resource.GetName())).Should().BeEquivalentTo(new[]
        {
            (ResourceWatchEventType.Deleted, "fake-deleted"),
            (ResourceWatchEventType.Updated, "fake-updated"),
            (ResourceWatchEventType.Created, "fake-created")
        });
        events.Skip(3).Select(e => (e.Type, e.Resource.GetName())).Should().Equal((ResourceWatchEventType.Created, "fake-after-relist"));
    }

    [Fact]
    public async Task Watch_Should_Complete_Only_When_Disposed()
    {
        //arrange
        var handler = new RecordingHandler();
        var database = this.BuildDatabase(options => options.WatchTimeout = TimeSpan.FromSeconds(1), handler);
        var completed = new TaskCompletionSource();
        var watch = await database.WatchResourcesAsync(Group, Version, ClusterPlural);
        using var subscription = watch.Subscribe(_ => { }, () => completed.TrySetResult());
        await WaitUntilAsync(() => handler.WatchRequestCount >= 2);
        completed.Task.IsCompleted.Should().BeFalse();

        //act
        await watch.DisposeAsync();

        //assert
        await completed.Task.WaitAsync(EventTimeout);
        var watchRequestCount = handler.WatchRequestCount;
        await Task.Delay(1500);
        handler.WatchRequestCount.Should().Be(watchRequestCount);
    }

    IDatabase BuildDatabase(Action<KubernetesDatabaseOptions>? setup = null, params DelegatingHandler[] handlers) => this.BuildServices(setup, handlers).GetRequiredService<IDatabase>();

    ServiceProvider BuildServices(Action<KubernetesDatabaseOptions>? setup = null, params DelegatingHandler[] handlers)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSerialization();
        services.AddSingleton(_ => this.Cluster.CreateClient(handlers));
        services.AddKubernetesDatabase(setup ?? (_ => { }));
        var provider = services.BuildServiceProvider();
        this._serviceProviders.Add(provider);
        return provider;
    }

    static FakeNamespacedResource NewResource(string name, IDictionary<string, string>? labels = null) => new(new(name, Namespace, labels), new());

    static async Task ShouldFailWithConflictAsync(Func<Task> test, string targetResourceVersion, string actualResourceVersion) => ShouldBeConflict(await CaptureProblemAsync(test), targetResourceVersion, actualResourceVersion);

    static void ShouldBeConflict(ProblemDetails problem, string targetResourceVersion, string actualResourceVersion)
    {
        problem.Type.Should().Be(ProblemTypes.OptimisticConcurrencyCheckFailed);
        problem.Status.Should().Be((int)HttpStatusCode.Conflict);
        problem.Detail.Should().Contain(actualResourceVersion);
        if (!string.IsNullOrEmpty(targetResourceVersion)) problem.Detail.Should().Contain(targetResourceVersion);
    }

    static async Task<ProblemDetails> CaptureProblemAsync(Func<Task> test) => (await test.Should().ThrowAsync<ProblemDetailsException>()).Which.Problem;

    static async Task WaitUntilAsync(Func<bool> condition)
    {
        var timeout = DateTimeOffset.UtcNow.Add(EventTimeout);
        while (!condition())
        {
            if (DateTimeOffset.UtcNow > timeout) throw new TimeoutException("The expected condition has not been met in time");
            await Task.Delay(50);
        }
    }

    enum WatchFault
    {
        None,
        NetworkError,
        ServerError,
        Gone
    }

    /// <summary>
    /// Records the requests sent to the API server, and optionally simulates failures of watch requests
    /// </summary>
    sealed class RecordingHandler
        : DelegatingHandler
    {

        int _watchRequestCount;
        int _faultedWatchRequestCount;
        int _goneResponseCount;

        public ConcurrentQueue<Uri> Requests { get; } = new();

        public volatile WatchFault WatchFault;

        public int WatchRequestCount => this._watchRequestCount;

        public int FaultedWatchRequestCount => this._faultedWatchRequestCount;

        public int GoneResponseCount => this._goneResponseCount;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.Requests.Enqueue(request.RequestUri!);
            if (!request.RequestUri!.Query.Contains("watch=true")) return base.SendAsync(request, cancellationToken);
            Interlocked.Increment(ref this._watchRequestCount);
            switch (this.WatchFault)
            {
                case WatchFault.NetworkError:
                    Interlocked.Increment(ref this._faultedWatchRequestCount);
                    throw new HttpRequestException("Simulated network error");
                case WatchFault.ServerError:
                    Interlocked.Increment(ref this._faultedWatchRequestCount);
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        RequestMessage = request,
                        Content = new StringContent("""{"kind":"Status","apiVersion":"v1","status":"Failure","message":"simulated server error","reason":"InternalError","code":500}""", Encoding.UTF8, "application/json")
                    });
                case WatchFault.Gone:
                    this.WatchFault = WatchFault.None;
                    Interlocked.Increment(ref this._goneResponseCount);
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        RequestMessage = request,
                        Content = new StringContent("""{"type":"ERROR","object":{"kind":"Status","apiVersion":"v1","metadata":{},"status":"Failure","message":"too old resource version","reason":"Expired","code":410}}""" + "\n", Encoding.UTF8, "application/json")
                    });
                default:
                    return base.SendAsync(request, cancellationToken);
            }
        }

    }

}
