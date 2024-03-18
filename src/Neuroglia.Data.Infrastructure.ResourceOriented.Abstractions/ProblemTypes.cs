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

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Exposes common resource-oriented problem types
/// </summary>
public static class ProblemTypes
{

    static readonly Uri BaseUri = new("https://neuroglia.io/docs/problems/resources/");

    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem due to refusal of an operation on a specific resource
    /// </summary>
    public static readonly Uri AdmissionFailed = new(BaseUri, "admission-failed");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a failure to convert a resource to a specific version
    /// </summary>
    public static readonly Uri ConversionFailed = new(BaseUri, "conversion-failed");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem due to failing to validate a resource against its schema
    /// </summary>
    public static readonly Uri SchemaValidationFailed = new(BaseUri, "schema-validation-failed");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem describing a failure to find a resource
    /// </summary>
    public static readonly Uri NotFound = new(BaseUri, "not-found");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem describing a failure to patch a resource
    /// </summary>
    public static readonly Uri NotModified = new(BaseUri, "not-modified");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem describing a failure to find a resource definition
    /// </summary>
    public static readonly Uri DefinitionNotFound = new(BaseUri, "definition-not-found");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem describing a failure to find a specific version of a resource definition
    /// </summary>
    public static readonly Uri VersionNotFound = new(BaseUri, "version-not-found");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem describing a failure to find a the storage version of a resource definition
    /// </summary>
    public static readonly Uri StorageVersionNotFound = new(BaseUri, "storage-version-not-found");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem describing an error due to an unsupported sub resource 
    /// </summary>
    public static readonly Uri UnsupportedSubResource = new(BaseUri, "unsupported-subresource");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem describing an error due to an invalid patch
    /// </summary>
    public static readonly Uri InvalidPatch = new(BaseUri, "invalid-patch");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem describing an error due to an invalid sub resource patch
    /// </summary>
    public static readonly Uri InvalidSubResourcePatch = new(BaseUri, "invalid-subresource-patch");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem due to the '.metadata.resourceVersion' property not being set in the context of a replace operation
    /// </summary>
    public static readonly Uri ResourceVersionRequired = new(BaseUri, "resource-version-required");
    /// <summary>
    /// Gets the <see cref="Uri"/> that describes a problem due to the failure of an optimistic concurrency check on a resource
    /// </summary>
    public static readonly Uri OptimisticConcurrencyCheckFailed = new(BaseUri, "concurrency-check-failed");

}
