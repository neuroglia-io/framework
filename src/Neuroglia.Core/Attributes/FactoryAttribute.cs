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
/// Represents the <see cref="Attribute"/> used to indicate the marked object should be instantiated using an <see cref="IFactory"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class FactoryAttribute
    : Attribute
{

    /// <summary>
    /// Initializes a new <see cref="FactoryAttribute"/>
    /// </summary>
    /// <param name="factoryType">The type of the <see cref="IFactory"/> to use. Must be a non-generic concrete implementation of the <see cref="IFactory"/> interface</param>
    public FactoryAttribute(Type factoryType)
    {
        if(!factoryType.IsClass || factoryType.IsAbstract || factoryType.IsInterface || factoryType.IsGenericType) throw new ArgumentException($"The specified type '{factoryType.FullName}' is not a non-generic concrete implementation of the {nameof(IFactory)} interface", nameof(factoryType));
        this.FactoryType = factoryType;
    }

    /// <summary>
    /// Gets the type of the <see cref="IFactory"/> to use. Must be a non-generic concrete implementation of the <see cref="IFactory"/> interface
    /// </summary>
    public virtual Type FactoryType { get; }

}
