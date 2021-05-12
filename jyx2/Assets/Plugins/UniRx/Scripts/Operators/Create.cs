using System;

namespace UniRx.Operators
{
    internal class CreateObservable<T> : OperatorObservableBase<T>
    {
        readonly Func<IObserver<T>, IDisposable> subscribe;

        public CreateObservable(Func<IObserver<T>, IDisposable> subscribe)
            : base(true) // fail safe
        {
            this.subscribe = subscribe;
        }

        public CreateObservable(Func<IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
            : base(isRequiredSubscribeOnCurrentThread)
        {
            this.subscribe = subscribe;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            observer = new Create(observer, cancel);
            return subscribe(observer) ?? Disposable.Empty;
        }

        class Create : OperatorObserverBase<T, T>
        {
            public Create(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
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

    internal class CreateObservable<T, TState> : OperatorObservableBase<T>
    {
        readonly TState state;
        readonly Func<TState, IObserver<T>, IDisposable> subscribe;

        public CreateObservable(TState state, Func<TState, IObserver<T>, IDisposable> subscribe)
            : base(true) // fail safe
        {
            this.state = state;
            this.subscribe = subscribe;
        }

        public CreateObservable(TState state, Func<TState, IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
            : base(isRequiredSubscribeOnCurrentThread)
        {
            this.state = state;
            this.subscribe = subscribe;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            observer = new Create(observer, cancel);
            return subscribe(state, observer) ?? Disposable.Empty;
        }

        class Create : OperatorObserverBase<T, T>
        {
            public Create(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
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

    internal class CreateSafeObservable<T> : OperatorObservableBase<T>
    {
        readonly Func<IObserver<T>, IDisposable> subscribe;

        public CreateSafeObservable(Func<IObserver<T>, IDisposable> subscribe)
            : base(true) // fail safe
        {
            this.subscribe = subscribe;
        }

        public CreateSafeObservable(Func<IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
            : base(isRequiredSubscribeOnCurrentThread)
        {
            this.subscribe = subscribe;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            observer = new CreateSafe(observer, cancel);
            return subscribe(observer) ?? Disposable.Empty;
        }

        class CreateSafe : OperatorObserverBase<T, T>
        {
            public CreateSafe(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
            }

            public override void OnNext(T value)
            {
                try
                {
                    base.observer.OnNext(value);
                }
                catch
                {
                    Dispose(); // safe
                    throw;
                }
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