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

using Neuroglia.Data.Infrastructure.ObjectStorage.Services;

namespace Neuroglia.Data.Infrastructure.ObjectStorage;

/// <summary>
/// Defines the fundamentals of an object used to describe an object stored on an <see cref="IObjectStorage"/>
/// </summary>
public interface IObjectDescriptor
{

    /// <summary>
    /// Gets the date and time the object has been last been modified
    /// </summary>
    DateTimeOffset? LastModified { get; }

    /// <summary>
    /// Gets the name of the bucket the object belongs to
    /// </summary>
    string BucketName { get; }

    /// <summary>
    /// Gets the object's name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the object's content type
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// Gets the object's size
    /// </summary>
    ulong Size { get; }

    /// <summary>
    /// Gets the object's ETag
    /// </summary>
    string ETag { get; }

    /// <summary>
    /// Gets a list containing the object's tags, if any
    /// </summary>
    IReadOnlyDictionary<string, string>? Tags { get; }

}