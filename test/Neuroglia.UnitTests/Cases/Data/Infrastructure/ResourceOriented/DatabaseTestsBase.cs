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

using AutoBogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data;
using Neuroglia.Data.Infrastructure.ResourceOriented;
using Neuroglia.Data.Infrastructure.ResourceOriented.Services;
using Neuroglia.Serialization.Json;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.ResourceOriented;

[TestCaseOrderer("Neuroglia.UnitTests.Services.PriorityTestCaseOrderer", "Neuroglia.UnitTests")]
public abstract class DatabaseTestsBase
    : IDisposable
{

    internal const string FakeNamespaceName = "fake-namespace";
    bool _disposed;

    protected DatabaseTestsBase(Action<IConfiguration, IServiceCollection> setup)
    {
        this.Setup = setup;
        this.RepositoryBuilder = new(this.Setup);
    }

    protected Action<IConfiguration, IServiceCollection> Setup { get; }

    TestRepositoryBuilder RepositoryBuilder { get; }

    [Fact, Priority(1)]
    public async Task Add_Resource_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        using var resources = await this.RepositoryBuilder
            .WithDefinition<FakeNamespacedResourceDefinition>()
            .WithResource(new Namespace(@namespace))
            .BuildAsync();
        var name = "fake-resource-1";
        var labels = new Dictionary<string, string>() { { "fake-label-key-1", "fake-label-value-1" } };
        var spec = new FakeResourceSpec();
        var status = new FakeResourceStatus();
        var resource = new FakeNamespacedResource(new ResourceMetadata(name, @namespace, labels), spec, status);
        var resourceNode = JsonSerializer.Default.SerializeToNode(resource)!.AsObject();
        resourceNode.Remove(nameof(IMetadata.Metadata).ToCamelCase());
        var json = JsonSerializer.Default.SerializeToText(resourceNode);

        //act
        var persistedResource = await resources.AddAsync(resource);

        //assert
        persistedResource.Should().NotBeNull();
        persistedResource.Metadata.Name.Should().Be(name);
        persistedResource.Metadata.Namespace.Should().Be(@namespace);
        persistedResource.Metadata.CreationTimestamp.Should().NotBeNull();
        persistedResource.Metadata.Generation.Should().Be(1);
        persistedResource.Metadata.ResourceVersion.Should().NotBeNullOrWhiteSpace();
        persistedResource.Spec.Should().BeEquivalentTo(spec);
        persistedResource.Status.Should().BeEquivalentTo(status);
    }

    [Fact, Priority(2)]
    public async Task Get_ResourceDefinition_Should_Work()
    {
        //arrange
        using var resources = await this.RepositoryBuilder
            .BuildAsync();

        //act
        var namespaceDefinition = await resources.GetDefinitionAsync(NamespaceDefinition.ResourceGroup, NamespaceDefinition.ResourcePlural);

        //assert
        namespaceDefinition.Should().NotBeNull();
    }

    [Fact, Priority(3)]
    public async Task List_NamespacedResources_Should_Work()
    {
        //arrange
        var resourceCount = 10;
        var @namespace = FakeNamespaceName;
        var resources = new List<FakeNamespacedResource>(resourceCount);
        var repository = await this.RepositoryBuilder
            .WithDefinition<FakeNamespacedResourceDefinition>()
            .WithResource(new Namespace(@namespace))
            .BuildAsync();
        FakeNamespacedResource.AutoIncrementIndex = 0;
        for (int i = 0; i < resourceCount; i++)
        {
            var resource = FakeNamespacedResource.Create(@namespace);
            resource = await repository.AddAsync(resource);
            resource.Metadata.ExtensionData = null;
            resources.Add(resource);
        }

        //act
        var collection = await repository.ListAsync<FakeNamespacedResource>(@namespace);

        //assert
        collection.Should().NotBeNull();
        collection.Items.Should().NotBeNull();
        collection.Items!.Select(i =>
        {
            i.Metadata.ExtensionData = null;
            return i;
        }).Should().BeEquivalentTo(resources);
    }

    [Fact, Priority(4)]
    public async Task List_NamespacedResources_AcrossAllNamespace_Should_Work()
    {
        //arrange
        var resourceCount = 10;
        var @namespace = FakeNamespaceName;
        var resources = new List<FakeNamespacedResource>(resourceCount);
        var repository = await this.RepositoryBuilder
            .WithDefinition<FakeNamespacedResourceDefinition>()
            .WithResource(new Namespace(@namespace))
            .BuildAsync();
        FakeNamespacedResource.AutoIncrementIndex = 0;
        for (int i = 0; i < resourceCount; i++)
        {
            var resource = FakeNamespacedResource.Create(@namespace);
            resource = await repository.AddAsync(resource);
            resource.Metadata.ExtensionData = null;
            resources.Add(resource);
        }

        //act
        var collection = await repository.ListAsync<FakeNamespacedResource>();

        //assert
        collection.Should().NotBeNull();
        collection.Items.Should().NotBeNull();
        collection.Items!.Select(i =>
        {
            i.Metadata.ExtensionData = null;
            return i;
        }).Should().BeEquivalentTo(resources);
    }

    [Fact, Priority(5)]
    public async Task ListAndFilter_NamespacedResources_Should_Work()
    {
        //arrange
        var resourceCount = 10;
        var @namespace = FakeNamespaceName;
        var resources = new List<FakeNamespacedResource>(resourceCount);
        var repository = await this.RepositoryBuilder
            .WithDefinition<FakeNamespacedResourceDefinition>()
            .WithResource(new Namespace(@namespace))
            .BuildAsync();
        FakeNamespacedResource.AutoIncrementIndex = 0;
        for (int i = 0; i < resourceCount; i++)
        {
            var resource = FakeNamespacedResource.Create(@namespace, new Dictionary<string, string>() { { "fake-label", $"{(i < 5 ? "foo" : i < 8 ? "bar" : "baz")}" } });
            resource = await repository.AddAsync(resource);
            resources.Add(resource);
        }

        //act
        var fooCollection = await repository.ListAsync<FakeNamespacedResource>(@namespace, new LabelSelector[] { new("fake-label", LabelSelectionOperator.Equals, "foo") });
        var barCollection = await repository.ListAsync<FakeNamespacedResource>(@namespace, new LabelSelector[] { new("fake-label", LabelSelectionOperator.Contains, "bar") });
        var bazCollection = await repository.ListAsync<FakeNamespacedResource>(@namespace, new LabelSelector[] { new("fake-label", LabelSelectionOperator.NotContains, "foo", "bar") });
        var barBazCollection = await repository.ListAsync<FakeNamespacedResource>(@namespace, new LabelSelector[] { new("fake-label", LabelSelectionOperator.Contains, "bar", "baz") });
        var notFooCollection = await repository.ListAsync<FakeNamespacedResource>(@namespace, new LabelSelector[] { new("fake-label", LabelSelectionOperator.NotEquals, "foo") });

        //assert
        fooCollection.Should().NotBeNull();
        fooCollection.Items.Should().NotBeNull();
        fooCollection.Items.Should().HaveCount(5);

        barCollection.Should().NotBeNull();
        barCollection.Items.Should().NotBeNull();
        barCollection.Items.Should().HaveCount(3);

        bazCollection.Should().NotBeNull();
        bazCollection.Items.Should().NotBeNull();
        bazCollection.Items.Should().HaveCount(2);

        barBazCollection.Should().NotBeNull();
        barBazCollection.Items.Should().NotBeNull();
        barBazCollection.Items.Should().HaveCount(5);

        notFooCollection.Items!.Select(i =>
        {
            i.Metadata.ExtensionData = null;
            return i;
        }).Should().BeEquivalentTo(barBazCollection.Items!.Select(i =>
        {
            i.Metadata.ExtensionData = null;
            return i;
        }));
    }

    [Fact, Priority(6)]
    public async Task List_ClusterResources_Should_Work()
    {
        //arrange
        var resourceCount = 10;
        var resources = new List<FakeClusterResource>(resourceCount);
        var repository = await this.RepositoryBuilder
            .WithDefinition<FakeClusterResourceDefinition>()
            .BuildAsync();
        FakeNamespacedResource.AutoIncrementIndex = 0;
        for (int i = 0; i < resourceCount; i++)
        {
            var resource = FakeClusterResource.Create();
            resource = await repository.AddAsync(resource);
            resource.Metadata.ExtensionData = null;
            resources.Add(resource);
        }

        //act
        var collection = await repository.ListAsync<FakeClusterResource>();

        //assert
        collection.Should().NotBeNull();
        collection.Items.Should().NotBeNull();
        collection.Items!.Select(i =>
        {
            i.Metadata.ExtensionData = null;
            return i;
        }).Should().BeEquivalentTo(resources);
    }

    [Fact, Priority(7)]
    public async Task ListAndFilter_ClusterResources_Should_Work()
    {
        //arrange
        var resourceCount = 10;
        var resources = new List<FakeClusterResource>(resourceCount);
        var repository = await this.RepositoryBuilder
            .WithDefinition<FakeClusterResourceDefinition>()
            .BuildAsync();
        FakeClusterResource.AutoIncrementIndex = 0;
        for (int i = 0; i < resourceCount; i++)
        {
            var resource = FakeClusterResource.Create(new Dictionary<string, string>() { { "fake-label", $"{(i < 5 ? "foo" : i < 8 ? "bar" : "baz")}" } });
            resource = await repository.AddAsync(resource);
            resources.Add(resource);
        }

        //act
        var fooCollection = await repository.ListAsync<FakeClusterResource>(null, new LabelSelector[] { new("fake-label", LabelSelectionOperator.Equals, "foo") });
        var barCollection = await repository.ListAsync<FakeClusterResource>(null, new LabelSelector[] { new("fake-label", LabelSelectionOperator.Contains, "bar") });
        var bazCollection = await repository.ListAsync<FakeClusterResource>(null, new LabelSelector[] { new("fake-label", LabelSelectionOperator.NotContains, "foo", "bar") });
        var barBazCollection = await repository.ListAsync<FakeClusterResource>(null, new LabelSelector[] { new("fake-label", LabelSelectionOperator.Contains, "bar", "baz") });
        var notFooCollection = await repository.ListAsync<FakeClusterResource>(null, new LabelSelector[] { new("fake-label", LabelSelectionOperator.NotEquals, "foo") });

        //assert
        fooCollection.Should().NotBeNull();
        fooCollection.Items.Should().NotBeNull();
        fooCollection.Items.Should().HaveCount(5);

        barCollection.Should().NotBeNull();
        barCollection.Items.Should().NotBeNull();
        barCollection.Items.Should().HaveCount(3);

        bazCollection.Should().NotBeNull();
        bazCollection.Items.Should().NotBeNull();
        bazCollection.Items.Should().HaveCount(2);

        barBazCollection.Should().NotBeNull();
        barBazCollection.Items.Should().NotBeNull();
        barBazCollection.Items.Should().HaveCount(5);

        notFooCollection.Items!.Select(i =>
        {
            i.Metadata.ExtensionData = null;
            return i;
        }).Should().BeEquivalentTo(barBazCollection.Items!.Select(i =>
        {
            i.Metadata.ExtensionData = null;
            return i;
        }));
    }

    [Fact, Priority(8)]
    public async Task Get_NamespacedResources_Should_Work()
    {
        //arrange
        var resourceCount = 10;
        var @namespace = FakeNamespaceName;
        var resources = new List<FakeNamespacedResource>(resourceCount);
        var repository = await this.RepositoryBuilder
            .WithDefinition<FakeNamespacedResourceDefinition>()
            .WithResource(new Namespace(@namespace))
            .BuildAsync();
        FakeNamespacedResource.AutoIncrementIndex = 0;
        for (int i = 0; i < resourceCount; i++)
        {
            var resource = FakeNamespacedResource.Create(@namespace);
            resource = await repository.AddAsync(resource);
            resource.Metadata.ExtensionData = null;
            resources.Add(resource);
        }

        //act
        var collection = repository.GetAllAsync<FakeNamespacedResource>(@namespace);
        var items = await collection.ToListAsync();

        //assert
        collection.Should().NotBeNull();
        items.Should().NotBeNull();
        items.Select(i =>
        {
            i.Metadata.ExtensionData = null;
            return i;
        }).Should().BeEquivalentTo(resources);
    }

    [Fact, Priority(9)]
    public async Task Get_NamespacedResources_AcrossAllNamespaces_Should_Work()
    {
        //arrange
        var resourceCount = 10;
        var @namespace = FakeNamespaceName;
        var resources = new List<IResource>(resourceCount);
        var repository = await this.RepositoryBuilder
            .WithDefinition<FakeNamespacedResourceDefinition>()
            .WithResource(new Namespace(@namespace))
            .BuildAsync();
        FakeNamespacedResource.AutoIncrementIndex = 0;
        for (int i = 0; i < resourceCount; i++)
        {
            var resource = FakeNamespacedResource.Create(@namespace);
            resource = await repository.AddAsync(resource);
            resource.Metadata.ExtensionData = null;
            resources.Add(resource);
        }

        //act
        var collection = repository.GetAllAsync<FakeNamespacedResource>();
        var items = await collection.ToListAsync();

        //assert
        collection.Should().NotBeNull();
        items.Should().NotBeNull();
        items.Select(i =>
        {
            i.Metadata.ExtensionData = null;
            return i;
        }).Should().BeEquivalentTo(resources);
    }

    [Fact, Priority(10)]
    public async Task Get_ClusterResources_Should_Work()
    {
        //arrange
        var resourceCount = 10;
        var resources = new List<FakeClusterResource>(resourceCount);
        var repository = await this.RepositoryBuilder
            .WithDefinition<FakeClusterResourceDefinition>()
            .BuildAsync();
        FakeClusterResource.AutoIncrementIndex = 0;
        for (int i = 0; i < resourceCount; i++)
        {
            var resource = FakeClusterResource.Create();
            resource = await repository.AddAsync(resource);
            resource.Metadata.ExtensionData = null;
            resources.Add(resource);
        }

        //act
        var collection = repository.GetAllAsync<FakeClusterResource>();
        var items = await collection.ToListAsync();

        //assert
        collection.Should().NotBeNull();
        items.Should().NotBeNull();
        items.Select(i =>
        {
            i.Metadata.ExtensionData = null;
            return i;
        }).Should().BeEquivalentTo(resources);
    }

    [Fact, Priority(11)]
    public async Task Watch_NamespacedResources_Should_Work()
    {
        //arrange
        var resourceCount = 10;
        var @namespace = FakeNamespaceName;
        using var resourceRepository = await this.RepositoryBuilder
             .WithDefinition<FakeNamespacedResourceDefinition>()
             .WithResource(new Namespace(@namespace))
             .BuildAsync();

        var processedWatchEvents = new List<IResourceWatchEvent>(resourceCount);

        //act
        await using var watch = await resourceRepository.WatchAsync<FakeNamespacedResource>(@namespace);
        watch.Subscribe(processedWatchEvents.Add);
        FakeNamespacedResource.AutoIncrementIndex = 0;
        for (int i = 0; i < resourceCount; i++)
        {
            await resourceRepository.AddAsync(FakeNamespacedResource.Create(@namespace));
        }
        await Task.Delay(100);

        //assert
        processedWatchEvents.ToList().Should().HaveCount(resourceCount);
        processedWatchEvents.ToList().Should().AllSatisfy(e => e.Type.Should().Be(ResourceWatchEventType.Created));
    }

    [Fact, Priority(12)]
    public async Task Watch_ClusterResources_Should_Work()
    {
        //arrange
        var resourceCount = 10;
        using var resourceRepository = await this.RepositoryBuilder
             .WithDefinition<FakeClusterResourceDefinition>()
             .BuildAsync();

        var processedWatchEvents = new List<IResourceWatchEvent>(resourceCount);

        //act
        await using var watch = await resourceRepository.WatchAsync<FakeClusterResource>();
        watch.Subscribe(processedWatchEvents.Add);
        FakeClusterResource.AutoIncrementIndex = 0;
        for (int i = 0; i < resourceCount; i++)
        {
            await resourceRepository.AddAsync(FakeClusterResource.Create());
        }
        await Task.Delay(100);

        //assert
        processedWatchEvents.ToList().Should().HaveCount(resourceCount);
        processedWatchEvents.ToList().Should().AllSatisfy(e => e.Type.Should().Be(ResourceWatchEventType.Created));
    }

    [Fact, Priority(13)]
    public async Task Get_NamespacedResource_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        using var resources = await this.RepositoryBuilder
             .WithDefinition<FakeNamespacedResourceDefinition>()
             .WithResource(new Namespace(@namespace))
             .BuildAsync();
        var resource = await resources.AddAsync(FakeNamespacedResource.Create(@namespace));
        resource.Metadata.ExtensionData = null;

        //act
        var fetchedResource = await resources.GetAsync<FakeNamespacedResource>(resource.GetName(), resource.GetNamespace());

        //assert
        fetchedResource.Should().NotBeNull();
        fetchedResource!.Metadata.ExtensionData = null;
        fetchedResource.Should().BeEquivalentTo(resource);
    }

    [Fact, Priority(14)]
    public async Task Get_ClusterResource_Should_Work()
    {
        //arrange
        using var resources = await this.RepositoryBuilder
             .WithDefinition<FakeClusterResourceDefinition>()
             .BuildAsync();
        var resource = await resources.AddAsync(FakeClusterResource.Create());
        resource.Metadata.ExtensionData = null;

        //act
        var fetchedResource = await resources.GetAsync<FakeClusterResource>(resource.GetName(), resource.GetNamespace());

        //assert
        fetchedResource.Should().NotBeNull();
        fetchedResource!.Metadata.ExtensionData = null;
        fetchedResource.Should().BeEquivalentTo(resource);
    }

    [Fact, Priority(15)]
    public async Task Patch_NamespacedResource_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        var resource = FakeNamespacedResource.Create(@namespace);
        using var resources = await this.RepositoryBuilder
             .WithDefinition<FakeNamespacedResourceDefinition>()
             .WithResource(new Namespace(@namespace))
             .WithResource(resource)
             .BuildAsync();
        var updatedResource = resource.Clone()!;
        updatedResource.Spec.FakeProperty1 = "Updated Fake Value";
        updatedResource.Spec.FakeProperty2 = 6;
        var jsonPatch = JsonPatchUtility.CreateJsonPatchFromDiff(resource, updatedResource);
        var patch = new Patch(PatchType.JsonPatch, jsonPatch);

        //act
        var patchedResource = await resources.PatchAsync<FakeNamespacedResource>(patch, resource.GetName(), resource.GetNamespace());

        //assert
        patchedResource.Should().NotBeNull();
        patchedResource.Metadata.Generation.Should().NotBe(resource.Metadata.Generation);
        patchedResource.Metadata.ResourceVersion.Should().NotBe(resource.Metadata.ResourceVersion);
        patchedResource.Spec.Should().BeEquivalentTo(updatedResource.Spec);
        patchedResource.Status.Should().BeEquivalentTo(updatedResource.Status);
    }

    [Fact, Priority(16)]
    public async Task Patch_ClusterResource_Should_Work()
    {
        //arrange
        var resource = FakeClusterResource.Create();
        using var resources = await this.RepositoryBuilder
             .WithDefinition<FakeClusterResourceDefinition>()
             .WithResource(resource)
             .BuildAsync();
        var updatedResource = resource.Clone()!;
        updatedResource.Spec.FakeProperty1 = "Updated Fake Value";
        updatedResource.Spec.FakeProperty2 = 6;
        var jsonPatch = JsonPatchUtility.CreateJsonPatchFromDiff(resource, updatedResource);
        var patch = new Patch(PatchType.JsonPatch, jsonPatch);

        //act
        var patchedResource = await resources.PatchAsync<FakeClusterResource>(patch, resource.GetName());

        //assert
        patchedResource.Should().NotBeNull();
        patchedResource.Metadata.Generation.Should().NotBe(resource.Metadata.Generation);
        patchedResource.Metadata.ResourceVersion.Should().NotBe(resource.Metadata.ResourceVersion);
        patchedResource.Spec.Should().BeEquivalentTo(updatedResource.Spec);
        patchedResource.Status.Should().BeEquivalentTo(updatedResource.Status);
    }

    [Fact, Priority(17)]
    public async Task Patch_NamespacedResource_Metadata_Should_Fail()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        var resource = FakeNamespacedResource.Create(@namespace);
        using var resources = await this.RepositoryBuilder
             .WithDefinition<FakeNamespacedResourceDefinition>()
             .WithResource(new Namespace(@namespace))
             .WithResource(resource)
             .BuildAsync();
        var updatedResource = resource.Clone()!;
        updatedResource.Metadata.Name = "Updated Fake Name";
        var jsonPatch = JsonPatchUtility.CreateJsonPatchFromDiff(resource, updatedResource);
        var patch = new Patch(PatchType.JsonPatch, jsonPatch);

        //act
        var test = async () => await resources.PatchAsync<FakeNamespacedResource>(patch, resource.GetName(), resource.GetNamespace());

        //assert
        await test.Should().ThrowAsync<ProblemDetailsException>();
    }

    [Fact, Priority(18)]
    public async Task Patch_ClusterResource_Metadata_Should_Fail()
    {
        //arrange
        var resource = FakeClusterResource.Create();
        using var resources = await this.RepositoryBuilder
             .WithDefinition<FakeClusterResourceDefinition>()
             .WithResource(resource)
             .BuildAsync();
        var updatedResource = resource.Clone()!;
        updatedResource.Metadata.Name = "Updated Fake Name";
        var jsonPatch = JsonPatchUtility.CreateJsonPatchFromDiff(resource, updatedResource);
        var patch = new Patch(PatchType.JsonPatch, jsonPatch);

        //act
        var test = async () => await resources.PatchAsync<FakeClusterResource>(patch, resource.GetName(), resource.GetNamespace());

        //assert
        await test.Should().ThrowAsync<ProblemDetailsException>();
    }

    [Fact, Priority(19)]
    public async Task Patch_NamespacedResource_LabelsAndAnnotations_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        var resource = FakeNamespacedResource.Create(@namespace);
        using var resources = await this.RepositoryBuilder
             .WithDefinition<FakeNamespacedResourceDefinition>()
             .WithResource(new Namespace(@namespace))
             .WithResource(resource)
             .BuildAsync();
        var updatedResource = resource.Clone()!;
        if (updatedResource.Metadata.Labels == null) updatedResource.Metadata.Labels = new Dictionary<string, string>();
        updatedResource.Metadata.Labels.Add("fake-label-1", "fake-label-value-1");
        if (updatedResource.Metadata.Annotations == null) updatedResource.Metadata.Annotations = new Dictionary<string, string>();
        updatedResource.Metadata.Annotations.Add("fake-annotation-1", "fake annotation value 1");
        var jsonPatch = JsonPatchUtility.CreateJsonPatchFromDiff(resource, updatedResource);
        var patch = new Patch(PatchType.JsonPatch, jsonPatch);

        //act
        var patchedResource = await resources.PatchAsync<FakeNamespacedResource>(patch, resource.GetName(), resource.GetNamespace());

        //assert
        patchedResource.Should().NotBeNull();
        patchedResource.Metadata.Generation.Should().NotBe(resource.Metadata.Generation);
        patchedResource.Metadata.ResourceVersion.Should().NotBe(resource.Metadata.ResourceVersion);
        patchedResource.Metadata.Annotations.Should().BeEquivalentTo(updatedResource.Metadata.Annotations);
        patchedResource.Metadata.Labels.Should().BeEquivalentTo(updatedResource.Metadata.Labels);
    }

    [Fact, Priority(20)]
    public async Task Patch_ClusterResource_LabelsAndAnnotations_Should_Work()
    {
        //arrange
        var resource = FakeClusterResource.Create();
        using var resources = await this.RepositoryBuilder
             .WithDefinition<FakeClusterResourceDefinition>()
             .WithResource(resource)
             .BuildAsync();
        var updatedResource = resource.Clone()!;
        if (updatedResource.Metadata.Labels == null) updatedResource.Metadata.Labels = new Dictionary<string, string>();
        updatedResource.Metadata.Labels.Add("fake-label-1", "fake-label-value-1");
        if (updatedResource.Metadata.Annotations == null) updatedResource.Metadata.Annotations = new Dictionary<string, string>();
        updatedResource.Metadata.Annotations.Add("fake-annotation-1", "fake annotation value 1");
        var jsonPatch = JsonPatchUtility.CreateJsonPatchFromDiff(resource, updatedResource);
        var patch = new Patch(PatchType.JsonPatch, jsonPatch);

        //act
        var patchedResource = await resources.PatchAsync<FakeClusterResource>(patch, resource.GetName(), resource.GetNamespace());

        //assert
        patchedResource.Should().NotBeNull();
        patchedResource.Metadata.Generation.Should().NotBe(resource.Metadata.Generation);
        patchedResource.Metadata.ResourceVersion.Should().NotBe(resource.Metadata.ResourceVersion);
        patchedResource.Metadata.Annotations.Should().BeEquivalentTo(updatedResource.Metadata.Annotations);
        patchedResource.Metadata.Labels.Should().BeEquivalentTo(updatedResource.Metadata.Labels);
    }

    [Fact, Priority(21)]
    public async Task Replace_NamespacedResource_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        var resource = FakeNamespacedResource.Create(@namespace);
        using var resources = await this.RepositoryBuilder
            .WithDefinition<FakeNamespacedResourceDefinition>()
            .WithResource(new Namespace(@namespace))
            .WithResource(resource)
            .BuildAsync();
        resource = (await resources.GetAsync<FakeNamespacedResource>(resource.GetName(), resource.GetNamespace()))!;
        var updatedResource = resource.Clone()!;
        updatedResource.Spec.FakeProperty1 = "Updated Fake Value";
        updatedResource.Spec.FakeProperty2 = 6;

        //act
        var patchedResource = await resources.ReplaceAsync(updatedResource);

        //assert
        patchedResource.Should().NotBeNull();
        patchedResource.Metadata.Generation.Should().NotBe(resource.Metadata.Generation);
        patchedResource.Metadata.ResourceVersion.Should().NotBe(resource.Metadata.ResourceVersion);
        patchedResource.Spec.Should().BeEquivalentTo(updatedResource.Spec);
        patchedResource.Status.Should().BeEquivalentTo(updatedResource.Status);
    }

    [Fact, Priority(22)]
    public async Task Replace_ClusterResource_Should_Work()
    {
        //arrange
        var resource = FakeClusterResource.Create();
        using var resources = await this.RepositoryBuilder
            .WithDefinition<FakeClusterResourceDefinition>()
            .WithResource(resource)
            .BuildAsync();
        resource = (await resources.GetAsync<FakeClusterResource>(resource.GetName()))!;
        var updatedResource = resource.Clone()!;
        updatedResource.Spec.FakeProperty1 = "Updated Fake Value";
        updatedResource.Spec.FakeProperty2 = 6;

        //act
        var patchedResource = await resources.ReplaceAsync(updatedResource);

        //assert
        patchedResource.Should().NotBeNull();
        patchedResource.Metadata.Generation.Should().NotBe(resource.Metadata.Generation);
        patchedResource.Metadata.ResourceVersion.Should().NotBe(resource.Metadata.ResourceVersion);
        patchedResource.Spec.Should().BeEquivalentTo(updatedResource.Spec);
        patchedResource.Status.Should().BeEquivalentTo(updatedResource.Status);
    }

    [Fact, Priority(23)]
    public async Task Patch_NamespacedResourceStatus_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        var resource = FakeNamespacedResource.Create(@namespace);
        using var resources = await this.RepositoryBuilder
             .WithDefinition<FakeNamespacedResourceDefinition>()
             .WithResource(new Namespace(@namespace))
             .WithResource(resource)
             .BuildAsync();
        resource = (await resources.GetAsync<FakeNamespacedResource>(resource.GetName(), resource.GetNamespace()))!;
        var updatedResource = resource.Clone()!;
        updatedResource.Status!.FakeProperty1 = "Updated Fake Value";
        updatedResource.Status!.FakeProperty2 = 6;
        var jsonPatch = JsonPatchUtility.CreateJsonPatchFromDiff(resource, updatedResource);
        var patch = new Patch(PatchType.JsonPatch, jsonPatch);

        //act
        var patchedResource = await resources.PatchStatusAsync<FakeNamespacedResource>(patch, resource.GetName(), resource.GetNamespace());

        //assert
        patchedResource.Should().NotBeNull();
        patchedResource.Metadata.Generation.Should().Be(resource.Metadata.Generation);
        patchedResource.Metadata.ResourceVersion.Should().NotBe(resource.Metadata.ResourceVersion);
        patchedResource.Status.Should().BeEquivalentTo(updatedResource.Status);
        patchedResource.Spec.Should().BeEquivalentTo(updatedResource.Spec);

    }

    [Fact, Priority(24)]
    public async Task Patch_ClusterResourceStatus_Should_Work()
    {
        //arrange
        var resource = FakeClusterResource.Create();
        using var resources = await this.RepositoryBuilder
             .WithDefinition<FakeClusterResourceDefinition>()
             .WithResource(resource)
             .BuildAsync();
        resource = (await resources.GetAsync<FakeClusterResource>(resource.GetName(), resource.GetNamespace()))!;
        var updatedResource = resource.Clone()!;
        updatedResource.Status!.FakeProperty1 = "Updated Fake Value";
        updatedResource.Status!.FakeProperty2 = 6;
        var jsonPatch = JsonPatchUtility.CreateJsonPatchFromDiff(resource, updatedResource);
        var patch = new Patch(PatchType.JsonPatch, jsonPatch);

        //act
        var patchedResource = await resources.PatchStatusAsync<FakeClusterResource>(patch, resource.GetName(), resource.GetNamespace());

        //assert
        patchedResource.Should().NotBeNull();
        patchedResource.Metadata.Generation.Should().Be(resource.Metadata.Generation);
        patchedResource.Metadata.ResourceVersion.Should().NotBe(resource.Metadata.ResourceVersion);
        patchedResource.Status.Should().BeEquivalentTo(updatedResource.Status);
        patchedResource.Spec.Should().BeEquivalentTo(updatedResource.Spec);

    }

    [Fact, Priority(25)]
    public async Task Replace_NamespacedResourceStatus_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        var resource = FakeNamespacedResource.Create(@namespace);
        using var resources = await this.RepositoryBuilder
             .WithDefinition<FakeNamespacedResourceDefinition>()
             .WithResource(new Namespace(@namespace))
             .WithResource(resource)
             .BuildAsync();
        resource = (await resources.GetAsync<FakeNamespacedResource>(resource.GetName(), resource.GetNamespace()))!;
        var updatedResource = resource.Clone()!;
        updatedResource.Status!.FakeProperty1 = "Updated Fake Value";
        updatedResource.Status!.FakeProperty2 = 6;

        //act
        var patchedResource = await resources.ReplaceStatusAsync(updatedResource);

        //assert
        patchedResource.Should().NotBeNull();
        patchedResource.Metadata.Generation.Should().Be(resource.Metadata.Generation);
        patchedResource.Metadata.ResourceVersion.Should().NotBe(resource.Metadata.ResourceVersion);
        patchedResource.Status.Should().BeEquivalentTo(updatedResource.Status);
        patchedResource.Spec.Should().BeEquivalentTo(updatedResource.Spec);
    }

    [Fact, Priority(26)]
    public async Task Replace_ClusterResourceStatus_Should_Work()
    {
        //arrange
        var resource = FakeClusterResource.Create();
        using var resources = await this.RepositoryBuilder
             .WithDefinition<FakeClusterResourceDefinition>()
             .WithResource(resource)
             .BuildAsync();
        resource = (await resources.GetAsync<FakeClusterResource>(resource.GetName()))!;
        var updatedResource = resource.Clone()!;
        updatedResource.Status!.FakeProperty1 = "Updated Fake Value";
        updatedResource.Status!.FakeProperty2 = 6;

        //act
        var patchedResource = await resources.ReplaceStatusAsync(updatedResource);

        //assert
        patchedResource.Should().NotBeNull();
        patchedResource.Metadata.Generation.Should().Be(resource.Metadata.Generation);
        patchedResource.Metadata.ResourceVersion.Should().NotBe(resource.Metadata.ResourceVersion);
        patchedResource.Status.Should().BeEquivalentTo(updatedResource.Status);
        patchedResource.Spec.Should().BeEquivalentTo(updatedResource.Spec);
    }

    [Fact, Priority(27)]
    public async Task Delete_NamespacedResource_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        var resource = FakeNamespacedResource.Create(@namespace);
        using var resources = await this.RepositoryBuilder
             .WithDefinition<FakeNamespacedResourceDefinition>()
             .WithResource(new Namespace(@namespace))
             .WithResource(resource)
             .BuildAsync();

        //act
        var deletedResource = await resources.RemoveAsync<FakeNamespacedResource>(resource.GetName(), resource.GetNamespace());

        //assert
        deletedResource.Should().NotBeNull();
        (await resources.GetAsync<FakeNamespacedResource>(resource.GetName(), resource.GetNamespace())).Should().BeNull();
    }

    [Fact, Priority(28)]
    public async Task Delete_ClusterResource_Should_Work()
    {
        //arrange
        var resource = FakeClusterResource.Create();
        using var resources = await this.RepositoryBuilder
             .WithDefinition<FakeClusterResourceDefinition>()
             .WithResource(resource)
             .BuildAsync();

        //act
        var deletedResource = await resources.RemoveAsync<FakeClusterResource>(resource.GetName());

        //assert
        deletedResource.Should().NotBeNull();
        (await resources.GetAsync<FakeClusterResource>(resource.GetName())).Should().BeNull();
    }

    [Fact, Priority(29)]
    public async Task Monitor_NamespacedResource_Should_Work()
    {
        //arrange
        var @namespace = FakeNamespaceName;
        using var resourceRepository = await this.RepositoryBuilder
             .WithDefinition<FakeNamespacedResourceDefinition>()
             .WithResource(new Namespace(@namespace))
             .BuildAsync();
        long updateCount = 0;
        var resource = await resourceRepository.AddAsync(FakeNamespacedResource.Create(@namespace));
        var updatedResource1 = resource with { Spec = resource.Spec with { FakeProperty1 = AutoFaker.Generate<string>() } };
        var updatedResource2 = updatedResource1 with { Spec = resource.Spec with { FakeProperty2 = AutoFaker.Generate<long>() } };

        //act
        await using var monitor = await resourceRepository.MonitorAsync<FakeNamespacedResource>(resource.GetName(), @namespace);
        monitor.Subscribe(_ => Interlocked.Increment(ref updateCount));
        var storedResource1 = await resourceRepository.ReplaceAsync(updatedResource1);
        var storedResource2 = await resourceRepository.ReplaceAsync(updatedResource2 with { Metadata = updatedResource2.Metadata with { ResourceVersion = storedResource1.Metadata.ResourceVersion } });
        await Task.Delay(10);

        //assert
        updateCount.Should().Be(Interlocked.Read(ref updateCount));
        storedResource1.Spec.Should().Be(updatedResource1.Spec);
        storedResource2.Spec.Should().Be(updatedResource2.Spec);
    }

    [Fact, Priority(30)]
    public async Task Monitor_ClusterResource_Should_Work()
    {
        //arrange
        using var resourceRepository = await this.RepositoryBuilder
             .WithDefinition<FakeClusterResourceDefinition>()
             .BuildAsync();
        long updateCount = 0;
        var resource = await resourceRepository.AddAsync(FakeClusterResource.Create());
        var updatedResource1 = resource with { Spec = resource.Spec with { FakeProperty1 = AutoFaker.Generate<string>() } };
        var updatedResource2 = updatedResource1 with { Spec = resource.Spec with { FakeProperty2 = AutoFaker.Generate<long>() } };

        //act
        await using var monitor = await resourceRepository.MonitorAsync<FakeClusterResource>(resource.GetName());
        monitor.Subscribe(_ => Interlocked.Increment(ref updateCount));
        var storedResource1 = await resourceRepository.ReplaceAsync(updatedResource1);
        var storedResource2 = await resourceRepository.ReplaceAsync(updatedResource2 with { Metadata = updatedResource2.Metadata with { ResourceVersion = storedResource1.Metadata.ResourceVersion } });
        await Task.Delay(10);

        //assert
        updateCount.Should().Be(Interlocked.Read(ref updateCount));
        storedResource1.Spec.Should().Be(updatedResource1.Spec);
        storedResource2.Spec.Should().Be(updatedResource2.Spec);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this.RepositoryBuilder.Dispose();
            }
            this._disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}
