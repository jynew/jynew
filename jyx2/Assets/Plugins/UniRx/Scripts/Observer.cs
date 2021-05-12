using System;
using System.Threading;
using UniRx.InternalUtil;

namespace UniRx
{
    public static class Observer
    {
        internal static IObserver<T> CreateSubscribeObserver<T>(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            // need compare for avoid iOS AOT
            if (onNext == Stubs<T>.Ignore)
            {
                return new Subscribe_<T>(onError, onCompleted);
            }
            else
            {
                return new Subscribe<T>(onNext, onError, onCompleted);
            }
        }

        internal static IObserver<T> CreateSubscribeWithStateObserver<T, TState>(TState state, Action<T, TState> onNext, Action<Exception, TState> onError, Action<TState> onCompleted)
        {
            return new Subscribe<T, TState>(state, onNext, onError, onCompleted);
        }

        internal static IObserver<T> CreateSubscribeWithState2Observer<T, TState1, TState2>(TState1 state1, TState2 state2, Action<T, TState1, TState2> onNext, Action<Exception, TState1, TState2> onError, Action<TState1, TState2> onCompleted)
        {
            return new Subscribe<T, TState1, TState2>(state1, state2, onNext, onError, onCompleted);
        }

        internal static IObserver<T> CreateSubscribeWithState3Observer<T, TState1, TState2, TState3>(TState1 state1, TState2 state2, TState3 state3, Action<T, TState1, TState2, TState3> onNext, Action<Exception, TState1, TState2, TState3> onError, Action<TState1, TState2, TState3> onCompleted)
        {
            return new Subscribe<T, TState1, TState2, TState3>(state1, state2, state3, onNext, onError, onCompleted);
        }

        public static IObserver<T> Create<T>(Action<T> onNext)
        {
            return Create<T>(onNext, UniRx.Stubs.Throw, UniRx.Stubs.Nop);
        }

        public static IObserver<T> Create<T>(Action<T> onNext, Action<Exception> onError)
        {
            return Create<T>(onNext, onError, UniRx.Stubs.Nop);
        }

        public static IObserver<T> Create<T>(Action<T> onNext, Action onCompleted)
        {
            return Create<T>(onNext, UniRx.Stubs.Throw, onCompleted);
        }

        public static IObserver<T> Create<T>(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            // need compare for avoid iOS AOT
            if (onNext == Stubs<T>.Ignore)
            {
                return new EmptyOnNextAnonymousObserver<T>(onError, onCompleted);
            }
            else
            {
                return new AnonymousObserver<T>(onNext, onError, onCompleted);
            }
        }

        public static IObserver<T> CreateAutoDetachObserver<T>(IObserver<T> observer, IDisposable disposable)
        {
            return new AutoDetachObserver<T>(observer, disposable);
        }

        class AnonymousObserver<T> : IObserver<T>
        {
            readonly Action<T> onNext;
            readonly Action<Exception> onError;
            readonly Action onCompleted;

            int isStopped = 0;

            public AnonymousObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
            {
                this.onNext = onNext;
                this.onError = onError;
                this.onCompleted = onCompleted;
            }

            public void OnNext(T value)
            {
                if (isStopped == 0)
                {
                    onNext(value);
                }
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onError(error);
                }
            }


