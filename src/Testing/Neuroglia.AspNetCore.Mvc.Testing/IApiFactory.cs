using System;
using System.Net.Http;

namespace Microsoft.AspNetCore.Mvc.Testing
{

    /// <summary>
    /// Defines the fundamentals of a service used to create test <see cref="HttpClient"/>s
    /// </summary>
    public interface IApiFactory
    {

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        IServiceProvider Services { get; }

        /// <summary>
        /// Creates a new test <see cref="HttpClient"/>
        /// </summary>
        /// <returns>A new test <see cref="HttpClient"/></returns>
        HttpClient CreateClient();

    }

}
