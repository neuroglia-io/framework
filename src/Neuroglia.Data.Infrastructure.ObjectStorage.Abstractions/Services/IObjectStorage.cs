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

namespace Neuroglia.Data.Infrastructure.ObjectStorage.Services;

/// <summary>
/// Defines the fundamentals of a storage that manages data as objects
/// </summary>
public interface IObjectStorage
{

    /// <summary>
    /// Creates a new bucket
    /// </summary>
    /// <param name="name">The name of the bucket to create</param>
    /// <param name="tags">A name/value mapping of the bucket's tags, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IBucketDescriptor"/></returns>
    Task<IBucketDescriptor> CreateBucketAsync(string name, IDictionary<string, string>? tags = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether or not the <see cref="IObjectStorage"/> contains the specified bucket
    /// </summary>
    /// <param name="name">The name of the bucket to check</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A boolean indicating whether or not the <see cref="IObjectStorage"/> contains the specified bucket</returns>
    Task<bool> ContainsBucketAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all buckets contained by the <see cref="IObjectStorage"/>
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/>, used to asynchronously enumerate <see cref="IBucketDescriptor"/>s</returns>
    IAsyncEnumerable<IBucketDescriptor> ListBucketsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the bucket with the specified name
    /// </summary>
    /// <param name="name">The name of the bucket to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IBucketDescriptor"/></returns>
    Task<IBucketDescriptor> GetBucketAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the tags of the specified bucket
    /// </summary>
    /// <param name="name">The name of the bucket to set the tags of</param>
    /// <param name="tags">A name/value mapping of the bucket's tags</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task SetBucketTagsAsync(string name, IDictionary<string, string> tags, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the tags of the specified bucket
    /// </summary>
    /// <param name="name">The name of the bucket to remove tags from</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task RemoveBucketTagsAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the specified bucket
    /// </summary>
    /// <param name="name">The name of the bucket to remove</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task RemoveBucketAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new object into the specified bucket
    /// </summary>
    /// <param name="bucketName">The name of the bucket to add the object to</param>
    /// <param name="name">The object's name</param>
    /// <param name="contentType">The object's content type</param>
    /// <param name="stream">The <see cref="Stream"/> that contains the object's data</param>
    /// <param name="size">The size of the object to upload. If not set, defaults to the object's <see cref="Stream"/>'s length</param>
    /// <param name="tags">A name/value mapping of the object's tags, if any</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IObjectDescriptor"/></returns>
    Task<IObjectDescriptor> PutObjectAsync(string bucketName, string name, string contentType, Stream stream, ulong? size = null, IDictionary<string, string>? tags = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether or not the specified object exists
    /// </summary>
    /// <param name="bucketName">The name of the bucket the object to check belongs to</param>
    /// <param name="name">The object to check</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A boolean indicating whether or not the specified object exists</returns>
    Task<bool> ContainsObjectAsync(string bucketName, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all buckets contained by the specified bucket
    /// </summary>
    /// <param name="bucketName">The name of the bucket to list the objects of</param>
    /// <param name="prefix">The prefix, if any, of the objects to list</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/>, used to asynchronously enumerate <see cref="IBucketDescriptor"/>s</returns>
    IAsyncEnumerable<IObjectDescriptor> ListObjectsAsync(string bucketName, string? prefix = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the specified object
    /// </summary>
    /// <param name="bucketName">The name of the bucket to object to get belongs to</param>
    /// <param name="name">The name of the object to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IObjectDescriptor"/>, used to describe the specified object</returns>
    Task<IObjectDescriptor> GetObjectAsync(string bucketName, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads the specified object
    /// </summary>
    /// <param name="bucketName">The name of the bucket the object to read belongs to</param>
    /// <param name="name">The name of the object to read</param>
    /// <param name="stream">The <see cref="Stream"/> to read the object's data to</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task ReadObjectAsync(string bucketName, string name, Stream stream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the tags of the specified object
    /// </summary>
    /// <param name="bucketName">The name of the bucket the object to tag belongs to</param>
    /// <param name="name">The name of the object to set the tags of</param>
    /// <param name="tags">A name/value mapping of the object's tags</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task SetObjectTagsAsync(string bucketName, string name, IDictionary<string, string> tags, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the tags of the specified object
    /// </summary>
    /// <param name="bucketName">The name of the bucket the object to remove the tags of belongs to</param>
    /// <param name="name">The name of the object to remove tags from</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task RemoveObjectTagsAsync(string bucketName, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the specified object
    /// </summary>
    /// <param name="bucketName">The name of the bucket the object to remove belongs to</param>
    /// <param name="name">The name of the object to remove</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task RemoveObjectAsync(string bucketName, string name, CancellationToken cancellationToken = default);

}
