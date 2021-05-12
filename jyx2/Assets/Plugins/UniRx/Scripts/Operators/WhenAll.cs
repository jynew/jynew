using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class WhenAllObservable<T> : OperatorObservableBase<T[]>
    {
        readonly IObservable<T>[] sources;
        readonly IEnumerable<IObservable<T>> sourcesEnumerable;

        public WhenAllObservable(IObservable<T>[] sources)
            : base(false)
        {
            this.sources = sources;
        }

        public WhenAllObservable(IEnumerable<IObservable<T>> sources)
            : base(false)
        {
            this.sourcesEnumerable = sources;
        }

        protected override IDisposable SubscribeCore(IObserver<T[]> observer, IDisposable cancel)
        {
            if (sources != null)
            {
                return new WhenAll(this.sources, observer, cancel).Run();
            }
            else
            {
                var xs = sourcesEnumerable as IList<IObservable<T>>;
                if (xs == null)
                {
                    xs = new List<IObservable<T>>(sourcesEnumerable); // materialize observables
                }
                return new WhenAll_(xs, observer, cancel).Run();
            }
        }

        class WhenAll : OperatorObserverBase<T[], T[]>
        {
            readonly IObservable<T>[] sources;
            readonly object gate = new object();
            int completedCount;
            int length;
            T[] values;

            public WhenAll(IObservable<T>[] sources, IObserver<T[]> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.sources = sources;
            }

            public IDisposable Run()
            {
                length = sources.Length;

                // fail safe...
                if (length == 0)
                {
                    OnNext(new T[0]);
                    try { observer.OnCompleted(); } finally { Dispose(); }
                    return Disposable.Empty;
                }

                completedCount = 0;
                values = new T[length];

                var subscriptions = new IDisposable[length];
                for (int index = 0; index < length; index++)
                {
                    var source = sources[index];
                    var observer = new WhenAllCollectionObserver(this, index);
                    subscriptions[index] = source.Subscribe(observer);
                }

                return StableCompositeDisposable.CreateUnsafe(subscriptions);
            }

            public override void OnNext(T[] value)
            {
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); } finally { Dispose(); }
            }

            class WhenAllCollectionObserver : IObserver<T>
            {
                readonly WhenAll parent;
                readonly int index;
                bool isCompleted = false;

                public WhenAllCollectionObserver(WhenAll parent, int index)
                {
                    this.parent = parent;
                    this.index = index;
                }

                public void OnNext(T value)
                {
                    lock (parent.gate)
                    {
                        if (!isCompleted)
                        {
                            parent.values[index] = value;
                        }
                    }
                }

                public void OnError(Exception error)
                {
                    lock (parent.gate)
                    {
                        if (!isCompleted)
                        {
                            parent.OnError(error);
                        }
                    }
                }

                public void OnCompleted()
                {
                    lock (parent.gate)
                    {
                        if (!isCompleted)
                        {
                            isCompleted = true;
                            parent.completedCount++;
                            if (parent.completedCount == parent.length)
                            {
                                parent.OnNext(parent.values);
                                parent.OnCompleted();
                            }
                        }
                    }
                }
            }
        }

        class WhenAll_ : OperatorObserverBase<T[], T[]>
        {
            readonly IList<IObservable<T>> sources;
            readonly object gate = new object();
            int completedCount;
            int length;
            T[] values;

            public WhenAll_(IList<IObservable<T>> sources, IObserver<T[]> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.sources = sources;
            }

            public IDisposable Run()
            {
                length = sources.Count;

                // fail safe...
                if (length == 0)
                {
                    OnNext(new T[0]);
                    try { observer.OnCompleted(); } finally { Dispose(); }
                    return Disposable.Empty;
                }

                completedCount = 0;
                values = new T[length];

                var subscriptions = new IDisposable[length];
                for (int index = 0; index < length; index++)
                {
                    var source = sources[index];
                    var observer = new WhenAllCollectionObserver(this, index);
                    subscriptions[index] = source.Subscribe(observer);
                }

                return StableCompositeDisposable.CreateUnsafe(subscriptions);
            }

            public override void OnNext(T[] value)
            {
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); } finally { Dispose(); }
            }

            class WhenAllCollectionObserver : IObserver<T>
            {
                readonly WhenAll_ parent;
                readonly int index;
                bool isCompleted = false;

                public WhenAllCollectionObserver(WhenAll_ parent, int index)
                {
                    this.parent = parent;
                    this.index = index;
                }

                public void OnNext(T value)
                {
                    lock (parent.gate)
                    {
                        if (!isCompleted)
                        {
                            parent.values[index] = value;
                        }
                    }
                }

                public void OnError(Exception error)
                {
                    lock (parent.gate)
                    {
                        if (!isCompleted)
                        {
                            parent.OnError(error);
                        }
                    }
                }

                public void OnCompleted()
                {
                    lock (parent.gate)
                    {
                        if (!isCompleted)
                        {
                            isCompleted = true;
                            parent.completedCount++;
                            if (parent.completedCount == parent.length)
                            {
                                parent.OnNext(parent.values);
                                parent.OnCompleted();
                            }
                        }
                    }
                }
            }
        }
    }

    internal class WhenAllObservable : OperatorObservableBase<Unit>
    {
        readonly IObservable<Unit>[] sources;
        readonly IEnumerable<IObservable<Unit>> sourcesEnumerable;

        public WhenAllObservable(IObservable<Unit>[] sources)
            : base(false)
        {
            this.sources = sources;
        }

        public WhenAllObservable(IEnumerable<IObservable<Unit>> sources)
            : base(false)
        {
            this.sourcesEnumerable = sources;
        }

        protected override IDisposable SubscribeCore(IObserver<Unit> observer, IDisposable cancel)
        {
            if (sources != null)
            {
                return new WhenAll(this.sources, observer, cancel).Run();
            }
            else
            {
                var xs = sourcesEnumerable as IList<IObservable<Unit>>;
                if (xs == null)
                {
                    xs = new List<IObservable<Unit>>(sourcesEnumerable); // materialize observables
                }
                return new WhenAll_(xs, observer, cancel).Run();
            }
        }

        class WhenAll : OperatorObserverBase<Unit, Unit>
        {
            readonly IObservable<Unit>[] sources;
            readonly object gate = new object();
            int completedCount;
            int length;

            public WhenAll(IObservable<Unit>[] sources, IObserver<Unit> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.sources = sources;
            }

            public IDisposable Run()
            {
                length = sources.Length;

                // fail safe...
                if (length == 0)
                {
                    OnNext(Unit.Default);
                    try { observer.OnCompleted(); } finally { Dispose(); }
                    return Disposable.Empty;
                }

                completedCount = 0;

                var subscriptions = new IDisposable[length];
                for (int index = 0; index < sources.Length; index++)
                {
                    var source = sources[index];
                    var observer = new WhenAllCollectionObserver(this);
                    subscriptions[index] = source.Subscribe(observer);
                }

                return StableCompositeDisposable.CreateUnsafe(subscriptions);
            }

            public override void OnNext(Unit value)
            {
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); } finally { Dispose(); }
            }

            class WhenAllCollectionObserver : IObserver<Unit>
            {
                readonly WhenAll parent;
                bool isCompleted = false;

                public WhenAllCollectionObserver(WhenAll parent)
                {
                    this.parent = parent;
                }

                public void OnNext(Unit value)
                {
                }

                public void OnError(Exception error)
                {
                    lock (parent.gate)
                    {
                        if (!isCompleted)
                        {
                            parent.OnError(error);
                        }
                    }
                }

                public void OnCompleted()
                {
                    lock (parent.gate)
                    {
                        if (!isCompleted)
                        {
                            isCompleted = true;
                            parent.completedCount++;
                            if (parent.completedCount == parent.length)
                            {
                                parent.OnNext(Unit.Default);
                                parent.OnCompleted();
                            }
                        }
                    }
                }
            }
        }

        class WhenAll_ : OperatorObserverBase<Unit, Unit>
        {
            readonly IList<IObservable<Unit>> sources;
            readonly object gate = new object();
            int completedCount;
            int length;

            public WhenAll_(IList<IObservable<Unit>> sources, IObserver<Unit> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.sources = sources;
            }

            public IDisposable Run()
            {
                length = sources.Count;

                // fail safe...
                if (length == 0)
                {
                    OnNext(Unit.Default);
                    try { observer.OnCompleted(); } finally { Dispose(); }
                    return Disposable.Empty;
                }

                completedCount = 0;

                var subscriptions = new IDisposable[length];
                for (int index = 0; index < length; index++)
                {
                    var source = sources[index];
                    var observer = new WhenAllCollectionObserver(this);
                    subscriptions[index] = source.Subscribe(observer);
                }

                return StableCompositeDisposable.CreateUnsafe(subscriptions);
            }

            public override void OnNext(Unit value)
            {
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); } finally { Dispose(); }
            }

            class WhenAllCollectionObserver : IObserver<Unit>
            {
                readonly WhenAll_ parent;
                bool isCompleted = false;

                public WhenAllCollectionObserver(WhenAll_ parent)
                {
                    this.parent = parent;
                }

                public void OnNext(Unit value)
                {
                }

                public void OnError(Exception error)
                {
                    lock (parent.gate)
                    {
                        if (!isCompleted)
                        {
                            parent.OnError(error);
                        }
                    }
                }

                public void OnCompleted()
                {
                    lock (parent.gate)
                    {
                        if (!isCompleted)
                        {
                            isCompleted = true;
                            parent.completedCount++;
                            if (parent.completedCount == parent.length)
                            {
                                parent.OnNext(Unit.Default);
                                parent.OnCompleted();
                            }
                        }
                    }
                }
            }
        }
    }
}