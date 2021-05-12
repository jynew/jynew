using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class DelayFrameObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly int frameCount;
        readonly FrameCountType frameCountType;

        public DelayFrameObservable(IObservable<T> source, int frameCount, FrameCountType frameCountType)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.frameCount = frameCount;
            this.frameCountType = frameCountType;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new DelayFrame(this, observer, cancel).Run();
        }

        class DelayFrame : OperatorObserverBase<T, T>
        {
            readonly DelayFrameObservable<T> parent;
            readonly object gate = new object();
            readonly QueuePool pool = new QueuePool();
            int runningEnumeratorCount;
            bool readyDrainEnumerator;
            bool running;
            IDisposable sourceSubscription;
            Queue<T> currentQueueReference;
            bool calledCompleted;
            bool hasError;
            Exception error;
            BooleanDisposable cancelationToken;

            public DelayFrame(DelayFrameObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                cancelationToken = new BooleanDisposable();

                var _sourceSubscription = new SingleAssignmentDisposable();
                sourceSubscription = _sourceSubscription;
                _sourceSubscription.Disposable = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(cancelationToken, sourceSubscription);
            }

            IEnumerator DrainQueue(Queue<T> q, int frameCount)
            {
                lock (gate)
                {
                    readyDrainEnumerator = false; // use next queue.
                    running = false;
                }

                while (!cancelationToken.IsDisposed && frameCount-- != 0)
                {
                    yield return null;
                }

                try
                {
                    if (q != null)
                    {
                        while (q.Count > 0 && !hasError)
                        {
                            if (cancelationToken.IsDisposed) break;

                            lock (gate)
                            {
                                running = true;
                            }

                            var value = q.Dequeue();
                            observer.OnNext(value);

                            lock (gate)
                            {
                                running = false;
                            }
                        }

                        if (q.Count == 0)
                        {
                            pool.Return(q);
                        }
                    }

                    if (hasError)
                    {
                        if (!cancelationToken.IsDisposed)
                        {
                            cancelationToken.Dispose();

                            try { observer.OnError(error); } finally { Dispose(); }
                        }
                    }
                    else if (calledCompleted)
                    {
                        lock (gate)
                        {
                            // not self only
                            if (runningEnumeratorCount != 1) yield break;
                        }

                        if (!cancelationToken.IsDisposed)
                        {
                            cancelationToken.Dispose();

                            try { observer.OnCompleted(); }
                            finally { Dispose(); }
                        }
                    }
                }
                finally
                {
                    lock (gate)
                    {
                        runningEnumeratorCount--;
                    }
                }
            }

            public override void OnNext(T value)
            {
                if (cancelationToken.IsDisposed) return;

                Queue<T> targetQueue = null;
                lock (gate)
                {
                    if (!readyDrainEnumerator)
                    {
                        readyDrainEnumerator = true;
                        runningEnumeratorCount++;
                        targetQueue = currentQueueReference = pool.Get();
                        targetQueue.Enqueue(value);
                    }
                    else
                    {
                        if (currentQueueReference != null) // null - if doesn't start OnNext and start OnCompleted
                        {
                            currentQueueReference.Enqueue(value);
                        }
                        return;
                    }
                }

                switch (parent.frameCountType)
                {
                    case FrameCountType.Update:
                        MainThreadDispatcher.StartUpdateMicroCoroutine(DrainQueue(targetQueue, parent.frameCount));
                        break;
                    case FrameCountType.FixedUpdate:
                        MainThreadDispatcher.StartFixedUpdateMicroCoroutine(DrainQueue(targetQueue, parent.frameCount));
                        break;
                    case FrameCountType.EndOfFrame:
                        MainThreadDispatcher.StartEndOfFrameMicroCoroutine(DrainQueue(targetQueue, parent.frameCount));
                        break;
                    default:
                        throw new ArgumentException("Invalid FrameCountType:" + parent.frameCountType);
                }
            }

            public override void OnError(Exception error)
            {
                sourceSubscription.Dispose(); // stop subscription

                if (cancelationToken.IsDisposed) return;

                lock (gate)
                {
                    if (running)
                    {
                        hasError = true;
                        this.error = error;
                        return;
                    }
                }

                cancelationToken.Dispose();
                try { base.observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                sourceSubscription.Dispose(); // stop subscription

                if (cancelationToken.IsDisposed) return;

                lock (gate)
                {
                    calledCompleted = true;

                    if (!readyDrainEnumerator)
                    {
                        readyDrainEnumerator = true;
                        runningEnumeratorCount++;
                    }
                    else
                    {
                        return;
                    }
                }

                switch (parent.frameCountType)
                {
                    case FrameCountType.Update:
                        MainThreadDispatcher.StartUpdateMicroCoroutine(DrainQueue(null, parent.frameCount));
                        break;
                    case FrameCountType.FixedUpdate:
                        MainThreadDispatcher.StartFixedUpdateMicroCoroutine(DrainQueue(null, parent.frameCount));
                        break;
                    case FrameCountType.EndOfFrame:
                        MainThreadDispatcher.StartEndOfFrameMicroCoroutine(DrainQueue(null, parent.frameCount));
                        break;
                    default:
                        throw new ArgumentException("Invalid FrameCountType:" + parent.frameCountType);
                }
            }
        }

        class QueuePool
        {
            readonly object gate = new object();
            readonly Queue<Queue<T>> pool = new Queue<Queue<T>>(2);

            public Queue<T> Get()
            {
                lock (gate)
                {
                    if (pool.Count == 0)
                    {
                        return new Queue<T>(2);
                    }
                    else
                    {
                        return pool.Dequeue();
                    }
                }
            }

            public void Return(Queue<T> q)
            {
                lock (gate)
                {
                    pool.Enqueue(q);
                }
            }
        }
    }
}