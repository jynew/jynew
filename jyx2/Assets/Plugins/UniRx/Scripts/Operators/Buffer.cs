using System;
using System.Collections.Generic;
using UniRx.Operators;

namespace UniRx.Operators
{
    internal class BufferObservable<T> : OperatorObservableBase<IList<T>>
    {
        readonly IObservable<T> source;
        readonly int count;
        readonly int skip;

        readonly TimeSpan timeSpan;
        readonly TimeSpan timeShift;
        readonly IScheduler scheduler;

        public BufferObservable(IObservable<T> source, int count, int skip)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.count = count;
            this.skip = skip;
        }

        public BufferObservable(IObservable<T> source, TimeSpan timeSpan, TimeSpan timeShift, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.timeSpan = timeSpan;
            this.timeShift = timeShift;
            this.scheduler = scheduler;
        }

        public BufferObservable(IObservable<T> source, TimeSpan timeSpan, int count, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.timeSpan = timeSpan;
            this.count = count;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<IList<T>> observer, IDisposable cancel)
        {
            // count,skip
            if (scheduler == null)
            {
                if (skip == 0)
                {
                    return new Buffer(this, observer, cancel).Run();
                }
                else
                {
                    return new Buffer_(this, observer, cancel).Run();
                }
            }
            else
            {
                // time + count
                if (count > 0)
                {
                    return new BufferTC(this, observer, cancel).Run();
                }
                else
                {
                    if (timeSpan == timeShift)
                    {
                        return new BufferT(this, observer, cancel).Run();
                    }
                    else
                    {
                        return new BufferTS(this, observer, cancel).Run();
                    }
                }
            }
        }

        // count only
        class Buffer : OperatorObserverBase<T, IList<T>>
        {
            readonly BufferObservable<T> parent;
            List<T> list;

            public Buffer(BufferObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                list = new List<T>(parent.count);
                return parent.source.Subscribe(this);
            }

            public override void OnNext(T value)
            {
                list.Add(value);
                if (list.Count == parent.count)
                {
                    observer.OnNext(list);
                    list = new List<T>(parent.count);
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                if (list.Count > 0)
                {
                    observer.OnNext(list);
                }
                try { observer.OnCompleted(); } finally { Dispose(); }
            }
        }

        // count and skip
        class Buffer_ : OperatorObserverBase<T, IList<T>>
        {
            readonly BufferObservable<T> parent;
            Queue<List<T>> q;
            int index;

            public Buffer_(BufferObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                q = new Queue<List<T>>();
                index = -1;
                return parent.source.Subscribe(this);
            }

            public override void OnNext(T value)
            {
                index++;

                if (index % parent.skip == 0)
                {
                    q.Enqueue(new List<T>(parent.count));
                }

                var len = q.Count;
                for (int i = 0; i < len; i++)
                {
                    var list = q.Dequeue();
                    list.Add(value);
                    if (list.Count == parent.count)
                    {
                        observer.OnNext(list);
                    }
                    else
                    {
                        q.Enqueue(list);
                    }
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                foreach (var list in q)
                {
                    observer.OnNext(list);
                }
                try { observer.OnCompleted(); } finally { Dispose(); }
            }
        }

        // timespan = timeshift
        class BufferT : OperatorObserverBase<T, IList<T>>
        {
            static readonly T[] EmptyArray = new T[0];

            readonly BufferObservable<T> parent;
            readonly object gate = new object();

            List<T> list;

            public BufferT(BufferObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                list = new List<T>();

                var timerSubscription = Observable.Interval(parent.timeSpan, parent.scheduler)
                    .Subscribe(new Buffer(this));

                var sourceSubscription = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(timerSubscription, sourceSubscription);
            }

