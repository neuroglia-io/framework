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
using System.Collections.Generic;
using System.Reflection;

namespace Neuroglia
{
    /// <summary>
    /// Exposes helper methods to help discriminate types
    /// </summary>
    public static class TypeDiscriminator
    {

        private static Dictionary<Type, TypeDiscriminatorInfo> Mappings = new();

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> used to get the discriminator value of the specified type
        /// </summary>
        /// <param name="type">The type to get the discriminator property of</param>
        /// <returns>The <see cref="PropertyInfo"/> used to get the discriminator value of the specified type</returns>
        public static PropertyInfo GetDiscriminatorProperty(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!Mappings.TryGetValue(type, out var discriminatorInfo))
                discriminatorInfo = MapDiscriminatedType(type);
            if (discriminatorInfo == null)
                throw new Exception($"The specified type '{type.FullName}' cannot be discriminated. Make sure that it is marked with the '{nameof(DiscriminatorAttribute)}', and that its concretions are marked with the '{nameof(DiscriminatorValueAttribute)}'");
            return discriminatorInfo.Property;
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> used to get the discriminator value of the specified type
        /// </summary>
        /// <typeparam name="T">The type to get the discriminator property of</typeparam>
        /// <returns>The <see cref="PropertyInfo"/> used to get the discriminator value of the specified type</returns>
        public static PropertyInfo GetDiscriminatorProperty<T>()
        {
            return GetDiscriminatorProperty(typeof(T));
        }

        /// <summary>
        /// Discriminates the specified type
        /// </summary>
        /// <param name="type">The type to discriminate</param>
        /// <param name="discriminatorValue">The value used to discriminate the specified type</param>
        /// <returns>The discriminated type</returns>
        public static Type Discriminate(Type type, string discriminatorValue)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));
            if (discriminatorValue == null)
                throw new ArgumentNullException(nameof(discriminatorValue));
            if (!Mappings.TryGetValue(type, out var discriminatorInfo))
                discriminatorInfo = MapDiscriminatedType(type);
            if (discriminatorInfo == null)
                throw new Exception($"The specified type '{type.FullName}' cannot be discriminated. Make sure that it is marked with the '{nameof(DiscriminatorAttribute)}', and that its concretions are marked with the '{nameof(DiscriminatorValueAttribute)}'");
            return discriminatorInfo.Mappings[discriminatorValue];
        }

        /// <summary>
        /// Discriminates the specified type
        /// </summary>
        /// <typeparam name="T">The type to discriminate</typeparam>
        /// <param name="discriminatorValue">The value used to discriminate the specified type</param>
        /// <returns>The discriminated type</returns>
        public static Type Discriminate<T>(string discriminatorValue)
        {
            return Discriminate(typeof(T), discriminatorValue);
        }

        private static TypeDiscriminatorInfo MapDiscriminatedType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!type.TryGetCustomAttribute(out DiscriminatorAttribute discriminatorAttribute))
                return null;
            var property = type.GetProperty(discriminatorAttribute.Property, BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (property == null)
                throw new MissingMemberException($"Failed to find the discriminator property with name '{property.Name}'");
            var mappings = new Dictionary<string, Type>();
            foreach (var derivedType in TypeCacheUtil.FindFilteredTypes($"nposm:json-polymorph:{type.Name}",
                (t) => t.IsClass && !t.IsAbstract && t.BaseType == type))
            {
                var discriminatorValueAttribute = derivedType.GetCustomAttribute<DiscriminatorValueAttribute>();
                if (discriminatorValueAttribute == null)
                    continue;
                var discriminatorValue = null as string;
                if (discriminatorValueAttribute.Value.GetType().IsEnum)
                    discriminatorValue = EnumHelper.Stringify((Enum)discriminatorValueAttribute.Value, property.PropertyType);
                else
                    discriminatorValue = discriminatorValueAttribute.Value.ToString();
                mappings.Add(discriminatorValue, derivedType);
            }
            var discriminator = new TypeDiscriminatorInfo(property, mappings);
            Mappings.Add(type, discriminator);
            return discriminator;
        }

        class TypeDiscriminatorInfo
        {

            public TypeDiscriminatorInfo(PropertyInfo property, IDictionary<string, Type> mappings)
            {
                this.Property = property ?? throw new ArgumentNullException(nameof(property));
                this.Mappings = mappings ?? throw new ArgumentNullException(nameof(mappings));
            }

            public PropertyInfo Property { get; }

            public IDictionary<string, Type> Mappings { get; }

        }

    }

}