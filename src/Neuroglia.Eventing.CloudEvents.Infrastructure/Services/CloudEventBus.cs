// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Reactive.Subjects;

namespace Neuroglia.Eventing.CloudEvents.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of the <see cref="ICloudEventBus"/> interface
/// </summary>
public class CloudEventBus
    : ICloudEventBus
{

    bool _disposed;

    /// <summary>
    /// Gets the stream of events ingested by the application
    /// </summary>
    public Subject<CloudEvent> InputStream { get; } = new();

    /// <summary>
    /// Gets the stream of events published by the application
    /// </summary>
    public Subject<CloudEvent> OutputStream { get; } = new();

    ISubject<CloudEvent> ICloudEventBus.InputStream => this.InputStream;

    ISubject<CloudEvent> ICloudEventBus.OutputStream => this.OutputStream;

    /// <summary>
    /// Disposes of the <see cref="CloudEventBus"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="CloudEventBus"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this._disposed) return;
        if (disposing)
        {
            this.InputStream.Dispose();
            this.OutputStream.Dispose();
        }
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}
