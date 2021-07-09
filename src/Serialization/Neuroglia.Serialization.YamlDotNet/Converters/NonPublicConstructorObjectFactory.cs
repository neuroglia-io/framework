/*
 * Copyright © 2021 Neuroglia SPRL. All rights reserved.
 * <p>
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * <p>
 * http://www.apache.org/licenses/LICENSE-2.0
 * <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using System;
using System.Reflection;
using YamlDotNet.Serialization;

namespace Neuroglia.Serialization
{

    /// <summary>
    /// Represents an <see cref="IObjectFactory"/> implementation that can create instance of objects with non-public parameterless constructors
    /// </summary>
    public class NonPublicConstructorObjectFactory
        : IObjectFactory
    {

        /// <inheritdoc/>
        public virtual object Create(Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            ConstructorInfo constructor = type.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                Type.DefaultBinder,
                Type.EmptyTypes,
                null);
            if (constructor.IsPublic)
                return Activator.CreateInstance(type);
            return constructor.Invoke(null);
        }

    }

}
