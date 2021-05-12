using System;
using UniRx.Operators;

namespace UniRx.Operators
{
    internal class DefaultIfEmptyObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly T defaultValue;

        public DefaultIfEmptyObservable(IObservable<T> source, T defaultValue)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.defaultValue = defaultValue;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return source.Subscribe(new DefaultIfEmpty(this, observer, cancel));
        }

        class DefaultIfEmpty : OperatorObserverBase<T, T>
        {
            readonly DefaultIfEmptyObservable<T> parent;
            bool hasValue;

            public DefaultIfEmpty(DefaultIfEmptyObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.hasValue = false;
            }

            public override void OnNext(T value)
            {
                hasValue = true;
                observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                if (!hasValue)
                {
                    observer.OnNext(parent.defaultValue);
                }

                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }
    }
}