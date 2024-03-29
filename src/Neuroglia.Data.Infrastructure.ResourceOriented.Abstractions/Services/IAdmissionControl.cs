﻿// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
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
/// Defines the fundamentals of a service used to determine whether or not to admit operations on <see cref="Resource"/>s
/// </summary>
public interface IAdmissionControl
{

    /// <summary>
    /// Reviews the specified resource operation for admission
    /// </summary>
    /// <param name="request">The request to review</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="AdmissionReviewResponse"/> that describes the admission review result</returns>
    Task<AdmissionReviewResponse> ReviewAsync(AdmissionReviewRequest request, CancellationToken cancellationToken = default);

}