using System;

namespace UniRx.Operators
{
    internal class RepeatObservable<T> : OperatorObservableBase<T>
    {
        readonly T value;
        readonly int? repeatCount;
        readonly IScheduler scheduler;

        public RepeatObservable(T value, int? repeatCount, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.value = value;
            this.repeatCount = repeatCount;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            observer = new Repeat(observer, cancel);

            if (repeatCount == null)
            {
                return scheduler.Schedule((Action self) =>
                {
                    observer.OnNext(value);
                    self();
                });
            }
            else
            {
                if (scheduler == Scheduler.Immediate)
                {
                    var count = this.repeatCount.Value;
                    for (int i = 0; i < count; i++)
                    {
                        observer.OnNext(value);
                    }
                    observer.OnCompleted();
                    return Disposable.Empty;
                }
                else
                {
                    var currentCount = this.repeatCount.Value;
                    return scheduler.Schedule((Action self) =>
                    {
                        if (currentCount > 0)
                        {
                            observer.OnNext(value);
                            currentCount--;
                        }

                        if (currentCount == 0)
                        {
                            observer.OnCompleted();
                            return;
                        }

                        self();
                    });
                }
            }
        }

        class Repeat : OperatorObserverBase<T, T>
        {
            public Repeat(IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
            }

            public override void OnNext(T value)
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