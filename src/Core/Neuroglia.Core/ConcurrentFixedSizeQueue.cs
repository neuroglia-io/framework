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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace System.Linq
{
    /// <summary>
    /// Represents a queue of a fixed size. Adding elements after the queue's capacity has been reached triggers dequeuing
    /// </summary>
    /// <typeparam name="T">The type of element the <see cref="ConcurrentFixedSizeQueue{T}"/> is made out of</typeparam>
    /// <remarks>Code taken from <see href="https://stackoverflow.com/a/49821753/3637555">Josh's answer</see></remarks>
    public class ConcurrentFixedSizeQueue<T>
        : IEnumerable<T>
    {

        /// <summary>
        /// Initializes a new <see cref="ConcurrentFixedSizeQueue{T}"/>
        /// </summary>
        /// <param name="capacity">The <see cref="ConcurrentFixedSizeQueue{T}"/>'s capacity</param>
        public ConcurrentFixedSizeQueue(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            this.Queue = new(new List<T>(capacity));
            this.Capacity = capacity;
        }

        /// <summary>
        /// Gets the <see cref="ConcurrentQueue{T}"/>
        /// </summary>
        protected ConcurrentQueue<T> Queue { get; }

        /// <summary>
        /// Gets the <see cref="ConcurrentFixedSizeQueue{T}"/>'s capacity
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// Enqueues the specified element
        /// </summary>
        /// <param name="elem">The element to enqueue</param>
        public virtual void Enqueue(T elem)
        {
            while (this.Queue.Count + 1 > Capacity)
            {
                if (!this.Queue.TryDequeue(out _))
                    throw new InvalidOperationException("Concurrent dequeue operation failed");
            }
            this.Queue.Enqueue(elem);
        }

        /// <summary>
        /// Dequeues the <see cref="ConcurrentFixedSizeQueue{T}"/>
        /// </summary>
        /// <returns>The first element in queue</returns>
        public virtual T Dequeue()
        {
            if (this.Queue.TryDequeue(out var result))
            {
                return result;
            }

            return default;
        }

        /// <inheritdoc/>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return this.Queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    
    }

}
