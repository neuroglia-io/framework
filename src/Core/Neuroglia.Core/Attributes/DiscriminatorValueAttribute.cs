﻿/*
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

namespace Neuroglia;

/// <summary>
/// Represents the <see cref="Attribute"/> used to indicate the discriminator value of a derived type
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DiscriminatorValueAttribute
    : Attribute
{

    /// <summary>
    /// Initializes a new <see cref="DiscriminatorValueAttribute"/>
    /// </summary>
    /// <param name="value">The value used to discriminate the derived type marked by the <see cref="DiscriminatorValueAttribute"/></param>
    public DiscriminatorValueAttribute(object value)
    {
        this.Value = value;
    }

    /// <summary>
    /// Gets the value used to discriminate the derived type marked by the <see cref="DiscriminatorValueAttribute"/>
    /// </summary>
    public object Value { get; }

}
