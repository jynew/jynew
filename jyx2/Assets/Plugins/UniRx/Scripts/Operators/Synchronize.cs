using System;
using UniRx.Operators;

namespace UniRx.Operators
{
    internal class SynchronizeObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly object gate;

        public SynchronizeObservable(IObservable<T> source, object gate)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.gate = gate;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return source.Subscribe(new Synchronize(this, observer, cancel));
        }

        class Synchronize : OperatorObserverBase<T, T>
        {
            readonly SynchronizeObservable<T> parent;

            public Synchronize(SynchronizeObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                lock (parent.gate)
                {
                    base.observer.OnNext(value);
                }
            }

            public override void OnError(Exception error)
            {
                lock (parent.gate)
                {
                    try { observer.OnError(error); } finally { Dispose(); };
                }
            }

            public override void OnCompleted()
            {
                lock (parent.gate)
                {
                    try { observer.OnCompleted(); } finally { Dispose(); };
                }
            }
        }
    }
}