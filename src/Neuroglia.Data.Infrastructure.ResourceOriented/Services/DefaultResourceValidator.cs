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

using Json.Patch;
using Json.Pointer;
using Neuroglia.Serialization;

namespace Neuroglia.Data.Infrastructure.ResourceOriented.Services;

/// <summary>
/// Represents the <see cref="IResourceValidator"/> used to validate the semantics of <see cref="IResource"/>s being admitted, and possibly mutate them
/// </summary>
/// <remarks>
/// Initializes a new <see cref="DefaultResourceValidator"/>
/// </remarks>
/// <param name="jsonSerializer">The service used to serialize/deserialize objects to/from JSON</param>
/// <param name="repository">The service used to manage the application's <see cref="IResource"/>s</param>
public class DefaultResourceValidator(IJsonSerializer jsonSerializer, IRepository repository)
    : IResourceMutator
{

    /// <summary>
    /// Gets the service used to serialize/deserialize objects to/from JSON
    /// </summary>
    protected IJsonSerializer JsonSerializer { get; } = jsonSerializer;

    /// <summary>
    /// Gets the service used to manage the application's <see cref="IResource"/>s
    /// </summary>
    protected IRepository Repository { get; } = repository;

    /// <inheritdoc/>
    public bool AppliesTo(Operation operation, string group, string version, string plural, string? @namespace = null) => operation == Operation.Create;

    /// <inheritdoc/>
    public virtual async Task<AdmissionReviewResponse> MutateAsync(AdmissionReviewRequest context, CancellationToken cancellationToken = default)
    {
        var resourceDefinition = await this.Repository.GetDefinitionAsync(context.Resource.Definition.Group, context.Resource.Definition.Plural, cancellationToken).ConfigureAwait(false);
        var patchOperations = new List<PatchOperation>();
        if (resourceDefinition == null) return new AdmissionReviewResponse(context.Uid, false, null, ResourceProblemDetails.ResourceDefinitionNotFound(context.Resource.Definition));
        if (resourceDefinition.Spec.Scope == ResourceScope.Cluster && !string.IsNullOrWhiteSpace(context.Resource.Namespace))
            return new AdmissionReviewResponse(context.Uid, false, null, ResourceProblemDetails.ResourceAdmissionFailed(context.Operation, context.Resource,
                new KeyValuePair<string, string[]>(JsonPointer.Create<Resource>(r => r.Metadata.Namespace!).ToString().ToCamelCase(), [StringFormatter.Format(ProblemDescriptions.ClusterResourceCannotDefineNamespace, context.Resource)])));
        else if (resourceDefinition.Spec.Scope == ResourceScope.Namespaced && string.IsNullOrWhiteSpace(context.Resource.Namespace))
            patchOperations.Add(PatchOperation.Add(JsonPointer.Create<Resource>(r => r.Metadata.Namespace!).ToCamelCase(), this.JsonSerializer.SerializeToNode(Namespace.DefaultNamespaceName)));
        var resourceName = context.Resource.Name;
        if (resourceName.EndsWith('-')) resourceName = $"{context.Resource.Name}{Guid.NewGuid().ToShortString().ToLowerInvariant()}";
        if (!ObjectNamingConvention.Current.IsValidResourceName(resourceName))
            return new AdmissionReviewResponse(context.Uid, false, null, ResourceProblemDetails.ResourceAdmissionFailed(context.Operation, context.Resource,
                new KeyValuePair<string, string[]>(JsonPointer.Create<Resource>(r => r.Metadata.Name!).ToString().ToCamelCase(), [StringFormatter.Format(ProblemDescriptions.InvalidResourceName, context.Resource.Name)])));
        if (!string.IsNullOrWhiteSpace(context.Resource.Namespace) && !ObjectNamingConvention.Current.IsValidResourceName(context.Resource.Namespace))
            return new AdmissionReviewResponse(context.Uid, false, null, ResourceProblemDetails.ResourceAdmissionFailed(context.Operation, context.Resource,
                new KeyValuePair<string, string[]>(JsonPointer.Create<Resource>(r => r.Metadata.Namespace!).ToString().ToCamelCase(), [StringFormatter.Format(ProblemDescriptions.InvalidResourceName, context.Resource.Namespace)])));
        var patch = patchOperations.Count != 0 ? new Patch(PatchType.JsonPatch, new JsonPatch(patchOperations)) : null;
        return new(context.Uid, true, patch);
    }

}
