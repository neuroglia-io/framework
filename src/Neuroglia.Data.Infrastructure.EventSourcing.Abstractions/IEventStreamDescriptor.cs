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

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Defines the fundamentals of a service used to describe a stream of events
/// </summary>
public interface IEventStreamDescriptor
    : IIdentifiable
{

    /// <summary>
    /// Gets the stream's length, or events count
    /// </summary>
    long Length { get; }

    /// <summary>
    /// Gets the date and time at which the first event has been created
    /// </summary>
    DateTimeOffset? FirstEventAt { get; }

    /// <summary>
    /// Gets the date and time at which the last event has been created
    /// </summary>
    DateTimeOffset? LastEventAt { get; }

}
