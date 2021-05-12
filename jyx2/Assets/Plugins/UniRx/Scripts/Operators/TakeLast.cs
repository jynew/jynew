using System;
using System.Collections.Generic;
using UniRx.Operators;

namespace UniRx.Operators
{
    internal class TakeLastObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;

        // count
        readonly int count;

        // duration
        readonly TimeSpan duration;
        readonly IScheduler scheduler;

        public TakeLastObservable(IObservable<T> source, int count)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.count = count;
        }

        public TakeLastObservable(IObservable<T> source, TimeSpan duration, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.duration = duration;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (scheduler == null)
            {
                return new TakeLast(this, observer, cancel).Run();
            }
            else
            {
                return new TakeLast_(this, observer, cancel).Run();
            }
        }

        // count
        class TakeLast : OperatorObserverBase<T, T>
        {
            readonly TakeLastObservable<T> parent;
            readonly Queue<T> q;

            public TakeLast(TakeLastObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.q = new Queue<T>();
            }

            public IDisposable Run()
            {
                return parent.source.Subscribe(this);
            }

            public override void OnNext(T value)
            {
                q.Enqueue(value);
                if (q.Count > parent.count)
                {
                    q.Dequeue();
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                foreach (var item in q)
                {
                    observer.OnNext(item);
                }
                try { observer.OnCompleted(); } finally { Dispose(); }
            }
        }

        // time
        class TakeLast_ : OperatorObserverBase<T, T>
        {
            DateTimeOffset startTime;
            readonly TakeLastObservable<T> parent;
            readonly Queue<TimeInterval<T>> q;

            public TakeLast_(TakeLastObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.q = new Queue<TimeInterval<T>>();
            }

            public IDisposable Run()
            {
                startTime = parent.scheduler.Now;
                return parent.source.Subscribe(this);
            }

            public override void OnNext(T value)
            {
                var now = parent.scheduler.Now;
                var elapsed = now - startTime;
                q.Enqueue(new TimeInterval<T>(value, elapsed));
                Trim(elapsed);
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); };
            }

            public override void OnCompleted()
            {
                var now = parent.scheduler.Now;
                var elapsed = now - startTime;
                Trim(elapsed);

                foreach (var item in q)
                {
                    observer.OnNext(item.Value);
                }
                try { observer.OnCompleted(); } finally { Dispose(); };
            }

            void Trim(TimeSpan now)
            {
                while (q.Count > 0 && now - q.Peek().Interval >= parent.duration)
                {
                    q.Dequeue();
                }
            }
        }
    }
}