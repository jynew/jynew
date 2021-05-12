using System;
using System.Collections;

namespace UniRx
{
    public sealed class SerialDisposable : IDisposable, ICancelable
    {
        readonly object gate = new object();
        IDisposable current;
        bool disposed;

        public bool IsDisposed { get { lock (gate) { return disposed; } } }

        public IDisposable Disposable
        {
            get
            {
                return current;
            }
            set
            {
                var shouldDispose = false;
                var old = default(IDisposable);
                lock (gate)
                {
                    shouldDispose = disposed;
                    if (!shouldDispose)
                    {
                        old = current;
                        current = value;
                    }
                }
                if (old != null)
                {
                    old.Dispose();
                }
                if (shouldDispose && value != null)
                {
                    value.Dispose();
                }
            }
        }

        public void Dispose()
        {
            var old = default(IDisposable);

            lock (gate)
            {
                if (!disposed)
                {
                    disposed = true;
                    old = current;
                    current = null;
                }
            }

            if (old != null)
            {
                old.Dispose();
            }
        }
    }
}