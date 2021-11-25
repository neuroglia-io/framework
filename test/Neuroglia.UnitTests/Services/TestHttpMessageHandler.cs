using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.UnitTests.Services
{
    internal class TestHttpMessageHandler
        : DelegatingHandler
    {

        public static TaskCompletionSource SendTaskSource = new();

        public static int MessagesSent = 0;

        public HttpStatusCode ResponseStatusCode { get; set; } = HttpStatusCode.OK;

        public HttpContent ResponseContent { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if(!SendTaskSource.Task.IsCompleted)
                SendTaskSource.SetResult();
            Interlocked.Increment(ref MessagesSent);
            return Task.FromResult(new HttpResponseMessage(this.ResponseStatusCode) { Content = this.ResponseContent });
        }

    }
}
