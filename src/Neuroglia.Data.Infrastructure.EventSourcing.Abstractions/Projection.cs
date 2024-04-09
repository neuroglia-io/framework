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

using Neuroglia.Data.Infrastructure.EventSourcing.Services;

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Defines constants and statics used to help expressing event-driven projections
/// </summary>
public static class Projection
{

    /// <summary>
    /// Placeholder method used by <see cref="IProjectionBuilder{TState}"/> implementations to link a processed event to a specific stream
    /// </summary>
    /// <param name="stream">The stream to link the processed event to</param>
    /// <param name="e">The processed event</param>
    public static void LinkEventTo(string stream, IEventRecord e)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stream);
        ArgumentNullException.ThrowIfNull(e);
    }

}
