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

namespace Neuroglia
{

    /// <summary>
    /// Represents object made of out equatable atomic values
    /// </summary>
    public abstract class ValueObject
        : IValueObject
    {

        /// <summary>
        /// Retrieves the atomic values the <see cref="ValueObject"/> is made out of
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/> containg the atomic values the <see cref="ValueObject"/> is made out of</returns>
        protected abstract IEnumerable<object> GetAtomicValues();

        /// <summary>
        /// Determines whether or not the <see cref="ValueObject"/> equals the specified value
        /// </summary>
        /// <param name="obj">The value to compare to the <see cref="ValueObject"/></param>
        /// <returns>A boolean indicating whether or not the <see cref="ValueObject"/> equals the specified value</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            ValueObject other = (ValueObject)obj;
            IEnumerator<object> thisValues = GetAtomicValues().GetEnumerator();
            IEnumerator<object> otherValues = other.GetAtomicValues().GetEnumerator();
            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                if (ReferenceEquals(thisValues.Current, null) ^
                    ReferenceEquals(otherValues.Current, null))
                    return false;
                if (thisValues.Current != null &&
                    !thisValues.Current.Equals(otherValues.Current))
                    return false;
            }
            return !thisValues.MoveNext() && !otherValues.MoveNext();
        }

        /// <summary>
        /// Serves as the default hash function
        /// </summary>
        /// <returns>A hash code for the current object</returns>
        public override int GetHashCode()
        {
            return this.GetAtomicValues()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

        /// <inheritdoc/>
        bool IEquatable<IValueObject>.Equals(IValueObject other)
        {
            if (other == null)
                return false;
            return this.Equals(other);
        }

        /// <summary>
        /// Determines whether or not the specified <see cref="ValueObject"/>s are equal
        /// </summary>
        /// <param name="left">A <see cref="ValueObject"/> to compare</param>
        /// <param name="right">A <see cref="ValueObject"/> to compare</param>
        /// <returns>A boolean indicating whether or not the specified <see cref="ValueObject"/>s are equal</returns>
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
                return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        /// <summary>
        /// Determines whether or not the specified <see cref="ValueObject"/>s are not equal
        /// </summary>
        /// <param name="left">A <see cref="ValueObject"/> to compare</param>
        /// <param name="right">A <see cref="ValueObject"/> to compare</param>
        /// <returns>A boolean indicating whether or not the specified <see cref="ValueObject"/>s are not equal</returns>
        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !(EqualOperator(left, right));
        }

        /// <summary>
        /// Determines whether or not the two specified values are equal
        /// </summary>
        /// <param name="left">A <see cref="ValueObject"/> to compare</param>
        /// <param name="right">A <see cref="ValueObject"/> to compare</param>
        /// <returns>A boolean indicating whether or not the two specified values are equal</returns>
        public static bool operator ==(ValueObject left, ValueObject right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
                return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether or not the two specified values are equal
        /// </summary>
        /// <param name="left">A <see cref="ValueObject"/> to compare</param>
        /// <param name="right">A <see cref="ValueObject"/> to compare</param>
        /// <returns>A boolean indicating whether or not the two specified values are equal</returns>
        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return !(left == right);
        }

    }

}
