#if UNITY_METRO

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#if NETFX_CORE
using System.Threading.Tasks;
#endif

namespace UniRx
{
    public static partial class Scheduler
    {
        public static readonly IScheduler ThreadPool = new ThreadPoolScheduler();

        class ThreadPoolScheduler : IScheduler
        {
            public DateTimeOffset Now
            {
                get { return Scheduler.Now; }
            }

            public IDisposable Schedule(Action action)
            {
                var d = new BooleanDisposable();
#if NETFX_CORE

                Task.Run(()=>
                {
                    if (!d.IsDisposed)
                    {
                        action();
                    }
                });

#else
                Action act = () =>
                {
                    if (!d.IsDisposed)
                    {
                        action();
                    }
                };

                act.BeginInvoke(ar => act.EndInvoke(ar), null);

#endif

                return d;
            }

            public IDisposable Schedule(TimeSpan dueTime, Action action)
            {
                var wait = Scheduler.Normalize(dueTime);

                var d = new BooleanDisposable();

#if NETFX_CORE

                Task.Run(()=>
                {
                    if (!d.IsDisposed)
                    {
                        if (wait.Ticks > 0)
                        {
                            Thread.Sleep(wait);
                        }
                        action();
                    }
                });

#else

                Action act = () =>
                {
                    if (!d.IsDisposed)
                    {
                        if (wait.Ticks > 0)
                        {
                            Thread.Sleep(wait);
                        }
                        action();
                    }
                };

                act.BeginInvoke(ar => act.EndInvoke(ar), null);

#endif

                return d;
            }
        }
    }
}

#endif