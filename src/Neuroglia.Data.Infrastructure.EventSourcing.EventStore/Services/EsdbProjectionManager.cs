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

using EventStore.Client;
using Neuroglia.Serialization.Json;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Represents the EventStore implementation of the <see cref="IProjectionManager"/> interface
/// </summary>
/// <param name="projections">The underlying EventStore projection management API client</param>
public class EsdbProjectionManager(EventStoreProjectionManagementClient projections)
    : IProjectionManager
{

    /// <summary>
    /// Gets the underlying EventStore projection management API client
    /// </summary>
    protected EventStoreProjectionManagementClient Projections { get; } = projections;

    /// <inheritdoc/>
    public virtual async Task CreateAsync<TState>(string name, Action<IProjectionSourceBuilder<TState>> setup, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(setup);
        var builder = new EsdbProjectionBuilder<TState>(name, this.Projections);
        setup(builder);
        await builder.BuildAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<TState> GetStateAsync<TState>(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return await this.Projections.GetResultAsync<TState>(name, serializerOptions: JsonSerializer.DefaultOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

}
