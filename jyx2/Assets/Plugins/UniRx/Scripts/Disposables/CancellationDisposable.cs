// original code from GitHub Reactive-Extensions/Rx.NET
// some modified.

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

#if (NETFX_CORE || NET_4_6 || NET_STANDARD_2_0 || UNITY_WSA_10_0)

using System;
using System.Threading;

namespace UniRx
{
    /// <summary>
    /// Represents a disposable resource that has an associated <seealso cref="T:System.Threading.CancellationToken"/> that will be set to the cancellation requested state upon disposal.
    /// </summary>
    public sealed class CancellationDisposable : ICancelable
    {
        private readonly CancellationTokenSource _cts;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Reactive.Disposables.CancellationDisposable"/> class that uses an existing <seealso cref="T:System.Threading.CancellationTokenSource"/>.
        /// </summary>
        /// <param name="cts"><seealso cref="T:System.Threading.CancellationTokenSource"/> used for cancellation.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cts"/> is null.</exception>
        public CancellationDisposable(CancellationTokenSource cts)
        {
            if (cts == null)
                throw new ArgumentNullException("cts");

            _cts = cts;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Reactive.Disposables.CancellationDisposable"/> class that uses a new <seealso cref="T:System.Threading.CancellationTokenSource"/>.
        /// </summary>
        public CancellationDisposable()
            : this(new CancellationTokenSource())
        {
        }

        /// <summary>
        /// Gets the <see cref="T:System.Threading.CancellationToken"/> used by this CancellationDisposable.
        /// </summary>
        public CancellationToken Token
        {
            get { return _cts.Token; }
        }

        /// <summary>
        /// Cancels the underlying <seealso cref="T:System.Threading.CancellationTokenSource"/>.
        /// </summary>
        public void Dispose()
        {
            _cts.Cancel();
        }

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return _cts.IsCancellationRequested; }
        }
    }
}

#endif