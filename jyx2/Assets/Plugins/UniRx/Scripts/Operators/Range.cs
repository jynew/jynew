using System;

namespace UniRx.Operators
{
    internal class RangeObservable : OperatorObservableBase<int>
    {
        readonly int start;
        readonly int count;
        readonly IScheduler scheduler;

        public RangeObservable(int start, int count, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            if (count < 0) throw new ArgumentOutOfRangeException("count < 0");

            this.start = start;
            this.count = count;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<int> observer, IDisposable cancel)
        {
            observer = new Range(observer, cancel);

            if (scheduler == Scheduler.Immediate)
            {
                for (int i = 0; i < count; i++)
                {
                    int v = start + i;
                    observer.OnNext(v);
                }
                observer.OnCompleted();

                return Disposable.Empty;
            }
            else
            {
                var i = 0;
                return scheduler.Schedule((Action self) =>
                {
                    if (i < count)
                    {
                        int v = start + i;
                        observer.OnNext(v);
                        i++;
                        self();
                    }
                    else
                    {
                        observer.OnCompleted();
                    }
                });
            }
        }

        class Range : OperatorObserverBase<int, int>
        {
            public Range(IObserver<int> observer, IDisposable cancel)
                : base(observer, cancel)
            {
            }

            public override void OnNext(int value)
            {
                try
                {
                    base.observer.OnNext(value);
                }
                catch
                {
                    Dispose();
                    throw;
                }
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