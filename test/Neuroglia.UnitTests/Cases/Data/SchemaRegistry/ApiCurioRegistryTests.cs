using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.SchemaRegistry;
using Neuroglia.Data.SchemaRegistry.Models;
using Neuroglia.Data.SchemaRegistry.Services;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Containers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Data.SchemaRegistry
{

    [TestCaseOrderer("Xunit.PriorityTestCaseOrderer", "Neuroglia.Xunit")]
    public class ApiCurioRegistryTests
        : IDisposable
    {

        public ApiCurioRegistryTests()
        {
            var services = new ServiceCollection();
            services.AddNewtonsoftJsonSerializer();
            services.AddApiCurioRegistryClient(options =>
            {
                options.ServerUri = ApiCurioRegistryContainerBuilder.Build().ServiceUri;
            });
            this.ServiceScope = services.BuildServiceProvider().CreateScope();
            this.ApiCurioRegistryClient = this.ServiceScope.ServiceProvider.GetService<ApiCurioRegistryClient>();
        }

        IServiceScope ServiceScope { get; }

        IApiCurioRegistryClient ApiCurioRegistryClient { get; }

        static Artifact Artifact;

        static string ArtifactContent;

        [Fact, Priority(1)]
        public async Task CreateArtifact()
        {
            //arrange
            var artifactType = ArtifactType.OPENAPI;
            var content = File.ReadAllText(Path.Combine("Assets", "openapi.json"));
            var ifExistsAction = IfArtifactExistsAction.Fail;
            var id = "fake-artifact-id";
            var groupId = "fake-group-id";
            var version = "1.0.0";
            var name = "fake-artifact-name";
            var description = "fake-artifact-description";

            //act
            var artifact = await this.ApiCurioRegistryClient.CreateArtifactAsync(artifactType, content, ifExistsAction, id, groupId, false, version, name, description);

            //assert
            artifact.Should().NotBeNull();
            artifact.Type.Should().Be(artifactType);
            artifact.Id.Should().Be(id);
            artifact.GroupId.Should().Be(groupId);
            artifact.Version.Should().Be(version);
            artifact.Name.Should().Be(name);
            artifact.Description.Should().Be(description);

            Artifact = artifact;
            ArtifactContent = content;
            await this.ApiCurioRegistryClient.CreateArtifactAsync(ArtifactType.OPENAPI, File.ReadAllText(Path.Combine("Assets", "openapi.json")), IfArtifactExistsAction.Fail, "default-artifact", "fake-group-id");
        }

        [Fact, Priority(2)]
        public async Task CreateArtifactVersion()
        {
            //arrange
            var id = Artifact.Id;
            var groupId = Artifact.GroupId;
            var content = ArtifactContent;
            var version = "1.0.1";
            var name = "fake-artifact-version-name";
            var description = "fake-artifact-version-description";

            //act
            var artifact = await this.ApiCurioRegistryClient.CreateArtifactVersionAsync(id, groupId, content, version, name, description);

            //assert
            artifact.Should().NotBeNull();
            artifact.Id.Should().Be(id);
            artifact.GroupId.Should().Be(groupId);
            artifact.Version.Should().Be(version);
            artifact.Name.Should().Be(name);
            artifact.Description.Should().Be(description);
        }

        [Fact, Priority(3)]
        public async Task GetLatestArtifact()
        {
            //arrange
            var id = Artifact.Id;
            var groupId = Artifact.GroupId;
            var originalContent = ArtifactContent;

            //act
            var content = await this.ApiCurioRegistryClient.GetLatestArtifactAsync(id, groupId);

            //assert
            content.Should().Be(originalContent);
        }

        [Fact, Priority(4)]
        public async Task GetArtifactContentById()
        {
            //arrange
            var contentId = Artifact.ContentId;
            var originalContent = ArtifactContent;

            //act
            var content = await this.ApiCurioRegistryClient.GetArtifactContentByIdAsync(contentId);

            //assert
            content.Should().Be(originalContent);
        }

        [Fact, Priority(5)]
        public async Task GetArtifactContentByGlobalId()
        {
            //arrange
            var globalId = Artifact.GlobalId;
            var groupId = Artifact.GroupId;
            var originalContent = ArtifactContent;

            //act
            var content = await this.ApiCurioRegistryClient.GetArtifactContentByGlobalIdAsync(globalId);

            //assert
            content.Should().Be(originalContent);
        }

        [Fact, Priority(6)]
        public async Task GetArtifactContentBySha256Hash()
        {
            //arrange
            var originalContent = ArtifactContent;
            var contentHash = HashHelper.SHA256Hash(originalContent);

            //act
            var content = await this.ApiCurioRegistryClient.GetArtifactContentBySha256HashAsync(contentHash);

            //assert
            content.Should().Be(originalContent);
        }

        [Fact, Priority(7)]
        public async Task GetArtifactVersionMetadata()
        {
            //arrange
            var id = Artifact.Id;
            var groupId = Artifact.GroupId;
            var version = Artifact.Version;

            //act
            var artifact = await this.ApiCurioRegistryClient.GetArtifactVersionMetadataAsync(id, groupId, version);

            //assert
            artifact.Should().NotBeNull();
        }

        [Fact, Priority(8)]
        public async Task GetArtifactVersionMetadataByContent()
        {
            //arrange
            var id = Artifact.Id;
            var groupId = Artifact.GroupId;
            var content = ArtifactContent;

            //act
            var artifact = await this.ApiCurioRegistryClient.GetArtifactVersionMetadataByContentAsync(id, groupId, content);

            //assert
            artifact.Should().NotBeNull();
        }

        [Fact, Priority(9)]
        public async Task SearchForArtifacts()
        {
            //arrange
            var query = new SearchForArtifactsQuery()
            {
                Name = Artifact.Name
            };

            //act
            var result = await this.ApiCurioRegistryClient.SearchForArtifactsAsync(query);

            //assert
            result.Should().NotBeNull();
            result.Artifacts.Should().NotBeNullOrEmpty();
        }

        [Fact, Priority(10)]
        public async Task SearchForArtifactsByContent()
        {
            //arrange
            var query = new SearchForArtifactsByContentQuery();
            var content = ArtifactContent;

            //act
            var result = await this.ApiCurioRegistryClient.SearchForArtifactsByContentAsync(content, query);

            //assert
            result.Should().NotBeNull();
            result.Artifacts.Should().NotBeNullOrEmpty();
        }

        [Fact, Priority(11)]
        public async Task ListArtifactsInGroup()
        {
            //arrange
            var groupId = Artifact.GroupId;

            //act
            var result = await this.ApiCurioRegistryClient.ListArtifactsInGroupAsync(groupId);

            //assert
            result.Should().NotBeNull();
            result.Artifacts.Should().NotBeNullOrEmpty();
        }

        [Fact, Priority(12)]
        public async Task UpdateArtifactState()
        {
            //arrange
            var id = Artifact.Id;
            var groupId = Artifact.GroupId;
            var state = ArtifactState.Deprecated;

            //act
            await this.ApiCurioRegistryClient.UpdateArtifactStateAsync(id, state, groupId);
            var artifact = await this.ApiCurioRegistryClient.GetArtifactMetadataAsync(id, groupId);

            //assert
            artifact.State.Should().Be(state);
        }

        [Fact, Priority(13)]
        public async Task UpdateArtifact()
        {
            //arrange
            var id = Artifact.Id;
            var groupId = Artifact.GroupId;
            var content = ArtifactContent;
            var version = "1.0.2";
            var name = "new-fake-artifact-name";
            var description = "new-fake-artifact-description";

            //act
            var artifact = await this.ApiCurioRegistryClient.UpdateArtifactAsync(id, content, groupId, version, name, description);
            var artifactContent = await this.ApiCurioRegistryClient.GetArtifactContentByIdAsync(artifact.ContentId);

            //assert
            artifact.Should().NotBeNull();
            artifactContent.Should().Be(content);
            artifact.Version.Should().Be(version);
            artifact.Name.Should().Be(name);
            artifact.Description.Should().Be(description);
        }

        [Fact, Priority(14)]
        public async Task UpdateArtifactMetadata()
        {
            //arrange
            var id = Artifact.Id;
            var groupId = Artifact.GroupId;
            var name = "updated-fake-artifact-name";
            var description = "updated-fake-artifact-description";
            var labels = new List<string>() { "fake-label-1", "fake-label-2" };
            var properties = new Dictionary<string, string>() { { "fake-property", "fake-property-value" } };

            //act
            await this.ApiCurioRegistryClient.UpdateArtifactMetadataAsync(id, groupId, name, description, labels, properties);
            var artifact = await this.ApiCurioRegistryClient.GetArtifactMetadataAsync(id, groupId);

            //assert
            artifact.Should().NotBeNull();
            artifact.Name.Should().Be(name);
            artifact.Description.Should().Be(description);
            artifact.Labels.Should().BeEquivalentTo(labels);
            artifact.Properties.Should().BeEquivalentTo(properties);
        }

        [Fact, Priority(15)]
        public async Task UpdateArtifactVersionMetadata()
        {
            //arrange
            var id = Artifact.Id;
            var groupId = Artifact.GroupId;
            var version = Artifact.Version;
            var name = "new-updated-fake-artifact-name";
            var description = "new-updated-fake-artifact-description";
            var labels = new List<string>() { "new-fake-label-1", "new-fake-label-2" };
            var properties = new Dictionary<string, string>() { { "new-fake-property", "new-fake-property-value" } };

            //act
            await this.ApiCurioRegistryClient.UpdateArtifactVersionMetadataAsync(id, groupId, version, name, description, labels, properties);
            var artifact = await this.ApiCurioRegistryClient.GetArtifactVersionMetadataAsync(id, groupId, version);

            //assert
            artifact.Should().NotBeNull();
            artifact.Name.Should().Be(name);
            artifact.Description.Should().Be(description);
            artifact.Labels.Should().BeEquivalentTo(labels);
            artifact.Properties.Should().BeEquivalentTo(properties);
        }

        [Fact, Priority(16)]
        public async Task DeleteArtifactVersionMetadata()
        {
            //arrange
            var id = Artifact.Id;
            var groupId = Artifact.GroupId;
            var version = Artifact.Version;

            //act
            await this.ApiCurioRegistryClient.DeleteArtifactVersionMetadataAsync(id, groupId, version);
            var artifact = await this.ApiCurioRegistryClient.GetArtifactVersionMetadataAsync(id, groupId, version);

            //assert
            artifact.Name.Should().BeNullOrWhiteSpace();
            artifact.Description.Should().BeNullOrWhiteSpace();
            artifact.Labels.Should().BeNullOrEmpty();
            artifact.Properties.Should().BeNullOrEmpty();
        }

        [Fact, Priority(17)]
        public async Task DeleteArtifact()
        {
            //arrange
            var id = Artifact.Id;
            var groupId = Artifact.GroupId;

            //act
            await this.ApiCurioRegistryClient.DeleteArtifactAsync(id, groupId);

            //assert
            var action = async () => await this.ApiCurioRegistryClient.GetArtifactMetadataAsync(id, groupId);
            await Assert.ThrowsAnyAsync<HttpRequestException>(action);
        }

        [Fact, Priority(18)]
        public async Task DeleteAllArtifactsInGroup()
        {
            //arrange
            var groupId = Artifact.GroupId;

            //act
            await this.ApiCurioRegistryClient.DeleteAllArtifactsInGroupAsync(groupId);

            //assert
            (await this.ApiCurioRegistryClient.ListArtifactsInGroupAsync(groupId)).Artifacts.Should().BeEmpty();
        }

        public void Dispose()
        {
            this.ServiceScope.Dispose();
        }

    }

}
