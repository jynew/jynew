using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    public delegate TR ZipFunc<T1, T2, T3, TR>(T1 arg1, T2 arg2, T3 arg3);
    public delegate TR ZipFunc<T1, T2, T3, T4, TR>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate TR ZipFunc<T1, T2, T3, T4, T5, TR>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    public delegate TR ZipFunc<T1, T2, T3, T4, T5, T6, TR>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    public delegate TR ZipFunc<T1, T2, T3, T4, T5, T6, T7, TR>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

    // binary
    internal class ZipObservable<TLeft, TRight, TResult> : OperatorObservableBase<TResult>
    {
        readonly IObservable<TLeft> left;
        readonly IObservable<TRight> right;
        readonly Func<TLeft, TRight, TResult> selector;

        public ZipObservable(IObservable<TLeft> left, IObservable<TRight> right, Func<TLeft, TRight, TResult> selector)
            : base(left.IsRequiredSubscribeOnCurrentThread() || right.IsRequiredSubscribeOnCurrentThread())
        {
            this.left = left;
            this.right = right;
            this.selector = selector;
        }

        protected override IDisposable SubscribeCore(IObserver<TResult> observer, IDisposable cancel)
        {
            return new Zip(this, observer, cancel).Run();
        }

        class Zip : OperatorObserverBase<TResult, TResult>
        {
            readonly ZipObservable<TLeft, TRight, TResult> parent;

            readonly object gate = new object();
            readonly Queue<TLeft> leftQ = new Queue<TLeft>();
            bool leftCompleted = false;
            readonly Queue<TRight> rightQ = new Queue<TRight>();
            bool rightCompleted = false;

            public Zip(ZipObservable<TLeft, TRight, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var l = parent.left.Subscribe(new LeftZipObserver(this));
                var r = parent.right.Subscribe(new RightZipObserver(this));

                return StableCompositeDisposable.Create(l, r, Disposable.Create(() =>
                {
                    lock (gate)
                    {
                        leftQ.Clear();
                        rightQ.Clear();
                    }
                }));
            }

            // dequeue is in the lock
            void Dequeue()
            {
                TLeft lv;
                TRight rv;
                TResult v;

                if (leftQ.Count != 0 && rightQ.Count != 0)
                {
                    lv = leftQ.Dequeue();
                    rv = rightQ.Dequeue();
                }
                else if (leftCompleted || rightCompleted)
                {
                    try { observer.OnCompleted(); }
                    finally { Dispose(); }
                    return;
                }
                else
                {
                    return;
                }

                try
                {
                    v = parent.selector(lv, rv);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); }
                    finally { Dispose(); }
                    return;
                }

                OnNext(v);
            }

            public override void OnNext(TResult value)
            {
                base.observer.OnNext(value);
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

            class LeftZipObserver : IObserver<TLeft>
            {
                readonly Zip parent;

                public LeftZipObserver(Zip parent)
                {
                    this.parent = parent;
                }

                public void OnNext(TLeft value)
                {
                    lock (parent.gate)
                    {
                        parent.leftQ.Enqueue(value);
                        parent.Dequeue();
                    }
                }

                public void OnError(Exception ex)
                {
                    lock (parent.gate)
                    {
                        parent.OnError(ex);
                    }
                }

                public void OnCompleted()
                {
                    lock (parent.gate)
                    {
                        parent.leftCompleted = true;
                        if (parent.rightCompleted) parent.OnCompleted();
                    }
                }
            }

            class RightZipObserver : IObserver<TRight>
            {
                readonly Zip parent;

                public RightZipObserver(Zip parent)
                {
                    this.parent = parent;
                }

                public void OnNext(TRight value)
                {
                    lock (parent.gate)
                    {
                        parent.rightQ.Enqueue(value);
                        parent.Dequeue();
                    }
                }

                public void OnError(Exception ex)
                {
                    lock (parent.gate)
                    {
                        parent.OnError(ex);
                    }
                }

                public void OnCompleted()
                {
                    lock (parent.gate)
                    {
                        parent.rightCompleted = true;
                        if (parent.leftCompleted) parent.OnCompleted();
                    }
                }
            }
        }
    }

    // array
    internal class ZipObservable<T> : OperatorObservableBase<IList<T>>
    {
        readonly IObservable<T>[] sources;

        public ZipObservable(IObservable<T>[] sources)
            : base(true)
        {
            this.sources = sources;
        }

        protected override IDisposable SubscribeCore(IObserver<IList<T>> observer, IDisposable cancel)
        {
            return new Zip(this, observer, cancel).Run();
        }

        class Zip : OperatorObserverBase<IList<T>, IList<T>>
        {
            readonly ZipObservable<T> parent;
            readonly object gate = new object();

            Queue<T>[] queues;
            bool[] isDone;
            int length;

            public Zip(ZipObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                length = parent.sources.Length;
                queues = new Queue<T>[length];
                isDone = new bool[length];

                for (int i = 0; i < length; i++)
                {
                    queues[i] = new Queue<T>();
                }

                var disposables = new IDisposable[length + 1];
                for (int i = 0; i < length; i++)
                {
                    var source = parent.sources[i];
                    disposables[i] = source.Subscribe(new ZipObserver(this, i));
                }

                disposables[length] = Disposable.Create(() =>
                {
                    lock (gate)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            var q = queues[i];
                            q.Clear();
                        }
                    }
                });

                return StableCompositeDisposable.CreateUnsafe(disposables);
            }

            // dequeue is in the lock
            void Dequeue(int index)
            {
                var allQueueHasValue = true;
                for (int i = 0; i < length; i++)
                {
                    if (queues[i].Count == 0)
                    {
                        allQueueHasValue = false;
                        break;
                    }
                }

                if (!allQueueHasValue)
                {
                    var allCompletedWithoutSelf = true;
                    for (int i = 0; i < length; i++)
                    {
                        if (i == index) continue;
                        if (!isDone[i])
                        {
                            allCompletedWithoutSelf = false;
                            break;
                        }
                    }

                    if (allCompletedWithoutSelf)
                    {
                        try { observer.OnCompleted(); }
                        finally { Dispose(); }
                        return;
                    }
                    else
                    {
                        return;
                    }
                }

                var array = new T[length];
                for (int i = 0; i < length; i++)
                {
                    array[i] = queues[i].Dequeue();
                }

                OnNext(array);
            }

            public override void OnNext(IList<T> value)
            {
                base.observer.OnNext(value);
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

            class ZipObserver : IObserver<T>
            {
                readonly Zip parent;
                readonly int index;

                public ZipObserver(Zip parent, int index)
                {
                    this.parent = parent;
                    this.index = index;
                }

                public void OnNext(T value)
                {
                    lock (parent.gate)
                    {
                        parent.queues[index].Enqueue(value);
                        parent.Dequeue(index);
                    }
                }

                public void OnError(Exception ex)
                {
                    lock (parent.gate)
                    {
                        parent.OnError(ex);
                    }
                }

                public void OnCompleted()
                {
                    lock (parent.gate)
                    {
                        parent.isDone[index] = true;
                        var allTrue = true;
                        for (int i = 0; i < parent.length; i++)
                        {
                            if (!parent.isDone[i])
                            {
                                allTrue = false;
                                break;
                            }
                        }

                        if (allTrue)
                        {
                            parent.OnCompleted();
                        }
                    }
                }
            }
        }
    }

    // Generated from UniRx.Console.ZipGenerator.tt
    #region NTH

    internal class ZipObservable<T1, T2, T3, TR> : OperatorObservableBase<TR>
    {
        IObservable<T1> source1;
        IObservable<T2> source2;
        IObservable<T3> source3;
        ZipFunc<T1, T2, T3, TR> resultSelector;

        public ZipObservable(
            IObservable<T1> source1,
            IObservable<T2> source2,
            IObservable<T3> source3,
              ZipFunc<T1, T2, T3, TR> resultSelector)
            : base(
                source1.IsRequiredSubscribeOnCurrentThread() ||
                source2.IsRequiredSubscribeOnCurrentThread() ||
                source3.IsRequiredSubscribeOnCurrentThread() ||
                false)
        {
            this.source1 = source1;
            this.source2 = source2;
            this.source3 = source3;
            this.resultSelector = resultSelector;
        }

        protected override IDisposable SubscribeCore(IObserver<TR> observer, IDisposable cancel)
        {
            return new Zip(this, observer, cancel).Run();
        }

        class Zip : NthZipObserverBase<TR>
        {
            readonly ZipObservable<T1, T2, T3, TR> parent;
            readonly object gate = new object();
            readonly Queue<T1> q1 = new Queue<T1>();
            readonly Queue<T2> q2 = new Queue<T2>();
            readonly Queue<T3> q3 = new Queue<T3>();

            public Zip(ZipObservable<T1, T2, T3, TR> parent, IObserver<TR> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                base.SetQueue(new System.Collections.ICollection[] { q1, q2, q3 });
                var s1 = parent.source1.Subscribe(new ZipObserver<T1>(gate, this, 0, q1));
                var s2 = parent.source2.Subscribe(new ZipObserver<T2>(gate, this, 1, q2));
                var s3 = parent.source3.Subscribe(new ZipObserver<T3>(gate, this, 2, q3));

                return StableCompositeDisposable.Create(s1, s2, s3, Disposable.Create(() =>
                {
                    lock (gate)
                    {
                        q1.Clear(); q2.Clear(); q3.Clear();
                    }
                }));
            }

            public override TR GetResult()
            {
                return parent.resultSelector(q1.Dequeue(), q2.Dequeue(), q3.Dequeue());
            }

            public override void OnNext(TR value)
            {
                base.observer.OnNext(value);
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


    internal class ZipObservable<T1, T2, T3, T4, TR> : OperatorObservableBase<TR>
    {
        IObservable<T1> source1;
        IObservable<T2> source2;
        IObservable<T3> source3;
        IObservable<T4> source4;
        ZipFunc<T1, T2, T3, T4, TR> resultSelector;

        public ZipObservable(
            IObservable<T1> source1,
            IObservable<T2> source2,
            IObservable<T3> source3,
            IObservable<T4> source4,
              ZipFunc<T1, T2, T3, T4, TR> resultSelector)
            : base(
                source1.IsRequiredSubscribeOnCurrentThread() ||
                source2.IsRequiredSubscribeOnCurrentThread() ||
                source3.IsRequiredSubscribeOnCurrentThread() ||
                source4.IsRequiredSubscribeOnCurrentThread() ||
                false)
        {
            this.source1 = source1;
            this.source2 = source2;
            this.source3 = source3;
            this.source4 = source4;
            this.resultSelector = resultSelector;
        }

        protected override IDisposable SubscribeCore(IObserver<TR> observer, IDisposable cancel)
        {
            return new Zip(this, observer, cancel).Run();
        }

        class Zip : NthZipObserverBase<TR>
        {
            readonly ZipObservable<T1, T2, T3, T4, TR> parent;
            readonly object gate = new object();
            readonly Queue<T1> q1 = new Queue<T1>();
            readonly Queue<T2> q2 = new Queue<T2>();
            readonly Queue<T3> q3 = new Queue<T3>();
            readonly Queue<T4> q4 = new Queue<T4>();

            public Zip(ZipObservable<T1, T2, T3, T4, TR> parent, IObserver<TR> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                base.SetQueue(new System.Collections.ICollection[] { q1, q2, q3, q4 });
                var s1 = parent.source1.Subscribe(new ZipObserver<T1>(gate, this, 0, q1));
                var s2 = parent.source2.Subscribe(new ZipObserver<T2>(gate, this, 1, q2));
                var s3 = parent.source3.Subscribe(new ZipObserver<T3>(gate, this, 2, q3));
                var s4 = parent.source4.Subscribe(new ZipObserver<T4>(gate, this, 3, q4));

                return StableCompositeDisposable.Create(s1, s2, s3, s4, Disposable.Create(() =>
                {
                    lock (gate)
                    {
                        q1.Clear(); q2.Clear(); q3.Clear(); q4.Clear();
                    }
                }));
            }

            public override TR GetResult()
            {
                return parent.resultSelector(q1.Dequeue(), q2.Dequeue(), q3.Dequeue(), q4.Dequeue());
            }

            public override void OnNext(TR value)
            {
                base.observer.OnNext(value);
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


    internal class ZipObservable<T1, T2, T3, T4, T5, TR> : OperatorObservableBase<TR>
    {
        IObservable<T1> source1;
        IObservable<T2> source2;
        IObservable<T3> source3;
        IObservable<T4> source4;
        IObservable<T5> source5;
        ZipFunc<T1, T2, T3, T4, T5, TR> resultSelector;

        public ZipObservable(
            IObservable<T1> source1,
            IObservable<T2> source2,
            IObservable<T3> source3,
            IObservable<T4> source4,
            IObservable<T5> source5,
              ZipFunc<T1, T2, T3, T4, T5, TR> resultSelector)
            : base(
                source1.IsRequiredSubscribeOnCurrentThread() ||
                source2.IsRequiredSubscribeOnCurrentThread() ||
                source3.IsRequiredSubscribeOnCurrentThread() ||
                source4.IsRequiredSubscribeOnCurrentThread() ||
                source5.IsRequiredSubscribeOnCurrentThread() ||
                false)
        {
            this.source1 = source1;
            this.source2 = source2;
            this.source3 = source3;
            this.source4 = source4;
            this.source5 = source5;
            this.resultSelector = resultSelector;
        }

        protected override IDisposable SubscribeCore(IObserver<TR> observer, IDisposable cancel)
        {
            return new Zip(this, observer, cancel).Run();
        }

        class Zip : NthZipObserverBase<TR>
        {
            readonly ZipObservable<T1, T2, T3, T4, T5, TR> parent;
            readonly object gate = new object();
            readonly Queue<T1> q1 = new Queue<T1>();
            readonly Queue<T2> q2 = new Queue<T2>();
            readonly Queue<T3> q3 = new Queue<T3>();
            readonly Queue<T4> q4 = new Queue<T4>();
            readonly Queue<T5> q5 = new Queue<T5>();

            public Zip(ZipObservable<T1, T2, T3, T4, T5, TR> parent, IObserver<TR> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                base.SetQueue(new System.Collections.ICollection[] { q1, q2, q3, q4, q5 });
                var s1 = parent.source1.Subscribe(new ZipObserver<T1>(gate, this, 0, q1));
                var s2 = parent.source2.Subscribe(new ZipObserver<T2>(gate, this, 1, q2));
                var s3 = parent.source3.Subscribe(new ZipObserver<T3>(gate, this, 2, q3));
                var s4 = parent.source4.Subscribe(new ZipObserver<T4>(gate, this, 3, q4));
                var s5 = parent.source5.Subscribe(new ZipObserver<T5>(gate, this, 4, q5));

                return StableCompositeDisposable.Create(s1, s2, s3, s4, s5, Disposable.Create(() =>
                {
                    lock (gate)
                    {
                        q1.Clear(); q2.Clear(); q3.Clear(); q4.Clear(); q5.Clear();
                    }
                }));
            }

            public override TR GetResult()
            {
                return parent.resultSelector(q1.Dequeue(), q2.Dequeue(), q3.Dequeue(), q4.Dequeue(), q5.Dequeue());
            }

            public override void OnNext(TR value)
            {
                base.observer.OnNext(value);
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


    internal class ZipObservable<T1, T2, T3, T4, T5, T6, TR> : OperatorObservableBase<TR>
    {
        IObservable<T1> source1;
        IObservable<T2> source2;
        IObservable<T3> source3;
        IObservable<T4> source4;
        IObservable<T5> source5;
        IObservable<T6> source6;
        ZipFunc<T1, T2, T3, T4, T5, T6, TR> resultSelector;

        public ZipObservable(
            IObservable<T1> source1,
            IObservable<T2> source2,
            IObservable<T3> source3,
            IObservable<T4> source4,
            IObservable<T5> source5,
            IObservable<T6> source6,
              ZipFunc<T1, T2, T3, T4, T5, T6, TR> resultSelector)
            : base(
                source1.IsRequiredSubscribeOnCurrentThread() ||
                source2.IsRequiredSubscribeOnCurrentThread() ||
                source3.IsRequiredSubscribeOnCurrentThread() ||
                source4.IsRequiredSubscribeOnCurrentThread() ||
                source5.IsRequiredSubscribeOnCurrentThread() ||
                source6.IsRequiredSubscribeOnCurrentThread() ||
                false)
        {
            this.source1 = source1;
            this.source2 = source2;
            this.source3 = source3;
            this.source4 = source4;
            this.source5 = source5;
            this.source6 = source6;
            this.resultSelector = resultSelector;
        }

        protected override IDisposable SubscribeCore(IObserver<TR> observer, IDisposable cancel)
        {
            return new Zip(this, observer, cancel).Run();
        }

        class Zip : NthZipObserverBase<TR>
        {
            readonly ZipObservable<T1, T2, T3, T4, T5, T6, TR> parent;
            readonly object gate = new object();
            readonly Queue<T1> q1 = new Queue<T1>();
            readonly Queue<T2> q2 = new Queue<T2>();
            readonly Queue<T3> q3 = new Queue<T3>();
            readonly Queue<T4> q4 = new Queue<T4>();
            readonly Queue<T5> q5 = new Queue<T5>();
            readonly Queue<T6> q6 = new Queue<T6>();

            public Zip(ZipObservable<T1, T2, T3, T4, T5, T6, TR> parent, IObserver<TR> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                base.SetQueue(new System.Collections.ICollection[] { q1, q2, q3, q4, q5, q6 });
                var s1 = parent.source1.Subscribe(new ZipObserver<T1>(gate, this, 0, q1));
                var s2 = parent.source2.Subscribe(new ZipObserver<T2>(gate, this, 1, q2));
                var s3 = parent.source3.Subscribe(new ZipObserver<T3>(gate, this, 2, q3));
                var s4 = parent.source4.Subscribe(new ZipObserver<T4>(gate, this, 3, q4));
                var s5 = parent.source5.Subscribe(new ZipObserver<T5>(gate, this, 4, q5));
                var s6 = parent.source6.Subscribe(new ZipObserver<T6>(gate, this, 5, q6));

                return StableCompositeDisposable.Create(s1, s2, s3, s4, s5, s6, Disposable.Create(() =>
                {
                    lock (gate)
                    {
                        q1.Clear(); q2.Clear(); q3.Clear(); q4.Clear(); q5.Clear(); q6.Clear();
                    }
                }));
            }

            public override TR GetResult()
            {
                return parent.resultSelector(q1.Dequeue(), q2.Dequeue(), q3.Dequeue(), q4.Dequeue(), q5.Dequeue(), q6.Dequeue());
            }

            public override void OnNext(TR value)
            {
                base.observer.OnNext(value);
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


    internal class ZipObservable<T1, T2, T3, T4, T5, T6, T7, TR> : OperatorObservableBase<TR>
    {
        IObservable<T1> source1;
        IObservable<T2> source2;
        IObservable<T3> source3;
        IObservable<T4> source4;
        IObservable<T5> source5;
        IObservable<T6> source6;
        IObservable<T7> source7;
        ZipFunc<T1, T2, T3, T4, T5, T6, T7, TR> resultSelector;

        public ZipObservable(
            IObservable<T1> source1,
            IObservable<T2> source2,
            IObservable<T3> source3,
            IObservable<T4> source4,
            IObservable<T5> source5,
            IObservable<T6> source6,
            IObservable<T7> source7,
              ZipFunc<T1, T2, T3, T4, T5, T6, T7, TR> resultSelector)
            : base(
                source1.IsRequiredSubscribeOnCurrentThread() ||
                source2.IsRequiredSubscribeOnCurrentThread() ||
                source3.IsRequiredSubscribeOnCurrentThread() ||
                source4.IsRequiredSubscribeOnCurrentThread() ||
                source5.IsRequiredSubscribeOnCurrentThread() ||
                source6.IsRequiredSubscribeOnCurrentThread() ||
                source7.IsRequiredSubscribeOnCurrentThread() ||
                false)
        {
            this.source1 = source1;
            this.source2 = source2;
            this.source3 = source3;
            this.source4 = source4;
            this.source5 = source5;
            this.source6 = source6;
            this.source7 = source7;
            this.resultSelector = resultSelector;
        }

        protected override IDisposable SubscribeCore(IObserver<TR> observer, IDisposable cancel)
        {
            return new Zip(this, observer, cancel).Run();
        }

        class Zip : NthZipObserverBase<TR>
        {
            readonly ZipObservable<T1, T2, T3, T4, T5, T6, T7, TR> parent;
            readonly object gate = new object();
            readonly Queue<T1> q1 = new Queue<T1>();
            readonly Queue<T2> q2 = new Queue<T2>();
            readonly Queue<T3> q3 = new Queue<T3>();
            readonly Queue<T4> q4 = new Queue<T4>();
            readonly Queue<T5> q5 = new Queue<T5>();
            readonly Queue<T6> q6 = new Queue<T6>();
            readonly Queue<T7> q7 = new Queue<T7>();

            public Zip(ZipObservable<T1, T2, T3, T4, T5, T6, T7, TR> parent, IObserver<TR> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                base.SetQueue(new System.Collections.ICollection[] { q1, q2, q3, q4, q5, q6, q7 });
                var s1 = parent.source1.Subscribe(new ZipObserver<T1>(gate, this, 0, q1));
                var s2 = parent.source2.Subscribe(new ZipObserver<T2>(gate, this, 1, q2));
                var s3 = parent.source3.Subscribe(new ZipObserver<T3>(gate, this, 2, q3));
                var s4 = parent.source4.Subscribe(new ZipObserver<T4>(gate, this, 3, q4));
                var s5 = parent.source5.Subscribe(new ZipObserver<T5>(gate, this, 4, q5));
                var s6 = parent.source6.Subscribe(new ZipObserver<T6>(gate, this, 5, q6));
                var s7 = parent.source7.Subscribe(new ZipObserver<T7>(gate, this, 6, q7));

                return StableCompositeDisposable.Create(s1, s2, s3, s4, s5, s6, s7, Disposable.Create(() =>
                {
                    lock (gate)
                    {
                        q1.Clear(); q2.Clear(); q3.Clear(); q4.Clear(); q5.Clear(); q6.Clear(); q7.Clear();
                    }
                }));
            }

            public override TR GetResult()
            {
                return parent.resultSelector(q1.Dequeue(), q2.Dequeue(), q3.Dequeue(), q4.Dequeue(), q5.Dequeue(), q6.Dequeue(), q7.Dequeue());
            }

            public override void OnNext(TR value)
            {
                base.observer.OnNext(value);
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

    #endregion

    // Nth infrastructure

    internal interface IZipObservable
    {
        void Dequeue(int index);
        void Fail(Exception error);
        void Done(int index);
    }

    internal abstract class NthZipObserverBase<T> : OperatorObserverBase<T, T>, IZipObservable
    {
        System.Collections.ICollection[] queues;
        bool[] isDone;
        int length;

        public NthZipObserverBase(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
        {
        }

        protected void SetQueue(System.Collections.ICollection[] queues)
        {
            this.queues = queues;
            this.length = queues.Length;
            this.isDone = new bool[length];
        }

        public abstract T GetResult();

        // operators in lock
        public void Dequeue(int index)
        {
            var allQueueHasValue = true;
            for (int i = 0; i < length; i++)
            {
                if (queues[i].Count == 0)
                {
                    allQueueHasValue = false;
                    break;
                }
            }

            if (!allQueueHasValue)
            {
                var allCompletedWithoutSelf = true;
                for (int i = 0; i < length; i++)
                {
                    if (i == index) continue;
                    if (!isDone[i])
                    {
                        allCompletedWithoutSelf = false;
                        break;
                    }
                }

                if (allCompletedWithoutSelf)
                {
                    try { observer.OnCompleted(); }
                    finally { Dispose(); }
                    return;
                }
                else
                {
                    return;
                }
            }

            var result = default(T);
            try
            {
                result = GetResult();
            }
            catch (Exception ex)
            {
                try { observer.OnError(ex); }
                finally { Dispose(); }
                return;
            }
            OnNext(result);
        }

        public void Done(int index)
        {
            isDone[index] = true;
            var allTrue = true;
            for (int i = 0; i < length; i++)
            {
                if (!isDone[i])
                {
                    allTrue = false;
                    break;
                }
            }

            if (allTrue)
            {
                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }

        public void Fail(Exception error)
        {
            try { observer.OnError(error); }
            finally { Dispose(); }
        }
    }


    // nth
    internal class ZipObserver<T> : IObserver<T>
    {
        readonly object gate;
        readonly IZipObservable parent;
        readonly int index;
        readonly Queue<T> queue;

        public ZipObserver(object gate, IZipObservable parent, int index, Queue<T> queue)
        {
            this.gate = gate;
            this.parent = parent;
            this.index = index;
            this.queue = queue;
        }

        public void OnNext(T value)
        {
            lock (gate)
            {
                queue.Enqueue(value);
                parent.Dequeue(index);
            }
        }

        public void OnError(Exception error)
        {
            lock (gate)
            {
                parent.Fail(error);
            }
        }

        public void OnCompleted()
        {
            lock (gate)
            {
                parent.Done(index);
            }
        }
    }
}