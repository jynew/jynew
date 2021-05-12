using System;
using UniRx.Operators;

namespace UniRx.Operators
{
    internal class AsObservableObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;

        public AsObservableObservable(IObservable<T> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return source.Subscribe(new AsObservable(observer, cancel));
        }

        class AsObservable : OperatorObserverBase<T, T>
        {
            public AsObservable(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
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