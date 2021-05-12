using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace UniRx
{
    public static partial class Scheduler
    {
        public static readonly IScheduler Immediate = new ImmediateScheduler();

        class ImmediateScheduler : IScheduler
        {
            public ImmediateScheduler()
            {
            }

            public DateTimeOffset Now
            {
                get { return Scheduler.Now; }
            }

            public IDisposable Schedule(Action action)
            {
                action();
                return Disposable.Empty;
            }

            public IDisposable Schedule(TimeSpan dueTime, Action action)
            {
                var wait = Scheduler.Normalize(dueTime);
                if (wait.Ticks > 0)
                {
                    Thread.Sleep(wait);
                }

                action();
                return Disposable.Empty;
            }
        }
    }
}