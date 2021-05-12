using System;

namespace UniRx.Operators
{
    internal class TimeoutObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly TimeSpan? dueTime;
        readonly DateTimeOffset? dueTimeDT;
        readonly IScheduler scheduler;

        public TimeoutObservable(IObservable<T> source, TimeSpan dueTime, IScheduler scheduler) 
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.dueTime = dueTime;
            this.scheduler = scheduler;
        }

        public TimeoutObservable(IObservable<T> source, DateTimeOffset dueTime, IScheduler scheduler) 
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.dueTimeDT = dueTime;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (dueTime != null)
            {
                return new Timeout(this, observer, cancel).Run();
            }
            else
            {
                return new Timeout_(this, observer, cancel).Run();
            }
        }

        class Timeout : OperatorObserverBase<T, T>
        {
            readonly TimeoutObservable<T> parent;
            readonly object gate = new object();
            ulong objectId = 0ul;
            bool isTimeout = false;
            SingleAssignmentDisposable sourceSubscription;
            SerialDisposable timerSubscription;

            public Timeout(TimeoutObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                sourceSubscription = new SingleAssignmentDisposable();
                timerSubscription = new SerialDisposable();
                timerSubscription.Disposable = RunTimer(objectId);
                sourceSubscription.Disposable = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(timerSubscription, sourceSubscription);
            }

            IDisposable RunTimer(ulong timerId)
            {
                return parent.scheduler.Schedule(parent.dueTime.Value, () =>
                {
                    lock (gate)
                    {
                        if (objectId == timerId)
                        {
                            isTimeout = true;
                        }
                    }
                    if (isTimeout)
                    {
                        try { observer.OnError(new TimeoutException()); } finally { Dispose(); }
                    }
                });
            }

            public override void OnNext(T value)
            {
                ulong useObjectId;
                bool timeout;
                lock (gate)
                {
                    timeout = isTimeout;
                    objectId++;
                    useObjectId = objectId;
                }
                if (timeout) return;

                timerSubscription.Disposable = Disposable.Empty; // cancel old timer
                observer.OnNext(value);
                timerSubscription.Disposable = RunTimer(useObjectId);
            }

            public override void OnError(Exception error)
            {
                bool timeout;
                lock (gate)
                {
                    timeout = isTimeout;
                    objectId++;
                }
                if (timeout) return;

                timerSubscription.Dispose();
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                bool timeout;
                lock (gate)
                {
                    timeout = isTimeout;
                    objectId++;
                }
                if (timeout) return;

                timerSubscription.Dispose();
                try { observer.OnCompleted(); } finally { Dispose(); }
            }
        }

        class Timeout_ : OperatorObserverBase<T, T>
        {
            readonly TimeoutObservable<T> parent;
            readonly object gate = new object();
            bool isFinished = false;
            SingleAssignmentDisposable sourceSubscription;
            IDisposable timerSubscription;

            public Timeout_(TimeoutObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                sourceSubscription = new SingleAssignmentDisposable();

                timerSubscription = parent.scheduler.Schedule(parent.dueTimeDT.Value, OnNext);
                sourceSubscription.Disposable = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(timerSubscription, sourceSubscription);
            }

            // in timer
            void OnNext()
            {
                lock (gate)
                {
                    if (isFinished) return;
                    isFinished = true;
                }

                sourceSubscription.Dispose();
                try { observer.OnError(new TimeoutException()); } finally { Dispose(); }
            }

            public override void OnNext(T value)
            {
                lock (gate)
                {
                    if (!isFinished) observer.OnNext(value);
                }
            }

            public override void OnError(Exception error)
            {
                lock (gate)
                {
                    if (isFinished) return;
                    isFinished = true;
                    timerSubscription.Dispose();
                }
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {

                lock (gate)
                {
                    if (!isFinished)
                    {
                        isFinished = true;
                        timerSubscription.Dispose();
                    }
                    try { observer.OnCompleted(); } finally { Dispose(); }
                }
            }
        }
    }
}