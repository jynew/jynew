using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class ToObservableObservable<T> : OperatorObservableBase<T>
    {
        readonly IEnumerable<T> source;
        readonly IScheduler scheduler;

        public ToObservableObservable(IEnumerable<T> source, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.source = source;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new ToObservable(this, observer, cancel).Run();
        }

        class ToObservable : OperatorObserverBase<T, T>
        {
            readonly ToObservableObservable<T> parent;

            public ToObservable(ToObservableObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var e = default(IEnumerator<T>);
                try
                {
                    e = parent.source.GetEnumerator();
                }
                catch (Exception exception)
                {
                    OnError(exception);
                    return Disposable.Empty;
                }

                if (parent.scheduler == Scheduler.Immediate)
                {
                    while (true)
                    {
                        bool hasNext;
                        var current = default(T);
                        try
                        {
                            hasNext = e.MoveNext();
                            if (hasNext) current = e.Current;
                        }
                        catch (Exception ex)
                        {
                            e.Dispose();
                            try { observer.OnError(ex); }
                            finally { Dispose(); }
                            break;
                        }

                        if (hasNext)
                        {
                            observer.OnNext(current);
                        }
                        else
                        {
                            e.Dispose();
                            try { observer.OnCompleted(); }
                            finally { Dispose(); }
                            break;
                        }
                    }

                    return Disposable.Empty;
                }

                var flag = new SingleAssignmentDisposable();
                flag.Disposable = parent.scheduler.Schedule(self =>
                {
                    if (flag.IsDisposed)
                    {
                        e.Dispose();
                        return;
                    }

                    bool hasNext;
                    var current = default(T);
                    try
                    {
                        hasNext = e.MoveNext();
                        if (hasNext) current = e.Current;
                    }
                    catch (Exception ex)
                    {
                        e.Dispose();
                        try { observer.OnError(ex); }
                        finally { Dispose(); }
                        return;
                    }

                    if (hasNext)
                    {
                        observer.OnNext(current);
                        self();
                    }
                    else
                    {
                        e.Dispose();
                        try { observer.OnCompleted(); }
                        finally { Dispose(); }
                    }
                });

                return flag;
            }

            public override void OnNext(T value)
            {
                // do nothing
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }
    }
}