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
    /// Represents the default implementation of the <see cref="IOperationResult"/> interface
    /// </summary>
    public class OperationResult
        : IOperationResult
    {

        /// <summary>
        /// Initializes a new <see cref="OperationResult"/>
        /// </summary>
        protected OperationResult()
        {

        }


        /// <summary>
        /// Initializes a new <see cref="OperationResult"/>
        /// </summary>
        /// <param name="code">The <see cref="OperationResult"/>'s code</param>
        /// <param name="errors">An array of <see cref="Error"/>s that have occured during the operation's execution</param>
        public OperationResult(string code, params Error[] errors)
        {
            this.Code = code;
            this._Errors = errors == null ? new List<Error>() : errors.ToList();
        }

        /// <summary>
        /// Initializes a new <see cref="OperationResult"/>
        /// </summary>
        /// <param name="code">The <see cref="OperationResult"/>'s code</param>
        public OperationResult(string code)
            : this(code, Array.Empty<Error>())
        {

        }

        /// <inheritdoc/>
        public virtual string Code { get; protected set; }

        private List<Error> _Errors = new();
        /// <inheritdoc/>
        public virtual IEnumerable<Error> Errors
        {
            get
            {
                return this._Errors.AsReadOnly();
            }
            protected set
            {
                this._Errors = value.ToList();
            }
        }

        /// <inheritdoc/>
        public virtual bool Returned => this.Data != null;

        /// <inheritdoc/>
        public virtual object Data { get; protected set; }

        /// <inheritdoc/>
        public virtual bool Succeeded => !this.Errors.Any();

        /// <summary>
        /// Adds an error to the <see cref="OperationResult"/>
        /// </summary>
        /// <param name="code">The error code</param>
        /// <param name="message">The error message</param>
        /// <returns>The <see cref="OperationResult"/></returns>
        public OperationResult AddError(string code, string message)
        {
            this._Errors.Add(new Error(code, message));
            return this;
        }

        IOperationResult IOperationResult.AddError(string key, string message)
        {
            return this.AddError(key, message);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Code;
        }

    }

    /// <summary>
    /// Represents the default implementation of the <see cref="IOperationResult"/> interface
    /// </summary>
    /// <typeparam name="T">The type of data returned by the operation</typeparam>
    public class OperationResult<T>
        : OperationResult, IOperationResult<T>
    {

        /// <summary>
        /// Initializes a new <see cref="OperationResult"/>
        /// </summary>
        /// <param name="code">The <see cref="OperationResult"/>'s code</param>
        /// <param name="errors">An array of <see cref="Error"/>s that have occured during the operation's execution</param>
        public OperationResult(string code, params Error[] errors)
            : base(code, errors)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="OperationResult"/>
        /// </summary>
        /// <param name="code">The <see cref="OperationResult"/>'s code</param>
        /// <param name="data">The data returned by the operation</param>
        public OperationResult(string code, T data)
            : base(code)
        {
            this.Data = data;
        }

        /// <summary>
        /// Initializes a new <see cref="OperationResult"/>
        /// </summary>
        /// <param name="data">The data returned by the operation</param>
        public OperationResult(T data)
            : this(OperationResultCode.Ok, data)
        {
           
        }

        /// <inheritdoc/>
        public virtual new T Data
        {
            get
            {
                return (T)base.Data;
            }
            protected set
            {
                base.Data = value;
            }
        }

        /// <summary>
        /// Adds an error to the <see cref="OperationResult{T}"/>
        /// </summary>
        /// <param name="code">The error code</param>
        /// <param name="message">The error message</param>
        /// <returns>The <see cref="OperationResult{T}"/></returns>
        new public OperationResult<T> AddError(string code, string message)
        {
            base.AddError(code, message);
            return this;
        }

        IOperationResult<T> IOperationResult<T>.AddError(string key, string message)
        {
            return this.AddError(key, message);
        }

    }

}
