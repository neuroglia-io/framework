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

namespace Neuroglia
{
    /// <summary>
    /// Represents a currency
    /// </summary>
    public class Currency
        : ValueObject
    {

        /// <summary>
        /// Initializes a new <see cref="Currency"/>
        /// </summary>
        protected Currency()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Currency"/>
        /// </summary>
        /// <param name="code">The <see cref="Currency"/>'s three letter <see href="https://en.wikipedia.org/wiki/ISO_4217">ISO 4217</see> code (ex: EUR)</param>
        /// <param name="number">The <see cref="Currency"/>'s three digit <see href="https://en.wikipedia.org/wiki/ISO_4217">ISO 4217</see> number (ex: 978)</param>
        /// <param name="exponent">The <see cref="Currency"/>'s <see href="https://en.wikipedia.org/wiki/ISO_4217">ISO 4217</see> exponent (ex: 2)</param>
        /// <param name="name">The <see cref="Currency"/>'s name (ex: EURO)</param>
        /// <param name="symbol">The <see cref="Currency"/>'s name (ex: €)</param>
        public Currency(string code, int number, int exponent, string name, string symbol)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));
            if (!code.IsAlphabetic() || code.Length != 3)
                throw new ArgumentException($"The specified value '{code}' is not a valid three letter ISO 4217 currency code", nameof(code));
            if (number <= 0 || number > 999)
                throw new ArgumentOutOfRangeException(nameof(number));
            if(exponent < 0 || exponent > 4)
                throw new ArgumentOutOfRangeException(nameof(exponent));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentNullException(nameof(symbol));
            this.Code = code;
            this.Number = number;
            this.Exponent = exponent;
            this.Name = name;
            this.Symbol = symbol;
        }

        /// <summary>
        /// Gets the <see cref="Currency"/>'s <see href="https://en.wikipedia.org/wiki/ISO_4217">ISO 4217</see> code
        /// </summary>
        public virtual string Code { get; protected set; }

        /// <summary>
        /// Gets the <see cref="Currency"/>'s <see href="https://en.wikipedia.org/wiki/ISO_4217">ISO 4217</see> number
        /// </summary>
        public virtual int Number { get; protected set; }

        /// <summary>
        /// Gets the number of digits after the decimal separator
        /// </summary>
        public virtual int Exponent { get; protected set; }

        /// <summary>
        /// Gets the <see cref="Currency"/>'s name
        /// </summary>
        public virtual string Name { get; protected set; }

        /// <summary>
        /// Gets the <see cref="Currency"/>'s symbol
        /// </summary>
        public virtual string Symbol { get; protected set; }

        /// <inheritdoc/>
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.Code;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Code;
        }

        #region Collection

        /// <summary>
        /// Gets the AUD <see cref="Currency"/>
        /// </summary>
        public static Currency AustralianDollar = new("AUD", 36, 2, "Australian Dollar", "AU$");

        /// <summary>
        /// Gets the BRL <see cref="Currency"/>
        /// </summary>
        public static Currency BrazilianReal = new("BRL", 986, 2, "Brazilian Real", "R$");

        /// <summary>
        /// Gets the EUR <see cref="Currency"/>
        /// </summary>
        public static Currency Euro = new("EUR", 978, 2, "Euro", "€");

        /// <summary>
        /// Gets the GBP <see cref="Currency"/>
        /// </summary>
        public static Currency PoundSterling = new("GBP", 826,2, "Pound Sterling", "£");

        /// <summary>
        /// Gets the RUB <see cref="Currency"/>
        /// </summary>
        public static Currency RussianRuble = new("RUB", 643, 2, "Russian Ruble", "₽");

        /// <summary>
        /// Gets the USD <see cref="Currency"/>
        /// </summary>
        public static Currency UnitedStatesDollar = new("USD", 840, 2, "United States Dollar", "US$");

        /// <summary>
        /// Gets the default <see cref="Currency"/> collection
        /// </summary>
        public static IEnumerable<Currency> DefaultCollection
        {
            get
            {
                yield return AustralianDollar;
                yield return BrazilianReal;
                yield return Euro;
                yield return PoundSterling;
                yield return RussianRuble;
                yield return UnitedStatesDollar;
            }
        }

        #endregion

    }

}
