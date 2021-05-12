using System;

#if UniRxLibrary
using UnityObservable = UniRx.ObservableUnity;
#else
using UnityObservable = UniRx.Observable;
#endif

namespace UniRx.Operators
{
    internal class TimeoutFrameObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly int frameCount;
        readonly FrameCountType frameCountType;

        public TimeoutFrameObservable(IObservable<T> source, int frameCount, FrameCountType frameCountType) : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.frameCount = frameCount;
            this.frameCountType = frameCountType;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new TimeoutFrame(this, observer, cancel).Run();
        }

        class TimeoutFrame : OperatorObserverBase<T, T>
        {
            readonly TimeoutFrameObservable<T> parent;
            readonly object gate = new object();
            ulong objectId = 0ul;
            bool isTimeout = false;
            SingleAssignmentDisposable sourceSubscription;
            SerialDisposable timerSubscription;

            public TimeoutFrame(TimeoutFrameObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                sourceSubscription = new SingleAssignmentDisposable();
                timerSubscription = new SerialDisposable();
                timerSubscription.Disposable = RunTimer(objectId);
                sourceSubscription.Disposable = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(timerSubscription, sourceSubscription);
            }

            IDisposable RunTimer(ulong timerId)
            {
                return UnityObservable.TimerFrame(parent.frameCount, parent.frameCountType)
                    .Subscribe(new TimeoutFrameTick(this, timerId));
            }

            public override void OnNext(T value)
            {
                ulong useObjectId;
                bool timeout;
                lock (gate)
                {
                    timeout = isTimeout;
                    objectId++;
                    useObjectId = objectId;
                }
                if (timeout) return;

                timerSubscription.Disposable = Disposable.Empty; // cancel old timer
                observer.OnNext(value);
                timerSubscription.Disposable = RunTimer(useObjectId);
            }

            public override void OnError(Exception error)
            {
                bool timeout;
                lock (gate)
                {
                    timeout = isTimeout;
                    objectId++;
                }
                if (timeout) return;

                timerSubscription.Dispose();
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                bool timeout;
                lock (gate)
                {
                    timeout = isTimeout;
                    objectId++;
                }
                if (timeout) return;

                timerSubscription.Dispose();
                try { observer.OnCompleted(); } finally { Dispose(); }
            }

            class TimeoutFrameTick : IObserver<long>
            {
                readonly TimeoutFrame parent;
                readonly ulong timerId;

                public TimeoutFrameTick(TimeoutFrame parent, ulong timerId)
                {
                    this.parent = parent;
                    this.timerId = timerId;
                }

                public void OnCompleted()
                {
                }

                public void OnError(Exception error)
                {
                }

                public void OnNext(long _)
                {


                    lock (parent.gate)
                    {
                        if (parent.objectId == timerId)
                        {
                            parent.isTimeout = true;
                        }
                    }
                    if (parent.isTimeout)
                    {
                        try { parent.observer.OnError(new TimeoutException()); } finally { parent.Dispose(); }
                    }
                }
            }
        }
    }
}