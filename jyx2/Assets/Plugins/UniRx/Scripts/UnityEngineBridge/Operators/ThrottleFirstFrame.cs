using System;

#if UniRxLibrary
using UnityObservable = UniRx.ObservableUnity;
#else
using UnityObservable = UniRx.Observable;
#endif

namespace UniRx.Operators
{
    internal class ThrottleFirstFrameObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly int frameCount;
        readonly FrameCountType frameCountType;

        public ThrottleFirstFrameObservable(IObservable<T> source, int frameCount, FrameCountType frameCountType) : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.frameCount = frameCount;
            this.frameCountType = frameCountType;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new ThrottleFirstFrame(this, observer, cancel).Run();
        }

        class ThrottleFirstFrame : OperatorObserverBase<T, T>
        {
            readonly ThrottleFirstFrameObservable<T> parent;
            readonly object gate = new object();
            bool open = true;
            SerialDisposable cancelable;

            ThrottleFirstFrameTick tick;

            public ThrottleFirstFrame(ThrottleFirstFrameObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                tick = new ThrottleFirstFrameTick(this);
                cancelable = new SerialDisposable();

                var subscription = parent.source.Subscribe(this);
                return StableCompositeDisposable.Create(cancelable, subscription);
            }

            void OnNext()
            {
                lock (gate)
                {
                    open = true;
                }
            }

            public override void OnNext(T value)
            {
                lock (gate)
                {
                    if (!open) return;
                    observer.OnNext(value);
                    open = false;
                }

                var d = new SingleAssignmentDisposable();
                cancelable.Disposable = d;
                d.Disposable = UnityObservable.TimerFrame(parent.frameCount, parent.frameCountType)
                    .Subscribe(tick);
            }

            public override void OnError(Exception error)
            {
                cancelable.Dispose();

                lock (gate)
                {
                    try { observer.OnError(error); } finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                cancelable.Dispose();

                lock (gate)
                {
                    try { observer.OnCompleted(); } finally { Dispose(); }
                }
            }

            // immutable, can share.
            class ThrottleFirstFrameTick : IObserver<long>
            {
                readonly ThrottleFirstFrame parent;

                public ThrottleFirstFrameTick(ThrottleFirstFrame parent)
                {
                    this.parent = parent;
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
                        parent.open = true;
                    }
                }
            }
        }
    }
}