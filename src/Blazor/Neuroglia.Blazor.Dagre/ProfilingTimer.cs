using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neuroglia.Blazor.Dagre
{
    /// <summary>
    /// A timer used to perform basic profiling
    /// </summary>
    public class ProfilingTimer
        : IDisposable
    {
        private static int NextId { get; set; } = 1;
        private int id { get; }
        private string label { get; }
        private Stopwatch stopwatch { get; }
        private int stackDepth { get; set; }
        private int captureCount { get; set;  }
        private List<StackFrame> stackFrames { get; set; }
        private string stackNames => string.Join(" <- ", this.stackFrames.Select(f => f.GetMethod()?.Name ?? "Unk"));

        private bool disposed = false;

        /// <summary>
        /// Creates a new <see cref="ProfilingTimer"/>
        /// </summary>
        /// <param name="label">The timer label</param>
        /// <param name="stackDepth">The stack trace depth to display</param>
        public ProfilingTimer(string label, int stackDepth = 1)
        {
            this.label = label;
            this.stopwatch = Stopwatch.StartNew();
            this.id = NextId++;
            this.captureCount = 0;
            this.stackDepth = stackDepth;
            this.stackFrames = new StackTrace().GetFrames().Skip(1).Take(stackDepth).ToList();
            Console.WriteLine($"Created timer {this.id} '{this.label}' - {this.stackNames}");
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        public void Start()
        {
            //Console.WriteLine($"Starting timer {this.id} '{this.label}' - {this.stackNames}");
            this.captureCount += 1;
            this.stopwatch.Restart();
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void Stop()
        {
            if (this.stopwatch.IsRunning)
            {
                this.stackFrames = new StackTrace().GetFrames().Skip(1).Take(this.stackDepth).ToList();
                this.stopwatch.Stop();
                Console.WriteLine($"Stopped timer {this.id} '{this.label}' after {this.stopwatch.ElapsedMilliseconds} ms - {this.stackNames}");
            }
        }

        /// <summary>
        /// Disposes the resources
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (this.stopwatch.IsRunning)
                    {
                        this.Stop();
                        Console.WriteLine($"Disposed timer {this.id} '{this.label}' - {this.stackNames}");
                    }
                }
                disposed = true;
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
}
