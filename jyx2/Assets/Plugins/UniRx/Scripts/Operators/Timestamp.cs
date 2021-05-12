using System;

namespace UniRx.Operators
{
    internal class TimestampObservable<T> : OperatorObservableBase<Timestamped<T>>
    {
        readonly IObservable<T> source;
        readonly IScheduler scheduler;

        public TimestampObservable(IObservable<T> source, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<Timestamped<T>> observer, IDisposable cancel)
        {
            return source.Subscribe(new Timestamp(this, observer, cancel));
        }

        class Timestamp : OperatorObserverBase<T, Timestamped<T>>
        {
            readonly TimestampObservable<T> parent;

            public Timestamp(TimestampObservable<T> parent, IObserver<Timestamped<T>> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(new Timestamped<T>(value, parent.scheduler.Now));
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