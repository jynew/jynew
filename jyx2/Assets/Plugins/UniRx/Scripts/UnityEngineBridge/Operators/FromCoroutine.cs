using System;
using System.Collections;
using System.Threading;

namespace UniRx.Operators
{
    internal class FromCoroutineObservable<T> : OperatorObservableBase<T>
    {
        readonly Func<IObserver<T>, CancellationToken, IEnumerator> coroutine;

        public FromCoroutineObservable(Func<IObserver<T>, CancellationToken, IEnumerator> coroutine)
            : base(false)
        {
            this.coroutine = coroutine;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            var fromCoroutineObserver = new FromCoroutine(observer, cancel);

#if (NETFX_CORE || NET_4_6 || NET_STANDARD_2_0 || UNITY_WSA_10_0)
            var moreCancel = new CancellationDisposable();
            var token = moreCancel.Token;
#else
            var moreCancel = new BooleanDisposable();
            var token = new CancellationToken(moreCancel);
#endif

            MainThreadDispatcher.SendStartCoroutine(coroutine(fromCoroutineObserver, token));

            return moreCancel;
        }

        class FromCoroutine : OperatorObserverBase<T, T>
        {
            public FromCoroutine(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
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

    internal class FromMicroCoroutineObservable<T> : OperatorObservableBase<T>
    {
        readonly Func<IObserver<T>, CancellationToken, IEnumerator> coroutine;
        readonly FrameCountType frameCountType;

        public FromMicroCoroutineObservable(Func<IObserver<T>, CancellationToken, IEnumerator> coroutine, FrameCountType frameCountType)
            : base(false)
        {
            this.coroutine = coroutine;
            this.frameCountType = frameCountType;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            var microCoroutineObserver = new FromMicroCoroutine(observer, cancel);

#if (NETFX_CORE || NET_4_6 || NET_STANDARD_2_0 || UNITY_WSA_10_0)
            var moreCancel = new CancellationDisposable();
            var token = moreCancel.Token;
#else
            var moreCancel = new BooleanDisposable();
            var token = new CancellationToken(moreCancel);
#endif

            switch (frameCountType)
            {
                case FrameCountType.Update:
                    MainThreadDispatcher.StartUpdateMicroCoroutine(coroutine(microCoroutineObserver, token));
                    break;
                case FrameCountType.FixedUpdate:
                    MainThreadDispatcher.StartFixedUpdateMicroCoroutine(coroutine(microCoroutineObserver, token));
                    break;
                case FrameCountType.EndOfFrame:
                    MainThreadDispatcher.StartEndOfFrameMicroCoroutine(coroutine(microCoroutineObserver, token));
                    break;
                default:
                    throw new ArgumentException("Invalid FrameCountType:" + frameCountType);
            }

            return moreCancel;
        }

        class FromMicroCoroutine : OperatorObserverBase<T, T>
        {
            public FromMicroCoroutine(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
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
}