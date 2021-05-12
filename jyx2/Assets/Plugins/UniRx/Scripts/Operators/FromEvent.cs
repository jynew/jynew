using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx.Operators
{
    // FromEvent, FromEventPattern

    internal class FromEventPatternObservable<TDelegate, TEventArgs> : OperatorObservableBase<EventPattern<TEventArgs>>
        where TEventArgs : EventArgs
    {
        readonly Func<EventHandler<TEventArgs>, TDelegate> conversion;
        readonly Action<TDelegate> addHandler;
        readonly Action<TDelegate> removeHandler;

        public FromEventPatternObservable(Func<EventHandler<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
            : base(false)
        {
            this.conversion = conversion;
            this.addHandler = addHandler;
            this.removeHandler = removeHandler;
        }

        protected override IDisposable SubscribeCore(IObserver<EventPattern<TEventArgs>> observer, IDisposable cancel)
        {
            var fe = new FromEventPattern(this, observer);
            return fe.Register() ?  fe : Disposable.Empty;
        }

        class FromEventPattern : IDisposable
        {
            readonly FromEventPatternObservable<TDelegate, TEventArgs> parent;
            readonly IObserver<EventPattern<TEventArgs>> observer;
            TDelegate handler;

            public FromEventPattern(FromEventPatternObservable<TDelegate, TEventArgs> parent, IObserver<EventPattern<TEventArgs>> observer)
            {
                this.parent = parent;
                this.observer = observer;
            }

            public bool Register()
            {
                handler = parent.conversion(OnNext);
                try
                {
                    parent.addHandler(handler);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    return false;
                }
                return true;
            }

            void OnNext(object sender, TEventArgs eventArgs)
            {
                observer.OnNext(new EventPattern<TEventArgs>(sender, eventArgs));
            }

            public void Dispose()
            {
                if (handler != null)
                {
                    parent.removeHandler(handler);
                    handler = default(TDelegate);
                }
            }
        }
    }

    internal class FromEventObservable<TDelegate> : OperatorObservableBase<Unit>
    {
        readonly Func<Action, TDelegate> conversion;
        readonly Action<TDelegate> addHandler;
        readonly Action<TDelegate> removeHandler;

        public FromEventObservable(Func<Action, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
            : base(false)
        {
            this.conversion = conversion;
            this.addHandler = addHandler;
            this.removeHandler = removeHandler;
        }

        protected override IDisposable SubscribeCore(IObserver<Unit> observer, IDisposable cancel)
        {
            var fe = new FromEvent(this, observer);
            return fe.Register() ?  fe : Disposable.Empty;
        }

        class FromEvent : IDisposable
        {
            readonly FromEventObservable<TDelegate> parent;
            readonly IObserver<Unit> observer;
            TDelegate handler;

            public FromEvent(FromEventObservable<TDelegate> parent, IObserver<Unit> observer)
            {
                this.parent = parent;
                this.observer = observer;
            }

            public bool Register()
            {
                handler = parent.conversion(OnNext);

                try
                {
                    parent.addHandler(handler);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    return false;
                }
                return true;
            }

            void OnNext()
            {
                observer.OnNext(Unit.Default);
            }

            public void Dispose()
            {
                if (handler != null)
                {
                    parent.removeHandler(handler);
                    handler = default(TDelegate);
                }
            }
        }
    }

    internal class FromEventObservable<TDelegate, TEventArgs> : OperatorObservableBase<TEventArgs>
    {
        readonly Func<Action<TEventArgs>, TDelegate> conversion;
        readonly Action<TDelegate> addHandler;
        readonly Action<TDelegate> removeHandler;

        public FromEventObservable(Func<Action<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
            : base(false)
        {
            this.conversion = conversion;
            this.addHandler = addHandler;
            this.removeHandler = removeHandler;
        }

        protected override IDisposable SubscribeCore(IObserver<TEventArgs> observer, IDisposable cancel)
        {
            var fe = new FromEvent(this, observer);
            return fe.Register() ?  fe : Disposable.Empty;
        }

        class FromEvent : IDisposable
        {
            readonly FromEventObservable<TDelegate, TEventArgs> parent;
            readonly IObserver<TEventArgs> observer;
            TDelegate handler;

            public FromEvent(FromEventObservable<TDelegate, TEventArgs> parent, IObserver<TEventArgs> observer)
            {
                this.parent = parent;
                this.observer = observer;
            }

            public bool Register()
            {
                handler = parent.conversion(OnNext);

                try
                {
                    parent.addHandler(handler);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    return false;
                }
                return true;
            }

            void OnNext(TEventArgs args)
            {
                observer.OnNext(args);
            }

            public void Dispose()
            {
                if (handler != null)
                {
                    parent.removeHandler(handler);
                    handler = default(TDelegate);
                }
            }
        }
    }

    internal class FromEventObservable : OperatorObservableBase<Unit>
    {
        readonly Action<Action> addHandler;
        readonly Action<Action> removeHandler;

        public FromEventObservable(Action<Action> addHandler, Action<Action> removeHandler)
            : base(false)
        {
            this.addHandler = addHandler;
            this.removeHandler = removeHandler;
        }

        protected override IDisposable SubscribeCore(IObserver<Unit> observer, IDisposable cancel)
        {
            var fe = new FromEvent(this, observer);
            return fe.Register() ?  fe : Disposable.Empty;
        }

        class FromEvent : IDisposable
        {
            readonly FromEventObservable parent;
            readonly IObserver<Unit> observer;
            Action handler;

            public FromEvent(FromEventObservable parent, IObserver<Unit> observer)
            {
                this.parent = parent;
                this.observer = observer;
                this.handler = OnNext;
            }

            public bool Register()
            {
                try
                {
                    parent.addHandler(handler);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    return false;
                }
                return true;
            }

            void OnNext()
            {
                observer.OnNext(Unit.Default);
            }

            public void Dispose()
            {
                if (handler != null)
                {
                    parent.removeHandler(handler);
                    handler = null;
                }
            }
        }
    }

    internal class FromEventObservable_<T> : OperatorObservableBase<T>
    {
        readonly Action<Action<T>> addHandler;
        readonly Action<Action<T>> removeHandler;

        public FromEventObservable_(Action<Action<T>> addHandler, Action<Action<T>> removeHandler)
            : base(false)
        {
            this.addHandler = addHandler;
            this.removeHandler = removeHandler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            var fe = new FromEvent(this, observer);
            return fe.Register() ?  fe : Disposable.Empty;
        }

        class FromEvent : IDisposable
        {
            readonly FromEventObservable_<T> parent;
            readonly IObserver<T> observer;
            Action<T> handler;

            public FromEvent(FromEventObservable_<T> parent, IObserver<T> observer)
            {
                this.parent = parent;
                this.observer = observer;
                this.handler = OnNext;
            }

            public bool Register()
            {
                try
                {
                    parent.addHandler(handler);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    return false;
                }
                return true;
            }

            void OnNext(T value)
            {
                observer.OnNext(value);
            }

            public void Dispose()
            {
                if (handler != null)
                {
                    parent.removeHandler(handler);
                    handler = null;
                }
            }
        }
    }
}