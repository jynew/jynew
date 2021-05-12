using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class DistinctObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IEqualityComparer<T> comparer;

        public DistinctObservable(IObservable<T> source, IEqualityComparer<T> comparer)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.comparer = comparer;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return source.Subscribe(new Distinct(this, observer, cancel));
        }

        class Distinct : OperatorObserverBase<T, T>
        {
            readonly HashSet<T> hashSet;

            public Distinct(DistinctObservable<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                hashSet = (parent.comparer == null)
                    ? new HashSet<T>()
                    : new HashSet<T>(parent.comparer);
            }

            public override void OnNext(T value)
            {
                var key = default(T);
                var isAdded = false;
                try
                {
                    key = value;
                    isAdded = hashSet.Add(key);
                }
                catch (Exception exception)
                {
                    try { observer.OnError(exception); } finally { Dispose(); }
                    return;
                }

                if (isAdded)
                {
                    observer.OnNext(value);
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); } finally { Dispose(); }
            }
        }
    }

    internal class DistinctObservable<T, TKey> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IEqualityComparer<TKey> comparer;
        readonly Func<T, TKey> keySelector;

        public DistinctObservable(IObservable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.comparer = comparer;
            this.keySelector = keySelector;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return source.Subscribe(new Distinct(this, observer, cancel));
        }

        class Distinct : OperatorObserverBase<T, T>
        {
            readonly DistinctObservable<T, TKey> parent;
            readonly HashSet<TKey> hashSet;

            public Distinct(DistinctObservable<T, TKey> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
                hashSet = (parent.comparer == null)
                    ? new HashSet<TKey>()
                    : new HashSet<TKey>(parent.comparer);
            }

            public override void OnNext(T value)
            {
                var key = default(TKey);
                var isAdded = false;
                try
                {
                    key = parent.keySelector(value);
                    isAdded = hashSet.Add(key);
                }
                catch (Exception exception)
                {
                    try { observer.OnError(exception); } finally { Dispose(); }
                    return;
                }

                if (isAdded)
                {
                    observer.OnNext(value);
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); } finally { Dispose(); }
            }
        }
    }
}