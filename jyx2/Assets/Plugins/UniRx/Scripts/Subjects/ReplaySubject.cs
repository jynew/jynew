using System;
using System.Collections.Generic;
using UniRx.InternalUtil;

namespace UniRx
{
    public sealed class ReplaySubject<T> : ISubject<T>, IOptimizedObservable<T>, IDisposable
    {
        object observerLock = new object();

        bool isStopped;
        bool isDisposed;
        Exception lastError;
        IObserver<T> outObserver = EmptyObserver<T>.Instance;

        readonly int bufferSize;
        readonly TimeSpan window;
        readonly DateTimeOffset startTime;
        readonly IScheduler scheduler;
        Queue<TimeInterval<T>> queue = new Queue<TimeInterval<T>>();


        public ReplaySubject()
            : this(int.MaxValue, TimeSpan.MaxValue, Scheduler.DefaultSchedulers.Iteration)
        {
        }

        public ReplaySubject(IScheduler scheduler)
            : this(int.MaxValue, TimeSpan.MaxValue, scheduler)
        {
        }

        public ReplaySubject(int bufferSize)
            : this(bufferSize, TimeSpan.MaxValue, Scheduler.DefaultSchedulers.Iteration)
        {
        }

        public ReplaySubject(int bufferSize, IScheduler scheduler)
            : this(bufferSize, TimeSpan.MaxValue, scheduler)
        {
        }

        public ReplaySubject(TimeSpan window)
            : this(int.MaxValue, window, Scheduler.DefaultSchedulers.Iteration)
        {
        }

        public ReplaySubject(TimeSpan window, IScheduler scheduler)
            : this(int.MaxValue, window, scheduler)
        {
        }

        // full constructor
        public ReplaySubject(int bufferSize, TimeSpan window, IScheduler scheduler)
        {
            if (bufferSize < 0) throw new ArgumentOutOfRangeException("bufferSize");
            if (window < TimeSpan.Zero) throw new ArgumentOutOfRangeException("window");
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            this.bufferSize = bufferSize;
            this.window = window;
            this.scheduler = scheduler;
            startTime = scheduler.Now;
        }

        void Trim()
        {
            var elapsedTime = Scheduler.Normalize(scheduler.Now - startTime);

            while (queue.Count > bufferSize)
            {
                queue.Dequeue();
            }
            while (queue.Count > 0 && elapsedTime.Subtract(queue.Peek().Interval).CompareTo(window) > 0)
            {
                queue.Dequeue();
            }
        }

        public void OnCompleted()
        {
            IObserver<T> old;
            lock (observerLock)
            {
                ThrowIfDisposed();
                if (isStopped) return;

                old = outObserver;
                outObserver = EmptyObserver<T>.Instance;
                isStopped = true;
                Trim();
            }

            old.OnCompleted();
        }

        public void OnError(Exception error)
        {
            if (error == null) throw new ArgumentNullException("error");

            IObserver<T> old;
            lock (observerLock)
            {
                ThrowIfDisposed();
                if (isStopped) return;

                old = outObserver;
                outObserver = EmptyObserver<T>.Instance;
                isStopped = true;
                lastError = error;
                Trim();
            }

            old.OnError(error);
        }

        public void OnNext(T value)
        {
            IObserver<T> current;
            lock (observerLock)
            {
                ThrowIfDisposed();
                if (isStopped) return;

                // enQ
                queue.Enqueue(new TimeInterval<T>(value, scheduler.Now - startTime));
                Trim();

                current = outObserver;
            }

            current.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");

            var ex = default(Exception);
            var subscription = default(Subscription);

            lock (observerLock)
            {
                ThrowIfDisposed();
                if (!isStopped)
                {
                    var listObserver = outObserver as ListObserver<T>;
                    if (listObserver != null)
                    {
                        outObserver = listObserver.Add(observer);
                    }
                    else
                    {
                        var current = outObserver;
                        if (current is EmptyObserver<T>)
                        {
                            outObserver = observer;
                        }
                        else
                        {
                            outObserver = new ListObserver<T>(new ImmutableList<IObserver<T>>(new[] { current, observer }));
                        }
                    }

                    subscription = new Subscription(this, observer);
                }

                ex = lastError;
                Trim();
                foreach (var item in queue)
                {
                    observer.OnNext(item.Value);
                }
            }

            if (subscription != null)
            {
                return subscription;
            }
            else if (ex != null)
            {
                observer.OnError(ex);
            }
            else
            {
                observer.OnCompleted();
            }

            return Disposable.Empty;
        }

        public void Dispose()
        {
            lock (observerLock)
            {
                isDisposed = true;
                outObserver = DisposedObserver<T>.Instance;
                lastError = null;
                queue = null;
            }
        }

        void ThrowIfDisposed()
        {
            if (isDisposed) throw new ObjectDisposedException("");
        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
        }

        class Subscription : IDisposable
        {
            readonly object gate = new object();
            ReplaySubject<T> parent;
            IObserver<T> unsubscribeTarget;

            public Subscription(ReplaySubject<T> parent, IObserver<T> unsubscribeTarget)
            {
                this.parent = parent;
                this.unsubscribeTarget = unsubscribeTarget;
            }

            public void Dispose()
            {
                lock (gate)
                {
                    if (parent != null)
                    {
                        lock (parent.observerLock)
                        {
                            var listObserver = parent.outObserver as ListObserver<T>;
                            if (listObserver != null)
                            {
                                parent.outObserver = listObserver.Remove(unsubscribeTarget);
                            }
                            else
                            {
                                parent.outObserver = EmptyObserver<T>.Instance;
                            }

                            unsubscribeTarget = null;
                            parent = null;
                        }
                    }
                }
            }
        }
    }
}