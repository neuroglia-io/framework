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

namespace Neuroglia.Data.Guards;

/// <summary>
/// Represents an exception thrown due to invalid data that has been guarded against
/// </summary>
/// <remarks>
/// Initializes a new <see cref="GuardException"/>
/// </remarks>
/// <param name="message">The <see cref="GuardException"/>'s message</param>
/// <param name="argumentName">The name of the argument to throw the <see cref="GuardException"/> for</param>
public class GuardException(string? message, string? argumentName)
    : Exception(message)
{

    /// <summary>
    /// Gets the name of the argument, if any, that has been guarded against
    /// </summary>
    public virtual string? ArgumentName { get; } = argumentName;

}