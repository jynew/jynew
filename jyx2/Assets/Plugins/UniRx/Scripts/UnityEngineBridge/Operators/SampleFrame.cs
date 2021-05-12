using System;

#if UniRxLibrary
using UnityObservable = UniRx.ObservableUnity;
#else
using UnityObservable = UniRx.Observable;
#endif

namespace UniRx.Operators
{
    internal class SampleFrameObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly int frameCount;
        readonly FrameCountType frameCountType;

        public SampleFrameObservable(IObservable<T> source, int frameCount, FrameCountType frameCountType) : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.frameCount = frameCount;
            this.frameCountType = frameCountType;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new SampleFrame(this, observer, cancel).Run();
        }

        class SampleFrame : OperatorObserverBase<T, T>
        {
            readonly SampleFrameObservable<T> parent;
            readonly object gate = new object();
            T latestValue = default(T);
            bool isUpdated = false;
            bool isCompleted = false;
            SingleAssignmentDisposable sourceSubscription;

            public SampleFrame(SampleFrameObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                sourceSubscription = new SingleAssignmentDisposable();
                sourceSubscription.Disposable = parent.source.Subscribe(this);
                
                var scheduling = UnityObservable.IntervalFrame(parent.frameCount, parent.frameCountType)
                    .Subscribe(new SampleFrameTick(this));

                return StableCompositeDisposable.Create(sourceSubscription, scheduling);
            }

            void OnNextTick(long _)
            {
                lock (gate)
                {
                    if (isUpdated)
                    {
                        var value = latestValue;
                        isUpdated = false;
                        observer.OnNext(value);
                    }
                    if (isCompleted)
                    {
                        try { observer.OnCompleted(); } finally { Dispose(); }
                    }
                }
            }

            public override void OnNext(T value)
            {
                lock (gate)
                {
                    latestValue = value;
                    isUpdated = true;
                }
            }

            public override void OnError(Exception error)
            {
                lock (gate)
                {
                    try { base.observer.OnError(error); } finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                lock (gate)
                {
                    isCompleted = true;
                    sourceSubscription.Dispose();
                }
            }
            class SampleFrameTick : IObserver<long>
            {
                readonly SampleFrame parent;

                public SampleFrameTick(SampleFrame parent)
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
                        if (parent.isUpdated)
                        {
                            var value = parent.latestValue;
                            parent.isUpdated = false;
                            parent.observer.OnNext(value);
                        }
                        if (parent.isCompleted)
                        {
                            try { parent.observer.OnCompleted(); } finally { parent.Dispose(); }
                        }
                    }
                }
            }
        }
    }
}