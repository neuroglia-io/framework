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

namespace Neuroglia.Security.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IUserInfoProvider"/> interface
/// </summary>
/// <remarks>
/// Initializes a new <see cref="UserInfoProvider"/>
/// </remarks>
/// <param name="userAccessor">The service used to access the current user</param>
public class UserInfoProvider(IUserAccessor userAccessor)
    : IUserInfoProvider
{

    /// <summary>
    /// Gets the service used to access the current user
    /// </summary>
    protected IUserAccessor UserAccessor { get; } = userAccessor;

    /// <summary>
    /// Gets information about the current user
    /// </summary>
    /// <returns>A new object used to provide information about the current user</returns>
    public virtual UserInfo? GetCurrentUser()
    {
        if (UserAccessor.User == null || UserAccessor.User.Identity?.IsAuthenticated != true) return null;
        return new(UserAccessor.User.Identity.Name!, UserAccessor.User.Identity.AuthenticationType!, UserAccessor.User.Claims.ToDictionary(c => c.Type, c => c.Value));
    }

}