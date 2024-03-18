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

namespace Neuroglia.Data.Infrastructure.ResourceOriented.Services;

/// <summary>
/// Represents the context of the conversion of a resource version
/// </summary>
/// <remarks>
/// Initializes a new <see cref="VersioningContext"/>
/// </remarks>
/// <param name="resourceReference">An object used to reference the <see cref="IResource"/> to evaluate</param>
/// <param name="resourceDefinition">The <see cref="IResourceDefinition"/> of the <see cref="IResource"/> to evaluate</param>
/// <param name="resource">The <see cref="IResource"/> to evaluate for admission</param>
public class VersioningContext(IResourceReference resourceReference, IResourceDefinition resourceDefinition, IResource resource)
     : IVersioningContext
{

    /// <summary>
    /// Gets an object used to reference the <see cref="IResource"/> to convert
    /// </summary>
    public IResourceReference ResourceReference { get; } = resourceReference ?? throw new ArgumentNullException(nameof(resourceReference));

    /// <summary>
    /// Gets the <see cref="IResourceDefinition"/> of the <see cref="IResource"/> to convert
    /// </summary>
    public IResourceDefinition ResourceDefinition { get; } = resourceDefinition ?? throw new ArgumentNullException(nameof(resourceDefinition));

    /// <summary>
    /// Gets the <see cref="IResource"/> to convert
    /// </summary>
    public IResource Resource { get; set; } = resource ?? throw new ArgumentNullException(nameof(resource));

}