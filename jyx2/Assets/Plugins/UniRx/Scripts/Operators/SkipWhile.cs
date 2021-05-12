using System;

namespace UniRx.Operators
{
    internal class SkipWhileObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly Func<T, bool> predicate;
        readonly Func<T, int, bool> predicateWithIndex;

        public SkipWhileObservable(IObservable<T> source, Func<T, bool> predicate)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.predicate = predicate;
        }

        public SkipWhileObservable(IObservable<T> source, Func<T, int, bool> predicateWithIndex)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.predicateWithIndex = predicateWithIndex;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (predicate != null)
            {
                return new SkipWhile(this, observer, cancel).Run();
            }
            else
            {
                return new SkipWhile_(this, observer, cancel).Run();
            }
        }

        class SkipWhile : OperatorObserverBase<T, T>
        {
            readonly SkipWhileObservable<T> parent;
            bool endSkip = false;

            public SkipWhile(SkipWhileObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                return parent.source.Subscribe(this);
            }

            public override void OnNext(T value)
            {
                if (!endSkip)
                {
                    try
                    {
                        endSkip = !parent.predicate(value);
                    }
                    catch (Exception ex)
                    {
                        try { observer.OnError(ex); } finally { Dispose(); }
                        return;
                    }

                    if (!endSkip) return;
                }

                observer.OnNext(value);
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

        class SkipWhile_ : OperatorObserverBase<T, T>
        {
            readonly SkipWhileObservable<T> parent;
            bool endSkip = false;
            int index = 0;

            public SkipWhile_(SkipWhileObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                return parent.source.Subscribe(this);
            }

            public override void OnNext(T value)
            {
                if (!endSkip)
                {
                    try
                    {
                        endSkip = !parent.predicateWithIndex(value, index++);
                    }
                    catch (Exception ex)
                    {
                        try { observer.OnError(ex); } finally { Dispose(); }
                        return;
                    }

                    if (!endSkip) return;
                }

                observer.OnNext(value);
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