            public override void OnNext(T value)
            {
                lock (gate)
                {
                    list.Add(value);
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                List<T> currentList;
                lock (gate)
                {
                    currentList = list;
                }
                observer.OnNext(currentList);
                try { observer.OnCompleted(); } finally { Dispose(); }
            }

            class Buffer : IObserver<long>
            {
                BufferT parent;

                public Buffer(BufferT parent)
                {
                    this.parent = parent;
                }

                public void OnNext(long value)
                {
                    var isZero = false;
                    List<T> currentList;
                    lock (parent.gate)
                    {
                        currentList = parent.list;
                        if (currentList.Count != 0)
                        {
                            parent.list = new List<T>();
                        }
                        else
                        {
                            isZero = true;
                        }
                    }

                    parent.observer.OnNext((isZero) ? (IList<T>)EmptyArray : currentList);
                }

                public void OnError(Exception error)
                {
                }

                public void OnCompleted()
                {
                }
            }
        }

        // timespan + timeshift
        class BufferTS : OperatorObserverBase<T, IList<T>>
        {
            readonly BufferObservable<T> parent;
            readonly object gate = new object();

            Queue<IList<T>> q;
            TimeSpan totalTime;
            TimeSpan nextShift;
            TimeSpan nextSpan;
            SerialDisposable timerD;

            public BufferTS(BufferObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                totalTime = TimeSpan.Zero;
                nextShift = parent.timeShift;
                nextSpan = parent.timeSpan;

                q = new Queue<IList<T>>();

                timerD = new SerialDisposable();
                q.Enqueue(new List<T>());
                CreateTimer();

                var subscription = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(subscription, timerD);
            }

            void CreateTimer()
            {
                var m = new SingleAssignmentDisposable();
                timerD.Disposable = m;

                var isSpan = false;
                var isShift = false;
                if (nextSpan == nextShift)
                {
                    isSpan = true;
                    isShift = true;
                }
                else if (nextSpan < nextShift)
                    isSpan = true;
                else
                    isShift = true;

                var newTotalTime = isSpan ? nextSpan : nextShift;
                var ts = newTotalTime - totalTime;
                totalTime = newTotalTime;

                if (isSpan)
                    nextSpan += parent.timeShift;
                if (isShift)
                    nextShift += parent.timeShift;

                m.Disposable = parent.scheduler.Schedule(ts, () =>
                {
                    lock (gate)
                    {
                        if (isShift)
                        {
                            var s = new List<T>();
                            q.Enqueue(s);
                        }
                        if (isSpan)
                        {
                            var s = q.Dequeue();
                            observer.OnNext(s);
                        }
                    }

                    CreateTimer();
                });
            }

            public override void OnNext(T value)
            {
                lock (gate)
                {
                    foreach (var s in q)
                    {
                        s.Add(value);
                    }
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                lock (gate)
                {
                    foreach (var list in q)
                    {
                        observer.OnNext(list);
                    }

                    try { observer.OnCompleted(); } finally { Dispose(); }
                }
            }
        }

        // timespan + count
        class BufferTC : OperatorObserverBase<T, IList<T>>
        {
            static readonly T[] EmptyArray = new T[0]; // cache

            readonly BufferObservable<T> parent;
            readonly object gate = new object();

            List<T> list;
            long timerId;
            SerialDisposable timerD;

            public BufferTC(BufferObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                list = new List<T>();
                timerId = 0L;
                timerD = new SerialDisposable();

                CreateTimer();
                var subscription = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(subscription, timerD);
            }

            void CreateTimer()
            {
                var currentTimerId = timerId;
                var timerS = new SingleAssignmentDisposable();
                timerD.Disposable = timerS; // restart timer(dispose before)


                var periodicScheduler = parent.scheduler as ISchedulerPeriodic;
                if (periodicScheduler != null)
                {
                    timerS.Disposable = periodicScheduler.SchedulePeriodic(parent.timeSpan, () => OnNextTick(currentTimerId));
                }
                else
                {
                    timerS.Disposable = parent.scheduler.Schedule(parent.timeSpan, self => OnNextRecursive(currentTimerId, self));
                }
            }

