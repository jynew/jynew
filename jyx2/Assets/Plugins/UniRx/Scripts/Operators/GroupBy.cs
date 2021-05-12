using System;
using System.Collections.Generic;
using UniRx.Operators;

namespace UniRx.Operators
{
    internal class GroupedObservable<TKey, TElement> : IGroupedObservable<TKey, TElement>
    {
        readonly TKey key;
        readonly IObservable<TElement> subject;
        readonly RefCountDisposable refCount;

        public TKey Key
        {
            get { return key; }
        }

        public GroupedObservable(TKey key, ISubject<TElement> subject, RefCountDisposable refCount)
        {
            this.key = key;
            this.subject = subject;
            this.refCount = refCount;
        }

        public IDisposable Subscribe(IObserver<TElement> observer)
        {
            var release = refCount.GetDisposable();
            var subscription = subject.Subscribe(observer);
            return StableCompositeDisposable.Create(release, subscription);
        }
    }

    internal class GroupByObservable<TSource, TKey, TElement> : OperatorObservableBase<IGroupedObservable<TKey, TElement>>
    {
        readonly IObservable<TSource> source;
        readonly Func<TSource, TKey> keySelector;
        readonly Func<TSource, TElement> elementSelector;
        readonly int? capacity;
        readonly IEqualityComparer<TKey> comparer;

        public GroupByObservable(IObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, int? capacity, IEqualityComparer<TKey> comparer)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.keySelector = keySelector;
            this.elementSelector = elementSelector;
            this.capacity = capacity;
            this.comparer = comparer;
        }

        protected override IDisposable SubscribeCore(IObserver<IGroupedObservable<TKey, TElement>> observer, IDisposable cancel)
        {
            return new GroupBy(this, observer, cancel).Run();
        }

        class GroupBy : OperatorObserverBase<TSource, IGroupedObservable<TKey, TElement>>
        {
            readonly GroupByObservable<TSource, TKey, TElement> parent;
            readonly Dictionary<TKey, ISubject<TElement>> map;
            ISubject<TElement> nullKeySubject;

            CompositeDisposable groupDisposable;
            RefCountDisposable refCountDisposable;

            public GroupBy(GroupByObservable<TSource, TKey, TElement> parent, IObserver<IGroupedObservable<TKey, TElement>> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
                if (parent.capacity.HasValue)
                {
                    map = new Dictionary<TKey, ISubject<TElement>>(parent.capacity.Value, parent.comparer);
                }
                else
                {
                    map = new Dictionary<TKey, ISubject<TElement>>(parent.comparer);
                }
            }

            public IDisposable Run()
            {
                groupDisposable = new CompositeDisposable();
                refCountDisposable = new RefCountDisposable(groupDisposable);

                groupDisposable.Add(parent.source.Subscribe(this));

                return refCountDisposable;
            }

            public override void OnNext(TSource value)
            {
                var key = default(TKey);
                try
                {
                    key = parent.keySelector(value);
                }
                catch (Exception exception)
                {
                    Error(exception);
                    return;
                }

                var fireNewMapEntry = false;
                var writer = default(ISubject<TElement>);
                try
                {
                    if (key == null)
                    {
                        if (nullKeySubject == null)
                        {
                            nullKeySubject = new Subject<TElement>();
                            fireNewMapEntry = true;
                        }

                        writer = nullKeySubject;
                    }
                    else
                    {
                        if (!map.TryGetValue(key, out writer))
                        {
                            writer = new Subject<TElement>();
                            map.Add(key, writer);
                            fireNewMapEntry = true;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Error(exception);
                    return;
                }

                if (fireNewMapEntry)
                {
                    var group = new GroupedObservable<TKey, TElement>(key, writer, refCountDisposable);
                    observer.OnNext(group);
                }

                var element = default(TElement);
                try
                {
                    element = parent.elementSelector(value);
                }
                catch (Exception exception)
                {
                    Error(exception);
                    return;
                }

                writer.OnNext(element);
            }

            public override void OnError(Exception error)
            {
                Error(error);
            }

            public override void OnCompleted()
            {
                try
                {
                    if (nullKeySubject != null) nullKeySubject.OnCompleted();

                    foreach (var s in map.Values)
                    {
                        s.OnCompleted();
                    }

                    observer.OnCompleted();
                }
                finally
                {
                    Dispose();
                }
            }

            void Error(Exception exception)
            {
                try
                {
                    if (nullKeySubject != null) nullKeySubject.OnError(exception);

                    foreach (var s in map.Values)
                    {
                        s.OnError(exception);
                    }

                    observer.OnError(exception);
                }
                finally
                {
                    Dispose();
                }
            }
        }
    }
}