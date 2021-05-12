using System;
using UniRx.InternalUtil;

namespace UniRx.Operators
{
    internal class Wait<T> : IObserver<T>
    {
        static readonly TimeSpan InfiniteTimeSpan = new TimeSpan(0, 0, 0, 0, -1); // from .NET 4.5

        readonly IObservable<T> source;
        readonly TimeSpan timeout;

        System.Threading.ManualResetEvent semaphore;

        bool seenValue = false;
        T value = default(T);
        Exception ex = default(Exception);

        public Wait(IObservable<T> source, TimeSpan timeout)
        {
            this.source = source;
            this.timeout = timeout;
        }

        public T Run()
        {
            semaphore = new System.Threading.ManualResetEvent(false);
            using (source.Subscribe(this))
            {
                var waitComplete = (timeout == InfiniteTimeSpan)
                    ? semaphore.WaitOne()
                    : semaphore.WaitOne(timeout);

                if (!waitComplete)
                {
                    throw new TimeoutException("OnCompleted not fired.");
                }
            }

            if (ex != null) ex.Throw();
            if (!seenValue) throw new InvalidOperationException("No Elements.");

            return value;
        }

        public void OnNext(T value)
        {
            seenValue = true;
            this.value = value;
        }

        public void OnError(Exception error)
        {
            this.ex = error;
            semaphore.Set();
        }

        public void OnCompleted()
        {
            semaphore.Set();
        }
    }
}