            public void OnCompleted()
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onCompleted();
                }
            }
        }

        class EmptyOnNextAnonymousObserver<T> : IObserver<T>
        {
            readonly Action<Exception> onError;
            readonly Action onCompleted;

            int isStopped = 0;

            public EmptyOnNextAnonymousObserver(Action<Exception> onError, Action onCompleted)
            {
                this.onError = onError;
                this.onCompleted = onCompleted;
            }

            public void OnNext(T value)
            {
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onError(error);
                }
            }

            public void OnCompleted()
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onCompleted();
                }
            }
        }

        // same as AnonymousObserver...
        class Subscribe<T> : IObserver<T>
        {
            readonly Action<T> onNext;
            readonly Action<Exception> onError;
            readonly Action onCompleted;

            int isStopped = 0;

            public Subscribe(Action<T> onNext, Action<Exception> onError, Action onCompleted)
            {
                this.onNext = onNext;
                this.onError = onError;
                this.onCompleted = onCompleted;
            }

            public void OnNext(T value)
            {
                if (isStopped == 0)
                {
                    onNext(value);
                }
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onError(error);
                }
            }


            public void OnCompleted()
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onCompleted();
                }
            }
        }

        // same as EmptyOnNextAnonymousObserver...
        class Subscribe_<T> : IObserver<T>
        {
            readonly Action<Exception> onError;
            readonly Action onCompleted;

            int isStopped = 0;

            public Subscribe_(Action<Exception> onError, Action onCompleted)
            {
                this.onError = onError;
                this.onCompleted = onCompleted;
            }

            public void OnNext(T value)
            {
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onError(error);
                }
            }

            public void OnCompleted()
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onCompleted();
                }
            }
        }

        // with state
        class Subscribe<T, TState> : IObserver<T>
        {
            readonly TState state;
            readonly Action<T, TState> onNext;
            readonly Action<Exception, TState> onError;
            readonly Action<TState> onCompleted;

            int isStopped = 0;

            public Subscribe(TState state, Action<T, TState> onNext, Action<Exception, TState> onError, Action<TState> onCompleted)
            {
                this.state = state;
                this.onNext = onNext;
                this.onError = onError;
                this.onCompleted = onCompleted;
            }

            public void OnNext(T value)
            {
                if (isStopped == 0)
                {
                    onNext(value, state);
                }
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onError(error, state);
                }
            }


            public void OnCompleted()
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onCompleted(state);
                }
            }
        }

        class Subscribe<T, TState1, TState2> : IObserver<T>
        {
            readonly TState1 state1;
            readonly TState2 state2;
            readonly Action<T, TState1, TState2> onNext;
            readonly Action<Exception, TState1, TState2> onError;
            readonly Action<TState1, TState2> onCompleted;

            int isStopped = 0;

            public Subscribe(TState1 state1, TState2 state2, Action<T, TState1, TState2> onNext, Action<Exception, TState1, TState2> onError, Action<TState1, TState2> onCompleted)
            {
                this.state1 = state1;
                this.state2 = state2;
                this.onNext = onNext;
                this.onError = onError;
                this.onCompleted = onCompleted;
            }

            public void OnNext(T value)
            {
                if (isStopped == 0)
                {
                    onNext(value, state1, state2);
                }
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onError(error, state1, state2);
                }
            }

            public void OnCompleted()
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onCompleted(state1, state2);
                }
            }
        }

        class Subscribe<T, TState1, TState2, TState3> : IObserver<T>
        {
            readonly TState1 state1;
            readonly TState2 state2;
            readonly TState3 state3;
            readonly Action<T, TState1, TState2, TState3> onNext;
            readonly Action<Exception, TState1, TState2, TState3> onError;
            readonly Action<TState1, TState2, TState3> onCompleted;

            int isStopped = 0;

            public Subscribe(TState1 state1, TState2 state2, TState3 state3, Action<T, TState1, TState2, TState3> onNext, Action<Exception, TState1, TState2, TState3> onError, Action<TState1, TState2, TState3> onCompleted)
            {
                this.state1 = state1;
                this.state2 = state2;
                this.state3 = state3;
                this.onNext = onNext;
                this.onError = onError;
                this.onCompleted = onCompleted;
            }

            public void OnNext(T value)
            {
                if (isStopped == 0)
                {
                    onNext(value, state1, state2, state3);
                }
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onError(error, state1, state2, state3);
                }
            }


            public void OnCompleted()
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onCompleted(state1, state2, state3);
                }
            }
        }

        class AutoDetachObserver<T> : UniRx.Operators.OperatorObserverBase<T, T>
        {
            public AutoDetachObserver(IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
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

    public static partial class ObserverExtensions
    {
        public static IObserver<T> Synchronize<T>(this IObserver<T> observer)
        {
            return new UniRx.Operators.SynchronizedObserver<T>(observer, new object());
        }

        public static IObserver<T> Synchronize<T>(this IObserver<T> observer, object gate)
        {
            return new UniRx.Operators.SynchronizedObserver<T>(observer, gate);
        }
    }

    public static partial class ObservableExtensions
    {
        public static IDisposable Subscribe<T>(this IObservable<T> source)
        {
            return source.Subscribe(UniRx.InternalUtil.ThrowObserver<T>.Instance);
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
        {
            return source.Subscribe(Observer.CreateSubscribeObserver(onNext, Stubs.Throw, Stubs.Nop));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError)
        {
            return source.Subscribe(Observer.CreateSubscribeObserver(onNext, onError, Stubs.Nop));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action onCompleted)
        {
            return source.Subscribe(Observer.CreateSubscribeObserver(onNext, Stubs.Throw, onCompleted));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return source.Subscribe(Observer.CreateSubscribeObserver(onNext, onError, onCompleted));
        }

        public static IDisposable SubscribeWithState<T, TState>(this IObservable<T> source, TState state, Action<T, TState> onNext)
        {
            return source.Subscribe(Observer.CreateSubscribeWithStateObserver(state, onNext, Stubs<TState>.Throw, Stubs<TState>.Ignore));
        }

        public static IDisposable SubscribeWithState<T, TState>(this IObservable<T> source, TState state, Action<T, TState> onNext, Action<Exception, TState> onError)
        {
            return source.Subscribe(Observer.CreateSubscribeWithStateObserver(state, onNext, onError, Stubs<TState>.Ignore));
        }

        public static IDisposable SubscribeWithState<T, TState>(this IObservable<T> source, TState state, Action<T, TState> onNext, Action<TState> onCompleted)
        {
            return source.Subscribe(Observer.CreateSubscribeWithStateObserver(state, onNext, Stubs<TState>.Throw, onCompleted));
        }

        public static IDisposable SubscribeWithState<T, TState>(this IObservable<T> source, TState state, Action<T, TState> onNext, Action<Exception, TState> onError, Action<TState> onCompleted)
        {
            return source.Subscribe(Observer.CreateSubscribeWithStateObserver(state, onNext, onError, onCompleted));
        }

        public static IDisposable SubscribeWithState2<T, TState1, TState2>(this IObservable<T> source, TState1 state1, TState2 state2, Action<T, TState1, TState2> onNext)
        {
            return source.Subscribe(Observer.CreateSubscribeWithState2Observer(state1, state2, onNext, Stubs<TState1, TState2>.Throw, Stubs<TState1, TState2>.Ignore));
        }

        public static IDisposable SubscribeWithState2<T, TState1, TState2>(this IObservable<T> source, TState1 state1, TState2 state2, Action<T, TState1, TState2> onNext, Action<Exception, TState1, TState2> onError)
        {
            return source.Subscribe(Observer.CreateSubscribeWithState2Observer(state1, state2, onNext, onError, Stubs<TState1, TState2>.Ignore));
        }

        public static IDisposable SubscribeWithState2<T, TState1, TState2>(this IObservable<T> source, TState1 state1, TState2 state2, Action<T, TState1, TState2> onNext, Action<TState1, TState2> onCompleted)
        {
            return source.Subscribe(Observer.CreateSubscribeWithState2Observer(state1, state2, onNext, Stubs<TState1, TState2>.Throw, onCompleted));
        }

        public static IDisposable SubscribeWithState2<T, TState1, TState2>(this IObservable<T> source, TState1 state1, TState2 state2, Action<T, TState1, TState2> onNext, Action<Exception, TState1, TState2> onError, Action<TState1, TState2> onCompleted)
        {
            return source.Subscribe(Observer.CreateSubscribeWithState2Observer(state1, state2, onNext, onError, onCompleted));
        }

        public static IDisposable SubscribeWithState3<T, TState1, TState2, TState3>(this IObservable<T> source, TState1 state1, TState2 state2, TState3 state3, Action<T, TState1, TState2, TState3> onNext)
        {
            return source.Subscribe(Observer.CreateSubscribeWithState3Observer(state1, state2, state3, onNext, Stubs<TState1, TState2, TState3>.Throw, Stubs<TState1, TState2, TState3>.Ignore));
        }

        public static IDisposable SubscribeWithState3<T, TState1, TState2, TState3>(this IObservable<T> source, TState1 state1, TState2 state2, TState3 state3, Action<T, TState1, TState2, TState3> onNext, Action<Exception, TState1, TState2, TState3> onError)
        {
            return source.Subscribe(Observer.CreateSubscribeWithState3Observer(state1, state2, state3, onNext, onError, Stubs<TState1, TState2, TState3>.Ignore));
        }

        public static IDisposable SubscribeWithState3<T, TState1, TState2, TState3>(this IObservable<T> source, TState1 state1, TState2 state2, TState3 state3, Action<T, TState1, TState2, TState3> onNext, Action<TState1, TState2, TState3> onCompleted)
        {
            return source.Subscribe(Observer.CreateSubscribeWithState3Observer(state1, state2, state3, onNext, Stubs<TState1, TState2, TState3>.Throw, onCompleted));
        }

        public static IDisposable SubscribeWithState3<T, TState1, TState2, TState3>(this IObservable<T> source, TState1 state1, TState2 state2, TState3 state3, Action<T, TState1, TState2, TState3> onNext, Action<Exception, TState1, TState2, TState3> onError, Action<TState1, TState2, TState3> onCompleted)
        {
            return source.Subscribe(Observer.CreateSubscribeWithState3Observer(state1, state2, state3, onNext, onError, onCompleted));
        }
    }

    internal static class Stubs
    {
        public static readonly Action Nop = () => { };
        public static readonly Action<Exception> Throw = ex => { ex.Throw(); };

        // marker for CatchIgnore and Catch avoid iOS AOT problem.
        public static IObservable<TSource> CatchIgnore<TSource>(Exception ex)
        {
            return Observable.Empty<TSource>();
        }
    }

    internal static class Stubs<T>
    {
        public static readonly Action<T> Ignore = (T t) => { };
        public static readonly Func<T, T> Identity = (T t) => t;
        public static readonly Action<Exception, T> Throw = (ex, _) => { ex.Throw(); };
    }

    internal static class Stubs<T1, T2>
    {
        public static readonly Action<T1, T2> Ignore = (x, y) => { };
        public static readonly Action<Exception, T1, T2> Throw = (ex, _, __) => { ex.Throw(); };
    }


    internal static class Stubs<T1, T2, T3>
    {
        public static readonly Action<T1, T2, T3> Ignore = (x, y, z) => { };
        public static readonly Action<Exception, T1, T2, T3> Throw = (ex, _, __, ___) => { ex.Throw(); };
    }
}