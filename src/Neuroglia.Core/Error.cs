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

namespace Neuroglia;

/// <summary>
/// Represents an object used to describe an error or problem, as defined by <see href="https://www.rfc-editor.org/rfc/rfc7807">RFC 7807</see>
/// </summary>
[DataContract]
public record Error
    : IExtensible
{

    /// <summary>
    /// Initialize a new <see cref="Error"/>
    /// </summary>
    public Error() { }

    /// <summary>
    /// Initialize a new <see cref="Error"/>
    /// </summary>
    /// <param name="type">An uri that reference the type of the described problem</param>
    /// <param name="title">A short, human-readable summary of the problem type.It SHOULD NOT change from occurrence to occurrence of the problem, except for purposes of localization</param>
    /// <param name="status">The status code produced by the described problem</param>
    /// <param name="detail">A human-readable explanation specific to this occurrence of the problem</param>
    /// <param name="instance">A <see cref="Uri"/> reference that identifies the specific occurrence of the problem.It may or may not yield further information if dereferenced</param>
    /// <param name="errors">An optional collection containing error messages mapped per error code</param>
    /// <param name="extensionData">A mapping containing problem details extension data, if any</param>
    public Error(Uri type, string title, int status, string? detail = null, Uri? instance = null, IEnumerable<KeyValuePair<string, string[]>>? errors = null, IDictionary<string, object>? extensionData = null)
    {
        this.Type = type ?? throw new ArgumentNullException(nameof(type));
        this.Title = title ?? throw new ArgumentNullException(nameof(title));
        this.Status = status;
        this.Detail = detail;
        this.Instance = instance;
        this.Errors = errors?.WithValueSemantics();
        this.ExtensionData = extensionData;
    }

    /// <summary>
    /// Gets/sets an uri that reference the type of the described problem.
    /// </summary>
    [DataMember]
    public virtual Uri? Type { get; set; } = null!;

    /// <summary>
    /// Gets/sets a short, human-readable summary of the problem type.It SHOULD NOT change from occurrence to occurrence of the problem, except for purposes of localization.
    /// </summary>
    [DataMember]
    public virtual string? Title { get; set; } = null!;

    /// <summary>
    /// Gets/sets the status code produced by the described problem
    /// </summary>
    [DataMember]
    public virtual int Status { get; set; }

    /// <summary>
    /// Gets/sets a human-readable explanation specific to this occurrence of the problem.
    /// </summary>
    [DataMember]
    public virtual string? Detail { get; set; }

    /// <summary>
    /// Gets/sets a <see cref="Uri"/> reference that identifies the specific occurrence of the problem.It may or may not yield further information if dereferenced.
    /// </summary>
    [DataMember]
    public virtual Uri? Instance { get; set; }

    /// <summary>
    /// Gets/sets an optional collection containing error messages mapped per error code
    /// </summary>
    [DataMember]
    public virtual EquatableList<KeyValuePair<string, string[]>>? Errors { get; set; }

    /// <summary>
    /// Gets/sets a mapping containing problem details extension data, if any
    /// </summary>
    [DataMember, JsonExtensionData]
    public virtual IDictionary<string, object>? ExtensionData { get; set; }

    /// <inheritdoc/>
    public override string ToString() => this.Detail ?? Environment.NewLine + string.Join(Environment.NewLine, this.Errors?.Select(e => string.Join(Environment.NewLine, e.Value.Select(v => $"{e.Key}: {v}")))!);

}
