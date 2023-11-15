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

namespace Neuroglia.Data.Infrastructure.ObjectStorage;

/// <summary>
/// Represents the default implementation of the <see cref="IBucketDescriptor"/> interface
/// </summary>
public class BucketDescriptor
    : IBucketDescriptor
{

    /// <summary>
    /// Initializes a new <see cref="BucketDescriptor"/>
    /// </summary>
    protected BucketDescriptor() { }

    /// <summary>
    /// Initializes a new <see cref="BucketDescriptor"/>
    /// </summary>
    /// <param name="createdAt">The date and time the bucket has been created at</param>
    /// <param name="name">The bucket's name</param>
    /// <param name="tags">The bucket's tags, if any</param>
    public BucketDescriptor(DateTimeOffset? createdAt, string name, IDictionary<string, string>? tags = null)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        this.CreatedAt = createdAt;
        this.Name = name;
        this.Tags = tags;
    }

    /// <summary>
    /// Gets the date and time the bucket has been created at
    /// </summary>
    public virtual DateTimeOffset? CreatedAt { get; protected set; }

    /// <inheritdoc/>
    public virtual string Name { get; protected set; } = null!;

    /// <inheritdoc/>
    public virtual IDictionary<string, string>? Tags { get; protected set; }

}