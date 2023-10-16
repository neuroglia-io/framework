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

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Data;

/// <summary>
/// Represents a patch
/// </summary>
[DataContract]
public class Patch
{

    /// <summary>
    /// Initializes a new <see cref="Patch"/>
    /// </summary>
    [JsonConstructor]
    protected Patch() { }

    /// <summary>
    /// Initializes a new <see cref="Patch"/>
    /// </summary>
    /// <param name="type">The patch type</param>
    /// <param name="document">The patch document</param>
    public Patch(string type, object document)
    {
        this.Type = type;
        this.Document = document ?? throw new NullReferenceException(nameof(Document));
    }

    /// <summary>
    /// Gets the patch type
    /// </summary>
    [DataMember(IsRequired = true)]
    public virtual string Type { get; protected set; } = null!;

    /// <summary>
    /// Gets the patch document
    /// </summary>
    [DataMember(IsRequired = true)]
    public virtual object Document { get; protected set; } = null!;

}
