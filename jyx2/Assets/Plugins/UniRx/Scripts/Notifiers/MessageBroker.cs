using System;
using System.Collections.Generic;
using UniRx.InternalUtil;

namespace UniRx
{
    public interface IMessagePublisher
    {
        /// <summary>
        /// Send Message to all receiver.
        /// </summary>
        void Publish<T>(T message);
    }

    public interface IMessageReceiver
    {
        /// <summary>
        /// Subscribe typed message.
        /// </summary>
        IObservable<T> Receive<T>();
    }

    public interface IMessageBroker : IMessagePublisher, IMessageReceiver
    {
    }

    public interface IAsyncMessagePublisher
    {
        /// <summary>
        /// Send Message to all receiver and await complete.
        /// </summary>
        IObservable<Unit> PublishAsync<T>(T message);
    }

    public interface IAsyncMessageReceiver
    {
        /// <summary>
        /// Subscribe typed message.
        /// </summary>
        IDisposable Subscribe<T>(Func<T, IObservable<Unit>> asyncMessageReceiver);
    }

    public interface IAsyncMessageBroker : IAsyncMessagePublisher, IAsyncMessageReceiver
    {
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public class MessageBroker : IMessageBroker, IDisposable
    {
        /// <summary>
        /// MessageBroker in Global scope.
        /// </summary>
        public static readonly IMessageBroker Default = new MessageBroker();

        bool isDisposed = false;
        readonly Dictionary<Type, object> notifiers = new Dictionary<Type, object>();

        public void Publish<T>(T message)
        {
            object notifier;
            lock (notifiers)
            {
                if (isDisposed) return;

                if (!notifiers.TryGetValue(typeof(T), out notifier))
                {
                    return;
                }
            }
            ((ISubject<T>)notifier).OnNext(message);
        }

        public IObservable<T> Receive<T>()
        {
            object notifier;
            lock (notifiers)
            {
                if (isDisposed) throw new ObjectDisposedException("MessageBroker");

                if (!notifiers.TryGetValue(typeof(T), out notifier))
                {
                    ISubject<T> n = new Subject<T>().Synchronize();
                    notifier = n;
                    notifiers.Add(typeof(T), notifier);
                }
            }

            return ((IObservable<T>)notifier).AsObservable();
        }

        public void Dispose()
        {
            lock (notifiers)
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    notifiers.Clear();
                }
            }
        }
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public class AsyncMessageBroker : IAsyncMessageBroker, IDisposable
    {
        /// <summary>
        /// AsyncMessageBroker in Global scope.
        /// </summary>
        public static readonly IAsyncMessageBroker Default = new AsyncMessageBroker();

        bool isDisposed = false;
        readonly Dictionary<Type, object> notifiers = new Dictionary<Type, object>();

        public IObservable<Unit> PublishAsync<T>(T message)
        {
            UniRx.InternalUtil.ImmutableList<Func<T, IObservable<Unit>>> notifier;
            lock (notifiers)
            {
                if (isDisposed) throw new ObjectDisposedException("AsyncMessageBroker");

                object _notifier;
                if (notifiers.TryGetValue(typeof(T), out _notifier))
                {
                    notifier = (UniRx.InternalUtil.ImmutableList<Func<T, IObservable<Unit>>>)_notifier;
                }
                else
                {
                    return Observable.ReturnUnit();
                }
            }

            var data = notifier.Data;
            var awaiter = new IObservable<Unit>[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                awaiter[i] = data[i].Invoke(message);
            }
            return Observable.WhenAll(awaiter);
        }

        public IDisposable Subscribe<T>(Func<T, IObservable<Unit>> asyncMessageReceiver)
        {
            lock (notifiers)
            {
                if (isDisposed) throw new ObjectDisposedException("AsyncMessageBroker");

                object _notifier;
                if (!notifiers.TryGetValue(typeof(T), out _notifier))
                {
                    var notifier = UniRx.InternalUtil.ImmutableList<Func<T, IObservable<Unit>>>.Empty;
                    notifier = notifier.Add(asyncMessageReceiver);
                    notifiers.Add(typeof(T), notifier);
                }
                else
                {
                    var notifier = (ImmutableList<Func<T, IObservable<Unit>>>)_notifier;
                    notifier = notifier.Add(asyncMessageReceiver);
                    notifiers[typeof(T)] = notifier;
                }
            }

            return new Subscription<T>(this, asyncMessageReceiver);
        }

        public void Dispose()
        {
            lock (notifiers)
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    notifiers.Clear();
                }
            }
        }

        class Subscription<T> : IDisposable
        {
            readonly AsyncMessageBroker parent;
            readonly Func<T, IObservable<Unit>> asyncMessageReceiver;

            public Subscription(AsyncMessageBroker parent, Func<T, IObservable<Unit>> asyncMessageReceiver)
            {
                this.parent = parent;
                this.asyncMessageReceiver = asyncMessageReceiver;
            }

            public void Dispose()
            {
                lock (parent.notifiers)
                {
                    object _notifier;
                    if (parent.notifiers.TryGetValue(typeof(T), out _notifier))
                    {
                        var notifier = (ImmutableList<Func<T, IObservable<Unit>>>)_notifier;
                        notifier = notifier.Remove(asyncMessageReceiver);

                        parent.notifiers[typeof(T)] = notifier;
                    }
                }
            }
        }
    }
}