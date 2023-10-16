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
/// Represents the default implementation of the <see cref="IOperationResult"/> interface
/// </summary>
[DataContract]
public record OperationResult
    : IOperationResult
{

    [DataMember(Name = nameof(Errors)), JsonInclude, JsonPropertyName(nameof(Errors))]
    private List<Error>? _errors;

    /// <summary>
    /// Initializes a new <see cref="OperationResult"/>
    /// </summary>
    [JsonConstructor]
    protected OperationResult() { }

    /// <summary>
    /// Initializes a new <see cref="OperationResult"/>
    /// </summary>
    /// <param name="status">A value that describes the status of the operation result</param>
    /// <param name="data">The data, if any, returned by the operation in case of success</param>
    /// <param name="errors">A list of the errors that have occured, if any, during the execution of the operation</param>
    public OperationResult(int status, object? data = null, params Error[] errors)
    {
        this.Status = status;
        this.Data = data;
        this._errors = errors?.ToList();
    }

    /// <inheritdoc/>
    [DataMember]
    public virtual int Status { get; protected set; }

    /// <inheritdoc/>
    [DataMember]
    public virtual object? Data { get; protected set; }

    /// <inheritdoc/>
    [JsonIgnore, IgnoreDataMember]
    public IReadOnlyCollection<Error>? Errors => this._errors?.AsReadOnly();

}

/// <summary>
/// Represents the default implementation of the <see cref="IOperationResult{T}"/> interface
/// </summary>
/// <typeparam name="T">The type of data, if any, returned by the operation in case of success</typeparam>
[DataContract]
public record OperationResult<T>
    : OperationResult, IOperationResult<T>
{

    /// <summary>
    /// Initializes a new <see cref="OperationResult"/>
    /// </summary>
    [JsonConstructor]
    protected OperationResult() { }

    /// <summary>
    /// Initializes a new <see cref="OperationResult"/>
    /// </summary>
    /// <param name="status">A value that describes the status of the operation result</param>
    /// <param name="data">The data, if any, returned by the operation in case of success</param>
    /// <param name="errors">A list of the errors that have occured, if any, during the execution of the operation</param>
    public OperationResult(int status, object? data = null, params Error[] errors) : base(status, data, errors) { }

    /// <inheritdoc/>
    public virtual new T? Data
    {
        get
        {
            return (T?)base.Data;
        }
        protected set
        {
            base.Data = value;
        }
    }

}