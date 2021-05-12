using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class CatchObservable<T, TException> : OperatorObservableBase<T>
        where TException : Exception
    {
        readonly IObservable<T> source;
        readonly Func<TException, IObservable<T>> errorHandler;

        public CatchObservable(IObservable<T> source, Func<TException, IObservable<T>> errorHandler)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.errorHandler = errorHandler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new Catch(this, observer, cancel).Run();
        }

        class Catch : OperatorObserverBase<T, T>
        {
            readonly CatchObservable<T, TException> parent;
            SingleAssignmentDisposable sourceSubscription;
            SingleAssignmentDisposable exceptionSubscription;

            public Catch(CatchObservable<T, TException> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                this.sourceSubscription = new SingleAssignmentDisposable();
                this.exceptionSubscription = new SingleAssignmentDisposable();

                this.sourceSubscription.Disposable = parent.source.Subscribe(this);
                return StableCompositeDisposable.Create(sourceSubscription, exceptionSubscription);
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                var e = error as TException;
                if (e != null)
                {
                    IObservable<T> next;
                    try
                    {
                        if (parent.errorHandler == Stubs.CatchIgnore<T>)
                        {
                            next = Observable.Empty<T>(); // for avoid iOS AOT
                        }
                        else
                        {
                            next = parent.errorHandler(e);
                        }
                    }
                    catch (Exception ex)
                    {
                        try { observer.OnError(ex); } finally { Dispose(); };
                        return;
                    }

                    exceptionSubscription.Disposable = next.Subscribe(observer);
                }
                else
                {
                    try { observer.OnError(error); } finally { Dispose(); };
                    return;
                }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); } finally { Dispose(); };
            }
        }
    }


    internal class CatchObservable<T> : OperatorObservableBase<T>
    {
        readonly IEnumerable<IObservable<T>> sources;

        public CatchObservable(IEnumerable<IObservable<T>> sources)
            : base(true)
        {
            this.sources = sources;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new Catch(this, observer, cancel).Run();
        }

        class Catch : OperatorObserverBase<T, T>
        {
            readonly CatchObservable<T> parent;
            readonly object gate = new object();
            bool isDisposed;
            IEnumerator<IObservable<T>> e;
            SerialDisposable subscription;
            Exception lastException;
            Action nextSelf;

            public Catch(CatchObservable<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                isDisposed = false;
                e = parent.sources.GetEnumerator();
                subscription = new SerialDisposable();

                var schedule = Scheduler.DefaultSchedulers.TailRecursion.Schedule(RecursiveRun);

                return StableCompositeDisposable.Create(schedule, subscription, Disposable.Create(() =>
                {
                    lock (gate)
                    {
                        this.isDisposed = true;
                        this.e.Dispose();
                    }
                }));
            }

            void RecursiveRun(Action self)
            {
                lock (gate)
                {
                    nextSelf = self;
                    if (isDisposed) return;

                    var current = default(IObservable<T>);
                    var hasNext = false;
                    var ex = default(Exception);

                    try
                    {
                        hasNext = e.MoveNext();
                        if (hasNext)
                        {
                            current = e.Current;
                            if (current == null) throw new InvalidOperationException("sequence is null.");
                        }
                        else
                        {
                            e.Dispose();
                        }
                    }
                    catch (Exception exception)
                    {
                        ex = exception;
                        e.Dispose();
                    }

                    if (ex != null)
                    {
                        try { observer.OnError(ex); }
                        finally { Dispose(); }
                        return;
                    }

                    if (!hasNext)
                    {
                        if (lastException != null)
                        {
                            try { observer.OnError(lastException); }
                            finally { Dispose(); }
                        }
                        else
                        {
                            try { observer.OnCompleted(); }
                            finally { Dispose(); }
                        }
                        return;
                    }

                    var source = current;
                    var d = new SingleAssignmentDisposable();
                    subscription.Disposable = d;
                    d.Disposable = source.Subscribe(this);
                }
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                lastException = error;
                nextSelf();
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); }
                finally { Dispose(); }
                return;
            }
        }
    }
}