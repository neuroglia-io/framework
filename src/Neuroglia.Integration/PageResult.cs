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

using System.Collections;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia;

/// <summary>
/// Represents an object used to describe a page of items
/// </summary>
[DataContract]
public class PageResult
{

    /// <summary>
    /// Initializes a new <see cref="PageResult"/>
    /// </summary>
    public PageResult() { }

    /// <summary>
    /// Initializes a new <see cref="PageResult"/>
    /// </summary>
    /// <param name="value">The results of the query</param>
    /// <param name="count">The total result count</param>
    /// <param name="contextUri">The uri of the page result's context</param>
    public PageResult(IEnumerable value, long? count = null, Uri? contextUri = null)
    {
        this.Value = value;
        this.Count = count;
        this.ContextUri = contextUri;
    }

    /// <summary>
    /// Gets the total result count
    /// </summary>
    [DataMember, JsonPropertyName("@odata.context")]
    public virtual Uri? ContextUri { get; set; } = null!;

    /// <summary>
    /// Gets the results of the query
    /// </summary>
    [DataMember]
    public virtual IEnumerable? Value { get; set; }

    /// <summary>
    /// Gets the total result count
    /// </summary>
    [DataMember, JsonPropertyName("@odata.count")]
    public virtual long? Count { get; set; }

}

/// <summary>
/// Represents an object used to describe a page of items
/// </summary>
/// <typeparam name="T">The type of the paged results</typeparam>
[DataContract]
public class PageResult<T>
    : PageResult
{

    /// <summary>
    /// Gets the results of the query
    /// </summary>
    [DataMember]
    public new virtual IEnumerable<T>? Value { get; set; }

}