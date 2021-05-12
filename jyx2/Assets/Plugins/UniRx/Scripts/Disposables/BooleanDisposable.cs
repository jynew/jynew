using System;
using System.Collections;

namespace UniRx
{
    public sealed class BooleanDisposable : IDisposable, ICancelable
    {
        public bool IsDisposed { get; private set; }

        public BooleanDisposable()
        {

        }

        internal BooleanDisposable(bool isDisposed)
        {
            IsDisposed = isDisposed;
        }

        public void Dispose()
        {
            if (!IsDisposed) IsDisposed = true;
        }
    }
}