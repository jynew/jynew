using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx.Operators
{
    internal class SampleObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly TimeSpan interval;
        readonly IScheduler scheduler;

        public SampleObservable(IObservable<T> source, TimeSpan interval, IScheduler scheduler)
            : base(source.IsRequiredSubscribeOnCurrentThread() || scheduler == Scheduler.CurrentThread)
        {
            this.source = source;
            this.interval = interval;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new Sample(this, observer, cancel).Run();
        }

        class Sample : OperatorObserverBase<T, T>
        {
            readonly SampleObservable<T> parent;
            readonly object gate = new object();
            T latestValue = default(T);
            bool isUpdated = false;
            bool isCompleted = false;
            SingleAssignmentDisposable sourceSubscription;

            public Sample(SampleObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                sourceSubscription = new SingleAssignmentDisposable();
                sourceSubscription.Disposable = parent.source.Subscribe(this);


                IDisposable scheduling;
                var periodicScheduler = parent.scheduler as ISchedulerPeriodic;
                if (periodicScheduler != null)
                {
                    scheduling = periodicScheduler.SchedulePeriodic(parent.interval, OnNextTick);
                }
                else
                {
                    scheduling = parent.scheduler.Schedule(parent.interval, OnNextRecursive);
                }

                return StableCompositeDisposable.Create(sourceSubscription, scheduling);
            }

            void OnNextTick()
            {
                lock (gate)
                {
                    if (isUpdated)
                    {
                        var value = latestValue;
                        isUpdated = false;
                        observer.OnNext(value);
                    }
                    if (isCompleted)
                    {
                        try { observer.OnCompleted(); } finally { Dispose(); }
                    }
                }
            }

            void OnNextRecursive(Action<TimeSpan> self)
            {
                lock (gate)
                {
                    if (isUpdated)
                    {
                        var value = latestValue;
                        isUpdated = false;
                        observer.OnNext(value);
                    }
                    if (isCompleted)
                    {
                        try { observer.OnCompleted(); } finally { Dispose(); }
                    }
                }
                self(parent.interval);
            }

            public override void OnNext(T value)
            {
                lock (gate)
                {
                    latestValue = value;
                    isUpdated = true;
                }
            }

            public override void OnError(Exception error)
            {
                lock (gate)
                {
                    try { base.observer.OnError(error); } finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                lock (gate)
                {
                    isCompleted = true;
                    sourceSubscription.Dispose();
                }
            }
        }
    }

    internal class SampleObservable<T, T2> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IObservable<T2> intervalSource;

        public SampleObservable(IObservable<T> source, IObservable<T2> intervalSource)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.intervalSource = intervalSource;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new Sample(this, observer, cancel).Run();
        }

        class Sample : OperatorObserverBase<T, T>
        {
            readonly SampleObservable<T, T2> parent;
            readonly object gate = new object();
            T latestValue = default(T);
            bool isUpdated = false;
            bool isCompleted = false;
            SingleAssignmentDisposable sourceSubscription;

            public Sample(
                SampleObservable<T, T2> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                sourceSubscription = new SingleAssignmentDisposable();
                sourceSubscription.Disposable = parent.source.Subscribe(this);

                var scheduling = this.parent.intervalSource.Subscribe(new SampleTick(this));

                return StableCompositeDisposable.Create(sourceSubscription, scheduling);
            }

            public override void OnNext(T value)
            {
                lock (gate)
                {
                    latestValue = value;
                    isUpdated = true;
                }
            }

            public override void OnError(Exception error)
            {
                lock (gate)
                {
                    try { base.observer.OnError(error); } finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                lock (gate)
                {
                    isCompleted = true;
                    sourceSubscription.Dispose();
                }
            }

            class SampleTick : IObserver<T2>
            {
                readonly Sample parent;

                public SampleTick(Sample parent)
                {
                    this.parent = parent;
                }

                public void OnCompleted()
                {
                    lock (parent.gate)
                    {
                        if (parent.isUpdated)
                        {
                            parent.isUpdated = false;
                            parent.observer.OnNext(parent.latestValue);
                        }
                        if (parent.isCompleted)
                        {
                            try { parent.observer.OnCompleted(); } finally { parent.Dispose(); }
                        }
                    }
                }

                public void OnError(Exception error)
                {
                }

                public void OnNext(T2 _)
                {
                    lock (parent.gate)
                    {
                        if (parent.isUpdated)
                        {
                            var value = parent.latestValue;
                            parent.isUpdated = false;
                            parent.observer.OnNext(value);
                        }
                        if (parent.isCompleted)
                        {
                            try { parent.observer.OnCompleted(); } finally { parent.Dispose(); }
                        }
                    }
                }
            }
        }
    }
}