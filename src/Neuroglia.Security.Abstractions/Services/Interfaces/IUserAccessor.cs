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

using System.Security.Claims;

namespace Neuroglia.Security.Services;

/// <summary>
/// Defines the fundamentals of a service used to access the current user
/// </summary>
public interface IUserAccessor
{

    /// <summary>
    /// Gets the current <see cref="ClaimsPrincipal"/>, if any
    /// </summary>
    ClaimsPrincipal? User { get; }

}
