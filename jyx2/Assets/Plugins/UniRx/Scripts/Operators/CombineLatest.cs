using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx.Operators
{
    public delegate TR CombineLatestFunc<T1, T2, T3, TR>(T1 arg1, T2 arg2, T3 arg3);
    public delegate TR CombineLatestFunc<T1, T2, T3, T4, TR>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate TR CombineLatestFunc<T1, T2, T3, T4, T5, TR>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    public delegate TR CombineLatestFunc<T1, T2, T3, T4, T5, T6, TR>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    public delegate TR CombineLatestFunc<T1, T2, T3, T4, T5, T6, T7, TR>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);


    // binary
    internal class CombineLatestObservable<TLeft, TRight, TResult> : OperatorObservableBase<TResult>
    {
        readonly IObservable<TLeft> left;
        readonly IObservable<TRight> right;
        readonly Func<TLeft, TRight, TResult> selector;

        public CombineLatestObservable(IObservable<TLeft> left, IObservable<TRight> right, Func<TLeft, TRight, TResult> selector)
            : base(left.IsRequiredSubscribeOnCurrentThread() || right.IsRequiredSubscribeOnCurrentThread())
        {
            this.left = left;
            this.right = right;
            this.selector = selector;
        }

        protected override IDisposable SubscribeCore(IObserver<TResult> observer, IDisposable cancel)
        {
            return new CombineLatest(this, observer, cancel).Run();
        }

        class CombineLatest : OperatorObserverBase<TResult, TResult>
        {
            readonly CombineLatestObservable<TLeft, TRight, TResult> parent;
            readonly object gate = new object();

            TLeft leftValue = default(TLeft);
            bool leftStarted = false;
            bool leftCompleted = false;

            TRight rightValue = default(TRight);
            bool rightStarted = false;
            bool rightCompleted = false;

            public CombineLatest(CombineLatestObservable<TLeft, TRight, TResult> parent, IObserver<TResult> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var l = parent.left.Subscribe(new LeftObserver(this));
                var r = parent.right.Subscribe(new RightObserver(this));

                return StableCompositeDisposable.Create(l, r);
            }

            // publish in lock
            public void Publish()
            {
                if ((leftCompleted && !leftStarted) || (rightCompleted && !rightStarted))
                {
                    try { observer.OnCompleted(); }
                    finally { Dispose(); }
                    return;
                }
                else if (!(leftStarted && rightStarted))
                {
                    return;
                }

                TResult v;
                try
                {
                    v = parent.selector(leftValue, rightValue);
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

            class LeftObserver : IObserver<TLeft>
            {
                readonly CombineLatest parent;

                public LeftObserver(CombineLatest parent)
                {
                    this.parent = parent;
                }

                public void OnNext(TLeft value)
                {
                    lock (parent.gate)
                    {
                        parent.leftStarted = true;
                        parent.leftValue = value;
                        parent.Publish();
                    }
                }

                public void OnError(Exception error)
                {
                    lock (parent.gate)
                    {
                        parent.OnError(error);
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

            class RightObserver : IObserver<TRight>
            {
                readonly CombineLatest parent;

                public RightObserver(CombineLatest parent)
                {
                    this.parent = parent;
                }


                public void OnNext(TRight value)
                {
                    lock (parent.gate)
                    {
                        parent.rightStarted = true;
                        parent.rightValue = value;
                        parent.Publish();
                    }
                }

                public void OnError(Exception error)
                {
                    lock (parent.gate)
                    {
                        parent.OnError(error);
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
    internal class CombineLatestObservable<T> : OperatorObservableBase<IList<T>>
    {
        readonly IObservable<T>[] sources;

        public CombineLatestObservable(IObservable<T>[] sources)
            : base(true)
        {
            this.sources = sources;
        }

        protected override IDisposable SubscribeCore(IObserver<IList<T>> observer, IDisposable cancel)
        {
            return new CombineLatest(this, observer, cancel).Run();
        }

        class CombineLatest : OperatorObserverBase<IList<T>, IList<T>>
        {
            readonly CombineLatestObservable<T> parent;
            readonly object gate = new object();

            int length;
            T[] values;
            bool[] isStarted;
            bool[] isCompleted;
            bool isAllValueStarted;

            public CombineLatest(CombineLatestObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                length = parent.sources.Length;
                values = new T[length];
                isStarted = new bool[length];
                isCompleted = new bool[length];
                isAllValueStarted = false;

                var disposables = new IDisposable[length];
                for (int i = 0; i < length; i++)
                {
                    var source = parent.sources[i];
                    disposables[i] = source.Subscribe(new CombineLatestObserver(this, i));
                }

                return StableCompositeDisposable.CreateUnsafe(disposables);
            }

            // publish is in the lock
            void Publish(int index)
            {
                isStarted[index] = true;

                if (isAllValueStarted)
                {
                    OnNext(new List<T>(values));
                    return;
                }

                var allValueStarted = true;
                for (int i = 0; i < length; i++)
                {
                    if (!isStarted[i])
                    {
                        allValueStarted = false;
                        break;
                    }
                }

                isAllValueStarted = allValueStarted;

                if (isAllValueStarted)
                {
                    OnNext(new List<T>(values));
                    return;
                }
                else
                {
                    var allCompletedWithoutSelf = true;
                    for (int i = 0; i < length; i++)
                    {
                        if (i == index) continue;
                        if (!isCompleted[i])
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

            class CombineLatestObserver : IObserver<T>
            {
                readonly CombineLatest parent;
                readonly int index;

                public CombineLatestObserver(CombineLatest parent, int index)
                {
                    this.parent = parent;
                    this.index = index;
                }

                public void OnNext(T value)
                {
                    lock (parent.gate)
                    {
                        parent.values[index] = value;
                        parent.Publish(index);
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
                        parent.isCompleted[index] = true;

                        var allTrue = true;
                        for (int i = 0; i < parent.length; i++)
                        {
                            if (!parent.isCompleted[i])
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

    // generated from UniRx.Console.CombineLatestGenerator.tt
    #region NTH

    internal class CombineLatestObservable<T1, T2, T3, TR> : OperatorObservableBase<TR>
    {
        IObservable<T1> source1;
        IObservable<T2> source2;
        IObservable<T3> source3;
        CombineLatestFunc<T1, T2, T3, TR> resultSelector;

        public CombineLatestObservable(
            IObservable<T1> source1,
            IObservable<T2> source2,
            IObservable<T3> source3,
              CombineLatestFunc<T1, T2, T3, TR> resultSelector)
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
            return new CombineLatest(3, this, observer, cancel).Run();
        }

        class CombineLatest : NthCombineLatestObserverBase<TR>
        {
            readonly CombineLatestObservable<T1, T2, T3, TR> parent;
            readonly object gate = new object();
            CombineLatestObserver<T1> c1;
            CombineLatestObserver<T2> c2;
            CombineLatestObserver<T3> c3;

            public CombineLatest(int length, CombineLatestObservable<T1, T2, T3, TR> parent, IObserver<TR> observer, IDisposable cancel)
                : base(length, observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                c1 = new CombineLatestObserver<T1>(gate, this, 0);
                c2 = new CombineLatestObserver<T2>(gate, this, 1);
                c3 = new CombineLatestObserver<T3>(gate, this, 2);

                var s1 = parent.source1.Subscribe(c1);
                var s2 = parent.source2.Subscribe(c2);
                var s3 = parent.source3.Subscribe(c3);

                return StableCompositeDisposable.Create(s1, s2, s3);
            }

            public override TR GetResult()
            {
                return parent.resultSelector(c1.Value, c2.Value, c3.Value);
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


    internal class CombineLatestObservable<T1, T2, T3, T4, TR> : OperatorObservableBase<TR>
    {
        IObservable<T1> source1;
        IObservable<T2> source2;
        IObservable<T3> source3;
        IObservable<T4> source4;
        CombineLatestFunc<T1, T2, T3, T4, TR> resultSelector;

        public CombineLatestObservable(
            IObservable<T1> source1,
            IObservable<T2> source2,
            IObservable<T3> source3,
            IObservable<T4> source4,
              CombineLatestFunc<T1, T2, T3, T4, TR> resultSelector)
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
            return new CombineLatest(4, this, observer, cancel).Run();
        }

        class CombineLatest : NthCombineLatestObserverBase<TR>
        {
            readonly CombineLatestObservable<T1, T2, T3, T4, TR> parent;
            readonly object gate = new object();
            CombineLatestObserver<T1> c1;
            CombineLatestObserver<T2> c2;
            CombineLatestObserver<T3> c3;
            CombineLatestObserver<T4> c4;

            public CombineLatest(int length, CombineLatestObservable<T1, T2, T3, T4, TR> parent, IObserver<TR> observer, IDisposable cancel)
                : base(length, observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                c1 = new CombineLatestObserver<T1>(gate, this, 0);
                c2 = new CombineLatestObserver<T2>(gate, this, 1);
                c3 = new CombineLatestObserver<T3>(gate, this, 2);
                c4 = new CombineLatestObserver<T4>(gate, this, 3);

                var s1 = parent.source1.Subscribe(c1);
                var s2 = parent.source2.Subscribe(c2);
                var s3 = parent.source3.Subscribe(c3);
                var s4 = parent.source4.Subscribe(c4);

                return StableCompositeDisposable.Create(s1, s2, s3, s4);
            }

            public override TR GetResult()
            {
                return parent.resultSelector(c1.Value, c2.Value, c3.Value, c4.Value);
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


    internal class CombineLatestObservable<T1, T2, T3, T4, T5, TR> : OperatorObservableBase<TR>
    {
        IObservable<T1> source1;
        IObservable<T2> source2;
        IObservable<T3> source3;
        IObservable<T4> source4;
        IObservable<T5> source5;
        CombineLatestFunc<T1, T2, T3, T4, T5, TR> resultSelector;

        public CombineLatestObservable(
            IObservable<T1> source1,
            IObservable<T2> source2,
            IObservable<T3> source3,
            IObservable<T4> source4,
            IObservable<T5> source5,
              CombineLatestFunc<T1, T2, T3, T4, T5, TR> resultSelector)
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
            return new CombineLatest(5, this, observer, cancel).Run();
        }

        class CombineLatest : NthCombineLatestObserverBase<TR>
        {
            readonly CombineLatestObservable<T1, T2, T3, T4, T5, TR> parent;
            readonly object gate = new object();
            CombineLatestObserver<T1> c1;
            CombineLatestObserver<T2> c2;
            CombineLatestObserver<T3> c3;
            CombineLatestObserver<T4> c4;
            CombineLatestObserver<T5> c5;

            public CombineLatest(int length, CombineLatestObservable<T1, T2, T3, T4, T5, TR> parent, IObserver<TR> observer, IDisposable cancel)
                : base(length, observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                c1 = new CombineLatestObserver<T1>(gate, this, 0);
                c2 = new CombineLatestObserver<T2>(gate, this, 1);
                c3 = new CombineLatestObserver<T3>(gate, this, 2);
                c4 = new CombineLatestObserver<T4>(gate, this, 3);
                c5 = new CombineLatestObserver<T5>(gate, this, 4);

                var s1 = parent.source1.Subscribe(c1);
                var s2 = parent.source2.Subscribe(c2);
                var s3 = parent.source3.Subscribe(c3);
                var s4 = parent.source4.Subscribe(c4);
                var s5 = parent.source5.Subscribe(c5);

                return StableCompositeDisposable.Create(s1, s2, s3, s4, s5);
            }

            public override TR GetResult()
            {
                return parent.resultSelector(c1.Value, c2.Value, c3.Value, c4.Value, c5.Value);
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


    internal class CombineLatestObservable<T1, T2, T3, T4, T5, T6, TR> : OperatorObservableBase<TR>
    {
        IObservable<T1> source1;
        IObservable<T2> source2;
        IObservable<T3> source3;
        IObservable<T4> source4;
        IObservable<T5> source5;
        IObservable<T6> source6;
        CombineLatestFunc<T1, T2, T3, T4, T5, T6, TR> resultSelector;

        public CombineLatestObservable(
            IObservable<T1> source1,
            IObservable<T2> source2,
            IObservable<T3> source3,
            IObservable<T4> source4,
            IObservable<T5> source5,
            IObservable<T6> source6,
              CombineLatestFunc<T1, T2, T3, T4, T5, T6, TR> resultSelector)
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
            return new CombineLatest(6, this, observer, cancel).Run();
        }

        class CombineLatest : NthCombineLatestObserverBase<TR>
        {
            readonly CombineLatestObservable<T1, T2, T3, T4, T5, T6, TR> parent;
            readonly object gate = new object();
            CombineLatestObserver<T1> c1;
            CombineLatestObserver<T2> c2;
            CombineLatestObserver<T3> c3;
            CombineLatestObserver<T4> c4;
            CombineLatestObserver<T5> c5;
            CombineLatestObserver<T6> c6;

            public CombineLatest(int length, CombineLatestObservable<T1, T2, T3, T4, T5, T6, TR> parent, IObserver<TR> observer, IDisposable cancel)
                : base(length, observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                c1 = new CombineLatestObserver<T1>(gate, this, 0);
                c2 = new CombineLatestObserver<T2>(gate, this, 1);
                c3 = new CombineLatestObserver<T3>(gate, this, 2);
                c4 = new CombineLatestObserver<T4>(gate, this, 3);
                c5 = new CombineLatestObserver<T5>(gate, this, 4);
                c6 = new CombineLatestObserver<T6>(gate, this, 5);

                var s1 = parent.source1.Subscribe(c1);
                var s2 = parent.source2.Subscribe(c2);
                var s3 = parent.source3.Subscribe(c3);
                var s4 = parent.source4.Subscribe(c4);
                var s5 = parent.source5.Subscribe(c5);
                var s6 = parent.source6.Subscribe(c6);

                return StableCompositeDisposable.Create(s1, s2, s3, s4, s5, s6);
            }

            public override TR GetResult()
            {
                return parent.resultSelector(c1.Value, c2.Value, c3.Value, c4.Value, c5.Value, c6.Value);
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


    internal class CombineLatestObservable<T1, T2, T3, T4, T5, T6, T7, TR> : OperatorObservableBase<TR>
    {
        IObservable<T1> source1;
        IObservable<T2> source2;
        IObservable<T3> source3;
        IObservable<T4> source4;
        IObservable<T5> source5;
        IObservable<T6> source6;
        IObservable<T7> source7;
        CombineLatestFunc<T1, T2, T3, T4, T5, T6, T7, TR> resultSelector;

        public CombineLatestObservable(
            IObservable<T1> source1,
            IObservable<T2> source2,
            IObservable<T3> source3,
            IObservable<T4> source4,
            IObservable<T5> source5,
            IObservable<T6> source6,
            IObservable<T7> source7,
              CombineLatestFunc<T1, T2, T3, T4, T5, T6, T7, TR> resultSelector)
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
            return new CombineLatest(7, this, observer, cancel).Run();
        }

        class CombineLatest : NthCombineLatestObserverBase<TR>
        {
            readonly CombineLatestObservable<T1, T2, T3, T4, T5, T6, T7, TR> parent;
            readonly object gate = new object();
            CombineLatestObserver<T1> c1;
            CombineLatestObserver<T2> c2;
            CombineLatestObserver<T3> c3;
            CombineLatestObserver<T4> c4;
            CombineLatestObserver<T5> c5;
            CombineLatestObserver<T6> c6;
            CombineLatestObserver<T7> c7;

            public CombineLatest(int length, CombineLatestObservable<T1, T2, T3, T4, T5, T6, T7, TR> parent, IObserver<TR> observer, IDisposable cancel)
                : base(length, observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                c1 = new CombineLatestObserver<T1>(gate, this, 0);
                c2 = new CombineLatestObserver<T2>(gate, this, 1);
                c3 = new CombineLatestObserver<T3>(gate, this, 2);
                c4 = new CombineLatestObserver<T4>(gate, this, 3);
                c5 = new CombineLatestObserver<T5>(gate, this, 4);
                c6 = new CombineLatestObserver<T6>(gate, this, 5);
                c7 = new CombineLatestObserver<T7>(gate, this, 6);

                var s1 = parent.source1.Subscribe(c1);
                var s2 = parent.source2.Subscribe(c2);
                var s3 = parent.source3.Subscribe(c3);
                var s4 = parent.source4.Subscribe(c4);
                var s5 = parent.source5.Subscribe(c5);
                var s6 = parent.source6.Subscribe(c6);
                var s7 = parent.source7.Subscribe(c7);

                return StableCompositeDisposable.Create(s1, s2, s3, s4, s5, s6, s7);
            }

            public override TR GetResult()
            {
                return parent.resultSelector(c1.Value, c2.Value, c3.Value, c4.Value, c5.Value, c6.Value, c7.Value);
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

    internal interface ICombineLatestObservable
    {
        void Publish(int index);
        void Fail(Exception error);
        void Done(int index);
    }

    internal abstract class NthCombineLatestObserverBase<T> : OperatorObserverBase<T, T>, ICombineLatestObservable
    {
        readonly int length;
        bool isAllValueStarted;
        readonly bool[] isStarted;
        readonly bool[] isCompleted;

        public NthCombineLatestObserverBase(int length, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
        {
            this.length = length;
            this.isAllValueStarted = false;
            this.isStarted = new bool[length];
            this.isCompleted = new bool[length];
        }

        public abstract T GetResult();

        // operators in lock
        public void Publish(int index)
        {
            isStarted[index] = true;

            if (isAllValueStarted)
            {
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
                return;
            }

            var allValueStarted = true;
            for (int i = 0; i < length; i++)
            {
                if (!isStarted[i])
                {
                    allValueStarted = false;
                    break;
                }
            }

            isAllValueStarted = allValueStarted;

            if (isAllValueStarted)
            {
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
                return;
            }
            else
            {
                var allCompletedWithoutSelf = true;
                for (int i = 0; i < length; i++)
                {
                    if (i == index) continue;
                    if (!isCompleted[i])
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
        }

        public void Done(int index)
        {
            isCompleted[index] = true;

            var allTrue = true;
            for (int i = 0; i < length; i++)
            {
                if (!isCompleted[i])
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

    // Nth
    internal class CombineLatestObserver<T> : IObserver<T>
    {
        readonly object gate;
        readonly ICombineLatestObservable parent;
        readonly int index;
        T value;

        public T Value { get { return value; } }

        public CombineLatestObserver(object gate, ICombineLatestObservable parent, int index)
        {
            this.gate = gate;
            this.parent = parent;
            this.index = index;
        }

        public void OnNext(T value)
        {
            lock (gate)
            {
                this.value = value;
                parent.Publish(index);
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