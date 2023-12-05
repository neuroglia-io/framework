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

using Json.Schema;
using Json.Schema.Generation;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Schemas.Json;
using Neuroglia.Eventing.CloudEvents;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Data.Events;

namespace Neuroglia.UnitTests.Cases.Data.Schemas.Json;

public class JsonSchemaResolverTests
    : IAsyncLifetime
{

    public JsonSchemaResolverTests()
    {
        var services = new ServiceCollection();
        services.AddJsonSerializer();
        services.AddHttpClient();
        this.ServiceProvider = services.BuildServiceProvider();
        this.Resolver = ActivatorUtilities.CreateInstance<JsonSchemaResolver>(this.ServiceProvider);
    }

    protected ServiceProvider ServiceProvider { get; }

    protected IJsonSchemaResolver Resolver { get; }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await this.ServiceProvider.DisposeAsync();

    [Fact]
    public async Task Resolve_Schema_With_AllOf_Should_Work()
    {
        //arrang
        using var httpClient = new HttpClient();
        var allOfSchema = JsonSchema.FromText(await httpClient.GetStringAsync(CloudEventSpecVersion.V1.SchemaUri));
        var schema = new JsonSchemaBuilder()
            .Type(SchemaValueType.Object)
            .AllOf(allOfSchema)
            .Build();

        //act
        var resolved = await this.Resolver.ResolveSchemaAsync(schema);

        //assert
        resolved.Should().NotBeNull();
        resolved.GetKeyword<AllOfKeyword>().Should().BeNull();
        resolved.GetKeyword<PropertiesKeyword>().Should().NotBeNull();
        resolved.GetProperties().Should().HaveCount(10);
    }

    [Fact]
    public async Task Resolve_Schema_With_AllOf_ByExternalRef_Should_Work()
    {
        //arrange
        var schema = new JsonSchemaBuilder()
            .Type(SchemaValueType.Object)
            .AllOf(new JsonSchemaBuilder().Ref(CloudEventSpecVersion.V1.SchemaUri).Build())
            .Build();

        //act
        var resolved = await this.Resolver.ResolveSchemaAsync(schema);

        //assert
        resolved.Should().NotBeNull();
        resolved.GetKeyword<AllOfKeyword>().Should().BeNull();
        resolved.GetKeyword<PropertiesKeyword>().Should().NotBeNull();
        resolved.GetProperties().Should().HaveCount(10);
    }

    [Fact]
    public async Task Resolve_Schema_With_AllOf_ByInternalRef_Should_Work()
    {
        //arrange
        var refSchemaName = "fakeSchemaName";
        var refSchema = new JsonSchemaBuilder().FromType<UserCreatedEvent>(JsonSchemaGeneratorConfiguration.Default).Build();
        var schema = new JsonSchemaBuilder()
            .Type(SchemaValueType.Object)
            .Ref($"#/$defs/{refSchemaName}")
            .Defs((refSchemaName, refSchema))
            .Build();

        //act
        var resolved = await this.Resolver.ResolveSchemaAsync(schema);

        //assert
        resolved.Should().NotBeNull();
        resolved.GetKeyword<PropertiesKeyword>().Should().NotBeNull();
        resolved.GetProperties().Should().NotBeNull();
        resolved!.GetProperties().Should().HaveCount(refSchema.GetProperties()?.Count ?? 0);
    }

    [Fact]
    public async Task Resolve_Schema_With_InternalRef_Should_Work()
    {
        //arrange
        var propertyName = "fake";
        var refSchemaName = "fakeSchemaName";
        var refSchema = new JsonSchemaBuilder().FromType<UserCreatedEvent>(JsonSchemaGeneratorConfiguration.Default).Build();
        var schema = new JsonSchemaBuilder()
            .Type(SchemaValueType.Object)
            .Properties((propertyName, new JsonSchemaBuilder().Ref($"#/$defs/{refSchemaName}")))
            .Defs((refSchemaName, refSchema))
            .Build();

        //act
        var resolved = await this.Resolver.ResolveSchemaAsync(schema);

        //assert
        resolved.Should().NotBeNull();
        resolved.GetKeyword<PropertiesKeyword>().Should().NotBeNull();
        resolved.GetProperties().Should().NotBeNull();
        resolved.GetProperties()!.TryGetValue(propertyName, out var propertySchema).Should().BeTrue();
        propertySchema.Should().NotBeNull();
        propertySchema!.GetProperties().Should().NotBeNull();
        propertySchema!.GetProperties().Should().HaveCount(refSchema.GetProperties()?.Count ?? 0);
    }

    [Fact]
    public async Task Resolve_Schema_With_ExternalRef_Should_Work()
    {
        //arrange
        var propertyName = "event";
        var schema = new JsonSchemaBuilder()
           .Type(SchemaValueType.Object)
           .Properties((propertyName, new JsonSchemaBuilder().Ref(CloudEventSpecVersion.V1.SchemaUri).Build()))
           .Build();

        //act
        var resolved = await this.Resolver.ResolveSchemaAsync(schema);

        //assert
        resolved.Should().NotBeNull();
        resolved.GetKeyword<PropertiesKeyword>().Should().NotBeNull();
        resolved.GetProperties().Should().NotBeNull();
        resolved.GetProperties()!.TryGetValue(propertyName, out var propertySchema).Should().BeTrue();
        propertySchema.Should().NotBeNull();
        propertySchema!.GetKeyword<RefKeyword>().Should().BeNull();
        propertySchema.GetProperties().Should().NotBeNull();
        propertySchema.GetProperties().Should().HaveCount(10);
    }

}
