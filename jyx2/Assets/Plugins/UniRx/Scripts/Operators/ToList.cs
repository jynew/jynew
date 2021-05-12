using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class ToListObservable<TSource> : OperatorObservableBase<IList<TSource>>
    {
        readonly IObservable<TSource> source;

        public ToListObservable(IObservable<TSource> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<IList<TSource>> observer, IDisposable cancel)
        {
            return source.Subscribe(new ToList(observer, cancel));
        }

        class ToList : OperatorObserverBase<TSource, IList<TSource>>
        {
            readonly List<TSource> list = new List<TSource>();

            public ToList(IObserver<IList<TSource>> observer, IDisposable cancel)
                : base(observer, cancel)
            {
            }

            public override void OnNext(TSource value)
            {
                try
                {
                    list.Add(value); // sometimes cause error on multithread
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); } finally { Dispose(); }
                    return;
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                base.observer.OnNext(list);
                try { observer.OnCompleted(); } finally { Dispose(); };
            }
        }
    }
}