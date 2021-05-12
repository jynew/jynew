using System;

namespace UniRx.Operators
{
    internal class DematerializeObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<Notification<T>> source;

        public DematerializeObservable(IObservable<Notification<T>> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new Dematerialize(this, observer, cancel).Run();
        }

        class Dematerialize : OperatorObserverBase<Notification<T>, T>
        {
            readonly DematerializeObservable<T> parent;

            public Dematerialize(DematerializeObservable<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                return parent.source.Subscribe(this); 
            }

            public override void OnNext(Notification<T> value)
            {
                switch (value.Kind)
                {
                    case NotificationKind.OnNext:
                        observer.OnNext(value.Value);
                        break;
                    case NotificationKind.OnError:
                        try { observer.OnError(value.Exception); }
                        finally { Dispose(); }
                        break;
                    case NotificationKind.OnCompleted:
                        try { observer.OnCompleted(); }
                        finally { Dispose(); }
                        break;
                    default:
                        break;
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