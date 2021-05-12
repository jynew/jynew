using System;

namespace UniRx.Operators
{
    internal class TakeUntilObservable<T, TOther> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IObservable<TOther> other;

        public TakeUntilObservable(IObservable<T> source, IObservable<TOther> other)
            : base(source.IsRequiredSubscribeOnCurrentThread() || other.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.other = other;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new TakeUntil(this, observer, cancel).Run();
        }

        class TakeUntil : OperatorObserverBase<T, T>
        {
            readonly TakeUntilObservable<T, TOther> parent;
            object gate = new object();

            public TakeUntil(TakeUntilObservable<T, TOther> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var otherSubscription = new SingleAssignmentDisposable();
                var otherObserver = new TakeUntilOther(this, otherSubscription);
                otherSubscription.Disposable = parent.other.Subscribe(otherObserver);

                var sourceSubscription = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(otherSubscription, sourceSubscription);
            }

            public override void OnNext(T value)
            {
                lock (gate)
                {
                    observer.OnNext(value);
                }
            }

            public override void OnError(Exception error)
            {
                lock (gate)
                {
                    try { observer.OnError(error); } finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                lock (gate)
                {
                    try { observer.OnCompleted(); } finally { Dispose(); }
                }
            }

            class TakeUntilOther : IObserver<TOther>
            {
                readonly TakeUntil sourceObserver;
                readonly IDisposable subscription;

                public TakeUntilOther(TakeUntil sourceObserver, IDisposable subscription)
                {
                    this.sourceObserver = sourceObserver;
                    this.subscription = subscription;
                }

                public void OnNext(TOther value)
                {
                    lock (sourceObserver.gate)
                    {
                        try
                        {
                            sourceObserver.observer.OnCompleted();
                        }
                        finally
                        {
                            sourceObserver.Dispose();
                            subscription.Dispose();
                        }
                    }
                }

                public void OnError(Exception error)
                {
                    lock (sourceObserver.gate)
                    {
                        try
                        {
                            sourceObserver.observer.OnError(error);
                        }
                        finally
                        {
                            sourceObserver.Dispose();
                            subscription.Dispose();
                        }
                    }
                }

                public void OnCompleted()
                {
                    lock (sourceObserver.gate)
                    {
                        try
                        {
                            sourceObserver.observer.OnCompleted();
                        }
                        finally
                        {
                            sourceObserver.Dispose();
                            subscription.Dispose();
                        }
                    }
                }
            }
        }
    }
}