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

namespace Neuroglia
{
    /// <summary>
    /// Enumerates all types of response codes
    /// </summary>
    public class OperationResultCode
        : Enumeration
    {

        /// <summary>
        /// Initializes a new <see cref="OperationResultCode"/>
        /// </summary>
        protected OperationResultCode()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="OperationResultCode"/>
        /// </summary>
        /// <param name="value">The <see cref="OperationResultCode"/>'s value</param>
        /// <param name="name">The <see cref="OperationResultCode"/>'s name</param>
        public OperationResultCode(int value, string name)
            : base(value, name)
        {

        }

        /// <summary>
        /// Indicates a succesfull result
        /// </summary>
        public static OperationResultCode Ok = new(200, "OK");

        /// <summary>
        /// Indicates an invalid request
        /// </summary>
        public static OperationResultCode Invalid = new(400, "INVALID");

        /// <summary>
        /// Indicates that an object could not be found
        /// </summary>
        public static OperationResultCode NotFound = new(404, "NOT_FOUND");

        /// <summary>
        /// Indicates that an object was unexpectedly not modified
        /// </summary>
        public static OperationResultCode NotModified = new(304, "NOT_MODIFIED");

        /// <summary>
        /// Indicates that an object was unexpectedly not modified
        /// </summary>
        public static OperationResultCode Forbidden = new(403, "FORBIDDEN");

        /// <summary>
        /// Indicates an internal error
        /// </summary>
        public static OperationResultCode InternalError = new(500, "INTERNAL_ERROR");

    }

}
