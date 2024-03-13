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


using Neuroglia.Data.Infrastructure.ResourceOriented.Properties;
using System.Net;

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Exposes common resource-oriented problem details
/// </summary>
public static class ResourceProblemDetails
{

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes failure to validate a resource against its schema
    /// </summary>
    /// <param name="resource">A reference to the invalid resource</param>
    /// <param name="evaluationResults">The <see cref="EvaluationResults"/> to create new <see cref="ProblemDetails"/> for</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceSchemaValidationFailed(IResourceReference resource, EvaluationResults evaluationResults)
    {
        return new
        (
            ProblemTypes.SchemaValidationFailed,
            ProblemTitles.ValidationFailed,
            (int)HttpStatusCode.BadRequest,
            StringFormatter.Format(ProblemDescriptions.ResourceSchemaValidationFailed, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural),
            null,
            evaluationResults.Errors?.Select(e => new KeyValuePair<string, string[]>(e.Key, [e.Value]))
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes failure to find a specific resource
    /// </summary>
    /// <param name="resource">The resource that could not be found</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceNotFound(IResourceReference resource)
    {
        return new
        (
            ProblemTypes.NotFound,
            ProblemTitles.NotFound,
            (int)HttpStatusCode.NotFound,
            StringFormatter.Format(ProblemDescriptions.ResourceNotFound, resource)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes an error where a patch had no effect 
    /// </summary>
    /// <param name="resource">The resource that was not modified</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceNotModified(IResourceReference resource)
    {
        return new
        (
            ProblemTypes.NotModified,
            ProblemTitles.NotModified,
            (int)HttpStatusCode.NotModified,
            StringFormatter.Format(ProblemDescriptions.ResourceNotModified, resource)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes an error due to an unsupported sub resource 
    /// </summary>
    /// <param name="subResource">The unsupported sub resource</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails UnsupportedSubResource(ISubResourceReference subResource)
    {
        return new
        (
            ProblemTypes.UnsupportedSubResource,
            ProblemTitles.Unsupported,
            (int)HttpStatusCode.BadRequest,
            StringFormatter.Format(ProblemDescriptions.UnsupportedSubResource, subResource)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes failure to allow an operation on a resource
    /// </summary>
    /// <param name="operation">The operation that was rejected</param>
    /// <param name="resource">The resource that could not be admitted</param>
    /// <param name="errors">An array containing the errors that have occurred during admission</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceAdmissionFailed(Operation operation, IResourceReference resource, params KeyValuePair<string, string[]>[] errors)
    {
        return new
        (
            ProblemTypes.AdmissionFailed,
            ProblemTitles.AdmissionFailed,
            (int)HttpStatusCode.NotFound,
            StringFormatter.Format(ProblemDescriptions.ResourceAdmissionFailed, EnumHelper.Stringify(operation), resource, errors == null ? string.Empty : string.Join(Environment.NewLine, errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}")))
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes failure to convert a resource to the specified version
    /// </summary>
    /// <param name="resource">The resource that could not be converted</param>
    /// <param name="toVersion">The version the resource was converted to</param>
    /// <param name="errors">An array containing the errors that have occurred during conversion</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceConversionFailed(IResourceReference resource, string toVersion, params KeyValuePair<string, string[]>[] errors)
    {
        return new
        (
            ProblemTypes.ConversionFailed,
            ProblemTitles.ConversionFailed,
            (int)HttpStatusCode.BadRequest,
            StringFormatter.Format(ProblemDescriptions.ResourceConversionFailed, resource, toVersion, string.Join(Environment.NewLine, errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}")))
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes failure to find a specific resource definition
    /// </summary>
    /// <param name="resource">The invalid resource</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceDefinitionNotFound(IResourceDefinitionReference resource)
    {
        return new
       (
           ProblemTypes.DefinitionNotFound,
           ProblemTitles.NotFound,
           (int)HttpStatusCode.NotFound,
           StringFormatter.Format(ProblemDescriptions.ResourceDefinitionNotFound, resource.Group, resource.Version, resource.Plural)
       );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes failure to find a specific version of a resource definition
    /// </summary>
    /// <param name="resource">The invalid resource</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceVersionNotFound(IResourceReference resource)
    {
        return new
        (
            ProblemTypes.VersionNotFound,
            ProblemTitles.NotFound,
            (int)HttpStatusCode.NotFound,
            StringFormatter.Format(ProblemDescriptions.ResourceVersionNotFound, resource.Definition.Version, resource.Definition.Plural, resource.Definition.Group)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes failure to find a specific version of a resource definition
    /// </summary>
    /// <param name="resourceDefinition">The invalid resource</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceStorageVersionNotFound(IResourceDefinitionReference resourceDefinition)
    {
        return new
        (
            ProblemTypes.StorageVersionNotFound,
            ProblemTitles.NotFound,
            (int)HttpStatusCode.NotFound,
            StringFormatter.Format(ProblemDescriptions.ResourceStorageVersionNotFound, resourceDefinition.Plural, resourceDefinition.Group)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes an error due to the fact that the '.metadata.resourceVersion' has not been set in the context of a replace operation
    /// </summary>
    /// <param name="resourceReference">A reference to the resource that could not be replaced</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceVersionRequired(IResourceReference resourceReference)
    {
        return new
        (
            ProblemTypes.ResourceVersionRequired,
            ProblemTitles.ValidationFailed,
            (int)HttpStatusCode.BadRequest,
            StringFormatter.Format(ProblemDescriptions.ResourceVersionRequired, resourceReference)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes an error due to the fact that the '.metadata.resourceVersion' has not been set in the context of a sub resource replace operation
    /// </summary>
    /// <param name="subResourceReference">A reference to the sub resource that could not be replaced</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails SubResourceVersionRequired(ISubResourceReference subResourceReference)
    {
        return new
        (
            ProblemTypes.ResourceVersionRequired,
            ProblemTitles.ValidationFailed,
            (int)HttpStatusCode.BadRequest,
            StringFormatter.Format(ProblemDescriptions.ResourceVersionRequired, subResourceReference)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes an error due to an invalid resource patch
    /// </summary>
    /// <param name="resourceReference">The <see cref="IResource"/> that could not be patched</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails InvalidResourcePatch(IResourceReference resourceReference)
    {
        return new
        (
            ProblemTypes.InvalidPatch,
            ProblemTitles.ValidationFailed,
            (int)HttpStatusCode.BadRequest,
            StringFormatter.Format(ProblemDescriptions.InvalidResourcePatch, resourceReference)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes an error due to an invalid sub resource patch
    /// </summary>
    /// <param name="subResourceReference">A reference to the sub resource that could not be patched</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails InvalidSubResourcePatch(IResourceReference subResourceReference)
    {
        return new
        (
            ProblemTypes.InvalidPatch,
            ProblemTitles.ValidationFailed,
            (int)HttpStatusCode.BadRequest,
            StringFormatter.Format(ProblemDescriptions.InvalidResourcePatch, subResourceReference)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes failure to perform optimistic concurrency checks on the specified resource
    /// </summary>
    /// <param name="resource">The invalid resource</param>
    /// <param name="targetVersion">The target version of the resource</param>
    /// <param name="actualVersion">The current version of the resource</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceOptimisticConcurrencyCheckFailed(IResourceReference resource, string targetVersion, string actualVersion)
    {
        return new
        (
            ProblemTypes.OptimisticConcurrencyCheckFailed,
            ProblemTitles.Conflict,
            (int)HttpStatusCode.Conflict,
            StringFormatter.Format(ProblemDescriptions.ResourceOptimisticConcurrencyCheckFailed, resource, targetVersion, actualVersion)
        );
    }

}
