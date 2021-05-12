// this code is borrowed from RxOfficial(rx.codeplex.com) and modified

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel;
using System.Threading;
using UniRx.InternalUtil;
using UniRx;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace UniRx
{

    public static partial class Scheduler
    {
        public static readonly IScheduler CurrentThread = new CurrentThreadScheduler();

        public static bool IsCurrentThreadSchedulerScheduleRequired { get { return CurrentThreadScheduler.IsScheduleRequired; } }

        /// <summary>
        /// Represents an object that schedules units of work on the current thread.
        /// </summary>
        /// <seealso cref="Scheduler.CurrentThread">Singleton instance of this type exposed through this static property.</seealso>
        class CurrentThreadScheduler : IScheduler
        {
            [ThreadStatic]
            static SchedulerQueue s_threadLocalQueue;

            [ThreadStatic]
            static Stopwatch s_clock;

            private static SchedulerQueue GetQueue()
            {
                return s_threadLocalQueue;
            }

            private static void SetQueue(SchedulerQueue newQueue)
            {
                s_threadLocalQueue = newQueue;
            }

            private static TimeSpan Time
            {
                get
                {
                    if (s_clock == null)
                        s_clock = Stopwatch.StartNew();

                    return s_clock.Elapsed;
                }
            }

            /// <summary>
            /// Gets a value that indicates whether the caller must call a Schedule method.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Advanced)]
            public static bool IsScheduleRequired
            {
                get
                {
                    return GetQueue() == null;
                }
            }

            public IDisposable Schedule(Action action)
            {
                return Schedule(TimeSpan.Zero, action);
            }

            public IDisposable Schedule(TimeSpan dueTime, Action action)
            {
                if (action == null)
                    throw new ArgumentNullException("action");

                var dt = Time + Scheduler.Normalize(dueTime);

                var si = new ScheduledItem(action, dt);

                var queue = GetQueue();

                if (queue == null)
                {
                    queue = new SchedulerQueue(4);
                    queue.Enqueue(si);

                    CurrentThreadScheduler.SetQueue(queue);
                    try
                    {
                        Trampoline.Run(queue);
                    }
                    finally
                    {
                        CurrentThreadScheduler.SetQueue(null);
                    }
                }
                else
                {
                    queue.Enqueue(si);
                }

                return si.Cancellation;
            }

            static class Trampoline
            {
                public static void Run(SchedulerQueue queue)
                {
                    while (queue.Count > 0)
                    {
                        var item = queue.Dequeue();
                        if (!item.IsCanceled)
                        {
                            var wait = item.DueTime - CurrentThreadScheduler.Time;
                            if (wait.Ticks > 0)
                            {
                                Thread.Sleep(wait);
                            }

                            if (!item.IsCanceled)
                                item.Invoke();
                        }
                    }
                }
            }

            public DateTimeOffset Now
            {
                get { return Scheduler.Now; }
            }
        }
    }
}

