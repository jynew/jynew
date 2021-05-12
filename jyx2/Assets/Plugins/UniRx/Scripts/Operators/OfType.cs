using System;

namespace UniRx.Operators
{
    internal class OfTypeObservable<TSource, TResult> : OperatorObservableBase<TResult>
    {
        readonly IObservable<TSource> source;

        public OfTypeObservable(IObservable<TSource> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<TResult> observer, IDisposable cancel)
        {
            return source.Subscribe(new OfType(observer, cancel));
        }

        class OfType : OperatorObserverBase<TSource, TResult>
        {
            public OfType(IObserver<TResult> observer, IDisposable cancel)
                : base(observer, cancel)
            {
            }

            public override void OnNext(TSource value)
            {
                if (value is TResult)
                {
                    var castValue = (TResult)(object)value;
                    observer.OnNext(castValue);
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