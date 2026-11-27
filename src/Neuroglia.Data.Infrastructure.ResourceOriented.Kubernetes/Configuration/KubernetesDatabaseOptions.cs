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

namespace Neuroglia.Data.Infrastructure.ResourceOriented.Configuration;

/// <summary>
/// Represents the options used to configure a <see cref="KubernetesDatabase"/>
/// </summary>
public class KubernetesDatabaseOptions
{

    /// <summary>
    /// Gets the default maximum amount of resources to request per page when streaming or listing resources
    /// </summary>
    public const int DefaultPageSize = 250;

    /// <summary>
    /// Gets/sets the maximum amount of resources to request per page when streaming resources or when building the initial state of a watch. Defaults to <see cref="DefaultPageSize"/>
    /// </summary>
    public virtual int PageSize { get; set; } = DefaultPageSize;

    /// <summary>
    /// Gets/sets the duration after which the API server is asked to close a watch request, after which the watch transparently reconnects. If not set, the API server's default applies (a randomized duration of several minutes)
    /// </summary>
    public virtual TimeSpan? WatchTimeout { get; set; }

    /// <summary>
    /// Gets/sets the delay to wait before reconnecting a watch after a transient error. The delay doubles after each consecutive failure, up to <see cref="WatchMaxReconnectDelay"/>. Defaults to 1 second
    /// </summary>
    public virtual TimeSpan WatchMinReconnectDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets/sets the maximum delay to wait before reconnecting a watch after consecutive transient errors. Defaults to 30 seconds
    /// </summary>
    public virtual TimeSpan WatchMaxReconnectDelay { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets/sets the maximum amount of attempts made to apply a strategic merge patch when the resource is concurrently modified and no resource version has been specified. Defaults to 5
    /// </summary>
    public virtual int MaxPatchAttempts { get; set; } = 5;

}
