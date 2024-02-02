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

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Defines the fundamentals of a service used to monitor the state of a specific resource
/// </summary>
public interface IResourceMonitor
    : IDisposable, IAsyncDisposable, IObservable<IResourceWatchEvent>
{

    /// <summary>
    /// Gets the current state of the monitored <see cref="IResource"/>
    /// </summary>
    IResource Resource { get; }

}

/// <summary>
/// Defines the fundamentals of a service used to monitor the state of a specific resource
/// </summary>
/// <typeparam name="TResource">The type of the <see cref="IResource"/> to monitor</typeparam>
public interface IResourceMonitor<TResource>
    : IDisposable, IAsyncDisposable, IObservable<IResourceWatchEvent<TResource>>
    where TResource : class, IResource, new()
{

    /// <summary>
    /// Gets the current state of the monitored <see cref="IResource"/>
    /// </summary>
    TResource Resource { get; }

}
