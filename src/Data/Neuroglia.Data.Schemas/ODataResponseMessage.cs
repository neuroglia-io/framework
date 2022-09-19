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

using Microsoft.Data.OData;
using System.Net.Mime;

namespace Neuroglia.Data.Services
{
    internal class ODataResponseMessage 
        : IODataResponseMessage, IODataResponseMessageAsync, IDisposable
    {

        /// <summary>
        /// Initializes a new <see cref="ODataResponseMessage"/>
        /// </summary>
        /// <param name="stream">The underlying <see cref="Stream"/></param>
        private ODataResponseMessage(Stream stream)
        {
            this.Stream = stream;
        }

        private IDictionary<string, string> _headers = new Dictionary<string, string>();
        IEnumerable<KeyValuePair<string, string>> IODataResponseMessage.Headers => _headers;

        int IODataResponseMessage.StatusCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Gets the underlying <see cref="Stream"/>
        /// </summary>
        protected Stream Stream { get; }

        string IODataResponseMessage.GetHeader(string headerName)
        {
            if (this._headers.TryGetValue(headerName, out var headerValue))
                return headerValue;
            return headerName switch
            {
                "DataServiceVersion" => "1.0",
                "Content-Type" => MediaTypeNames.Application.Xml,
                _ => null!
            };
        }

        Stream IODataResponseMessage.GetStream()
        {
            return this.Stream;
        }


        async Task<Stream> IODataResponseMessageAsync.GetStreamAsync() => await Task.FromResult(this.Stream);

        void IODataResponseMessage.SetHeader(string headerName, string headerValue)
        {
            throw new NotImplementedException();
        }

        private bool _disposed;
        /// <summary>
        /// Disposes of the <see cref="ODataResponseMessage"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="ODataResponseMessage"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this.Stream.Dispose();
                }
                this._disposed = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Reads a new <see cref="ODataResponseMessage"/> from the specified <see cref="System.IO.Stream"/>
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> to read the <see cref="ODataResponseMessage"/> from</param>
        /// <param name="headers">An <see cref="IDictionary{TKey, TValue}"/> containing the ODATA headers, if any</param>
        /// <returns>A new <see cref="ODataResponseMessage"/></returns>
        public static ODataResponseMessage ReadFrom(Stream stream, IDictionary<string, string> headers)
        {
            return new(stream) { _headers = headers };
        }

    }

}
