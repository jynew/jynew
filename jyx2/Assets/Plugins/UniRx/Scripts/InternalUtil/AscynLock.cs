// this code is borrowed from RxOfficial(rx.codeplex.com) and modified

using System;
using System.Collections.Generic;

namespace UniRx.InternalUtil
{
    /// <summary>
    /// Asynchronous lock.
    /// </summary>
    internal sealed class AsyncLock : IDisposable
    {
        private readonly Queue<Action> queue = new Queue<Action>();
        private bool isAcquired = false;
        private bool hasFaulted = false;

        /// <summary>
        /// Queues the action for execution. If the caller acquires the lock and becomes the owner,
        /// the queue is processed. If the lock is already owned, the action is queued and will get
        /// processed by the owner.
        /// </summary>
        /// <param name="action">Action to queue for execution.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public void Wait(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            var isOwner = false;
            lock (queue)
            {
                if (!hasFaulted)
                {
                    queue.Enqueue(action);
                    isOwner = !isAcquired;
                    isAcquired = true;
                }
            }

            if (isOwner)
            {
                while (true)
                {
                    var work = default(Action);
                    lock (queue)
                    {
                        if (queue.Count > 0)
                            work = queue.Dequeue();
                        else
                        {
                            isAcquired = false;
                            break;
                        }
                    }

                    try
                    {
                        work();
                    }
                    catch
                    {
                        lock (queue)
                        {
                            queue.Clear();
                            hasFaulted = true;
                        }
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Clears the work items in the queue and drops further work being queued.
        /// </summary>
        public void Dispose()
        {
            lock (queue)
            {
                queue.Clear();
                hasFaulted = true;
            }
        }
    }
}
