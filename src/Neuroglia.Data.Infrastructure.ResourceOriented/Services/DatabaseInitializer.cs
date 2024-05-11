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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Neuroglia.Data.Infrastructure.ResourceOriented.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IDatabaseInitializer"/> interface
/// </summary>
public class DatabaseInitializer
    : IHostedService, IDatabaseInitializer
{

    /// <summary>
    /// Initializes a new <see cref="DatabaseInitializer"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    public DatabaseInitializer(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.ServiceProvider = serviceProvider;
        this.ServiceScope = this.ServiceProvider.CreateScope();
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the current <see cref="IServiceScope"/>
    /// </summary>
    protected IServiceScope ServiceScope { get; }

    /// <summary>
    /// Gets the <see cref="IDatabase"/> to initialize
    /// </summary>
    protected IDatabase Database => this.ServiceScope.ServiceProvider.GetRequiredService<IDatabase>();

    /// <inheritdoc/>
    public virtual Task StartAsync(CancellationToken stoppingToken) => this.InitializeAsync(stoppingToken);

    /// <inheritdoc/>
    public virtual Task StopAsync(CancellationToken stoppingToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        this.Logger.LogDebug("Initializing resource database...");
        if (await this.Database.InitializeAsync(cancellationToken).ConfigureAwait(false))
        {
            this.Logger.LogDebug("Seeding resource database...");
            try
            {
                await this.SeedAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger.LogError("An error occurred while seeding the resource database: {ex}", ex);
            }
            this.Logger.LogDebug("Resource database has been seeded");
        }
        this.ServiceScope.Dispose();
        this.Logger.LogDebug("Resource database has been successfully initialized");
    }

    /// <summary>
    /// Seeds the <see cref="IDatabase"/>
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual Task SeedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}