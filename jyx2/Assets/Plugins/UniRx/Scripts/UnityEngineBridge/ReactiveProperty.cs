#if CSHARP_7_OR_LATER || (UNITY_2018_3_OR_NEWER && (NET_STANDARD_2_0 || NET_4_6))
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#endif

using System;
using System.Collections.Generic;
using System.Threading;
using UniRx.InternalUtil;
#if !UniRxLibrary
using UnityEngine;
#endif
#if CSHARP_7_OR_LATER || (UNITY_2018_3_OR_NEWER && (NET_STANDARD_2_0 || NET_4_6))
using System.Threading.Tasks;
#endif

namespace UniRx
{
    public interface IReadOnlyReactiveProperty<T> : IObservable<T>
    {
        T Value { get; }
        bool HasValue { get; }
    }

    public interface IReactiveProperty<T> : IReadOnlyReactiveProperty<T>
    {
        new T Value { get; set; }
    }

    internal interface IObserverLinkedList<T>
    {
        void UnsubscribeNode(ObserverNode<T> node);
    }

    internal sealed class ObserverNode<T> : IObserver<T>, IDisposable
    {
        readonly IObserver<T> observer;
        IObserverLinkedList<T> list;

        public ObserverNode<T> Previous { get; internal set; }
        public ObserverNode<T> Next { get; internal set; }

        public ObserverNode(IObserverLinkedList<T> list, IObserver<T> observer)
        {
            this.list = list;
            this.observer = observer;
        }

        public void OnNext(T value)
        {
            observer.OnNext(value);
        }

        public void OnError(Exception error)
        {
            observer.OnError(error);
        }

        public void OnCompleted()
        {
            observer.OnCompleted();
        }

        public void Dispose()
        {
            var sourceList = Interlocked.Exchange(ref list, null);
            if (sourceList != null)
            {
                sourceList.UnsubscribeNode(this);
                sourceList = null;
            }
        }
    }

    /// <summary>
    /// Lightweight property broker.
    /// </summary>
    [Serializable]
    public class ReactiveProperty<T> : IReactiveProperty<T>, IDisposable, IOptimizedObservable<T>, IObserverLinkedList<T>
    {
#if !UniRxLibrary
        static readonly IEqualityComparer<T> defaultEqualityComparer = UnityEqualityComparer.GetDefault<T>();
#else
        static readonly IEqualityComparer<T> defaultEqualityComparer = EqualityComparer<T>.Default;
#endif

#if !UniRxLibrary
        [SerializeField]
#endif
        T value = default(T);

        [NonSerialized]
        ObserverNode<T> root;

        [NonSerialized]
        ObserverNode<T> last;

        [NonSerialized]
        bool isDisposed = false;

