using System;

namespace UniRx.Operators
{
    internal class LastObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly bool useDefault;
        readonly Func<T, bool> predicate;

        public LastObservable(IObservable<T> source, bool useDefault)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.useDefault = useDefault;
        }

        public LastObservable(IObservable<T> source, Func<T, bool> predicate, bool useDefault)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.predicate = predicate;
            this.useDefault = useDefault;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (predicate == null)
            {
                return source.Subscribe(new Last(this, observer, cancel));
            }
            else
            {
                return source.Subscribe(new Last_(this, observer, cancel));
            }
        }

        class Last : OperatorObserverBase<T, T>
        {
            readonly LastObservable<T> parent;
            bool notPublished;
            T lastValue;

            public Last(LastObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.notPublished = true;
            }

            public override void OnNext(T value)
            {
                notPublished = false;
                lastValue = value;
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                if (parent.useDefault)
                {
                    if (notPublished)
                    {
                        observer.OnNext(default(T));
                    }
                    else
                    {
                        observer.OnNext(lastValue);
                    }
                    try { observer.OnCompleted(); }
                    finally { Dispose(); }
                }
                else
                {
                    if (notPublished)
                    {
                        try { observer.OnError(new InvalidOperationException("sequence is empty")); }
                        finally { Dispose(); }
                    }
                    else
                    {
                        observer.OnNext(lastValue);
                        try { observer.OnCompleted(); }
                        finally { Dispose(); }
                    }
                }
            }
        }

        class Last_ : OperatorObserverBase<T, T>
        {
            readonly LastObservable<T> parent;
            bool notPublished;
            T lastValue;

            public Last_(LastObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.notPublished = true;
            }

            public override void OnNext(T value)
            {
                bool isPassed;
                try
                {
                    isPassed = parent.predicate(value);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); }
                    finally { Dispose(); }
                    return;
                }

                if (isPassed)
                {
                    notPublished = false;
                    lastValue = value;
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                if (parent.useDefault)
                {
                    if (notPublished)
                    {
                        observer.OnNext(default(T));
                    }
                    else
                    {
                        observer.OnNext(lastValue);
                    }
                    try { observer.OnCompleted(); }
                    finally { Dispose(); }
                }
                else
                {
                    if (notPublished)
                    {
                        try { observer.OnError(new InvalidOperationException("sequence is empty")); }
                        finally { Dispose(); }
                    }
                    else
                    {
                        observer.OnNext(lastValue);
                        try { observer.OnCompleted(); }
                        finally { Dispose(); }
                    }
                }
            }
        }
    }
}