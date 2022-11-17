using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;
using System;

namespace Neuroglia.UnitTests.Containers
{
    public sealed class ApiCurioRegistryContainer
        : HostedServiceContainer
    {

        public const int HostPort = 8080;

        internal ApiCurioRegistryContainer(ITestcontainersConfiguration configuration, ILogger logger) 
            : base(configuration, logger)
        {

        }

        public Uri ServiceUri => new($"http://{this.Hostname}:{this.GetMappedPublicPort(HostPort)}");

    }

}
