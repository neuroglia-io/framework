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

using System.Text;

namespace Neuroglia.Data.SchemaRegistry
{

    /// <summary>
    /// Defines helpers methods for generating hashes
    /// </summary>
    public static class HashHelper
    {

        /// <summary>
        /// Generates a new hash using the specified <see cref="HashAlgorithm"/>
        /// </summary>
        /// <param name="hashAlgorithm">The <see cref="HashAlgorithm"/> to use</param>
        /// <param name="value">The value to hash</param>
        /// <returns>The hashed value</returns>
        public static string Hash(HashAlgorithm hashAlgorithm, string value)
        {
            return hashAlgorithm switch
            {
                HashAlgorithm.SHA256 => SHA256Hash(value),
                HashAlgorithm.MD5 => MD5Hash(value),
                _ => throw new NotSupportedException($"The specified {nameof(HashAlgorithm)} '{hashAlgorithm}' is not supported")
            };
        }

        /// <summary>
        /// Generates a new SHA256 hash
        /// </summary>
        /// <param name="value">The value to hash</param>
        /// <returns>The hashed value</returns>
        public static string SHA256Hash(string value)
        {
            return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
        }

        /// <summary>
        /// Generates a new MD5 hash
        /// </summary>
        /// <param name="value">The value to hash</param>
        /// <returns>The hashed value</returns>
        public static string MD5Hash(string value)
        {
            return Convert.ToHexString(System.Security.Cryptography.MD5.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
        }

    }

}
