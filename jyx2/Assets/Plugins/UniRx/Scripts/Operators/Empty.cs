using System;

namespace UniRx.Operators
{
    internal class EmptyObservable<T> : OperatorObservableBase<T>
    {
        readonly IScheduler scheduler;

        public EmptyObservable(IScheduler scheduler)
            : base(false)
        {
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            observer = new Empty(observer, cancel);

            if (scheduler == Scheduler.Immediate)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }
            else
            {
                return scheduler.Schedule(observer.OnCompleted);
            }
        }

        class Empty : OperatorObserverBase<T, T>
        {
            public Empty(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
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
                    Dispose();
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

    internal class ImmutableEmptyObservable<T> : IObservable<T>, IOptimizedObservable<T>
    {
        internal static ImmutableEmptyObservable<T> Instance = new ImmutableEmptyObservable<T>();

        ImmutableEmptyObservable()
        {

        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            observer.OnCompleted();
            return Disposable.Empty;
        }
    }
}