        protected virtual IEqualityComparer<T> EqualityComparer
        {
            get
            {
                return defaultEqualityComparer;
            }
        }

        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                if (!EqualityComparer.Equals(this.value, value))
                {
                    SetValue(value);
                    if (isDisposed)
                        return;

                    RaiseOnNext(ref value);
                }
            }
        }

        // always true, allows empty constructor 'can' publish value on subscribe.
        // because sometimes value is deserialized from UnityEngine.
        public bool HasValue
        {
            get
            {
                return true;
            }
        }

        public ReactiveProperty()
            : this(default(T))
        {
        }

        public ReactiveProperty(T initialValue)
        {
            SetValue(initialValue);
        }

        void RaiseOnNext(ref T value)
        {
            var node = root;
            while (node != null)
            {
                node.OnNext(value);
                node = node.Next;
            }
        }

        protected virtual void SetValue(T value)
        {
            this.value = value;
        }

        public void SetValueAndForceNotify(T value)
        {
            SetValue(value);
            if (isDisposed)
                return;

            RaiseOnNext(ref value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (isDisposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            // raise latest value on subscribe
            observer.OnNext(value);

            // subscribe node, node as subscription.
            var next = new ObserverNode<T>(this, observer);
            if (root == null)
            {
                root = last = next;
            }
            else
            {
                last.Next = next;
                next.Previous = last;
                last = next;
            }
            return next;
        }

        void IObserverLinkedList<T>.UnsubscribeNode(ObserverNode<T> node)
        {
            if (node == root)
            {
                root = node.Next;
            }
            if (node == last)
            {
                last = node.Previous;
            }

            if (node.Previous != null)
            {
                node.Previous.Next = node.Next;
            }
            if (node.Next != null)
            {
                node.Next.Previous = node.Previous;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            var node = root;
            root = last = null;
            isDisposed = true;

            while (node != null)
            {
                node.OnCompleted();
                node = node.Next;
            }
        }

        public override string ToString()
        {
            return (value == null) ? "(null)" : value.ToString();
        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
        }
    }

    /// <summary>
    /// Lightweight property broker.
    /// </summary>
    public class ReadOnlyReactiveProperty<T> : IReadOnlyReactiveProperty<T>, IDisposable, IOptimizedObservable<T>, IObserverLinkedList<T>, IObserver<T>
    {
#if !UniRxLibrary
        static readonly IEqualityComparer<T> defaultEqualityComparer = UnityEqualityComparer.GetDefault<T>();
#else
        static readonly IEqualityComparer<T> defaultEqualityComparer = EqualityComparer<T>.Default;
#endif

        readonly bool distinctUntilChanged = true;
        bool canPublishValueOnSubscribe = false;
        bool isDisposed = false;
        bool isSourceCompleted = false;

        T latestValue = default(T);
        Exception lastException = null;
        IDisposable sourceConnection = null;

        ObserverNode<T> root;
        ObserverNode<T> last;

        public T Value
        {
            get
            {
                return latestValue;
            }
        }

        public bool HasValue
        {
            get
            {
                return canPublishValueOnSubscribe;
            }
        }

        protected virtual IEqualityComparer<T> EqualityComparer
        {
            get
            {
                return defaultEqualityComparer;
            }
        }

        public ReadOnlyReactiveProperty(IObservable<T> source)
        {
            this.sourceConnection = source.Subscribe(this);
        }

        public ReadOnlyReactiveProperty(IObservable<T> source, bool distinctUntilChanged)
        {
            this.distinctUntilChanged = distinctUntilChanged;
            this.sourceConnection = source.Subscribe(this);
        }

        public ReadOnlyReactiveProperty(IObservable<T> source, T initialValue)
        {
            this.latestValue = initialValue;
            this.canPublishValueOnSubscribe = true;
            this.sourceConnection = source.Subscribe(this);
        }

        public ReadOnlyReactiveProperty(IObservable<T> source, T initialValue, bool distinctUntilChanged)
        {
            this.distinctUntilChanged = distinctUntilChanged;
            this.latestValue = initialValue;
            this.canPublishValueOnSubscribe = true;
            this.sourceConnection = source.Subscribe(this);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (lastException != null)
            {
                observer.OnError(lastException);
                return Disposable.Empty;
            }

            if (isSourceCompleted)
            {
                if (canPublishValueOnSubscribe)
                {
                    observer.OnNext(latestValue);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }
                else
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                }
            }

            if (isDisposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            if (canPublishValueOnSubscribe)
            {
                observer.OnNext(latestValue);
            }

            // subscribe node, node as subscription.
            var next = new ObserverNode<T>(this, observer);
            if (root == null)
            {
                root = last = next;
            }
            else
            {
                last.Next = next;
                next.Previous = last;
                last = next;
            }

            return next;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;
            sourceConnection.Dispose();

            var node = root;
            root = last = null;
            isDisposed = true;

            while (node != null)
            {
                node.OnCompleted();
                node = node.Next;
            }
        }

        void IObserverLinkedList<T>.UnsubscribeNode(ObserverNode<T> node)
        {
            if (node == root)
            {
                root = node.Next;
            }
            if (node == last)
            {
                last = node.Previous;
            }

            if (node.Previous != null)
            {
                node.Previous.Next = node.Next;
            }
            if (node.Next != null)
            {
                node.Next.Previous = node.Previous;
            }
        }

        void IObserver<T>.OnNext(T value)
        {
            if (isDisposed) return;

            if (canPublishValueOnSubscribe)
            {
                if (distinctUntilChanged && EqualityComparer.Equals(this.latestValue, value))
                {
                    return;
                }
            }

            canPublishValueOnSubscribe = true;

            // SetValue
            this.latestValue = value;

            // call source.OnNext
            var node = root;
            while (node != null)
            {
                node.OnNext(value);
                node = node.Next;
            }
        }

        void IObserver<T>.OnError(Exception error)
        {
            lastException = error;

            // call source.OnError
            var node = root;
            while (node != null)
            {
                node.OnError(error);
                node = node.Next;
            }

            root = last = null;
        }

        void IObserver<T>.OnCompleted()
        {
            isSourceCompleted = true;
            root = last = null;
        }

        public override string ToString()
        {
            return (latestValue == null) ? "(null)" : latestValue.ToString();
        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
        }
    }

    /// <summary>
    /// Extension methods of ReactiveProperty&lt;T&gt;
    /// </summary>
    public static class ReactivePropertyExtensions
    {
        public static IReadOnlyReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source)
        {
            return new ReadOnlyReactiveProperty<T>(source);
        }

        public static IReadOnlyReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source, T initialValue)
        {
            return new ReadOnlyReactiveProperty<T>(source, initialValue);
        }

        public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(this IObservable<T> source)
        {
            return new ReadOnlyReactiveProperty<T>(source);
        }

#if CSHARP_7_OR_LATER || (UNITY_2018_3_OR_NEWER && (NET_STANDARD_2_0 || NET_4_6))

        static readonly Action<object> Callback = CancelCallback;

        static void CancelCallback(object state)
        {
            var tuple = (Tuple<ICancellableTaskCompletionSource, IDisposable>)state;
            tuple.Item2.Dispose();
            tuple.Item1.TrySetCanceled();
        }

        public static Task<T> WaitUntilValueChangedAsync<T>(this IReadOnlyReactiveProperty<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tcs = new CancellableTaskCompletionSource<T>();

            var disposable = new SingleAssignmentDisposable();
            if (source.HasValue)
            {
                // Skip first value
                var isFirstValue = true;
                disposable.Disposable = source.Subscribe(x =>
                {
                    if (isFirstValue)
                    {
                        isFirstValue = false;
                        return;
                    }
                    else
                    {
                        disposable.Dispose(); // finish subscription.
                        tcs.TrySetResult(x);
                    }
                }, ex => tcs.TrySetException(ex), () => tcs.TrySetCanceled());
            }
            else
            {
                disposable.Disposable = source.Subscribe(x =>
                {
                    disposable.Dispose(); // finish subscription.
                    tcs.TrySetResult(x);
                }, ex => tcs.TrySetException(ex), () => tcs.TrySetCanceled());
            }

            cancellationToken.Register(Callback, Tuple.Create(tcs, disposable.Disposable), false);

            return tcs.Task;
        }

        public static System.Runtime.CompilerServices.TaskAwaiter<T> GetAwaiter<T>(this IReadOnlyReactiveProperty<T> source)
        {
            return source.WaitUntilValueChangedAsync(CancellationToken.None).GetAwaiter();
        }

#endif

        /// <summary>
        /// Create ReadOnlyReactiveProperty with distinctUntilChanged: false.
        /// </summary>
        public static ReadOnlyReactiveProperty<T> ToSequentialReadOnlyReactiveProperty<T>(this IObservable<T> source)
        {
            return new ReadOnlyReactiveProperty<T>(source, distinctUntilChanged: false);
        }

        public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(this IObservable<T> source, T initialValue)
        {
            return new ReadOnlyReactiveProperty<T>(source, initialValue);
        }

        /// <summary>
        /// Create ReadOnlyReactiveProperty with distinctUntilChanged: false.
        /// </summary>
        public static ReadOnlyReactiveProperty<T> ToSequentialReadOnlyReactiveProperty<T>(this IObservable<T> source, T initialValue)
        {
            return new ReadOnlyReactiveProperty<T>(source, initialValue, distinctUntilChanged: false);
        }

        public static IObservable<T> SkipLatestValueOnSubscribe<T>(this IReadOnlyReactiveProperty<T> source)
        {
            return source.HasValue ? source.Skip(1) : source;
        }

        // for multiple toggle or etc..

        /// <summary>
        /// Lastest values of each sequence are all true.
        /// </summary>
        public static IObservable<bool> CombineLatestValuesAreAllTrue(this IEnumerable<IObservable<bool>> sources)
        {
            return sources.CombineLatest().Select(xs =>
            {
                foreach (var item in xs)
                {
                    if (item == false)
                        return false;
                }
                return true;
            });
        }


        /// <summary>
        /// Lastest values of each sequence are all false.
        /// </summary>
        public static IObservable<bool> CombineLatestValuesAreAllFalse(this IEnumerable<IObservable<bool>> sources)
        {
            return sources.CombineLatest().Select(xs =>
            {
                foreach (var item in xs)
                {
                    if (item == true)
                        return false;
                }
                return true;
            });
        }
    }
}