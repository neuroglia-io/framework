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
using System.Linq;
using System.Text.RegularExpressions;

namespace Neuroglia
{
    /// <summary>
    /// Represents an address
    /// </summary>
    public class Address
        : ValueObject
    {

        private static readonly Regex InterpolationArgumentsExpression = new(@"\{([^}]+)\}", RegexOptions.Compiled);

        /// <summary>
        /// Gets the default address format
        /// </summary>
        public const string DefaultFormat = "{street} {streetNumber}, {postalCode} {city}, {country}";

        /// <summary>
        /// Initializes a new <see cref="Address"/>
        /// </summary>
        protected Address()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Address"/>
        /// </summary>
        /// <param name="street">The <see cref="Address"/>'s street</param>
        /// <param name="streetNumber"><see cref="Address"/>'s street number</param>
        /// <param name="postalCode"><see cref="Address"/>'s postal code</param>
        /// <param name="city"><see cref="Address"/>'s city</param>
        /// <param name="country"><see cref="Address"/>'s country</param>
        public Address(string street, string streetNumber, string postalCode, string city, string country)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentNullException(nameof(street));
            if (string.IsNullOrWhiteSpace(streetNumber))
                throw new ArgumentNullException(nameof(streetNumber));
            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentNullException(nameof(postalCode));
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentNullException(nameof(city));
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentNullException(nameof(country));
            this.Street = street;
            this.StreetNumber = streetNumber;
            this.PostalCode = postalCode;
            this.City = city;
            this.Country = country;
        }

        /// <summary>
        /// Gets <see cref="Address"/>'s street
        /// </summary>
        public virtual string Street { get; protected set; }

        /// <summary>
        /// Gets <see cref="Address"/>'s street number
        /// </summary>
        public virtual string StreetNumber { get; protected set; }

        /// <summary>
        /// Gets <see cref="Address"/>'s postal code
        /// </summary>
        public virtual string PostalCode { get; protected set; }

        /// <summary>
        /// Gets <see cref="Address"/>'s city
        /// </summary>
        public virtual string City { get; protected set; }

        /// <summary>
        /// Gets <see cref="Address"/>'s country
        /// </summary>
        public virtual string Country { get; protected set; }

        /// <inheritdoc/>
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.Street;
            yield return this.StreetNumber;
            yield return this.PostalCode;
            yield return this.City;
            yield return this.Country;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.ToString(DefaultFormat);
        }

        /// <summary>
        /// Formats the <see cref="Address"/> according to the specified format
        /// </summary>
        /// <param name="format">The <see cref="Address"/>'s format</param>
        /// <returns>The formatted address</returns>
        public virtual string ToString(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
                throw new ArgumentNullException(nameof(format));
            List<string> matches = InterpolationArgumentsExpression.Matches(format)
                 .Select(m => m.Value)
                 .Distinct()
                 .ToList();
            object[] args = new object[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                switch (matches[i].ToLower())
                {
                    case "{street}":
                        args[i] = this.Street;
                        break;
                    case "{streetnumber}":
                        args[i] = this.StreetNumber;
                        break;
                    case "{postalcode}":
                        args[i] = this.PostalCode;
                        break;
                    case "{city}":
                        args[i] = this.City;
                        break;
                    case "{country}":
                        args[i] = this.Country;
                        break;
                    default:
                        continue;
                }
            }
            return StringExtensions.Format(format, args);
        }

    }

}
