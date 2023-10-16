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

namespace Neuroglia.Mediation;

/// <summary>
/// Defines the fundamentals of a service used to handle notifications of the specified type
/// </summary>
/// <typeparam name="TNotification">The type of notification to handle</typeparam>
public interface INotificationHandler<TNotification>
{

    /// <summary>
    /// Handles the specified notification
    /// </summary>
    /// <param name="notification">The notification to handle</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task HandleAsync(TNotification notification, CancellationToken cancellationToken = default);

}
