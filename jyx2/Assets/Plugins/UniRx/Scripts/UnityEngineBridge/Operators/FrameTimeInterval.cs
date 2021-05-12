using System;

namespace UniRx.Operators
{
    internal class FrameTimeIntervalObservable<T> : OperatorObservableBase<UniRx.TimeInterval<T>>
    {
        readonly IObservable<T> source;
        readonly bool ignoreTimeScale;

        public FrameTimeIntervalObservable(IObservable<T> source, bool ignoreTimeScale)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.ignoreTimeScale = ignoreTimeScale;
        }

        protected override IDisposable SubscribeCore(IObserver<UniRx.TimeInterval<T>> observer, IDisposable cancel)
        {
            return source.Subscribe(new FrameTimeInterval(this, observer, cancel));
        }

        class FrameTimeInterval : OperatorObserverBase<T, UniRx.TimeInterval<T>>
        {
            readonly FrameTimeIntervalObservable<T> parent;
            float lastTime;

            public FrameTimeInterval(FrameTimeIntervalObservable<T> parent, IObserver<UniRx.TimeInterval<T>> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
                this.lastTime = (parent.ignoreTimeScale)
                    ? UnityEngine.Time.unscaledTime
                    : UnityEngine.Time.time;
            }

            public override void OnNext(T value)
            {
                var now = (parent.ignoreTimeScale)
                    ? UnityEngine.Time.unscaledTime
                    : UnityEngine.Time.time;
                var span = now - lastTime;
                lastTime = now;

                base.observer.OnNext(new UniRx.TimeInterval<T>(value, TimeSpan.FromSeconds(span)));
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