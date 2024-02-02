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

using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Neuroglia.Data.Infrastructure.ResourceOriented.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IVersionControl"/> interface
/// </summary>
public class VersionControl
    : IVersionControl
{

    /// <summary>
    /// Initializes a new <see cref="VersionControl"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="httpClientFactory">The services used to create <see cref="System.Net.Http.HttpClient"/>s</param>
    public VersionControl(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.HttpClient = httpClientFactory.CreateClient();
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the <see cref="System.Net.Http.HttpClient"/> used to perform webhook requests
    /// </summary>
    protected HttpClient HttpClient { get; }

    /// <inheritdoc/>
    public virtual async Task<IResource> ConvertToStorageVersionAsync(IVersioningContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (context.ResourceDefinition.Spec.Conversion?.Strategy == ConversionStrategy.None) return context.Resource;
        if (!context.ResourceDefinition.TryGetVersion(context.ResourceReference.Definition.Version, out var version) || version == null) throw new ProblemDetailsException(ResourceProblemDetails.ResourceVersionNotFound(context.ResourceReference));
        var resource = context.Resource;
        if (version.Storage) return resource;
        var storageVersion = context.ResourceDefinition.GetStorageVersion();
        return storageVersion == null
            ? throw new ProblemDetailsException(ResourceProblemDetails.ResourceStorageVersionNotFound(context.ResourceReference.Definition))
            : (context.ResourceDefinition.Spec.Conversion?.Strategy) switch
        {
            ConversionStrategy.Webhook => await this.PerformWebhookConversionAsync(context, context.ResourceDefinition.Spec.Conversion?.Webhook?.Client, version, storageVersion, cancellationToken),
            _ => throw new NotSupportedException($"The specified strategy '{context.ResourceDefinition.Spec.Conversion?.Strategy}' is not supported")
        };
    }

    /// <summary>
    /// Performs the conversion of the current resource to the specified version
    /// </summary>
    /// <returns></returns>
    protected virtual async Task<IResource> PerformWebhookConversionAsync(IVersioningContext context, WebhookClientConfiguration? webhook,
        ResourceDefinitionVersion fromVersion, ResourceDefinitionVersion toVersion, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(webhook);
        ArgumentNullException.ThrowIfNull(fromVersion);
        ArgumentNullException.ThrowIfNull(toVersion);
        var conversionReview = new ConversionReview(new(Guid.NewGuid().ToShortString(), toVersion.Name, context.Resource));
        using var response = await this.HttpClient.PostAsJsonAsync(webhook.Uri, conversionReview, cancellationToken);
        response.EnsureSuccessStatusCode();
        conversionReview = await response.Content.ReadFromJsonAsync<ConversionReview>(cancellationToken: cancellationToken);
        if (conversionReview?.Response == null || conversionReview.Response.ConvertedResource == null)
        {
            this.Logger.LogWarning("Versioning webhook {webhook} responded with a success status code '{statusCode}' but did not return a valid conversion response or did not define a valid resource patch", webhook, response.StatusCode);
            throw new ProblemDetailsException(ResourceProblemDetails.ResourceConversionFailed(context.ResourceReference, toVersion.Name, conversionReview!.Response?.Errors?.ToArray()!));
        }
        if (!conversionReview.Response.Succeeded)
        {
            this.Logger.LogWarning("Versioning webhook {webhook} failed to convert the resource to version '{version}'", webhook, toVersion.Name);
            throw new ProblemDetailsException(ResourceProblemDetails.ResourceConversionFailed(context.ResourceReference, toVersion.Name, conversionReview.Response?.Errors?.ToArray()!));
        }
        this.Logger.LogDebug("Resource '{resource}' succesfully converted to version '{version}'", context.ResourceReference, toVersion.Name);
        return conversionReview.Response.ConvertedResource;
    }

}