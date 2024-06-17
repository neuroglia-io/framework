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

using System.Diagnostics;

namespace Neuroglia.Blazor.Dagre;

/// <summary>
/// A timer used to perform basic profiling
/// </summary>
public class ProfilingTimer
    : IDisposable
{

    static int NextId { get; set; } = 1;
    int Id { get; }
    string Label { get; }
    Stopwatch Stopwatch { get; }
    int StackDepth { get; set; }
    int CaptureCount { get; set;  }
    List<StackFrame> StackFrames { get; set; }
    string StackNames => string.Join(" <- ", this.StackFrames.Select(f => f.GetMethod()?.Name ?? "Unk"));

    private bool _disposed = false;

    /// <summary>
    /// Creates a new <see cref="ProfilingTimer"/>
    /// </summary>
    /// <param name="label">The timer label</param>
    /// <param name="stackDepth">The stack trace depth to display</param>
    public ProfilingTimer(string label, int stackDepth = 1)
    {
        this.Label = label;
        this.Stopwatch = Stopwatch.StartNew();
        this.Id = NextId++;
        this.CaptureCount = 0;
        this.StackDepth = stackDepth;
        this.StackFrames = new StackTrace().GetFrames().Skip(1).Take(stackDepth).ToList();
        Console.WriteLine($"Created timer {this.Id} '{this.Label}' - {this.StackNames}");
    }

    /// <summary>
    /// Starts the timer
    /// </summary>
    public void Start()
    {
        //Console.WriteLine($"Starting timer {this.id} '{this.label}' - {this.stackNames}");
        this.CaptureCount += 1;
        this.Stopwatch.Restart();
    }

    /// <summary>
    /// Stops the timer
    /// </summary>
    public void Stop()
    {
        if (this.Stopwatch.IsRunning)
        {
            this.StackFrames = new StackTrace().GetFrames().Skip(1).Take(this.StackDepth).ToList();
            this.Stopwatch.Stop();
            Console.WriteLine($"Stopped timer {this.Id} '{this.Label}' after {this.Stopwatch.ElapsedMilliseconds} ms - {this.StackNames}");
        }
    }

    /// <summary>
    /// Disposes the resources
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (this.Stopwatch.IsRunning)
                {
                    this.Stop();
                    Console.WriteLine($"Disposed timer {this.Id} '{this.Label}' - {this.StackNames}");
                }
            }
            _disposed = true;
        }
    }

    /// <summary>
    /// Implements <see cref="IDisposable"/>
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
