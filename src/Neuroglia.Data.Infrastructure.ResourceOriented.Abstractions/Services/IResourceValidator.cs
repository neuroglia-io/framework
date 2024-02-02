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
/// Defines the fundamentals of a service used to validate <see cref="Resource"/>s
/// </summary>
public interface IResourceValidator
{

    /// <summary>
    /// Determines wheter or not the <see cref="IResourceValidator"/> applies to an operation performed on the specified resource kind
    /// </summary>
    /// <param name="operation">The operation being performed</param>
    /// <param name="group">The API group the resource being admitted belons to</param>
    /// <param name="version">The version of the kind of the resource being admitted</param>
    /// <param name="plural">The plural name of the kind of resource being admitted</param>
    /// <param name="namespace">The namespace the resource being admitted belongs to, if any</param>
    /// <returns>A boolean indicating wheter or not the <see cref="IResourceValidator"/> supports the specified resource kind</returns>
    bool AppliesTo(Operation operation, string group, string version, string plural, string? @namespace = null);

    /// <summary>
    /// Validates the specified resource
    /// </summary>
    /// <param name="context">The context in which to perform the resource's validation</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="AdmissionReviewResponse"/> that describes the result of the operation</returns>
    Task<AdmissionReviewResponse> ValidateAsync(AdmissionReviewRequest context, CancellationToken cancellationToken = default);

}
