using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.AspNetCore.Mvc.Testing
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IApiFactory"/>
    /// </summary>
    /// <typeparam name="TStartup">The type of the startup class to use to configure the <see cref="WebApplicationFactory{TEntryPoint}"/></typeparam>
    public class ApiFactory<TStartup>
        : WebApplicationFactory<TStartup>, IApiFactory
        where TStartup : class
    {

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        public new IServiceProvider Services
        {
            get
            {
                if (this.Server == null)
                    this.CreateClient();
                return this.Server.Host.Services;
            }
        }

        /// <inheritdoc/>
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            return WebHost.CreateDefaultBuilder()
                .UseContentRoot(AppContext.BaseDirectory)
                .UseWebRoot(AppContext.BaseDirectory)
                .UseSolutionRelativeContentRoot(AppContext.BaseDirectory)
                .UseConfiguration(configuration)
                .UseStartup<TStartup>();
        }

        /// <inheritdoc/>
        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            builder.UseSolutionRelativeContentRoot(new Uri(AppDomain.CurrentDomain.BaseDirectory).LocalPath);
            return base.CreateServer(builder);
        }

    }

}
