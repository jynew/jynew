using System;
using UniRx.Operators;

namespace UniRx.Operators
{
    internal class IgnoreElementsObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;

        public IgnoreElementsObservable(IObservable<T> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return source.Subscribe(new IgnoreElements(observer, cancel));
        }

        class IgnoreElements : OperatorObserverBase<T, T>
        {
            public IgnoreElements(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
            }

            public override void OnNext(T value)
            {
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