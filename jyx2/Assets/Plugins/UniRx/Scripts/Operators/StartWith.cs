using System;

namespace UniRx.Operators
{
    internal class StartWithObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly T value;
        readonly Func<T> valueFactory;

        public StartWithObservable(IObservable<T> source, T value)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.value = value;
        }

        public StartWithObservable(IObservable<T> source, Func<T> valueFactory)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.valueFactory = valueFactory;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new StartWith(this, observer, cancel).Run();
        }

        class StartWith : OperatorObserverBase<T, T>
        {
            readonly StartWithObservable<T> parent;

            public StartWith(StartWithObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                T t;
                if (parent.valueFactory == null)
                {
                    t = parent.value;
                }
                else
                {
                    try
                    {
                        t = parent.valueFactory();
                    }
                    catch (Exception ex)
                    {
                        try { observer.OnError(ex); }
                        finally { Dispose(); }
                        return Disposable.Empty;
                    }
                }

                OnNext(t);
                return parent.source.Subscribe(base.observer); // good bye StartWithObserver
            }

            public override void OnNext(T value)
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
}