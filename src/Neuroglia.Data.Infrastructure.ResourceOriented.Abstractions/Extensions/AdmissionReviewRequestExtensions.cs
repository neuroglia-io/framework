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
/// Defines extensions for <see cref="AdmissionReviewRequest"/>s
/// </summary>
public static class AdmissionReviewRequestExtensions
{

    /// <summary>
    /// Creates a new <see cref="Patch"/> based on difference between the updated state and the original state of the admitted resource
    /// </summary>
    /// <param name="request">The extended <see cref="IResource"/></param>
    /// <returns>A new <see cref="Patch"/></returns>
    public static Patch GetDiffPatch(this AdmissionReviewRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return new(PatchType.JsonPatch, JsonPatchUtility.CreateJsonPatchFromDiff(request.OriginalState, request.UpdatedState));
    }

}