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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neuroglia.Data.Infrastructure.ObjectStorage.Services;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.ObjectStorage;

public abstract class ObjectStorageTestsBase
    : IAsyncLifetime
{

    protected ObjectStorageTestsBase(IServiceCollection services) { this.ServiceProvider = services.BuildServiceProvider(); }

    protected ServiceProvider ServiceProvider { get; }

    protected CancellationTokenSource CancellationTokenSource { get; } = new();

    protected virtual IObjectStorage ObjectStorage { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        foreach (var hostedService in this.ServiceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(CancellationTokenSource.Token).ConfigureAwait(false);
        }
        this.ObjectStorage = this.ServiceProvider.GetRequiredService<IObjectStorage>();
    }

    public async Task DisposeAsync() => await ServiceProvider.DisposeAsync().ConfigureAwait(false);

    [Fact]
    public async Task CreateBucket_Should_Work()
    {
        //arrange
        var bucketName = "fake-bucket";

        //act
        var bucket = await this.ObjectStorage.CreateBucketAsync(bucketName);

        //assert
        bucket.Should().NotBeNull();
        bucket.Name.Should().Be(bucketName);
    }

    [Fact]
    public async Task ContainsBucket_Should_Work()
    {
        //arrange
        var bucketName = "fake-bucket";
        await this.ObjectStorage.CreateBucketAsync(bucketName);

        //act
        var bucketExists = await this.ObjectStorage.ContainsBucketAsync(bucketName);

        //assert
        bucketExists.Should().BeTrue();
    }

    [Fact]
    public async Task ListBuckets_Should_Work()
    {
        //arrange
        var bucketNames = new string[] { "bucket-1", "bucket-2", "bucket-3" };
        foreach(var bucketName in bucketNames) await this.ObjectStorage.CreateBucketAsync(bucketName);

        //act
        var buckets = await (this.ObjectStorage.ListBucketsAsync()).ToListAsync();

        //assert
        buckets.Should().NotBeNullOrEmpty();
        buckets.Should().HaveSameCount(bucketNames);
        bucketNames.Should().AllSatisfy(n => buckets.Any(b => b.Name == n));
    }

    [Fact]
    public async Task GetBucket_Should_Work()
    {
        //arrange
        var bucketName = "fake-bucket";
        await this.ObjectStorage.CreateBucketAsync(bucketName);

        //act
        var bucket = await this.ObjectStorage.GetBucketAsync(bucketName);

        //assert
        bucket.Should().NotBeNull();
        bucket.Name.Should().Be(bucketName);
    }

    [Fact]
    public async Task SetBucketTags_Should_Work()
    {
        //arrange
        var bucketName = "fake-bucket";
        await this.ObjectStorage.CreateBucketAsync(bucketName);
        var tags = new Dictionary<string, string>() 
        {
            { "tag-1", "tag-1 value" },
            { "Tag-2", "tag-2 value" },
            { "tag-3", "Tag-3 value" }
        };

        //act
        await this.ObjectStorage.SetBucketTagsAsync(bucketName, tags);
        var bucket = await this.ObjectStorage.GetBucketAsync(bucketName);

        //assert
        bucket.Tags.Should().BeEquivalentTo(tags);
    }

    [Fact]
    public async Task RemoveBucketTags_Should_Work()
    {
        //arrange
        var bucketName = "fake-bucket";
        await this.ObjectStorage.CreateBucketAsync(bucketName);
        await this.ObjectStorage.SetBucketTagsAsync(bucketName, new Dictionary<string, string>()
        {
            { "tag-1", "tag-1 value" },
            { "Tag-2", "tag-2 value" },
            { "tag-3", "Tag-3 value" }
        });

        //act
        await this.ObjectStorage.RemoveBucketTagsAsync(bucketName);
        var bucket = await this.ObjectStorage.GetBucketAsync(bucketName);

        //assert
        bucket.Tags.Should().BeNullOrEmpty();
    }

}