            void OnNextTick(long currentTimerId)
            {
                var isZero = false;
                List<T> currentList;
                lock (gate)
                {
                    if (currentTimerId != timerId) return;

                    currentList = list;
                    if (currentList.Count != 0)
                    {
                        list = new List<T>();
                    }
                    else
                    {
                        isZero = true;
                    }
                }

                observer.OnNext((isZero) ? (IList<T>)EmptyArray : currentList);
            }

            void OnNextRecursive(long currentTimerId, Action<TimeSpan> self)
            {
                var isZero = false;
                List<T> currentList;
                lock (gate)
                {
                    if (currentTimerId != timerId) return;

                    currentList = list;
                    if (currentList.Count != 0)
                    {
                        list = new List<T>();
                    }
                    else
                    {
                        isZero = true;
                    }
                }

                observer.OnNext((isZero) ? (IList<T>)EmptyArray : currentList);
                self(parent.timeSpan);
            }

            public override void OnNext(T value)
            {
                List<T> currentList = null;
                lock (gate)
                {
                    list.Add(value);
                    if (list.Count == parent.count)
                    {
                        currentList = list;
                        list = new List<T>();
                        timerId++;
                        CreateTimer();
                    }
                }
                if (currentList != null)
                {
                    observer.OnNext(currentList);
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                List<T> currentList;
                lock (gate)
                {
                    timerId++;
                    currentList = list;
                }
                observer.OnNext(currentList);
                try { observer.OnCompleted(); } finally { Dispose(); }
            }
        }
    }

    internal class BufferObservable<TSource, TWindowBoundary> : OperatorObservableBase<IList<TSource>>
    {
        readonly IObservable<TSource> source;
        readonly IObservable<TWindowBoundary> windowBoundaries;

        public BufferObservable(IObservable<TSource> source, IObservable<TWindowBoundary> windowBoundaries)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.windowBoundaries = windowBoundaries;
        }

        protected override IDisposable SubscribeCore(IObserver<IList<TSource>> observer, IDisposable cancel)
        {
            return new Buffer(this, observer, cancel).Run();
        }

        class Buffer : OperatorObserverBase<TSource, IList<TSource>>
        {
            static readonly TSource[] EmptyArray = new TSource[0]; // cache

            readonly BufferObservable<TSource, TWindowBoundary> parent;
            object gate = new object();
            List<TSource> list;

            public Buffer(BufferObservable<TSource, TWindowBoundary> parent, IObserver<IList<TSource>> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                list = new List<TSource>();

                var sourceSubscription = parent.source.Subscribe(this);
                var windowSubscription = parent.windowBoundaries.Subscribe(new Buffer_(this));

                return StableCompositeDisposable.Create(sourceSubscription, windowSubscription);
            }

            public override void OnNext(TSource value)
            {
                lock (gate)
                {
                    list.Add(value);
                }
            }

            public override void OnError(Exception error)
            {
                lock (gate)
                {
                    try { observer.OnError(error); } finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                lock (gate)
                {
                    var currentList = list;
                    list = new List<TSource>(); // safe
                    observer.OnNext(currentList);
                    try { observer.OnCompleted(); } finally { Dispose(); }
                }
            }

            class Buffer_ : IObserver<TWindowBoundary>
            {
                readonly Buffer parent;

                public Buffer_(Buffer parent)
                {
                    this.parent = parent;
                }

                public void OnNext(TWindowBoundary value)
                {
                    var isZero = false;
                    List<TSource> currentList;
                    lock (parent.gate)
                    {
                        currentList = parent.list;
                        if (currentList.Count != 0)
                        {
                            parent.list = new List<TSource>();
                        }
                        else
                        {
                            isZero = true;
                        }
                    }
                    if (isZero)
                    {
                        parent.observer.OnNext(EmptyArray);
                    }
                    else
                    {
                        parent.observer.OnNext(currentList);
                    }
                }

                public void OnError(Exception error)
                {
                    parent.OnError(error);
                }

                public void OnCompleted()
                {
                    parent.OnCompleted();
                }
            }
        }
    }
}