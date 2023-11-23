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

namespace Neuroglia;

/// <summary>
/// Represents the base class for Data Transfer Objects used to describe an aggregate state
/// </summary>
/// <typeparam name="TKey">The type of key used to identify the described entity</typeparam>
[DataContract]
public abstract record AggregateStateDataTransferObject<TKey>
    : EntityDataTransferObject<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Gets the state's version
    /// </summary>
    [DataMember]
    public virtual ulong StateVersion { get; set; }

}