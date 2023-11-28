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

namespace Neuroglia;

/// <summary>
/// Represents the attribute used to mark a type as the version that should a specific type should be migrated to
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class MigrateFromAttribute
    : Attribute
{

    /// <summary>
    /// Initializes a new <see cref="MigrateFromAttribute"/>
    /// </summary>
    /// <param name="sourceType">The type that should be migrated to the marked class</param>
    public MigrateFromAttribute(Type sourceType) => this.SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));

    /// <summary>
    /// Gets the type that should be migrated to the marked class
    /// </summary>
    public virtual Type SourceType { get; }

}