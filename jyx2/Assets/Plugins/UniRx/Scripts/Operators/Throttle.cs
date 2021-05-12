using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx.Operators
{
    internal class ThrottleObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly TimeSpan dueTime;
        readonly IScheduler scheduler;

        public ThrottleObservable(IObservable<T> source, TimeSpan dueTime, IScheduler scheduler) 
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.dueTime = dueTime;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new Throttle(this, observer, cancel).Run();
        }

        class Throttle : OperatorObserverBase<T, T>
        {
            readonly ThrottleObservable<T> parent;
            readonly object gate = new object();
            T latestValue = default(T);
            bool hasValue = false;
            SerialDisposable cancelable;
            ulong id = 0;

            public Throttle(ThrottleObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                cancelable = new SerialDisposable();
                var subscription = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(cancelable, subscription);
            }

            void OnNext(ulong currentid)
            {
                lock (gate)
                {
                    if (hasValue && id == currentid)
                    {
                        observer.OnNext(latestValue);
                    }
                    hasValue = false;
                }
            }

            public override void OnNext(T value)
            {
                ulong currentid;
                lock (gate)
                {
                    hasValue = true;
                    latestValue = value;
                    id = unchecked(id + 1);
                    currentid = id;
                }

                var d = new SingleAssignmentDisposable();
                cancelable.Disposable = d;
                d.Disposable = parent.scheduler.Schedule(parent.dueTime, () => OnNext(currentid));
            }

            public override void OnError(Exception error)
            {
                cancelable.Dispose();

                lock (gate)
                {
                    hasValue = false;
                    id = unchecked(id + 1);
                    try { observer.OnError(error); } finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                cancelable.Dispose();

                lock (gate)
                {
                    if (hasValue)
                    {
                        observer.OnNext(latestValue);
                    }
                    hasValue = false;
                    id = unchecked(id + 1);
                    try { observer.OnCompleted(); } finally { Dispose(); }
                }
            }
        }
    }
}