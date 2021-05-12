using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class DistinctUntilChangedObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IEqualityComparer<T> comparer;

        public DistinctUntilChangedObservable(IObservable<T> source, IEqualityComparer<T> comparer)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.comparer = comparer;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return source.Subscribe(new DistinctUntilChanged(this, observer, cancel));
        }

        class DistinctUntilChanged : OperatorObserverBase<T, T>
        {
            readonly DistinctUntilChangedObservable<T> parent;
            bool isFirst = true;
            T prevKey = default(T);

            public DistinctUntilChanged(DistinctUntilChangedObservable<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                T currentKey;
                try
                {
                    currentKey = value;
                }
                catch (Exception exception)
                {
                    try { observer.OnError(exception); } finally { Dispose(); }
                    return;
                }

                var sameKey = false;
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    try
                    {
                        sameKey = parent.comparer.Equals(currentKey, prevKey);
                    }
                    catch (Exception ex)
                    {
                        try { observer.OnError(ex); } finally { Dispose(); }
                        return;
                    }
                }

                if (!sameKey)
                {
                    prevKey = currentKey;
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

    internal class DistinctUntilChangedObservable<T, TKey> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IEqualityComparer<TKey> comparer;
        readonly Func<T, TKey> keySelector;

        public DistinctUntilChangedObservable(IObservable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.comparer = comparer;
            this.keySelector = keySelector;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return source.Subscribe(new DistinctUntilChanged(this, observer, cancel));
        }

        class DistinctUntilChanged : OperatorObserverBase<T, T>
        {
            readonly DistinctUntilChangedObservable<T, TKey> parent;
            bool isFirst = true;
            TKey prevKey = default(TKey);

            public DistinctUntilChanged(DistinctUntilChangedObservable<T, TKey> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                TKey currentKey;
                try
                {
                    currentKey = parent.keySelector(value);
                }
                catch (Exception exception)
                {
                    try { observer.OnError(exception); } finally { Dispose(); }
                    return;
                }

                var sameKey = false;
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    try
                    {
                        sameKey = parent.comparer.Equals(currentKey, prevKey);
                    }
                    catch (Exception ex)
                    {
                        try { observer.OnError(ex); } finally { Dispose(); }
                        return;
                    }
                }

                if (!sameKey)
                {
                    prevKey = currentKey;
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