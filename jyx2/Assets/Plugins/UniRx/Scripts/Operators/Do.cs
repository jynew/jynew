using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx.Operators
{
    // Do, DoOnError, DoOnCompleted, DoOnTerminate, DoOnSubscribe, DoOnCancel

    internal class DoObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly Action<T> onNext;
        readonly Action<Exception> onError;
        readonly Action onCompleted;

        public DoObservable(IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.onNext = onNext;
            this.onError = onError;
            this.onCompleted = onCompleted;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new Do(this, observer, cancel).Run();
        }

        class Do : OperatorObserverBase<T, T>
        {
            readonly DoObservable<T> parent;

            public Do(DoObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                return parent.source.Subscribe(this);
            }

            public override void OnNext(T value)
            {
                try
                {
                    parent.onNext(value);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); } finally { Dispose(); };
                    return;
                }
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try
                {
                    parent.onError(error);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); } finally { Dispose(); };
                    return;
                }
                try { observer.OnError(error); } finally { Dispose(); };
            }

            public override void OnCompleted()
            {
                try
                {
                    parent.onCompleted();
                }
                catch (Exception ex)
                {
                    base.observer.OnError(ex);
                    Dispose();
                    return;
                }
                try { observer.OnCompleted(); } finally { Dispose(); };
            }
        }
    }

    internal class DoObserverObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IObserver<T> observer;

        public DoObserverObservable(IObservable<T> source, IObserver<T> observer)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.observer = observer;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new Do(this, observer, cancel).Run();
        }

        class Do : OperatorObserverBase<T, T>
        {
            readonly DoObserverObservable<T> parent;

            public Do(DoObserverObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                return parent.source.Subscribe(this);
            }

            public override void OnNext(T value)
            {
                try
                {
                    parent.observer.OnNext(value);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); }
                    finally { Dispose(); }
                    return;
                }
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try
                {
                    parent.observer.OnError(error);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); }
                    finally { Dispose(); }
                    return;
                }

                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try
                {
                    parent.observer.OnCompleted();
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); }
                    finally { Dispose(); }
                    return;
                }

                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }
    }

    internal class DoOnErrorObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly Action<Exception> onError;

        public DoOnErrorObservable(IObservable<T> source, Action<Exception> onError)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.onError = onError;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new DoOnError(this, observer, cancel).Run();
        }

        class DoOnError : OperatorObserverBase<T, T>
        {
            readonly DoOnErrorObservable<T> parent;

            public DoOnError(DoOnErrorObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                return parent.source.Subscribe(this);
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try
                {
                    parent.onError(error);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); }
                    finally { Dispose(); }
                    return;
                }


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

    internal class DoOnCompletedObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly Action onCompleted;

        public DoOnCompletedObservable(IObservable<T> source, Action onCompleted)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.onCompleted = onCompleted;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new DoOnCompleted(this, observer, cancel).Run();
        }

        class DoOnCompleted : OperatorObserverBase<T, T>
        {
            readonly DoOnCompletedObservable<T> parent;

            public DoOnCompleted(DoOnCompletedObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                return parent.source.Subscribe(this);
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
                try
                {
                    parent.onCompleted();
                }
                catch (Exception ex)
                {
                    base.observer.OnError(ex);
                    Dispose();
                    return;
                }
                try { observer.OnCompleted(); } finally { Dispose(); };
            }
        }
    }

    internal class DoOnTerminateObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly Action onTerminate;

        public DoOnTerminateObservable(IObservable<T> source, Action onTerminate)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.onTerminate = onTerminate;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new DoOnTerminate(this, observer, cancel).Run();
        }

        class DoOnTerminate : OperatorObserverBase<T, T>
        {
            readonly DoOnTerminateObservable<T> parent;

            public DoOnTerminate(DoOnTerminateObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                return parent.source.Subscribe(this);
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try
                {
                    parent.onTerminate();
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); }
                    finally { Dispose(); }
                    return;
                }
                try { observer.OnError(error); } finally { Dispose(); };
            }

            public override void OnCompleted()
            {
                try
                {
                    parent.onTerminate();
                }
                catch (Exception ex)
                {
                    base.observer.OnError(ex);
                    Dispose();
                    return;
                }
                try { observer.OnCompleted(); } finally { Dispose(); };
            }
        }
    }

    internal class DoOnSubscribeObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly Action onSubscribe;

        public DoOnSubscribeObservable(IObservable<T> source, Action onSubscribe)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.onSubscribe = onSubscribe;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new DoOnSubscribe(this, observer, cancel).Run();
        }

        class DoOnSubscribe : OperatorObserverBase<T, T>
        {
            readonly DoOnSubscribeObservable<T> parent;

            public DoOnSubscribe(DoOnSubscribeObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                try
                {
                    parent.onSubscribe();
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); }
                    finally { Dispose(); }
                    return Disposable.Empty;
                }

                return parent.source.Subscribe(this);
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
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

    internal class DoOnCancelObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly Action onCancel;

        public DoOnCancelObservable(IObservable<T> source, Action onCancel)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.onCancel = onCancel;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new DoOnCancel(this, observer, cancel).Run();
        }

        class DoOnCancel : OperatorObserverBase<T, T>
        {
            readonly DoOnCancelObservable<T> parent;
            bool isCompletedCall = false;

            public DoOnCancel(DoOnCancelObservable<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                return StableCompositeDisposable.Create(parent.source.Subscribe(this), Disposable.Create(() =>
                {
                    if (!isCompletedCall)
                    {
                        parent.onCancel();
                    }
                }));
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                isCompletedCall = true;
                try { observer.OnError(error); } finally { Dispose(); };
            }

            public override void OnCompleted()
            {
                isCompletedCall = true;
                try { observer.OnCompleted(); } finally { Dispose(); };
            }
        }
    }
}