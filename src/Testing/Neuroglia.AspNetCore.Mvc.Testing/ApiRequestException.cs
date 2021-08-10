using System.Net.Http;

namespace Microsoft.AspNetCore.Mvc.Testing
{

    /// <summary>
    /// Represents an <see cref="HttpRequestException"/> thrown by an <see cref="ApiControllerTest"/>
    /// </summary>
    public class ApiRequestException
        : HttpRequestException
    {

        /// <summary>
        /// Initializes a new <see cref="ApiRequestException"/>
        /// </summary>
        /// <param name="inner">The inner <see cref="HttpRequestException"/></param>
        /// <param name="responseContent">The response content, if any</param>
        public ApiRequestException(HttpRequestException inner, string responseContent)
            : base(inner.Message, inner)
        {
            this.ResponseContent = responseContent;
        }

        /// <summary>
        /// Gets the response content, if any
        /// </summary>
        public string ResponseContent { get; }

    }

}
