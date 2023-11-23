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

namespace Neuroglia;

/// <summary>
/// Represents the base class for all integration events
/// </summary>
public abstract record IntegrationEvent
    : DataTransferObject, IIntegrationEvent
{

    /// <summary>
    /// Gets/sets the id of the aggregate, if any, that has produced the event
    /// </summary>
    public virtual object? AggregateId { get; set; }

    /// <summary>
    /// Gets/sets the date and time at which the integration event has been produced
    /// </summary>
    public virtual DateTimeOffset CreatedAt { get; set; }

}

/// <summary>
/// Represents the base class for all integration events
/// </summary>
public abstract record IntegrationEvent<TKey>
    : IntegrationEvent
{

    /// <summary>
    /// Gets/sets the id of the aggregate, if any, that has produced the event
    /// </summary>
    public virtual new TKey? AggregateId { get; set; }

}
