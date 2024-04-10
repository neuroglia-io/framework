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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Represents the service used to create <see cref="EsdbProjectionManager"/> instances
/// </summary>
/// <remarks>
/// Initializes a new <see cref="EsdbProjectionManagerFactory"/>
/// </remarks>
/// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
public class EsdbProjectionManagerFactory(IServiceProvider serviceProvider)
    : IFactory<EsdbProjectionManager>
{

    /// <summary>
    /// Gets the name of the EventStore connection string
    /// </summary>
    public const string ConnectionStringName = "eventstore";

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; } = serviceProvider;

    /// <inheritdoc/>
    public virtual EsdbProjectionManager Create()
    {
        var configuration = this.ServiceProvider.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString(ConnectionStringName);
        if (string.IsNullOrWhiteSpace(connectionString)) throw new Exception($"An error occurred while attempting to create an ESEventStore instance. The '{ConnectionStringName}' connection string is not provided or is invalid. Please ensure that the connection string is properly configured in the application settings.");
        var settings = EventStoreClientSettings.Create(connectionString);
        return ActivatorUtilities.CreateInstance<EsdbProjectionManager>(this.ServiceProvider, new EventStoreProjectionManagementClient(settings));
    }

    object IFactory.Create() => this.Create();

}