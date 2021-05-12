using System;

namespace UniRx.Operators
{
    internal class ContinueWithObservable<TSource, TResult> : OperatorObservableBase<TResult>
    {
        readonly IObservable<TSource> source;
        readonly Func<TSource, IObservable<TResult>> selector;

        public ContinueWithObservable(IObservable<TSource> source, Func<TSource, IObservable<TResult>> selector)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.selector = selector;
        }

        protected override IDisposable SubscribeCore(IObserver<TResult> observer, IDisposable cancel)
        {
            return new ContinueWith(this, observer, cancel).Run();
        }

        class ContinueWith : OperatorObserverBase<TSource, TResult>
        {
            readonly ContinueWithObservable<TSource, TResult> parent;
            readonly SerialDisposable serialDisposable = new SerialDisposable();

            bool seenValue;
            TSource lastValue;

            public ContinueWith(ContinueWithObservable<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var sourceDisposable = new SingleAssignmentDisposable();
                serialDisposable.Disposable = sourceDisposable;

                sourceDisposable.Disposable = parent.source.Subscribe(this);
                return serialDisposable;
            }

            public override void OnNext(TSource value)
            {
                this.seenValue = true;
                this.lastValue = value;
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); };
            }

            public override void OnCompleted()
            {
                if (seenValue)
                {
                    try
	                {
		                var v = parent.selector(lastValue);
		                // dispose source subscription
		                serialDisposable.Disposable = v.Subscribe(observer);
	                }
	                catch (Exception error)
	                {
		                OnError(error);
	                }
                }
                else
                {
                    try { observer.OnCompleted(); } finally { Dispose(); };
                }
            }
        }
    }
}