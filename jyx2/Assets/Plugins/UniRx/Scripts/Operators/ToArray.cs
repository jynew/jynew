using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class ToArrayObservable<TSource> : OperatorObservableBase<TSource[]>
    {
        readonly IObservable<TSource> source;

        public ToArrayObservable(IObservable<TSource> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<TSource[]> observer, IDisposable cancel)
        {
            return source.Subscribe(new ToArray(observer, cancel));
        }

        class ToArray : OperatorObserverBase<TSource, TSource[]>
        {
            readonly List<TSource> list = new List<TSource>();

            public ToArray(IObserver<TSource[]> observer, IDisposable cancel)
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
                TSource[] result;
                try
                {
                    result = list.ToArray();
                }
                catch (Exception ex) 
                {
                    try { observer.OnError(ex); } finally { Dispose(); }
                    return;
                }

                base.observer.OnNext(result);
                try { observer.OnCompleted(); } finally { Dispose(); };
            }
        }
    }
}