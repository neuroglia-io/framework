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

using Neuroglia.Data.Infrastructure.ResourceOriented.Services;

namespace Neuroglia.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Defines extensions for <see cref="IResourceValidator"/>s
/// </summary>
public static class IResourceValidatorExtensions
{

    /// <summary>
    /// Determines wheter or not the <see cref="IResourceValidator"/> applies to an operation performed on the specified resource kind
    /// </summary>
    /// <param name="mutator">The <see cref="IResourceValidator"/> to check</param>
    /// <param name="request">The <see cref="AdmissionReviewRequest"/> to evaluate</param>
    /// <returns>A boolean indicating wheter or not the <see cref="IResourceValidator"/> supports the specified resource kind</returns>
    public static bool AppliesTo(this IResourceValidator mutator, AdmissionReviewRequest request) => mutator.AppliesTo(request.Operation, request.Resource.Definition.Group, request.Resource.Definition.Version, request.Resource.Definition.Plural, request.Resource.Namespace);

}
