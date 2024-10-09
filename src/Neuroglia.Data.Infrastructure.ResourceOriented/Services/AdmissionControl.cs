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
using Neuroglia.Data.PatchModel.Services;
using System.Net;

namespace Neuroglia.Data.Infrastructure.ResourceOriented.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IAdmissionControl"/> interface
/// </summary>
/// <remarks>
/// Initializes a new <see cref="AdmissionControl"/>
/// </remarks>
/// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
/// <param name="patchHandlers">An <see cref="IEnumerable{T}"/> containing the services used to handle and apply <see cref="Patch"/>es</param>
public class AdmissionControl(IServiceProvider serviceProvider, IEnumerable<IPatchHandler> patchHandlers)
    : IAdmissionControl
{

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; } = serviceProvider;

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing the services used to handle and apply <see cref="Patch"/>es
    /// </summary>
    protected IEnumerable<IPatchHandler> PatchHandlers { get; } = patchHandlers;

    /// <inheritdoc/>
    public virtual async Task<AdmissionReviewResponse> ReviewAsync(AdmissionReviewRequest request, CancellationToken cancellationToken = default)
    {
        var originalResource = request.UpdatedState;
        ArgumentNullException.ThrowIfNull(request);
        var result = await this.MutateAsync(request, cancellationToken).ConfigureAwait(false);
        if (!result.Allowed) return result;
        result = await this.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
        if (!result.Allowed) return result;
        Patch? patch = null;
        if(originalResource != null) patch = new(PatchType.JsonPatch, JsonPatchUtility.CreateJsonPatchFromDiff(originalResource, request.UpdatedState));
        return new(request.Uid, true, patch);
    }

    /// <summary>
    /// Mutates the specified <see cref="IResource"/> upon admission
    /// </summary>
    /// <param name="request">The <see cref="AdmissionReviewRequest"/> to evaluate</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="AdmissionReviewResponse"/> that describes the result of the operation</returns>
    protected virtual async Task<AdmissionReviewResponse> MutateAsync(AdmissionReviewRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Operation != Operation.Create && request.Operation != Operation.Replace && request.Operation != Operation.Patch) return new AdmissionReviewResponse(request.Uid, true);

        var mutators = this.ServiceProvider.GetServices<IResourceMutator>().Where(m => m.AppliesTo(request)).ToList();

        try
        {
            mutators.AddRange(await this.ServiceProvider.GetRequiredService<IResourceRepository>()
                .GetMutatingWebhooksFor(request.Operation, request.Resource, cancellationToken)
                .Select(wh => ActivatorUtilities.CreateInstance<WebhookResourceMutator>(this.ServiceProvider, wh))
                .ToListAsync(cancellationToken).ConfigureAwait(false));
        }
        catch
        {
            //todo: log
        }


        foreach (var mutator in mutators)
        {
            var result = await mutator.MutateAsync(request, cancellationToken).ConfigureAwait(false);
            if (!result.Allowed) return result!;
            if (result.Patch != null) 
            {
                var patchHandler = this.PatchHandlers.FirstOrDefault(h => h.Supports(result.Patch.Type)) ?? throw new NullReferenceException($"Failed to find a registered handler for patches of type '{result.Patch.Type}'");
                request.UpdatedState = await patchHandler.ApplyPatchAsync(result.Patch.Document, request.UpdatedState ?? request.OriginalState, cancellationToken).ConfigureAwait(false);
            }
        }

        return new(request.Uid, true, request.GetDiffPatch());
    }

    /// <summary>
    /// Validates the specified <see cref="IResource"/> upon admission
    /// </summary>
    /// <param name="request">The <see cref="AdmissionReviewRequest"/> to evaluate</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="AdmissionReviewResponse"/> that describes the result of the operation</returns>
    protected virtual async Task<AdmissionReviewResponse> ValidateAsync(AdmissionReviewRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validators = this.ServiceProvider.GetServices<IResourceValidator>().Where(m => m.AppliesTo(request)).ToList();
        try
        { 
            validators.AddRange(await this.ServiceProvider.GetRequiredService<IResourceRepository>()
                .GetMutatingWebhooksFor(request.Operation, request.Resource, cancellationToken)
                .Select(wh => ActivatorUtilities.CreateInstance<WebhookResourceValidator>(this.ServiceProvider, wh))
                .ToListAsync(cancellationToken));
        }
        catch
        {
            //todo: log
        }

        var tasks = new List<Task<AdmissionReviewResponse>>(validators.Count);
        foreach (var validator in validators)
        {
            tasks.Add(validator.ValidateAsync(request, cancellationToken));
        }
        await Task.WhenAll(tasks).ConfigureAwait(false);

        var results = tasks.Select(t => t.Result);
        if (results.All(t => t.Allowed)) return new(request.Uid, true);
        else return new(request.Uid, false, null, new ProblemDetails(ProblemTypes.AdmissionFailed, ProblemTitles.AdmissionFailed, (int)HttpStatusCode.BadRequest, errors: results.Where(r => !r.Allowed && r.Problem != null).Select(r => r.Problem!).ToDictionary(p => $"[{p.Status} - {p.Title ?? p.Type?.OriginalString ?? "Unknown"}]", p => new string[] { p.Detail! })));
    }

}
