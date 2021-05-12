using System;
using UniRx.Operators;

namespace UniRx.Operators
{
    internal class ScanObservable<TSource> : OperatorObservableBase<TSource>
    {
        readonly IObservable<TSource> source;
        readonly Func<TSource, TSource, TSource> accumulator;

        public ScanObservable(IObservable<TSource> source, Func<TSource, TSource, TSource> accumulator)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.accumulator = accumulator;
        }

        protected override IDisposable SubscribeCore(IObserver<TSource> observer, IDisposable cancel)
        {
            return source.Subscribe(new Scan(this, observer, cancel));
        }

        class Scan : OperatorObserverBase<TSource, TSource>
        {
            readonly ScanObservable<TSource> parent;
            TSource accumulation;
            bool isFirst;

            public Scan(ScanObservable<TSource> parent, IObserver<TSource> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.isFirst = true;
            }

            public override void OnNext(TSource value)
            {
                if (isFirst)
                {
                    isFirst = false;
                    accumulation = value;
                }
                else
                {
                    try
                    {
                        accumulation = parent.accumulator(accumulation, value);
                    }
                    catch (Exception ex)
                    {
                        try { observer.OnError(ex); }
                        finally { Dispose(); }
                        return;
                    }
                }

                observer.OnNext(accumulation);
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

    internal class ScanObservable<TSource, TAccumulate> : OperatorObservableBase<TAccumulate>
    {
        readonly IObservable<TSource> source;
        readonly TAccumulate seed;
        readonly Func<TAccumulate, TSource, TAccumulate> accumulator;

        public ScanObservable(IObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.seed = seed;
            this.accumulator = accumulator;
        }

        protected override IDisposable SubscribeCore(IObserver<TAccumulate> observer, IDisposable cancel)
        {
            return source.Subscribe(new Scan(this, observer, cancel));
        }

        class Scan : OperatorObserverBase<TSource, TAccumulate>
        {
            readonly ScanObservable<TSource, TAccumulate> parent;
            TAccumulate accumulation;
            bool isFirst;

            public Scan(ScanObservable<TSource, TAccumulate> parent, IObserver<TAccumulate> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.isFirst = true;
            }

            public override void OnNext(TSource value)
            {
                if (isFirst)
                {
                    isFirst = false;
                    accumulation = parent.seed;
                }

                try
                {
                    accumulation = parent.accumulator(accumulation, value);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); }
                    finally { Dispose(); }
                    return;
                }

                observer.OnNext(accumulation);
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