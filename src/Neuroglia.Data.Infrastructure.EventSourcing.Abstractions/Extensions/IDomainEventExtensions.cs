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

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Defines extensions for <see cref="IDomainEvent"/>s
/// </summary>
public static class IDomainEventExtensions
{

    /// <summary>
    /// Gets the <see cref="IDomainEvent"/>'s type name
    /// </summary>
    /// <param name="e">The <see cref="IDomainEvent"/> to get the type name of</param>
    /// <returns>The <see cref="IDomainEvent"/>'s type name</returns>
    public static string GetTypeName(this IDomainEvent e)
    {
        ArgumentNullException.ThrowIfNull(e);
        return e.GetType().Name.Replace("DomainEvent", "").SplitCamelCase().ToKebabCase();
    }

}
