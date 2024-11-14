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

using Neuroglia.Eventing.CloudEvents.Infrastructure.Services;
using Neuroglia.Serialization;
using System.Net;

namespace Neuroglia.Eventing.CloudEvents.AspNetCore;

/// <summary>
/// Represents the <see cref="RequestDelegate"/> used to handle <see cref="CloudEvent"/>s
/// </summary>
/// <remarks>
/// Initializes a new <see cref="CloudEventMiddleware"/>
/// </remarks>
/// <param name="next">The next <see cref="RequestDelegate"/> in the pipeline</param>
/// <param name="logger">The service used to perform logging</param>
public class CloudEventMiddleware(RequestDelegate next, ILogger<CloudEventMiddleware> logger)
{

    /// <summary>
    /// Gets the next <see cref="RequestDelegate"/> in the pipeline
    /// </summary>
    protected RequestDelegate Next { get; } = next;

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; } = logger;

    /// <inheritdoc/>
    public async Task InvokeAsync(HttpContext context, ICloudEventBus cloudEventBus)
    {
        if (context.Request.ContentType?.StartsWith(CloudEventContentType.Prefix) != true) { await this.Next(context); return; }
        try
        {
            var serializer = context.RequestServices.GetRequiredService<IJsonSerializer>();
            var e = serializer is IAsyncSerializer asyncSerializer ? (await asyncSerializer.DeserializeAsync<CloudEvent>(context.Request.Body).ConfigureAwait(false))! : serializer.Deserialize<CloudEvent>(context.Request.Body)!;
            cloudEventBus.InputStream.OnNext(e);
            context.Response.StatusCode = (int)HttpStatusCode.Accepted;
        }
        catch(Exception ex)
        {
            this.Logger.LogError("An error occurred while consuming an incoming cloud event: {ex}", ex.ToString());
            throw;
        }
    }

}