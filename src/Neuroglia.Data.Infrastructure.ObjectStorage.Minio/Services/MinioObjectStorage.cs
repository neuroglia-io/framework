using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Tags;
using Minio.Exceptions;
using Neuroglia.Data.Infrastructure.ObjectStorage.Services;
using System.Runtime.CompilerServices;

namespace Neuroglia.Data.Infrastructure.ObjectStorage.Minio.Services;

/// <summary>
/// Represents a <see href="https://min.io/">Minio</see> implementation of the <see cref="IObjectStorage"/> interface
/// </summary>
public class MinioObjectStorage
    : IObjectStorage
{

    /// <summary>
    /// Initializes a new <see cref="MinioObjectStorage"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="minioClient">The service used to interact with Minio</param>
    public MinioObjectStorage(ILoggerFactory loggerFactory, IMinioClient minioClient)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.MinioClient = minioClient;
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the service used to interact with Minio
    /// </summary>
    protected IMinioClient MinioClient { get; }

    /// <inheritdoc/>
    public virtual async Task<IBucketDescriptor> CreateBucketAsync(string name, IDictionary<string, string>? tags = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (await this.ContainsBucketAsync(name, cancellationToken).ConfigureAwait(false)) throw new ArgumentException($"The bucket with the specified name '{name}' already exists", nameof(name));

        try
        {
            await this.MinioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(name), cancellationToken).ConfigureAwait(false);
            if (tags != null) await this.SetBucketTagsAsync(name, tags, cancellationToken).ConfigureAwait(false);
            return new BucketDescriptor(DateTimeOffset.Now, name, tags);
        }
        catch (MinioException ex)
        {
            this.Logger.LogError("An error occured while creating the bucket with name '{bucket}': {ex}", name, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual Task<bool> ContainsBucketAsync(string name, CancellationToken cancellationToken = default) => this.MinioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(name), cancellationToken);

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<IBucketDescriptor> ListBucketsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        IEnumerable<Bucket> buckets;
        try
        {
            var response = await this.MinioClient.ListBucketsAsync(cancellationToken).ConfigureAwait(false);
            buckets = response.Buckets;
        }
        catch (MinioException ex)
        {
            this.Logger.LogError("An error occured while lising buckets: {ex}", ex);
            throw;
        }
        foreach(var bucket in buckets)
        {
            Tagging? tagging = null;
            try { tagging = await this.MinioClient.GetBucketTagsAsync(new GetBucketTagsArgs().WithBucket(bucket.Name), cancellationToken).ConfigureAwait(false); }
            catch { }
            yield return new BucketDescriptor(bucket.CreationDateDateTime, bucket.Name, tagging?.Tags);
        }
    }

    /// <inheritdoc/>
    public virtual async Task<IBucketDescriptor> GetBucketAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (!await this.ContainsBucketAsync(name, cancellationToken).ConfigureAwait(false)) throw new NullReferenceException($"Failed to find a bucket with the specified name '{name}'");

        Tagging? tagging = null!;
        try { tagging = await this.MinioClient.GetBucketTagsAsync(new GetBucketTagsArgs().WithBucket(name), cancellationToken).ConfigureAwait(false); }
        catch { }

        return new BucketDescriptor(null, name, tagging?.Tags);
    }

    /// <inheritdoc/>
    public virtual Task SetBucketTagsAsync(string name, IDictionary<string, string> tags, CancellationToken cancellationToken = default) => this.MinioClient.SetBucketTagsAsync(new SetBucketTagsArgs().WithBucket(name).WithTagging(Tagging.GetBucketTags(tags)), cancellationToken);

    /// <inheritdoc/>
    public virtual Task RemoveBucketTagsAsync(string name, CancellationToken cancellationToken = default) => this.MinioClient.RemoveBucketTagsAsync(new RemoveBucketTagsArgs().WithBucket(name), cancellationToken);

    /// <inheritdoc/>
    public virtual Task RemoveBucketAsync(string name, CancellationToken cancellationToken = default) => this.MinioClient.RemoveBucketAsync(new RemoveBucketArgs().WithBucket(name), cancellationToken);

    /// <inheritdoc/>
    public virtual async Task<IObjectDescriptor> PutObjectAsync(string bucketName, string name, string contentType, Stream stream, ulong? size = null, IDictionary<string, string>? tags = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bucketName)) throw new ArgumentNullException(nameof(bucketName));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(contentType)) throw new ArgumentNullException(nameof(contentType));
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (!size.HasValue) size = (ulong)stream.Length;

        try
        {
            var args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(name)
                .WithContentType(contentType)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length);
            var obj = await this.MinioClient.PutObjectAsync(args, cancellationToken).ConfigureAwait(false);
            if (tags != null) await this.SetObjectTagsAsync(bucketName, name, tags, cancellationToken).ConfigureAwait(false);
            return new ObjectDescriptor(DateTimeOffset.Now, bucketName, name, contentType, size.Value, obj.Etag, tags);
        }
        catch (MinioException ex)
        {
            this.Logger.LogError("An error occured while putting the object with name '{object}' in bucket with name '{bucket}': {ex}", name, bucketName, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<bool> ContainsObjectAsync(string bucketName, string name, CancellationToken cancellationToken = default)
    {
        try
        {
            var obj = await this.MinioClient.StatObjectAsync(new StatObjectArgs().WithBucket(bucketName).WithObject(name)).ConfigureAwait(false);
            return obj != null;
        }
        catch(ObjectNotFoundException)
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<IObjectDescriptor> ListObjectsAsync(string bucketName, string? prefix = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bucketName)) throw new ArgumentNullException(nameof(bucketName));
        IAsyncEnumerable<Item> items;
        try
        {
            var args = new ListObjectsArgs()
                .WithBucket(bucketName)
                .WithRecursive(true);
            if (!string.IsNullOrWhiteSpace(prefix)) args = args.WithPrefix(prefix);
            items = this.MinioClient.ListObjectsAsync(args, cancellationToken).ToAsyncEnumerable();
        }
        catch (MinioException ex)
        {
            this.Logger.LogError("An error occured while lising buckets: {ex}", ex);
            throw;
        }
        await foreach (var item in items.Where(i => !i.IsDir))
        {
            var tagging = await this.MinioClient.GetObjectTagsAsync(new GetObjectTagsArgs().WithBucket(bucketName).WithObject(item.Key), cancellationToken).ConfigureAwait(false);
            var objectInfo = await this.MinioClient.StatObjectAsync(new StatObjectArgs().WithBucket(bucketName).WithObject(item.Key), cancellationToken).ConfigureAwait(false);
            yield return new ObjectDescriptor(item.LastModifiedDateTime, bucketName, item.Key, objectInfo.ContentType, item.Size, item.ETag, tagging.Tags);
        }
    }

    /// <inheritdoc/>
    public virtual async Task<IObjectDescriptor> GetObjectAsync(string bucketName, string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bucketName)) throw new ArgumentNullException(nameof(bucketName));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        try
        {
            var objectInfo = await this.MinioClient.StatObjectAsync(new StatObjectArgs().WithBucket(bucketName).WithObject(name), cancellationToken).ConfigureAwait(false);
            var tagging = await this.MinioClient.GetObjectTagsAsync(new GetObjectTagsArgs().WithBucket(bucketName).WithObject(name), cancellationToken).ConfigureAwait(false);
            return new ObjectDescriptor(objectInfo.LastModified, bucketName, name, objectInfo.ContentType, (ulong)objectInfo.Size, objectInfo.ETag, tagging?.Tags);
        }
        catch (MinioException ex)
        {
            this.Logger.LogError("An error occured while creating the bucket with name '{bucket}': {ex}", name, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task ReadObjectAsync(string bucketName, string name, Stream stream, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bucketName)) throw new ArgumentNullException(nameof(bucketName));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var obj = await this.GetObjectAsync(bucketName, name, cancellationToken).ConfigureAwait(false);
        var args = new GetObjectArgs()
            .WithObject(name)
            .WithBucket(bucketName)
            .WithCallbackStream((source, token) => source.CopyToAsync(stream, (int)obj.Size, token));
        await this.MinioClient.GetObjectAsync(args, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual Task SetObjectTagsAsync(string bucketName, string name, IDictionary<string, string> tags, CancellationToken cancellationToken = default) => this.MinioClient.SetObjectTagsAsync(new SetObjectTagsArgs().WithBucket(bucketName).WithObject(name).WithTagging(Tagging.GetBucketTags(tags)), cancellationToken);

    /// <inheritdoc/>
    public virtual Task RemoveObjectTagsAsync(string bucketName, string name, CancellationToken cancellationToken = default) => this.MinioClient.RemoveObjectTagsAsync(new RemoveObjectTagsArgs().WithBucket(bucketName).WithObject(name), cancellationToken);

    /// <inheritdoc/>
    public virtual Task RemoveObjectAsync(string bucketName, string name, CancellationToken cancellationToken = default) => this.MinioClient.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(bucketName).WithObject(name), cancellationToken);

}