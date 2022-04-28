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
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia
{
    /// <summary>
    /// Represents an object used to lock aynschronous processes
    /// </summary>
    /// <remarks>Code based on <see href="https://medium.com/swlh/async-lock-mechanism-on-asynchronous-programing-d43f15ad0b3"/></remarks>
    public class AsyncLock
    {

        private readonly SemaphoreSlim _Semaphore = new SemaphoreSlim(1, 1);
        private readonly Task<IDisposable> _Releaser;

        /// <summary>
        /// Initializes a new <see cref="AsyncLock"/>
        /// </summary>
        public AsyncLock()
        {
            this._Releaser = Task.FromResult((IDisposable)new Releaser(this));
        }

        /// <summary>
        /// Locks asynchronously
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new object which releases the lock upon disposal</returns>
        public Task<IDisposable> LockAsync(CancellationToken cancellationToken = default)
        {
            var waitTask = this._Semaphore.WaitAsync(cancellationToken);
            return waitTask.IsCompleted
                ? this._Releaser
                : waitTask.ContinueWith((_, state) => (IDisposable)state, this._Releaser.Result, cancellationToken,
            TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        private sealed class Releaser
            : IDisposable
        {

            private readonly AsyncLock _ToRelease;

            internal Releaser(AsyncLock toRelease)
            {
                this._ToRelease = toRelease;
            }

            public void Dispose()
            {
                this._ToRelease._Semaphore.Release();
            }

        }

    }

}
