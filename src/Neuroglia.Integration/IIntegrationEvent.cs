﻿// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
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
/// Defines the fundamentals of an integration event, which is an event used to transport and share data across bounded contexts
/// </summary>
public interface IIntegrationEvent
{

    /// <summary>
    /// Gets/sets the date and time at which the integration event has been created
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets/sets the id of the aggregate, if any, that has produced the event
    /// </summary>
    object? AggregateId { get; }

    /// <summary>
    /// Gets/sets the version of the event's source aggregate, if any, at the time it produced the event
    /// </summary>
    ulong? AggregateVersion { get; }

}