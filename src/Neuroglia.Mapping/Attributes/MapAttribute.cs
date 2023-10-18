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

namespace Neuroglia.Mapping;

/// <summary>
/// Represents an <see cref="Attribute"/> used to mark a type as mappable to another 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class MapAttribute
    : Attribute
{

    /// <summary>
    /// Initializes a new <see cref="MapAttribute"/>
    /// </summary>
    /// <param name="destinationType">The type to which the class is mappable</param>
    public MapAttribute(Type destinationType) { this.DestinationType = destinationType ?? throw new ArgumentNullException(nameof(destinationType)); }

    /// <summary>
    /// Gets the type to which the class is mappable
    /// </summary>
    public virtual Type DestinationType { get; }

}