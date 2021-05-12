using System;
using System.Collections.Generic;
using System.Threading;

namespace UniRx
{
    /// <summary>
    /// Represents a group of disposable resources that are disposed together.
    /// </summary>
    public abstract class StableCompositeDisposable : ICancelable
    {
        /// <summary>
        /// Creates a new group containing two disposable resources that are disposed together.
        /// </summary>
        /// <param name="disposable1">The first disposable resoruce to add to the group.</param>
        /// <param name="disposable2">The second disposable resoruce to add to the group.</param>
        /// <returns>Group of disposable resources that are disposed together.</returns>
        public static ICancelable Create(IDisposable disposable1, IDisposable disposable2)
        {
            if (disposable1 == null) throw new ArgumentNullException("disposable1");
            if (disposable2 == null) throw new ArgumentNullException("disposable2");

            return new Binary(disposable1, disposable2);
        }

        /// <summary>
        /// Creates a new group containing three disposable resources that are disposed together.
        /// </summary>
        /// <param name="disposable1">The first disposable resoruce to add to the group.</param>
        /// <param name="disposable2">The second disposable resoruce to add to the group.</param>
        /// <param name="disposable3">The third disposable resoruce to add to the group.</param>
        /// <returns>Group of disposable resources that are disposed together.</returns>
        public static ICancelable Create(IDisposable disposable1, IDisposable disposable2, IDisposable disposable3)
        {
            if (disposable1 == null) throw new ArgumentNullException("disposable1");
            if (disposable2 == null) throw new ArgumentNullException("disposable2");
            if (disposable3 == null) throw new ArgumentNullException("disposable3");

            return new Trinary(disposable1, disposable2, disposable3);
        }

        /// <summary>
        /// Creates a new group containing four disposable resources that are disposed together.
        /// </summary>
        /// <param name="disposable1">The first disposable resoruce to add to the group.</param>
        /// <param name="disposable2">The second disposable resoruce to add to the group.</param>
        /// <param name="disposable3">The three disposable resoruce to add to the group.</param>
        /// <param name="disposable4">The four disposable resoruce to add to the group.</param>
        /// <returns>Group of disposable resources that are disposed together.</returns>
        public static ICancelable Create(IDisposable disposable1, IDisposable disposable2, IDisposable disposable3, IDisposable disposable4)
        {
            if (disposable1 == null) throw new ArgumentNullException("disposable1");
            if (disposable2 == null) throw new ArgumentNullException("disposable2");
            if (disposable3 == null) throw new ArgumentNullException("disposable3");
            if (disposable4 == null) throw new ArgumentNullException("disposable4");

            return new Quaternary(disposable1, disposable2, disposable3, disposable4);
        }

        /// <summary>
        /// Creates a new group of disposable resources that are disposed together.
        /// </summary>
        /// <param name="disposables">Disposable resources to add to the group.</param>
        /// <returns>Group of disposable resources that are disposed together.</returns>
        public static ICancelable Create(params IDisposable[] disposables)
        {
            if (disposables == null) throw new ArgumentNullException("disposables");

            return new NAry(disposables);
        }

        /// <summary>
        /// Creates a new group of disposable resources that are disposed together. Array is not copied, it's unsafe but optimized.
        /// </summary>
        /// <param name="disposables">Disposable resources to add to the group.</param>
        /// <returns>Group of disposable resources that are disposed together.</returns>
        public static ICancelable CreateUnsafe(IDisposable[] disposables)
        {
            return new NAryUnsafe(disposables);
        }

        /// <summary>
        /// Creates a new group of disposable resources that are disposed together.
        /// </summary>
        /// <param name="disposables">Disposable resources to add to the group.</param>
        /// <returns>Group of disposable resources that are disposed together.</returns>
        public static ICancelable Create(IEnumerable<IDisposable> disposables)
        {
            if (disposables == null) throw new ArgumentNullException("disposables");

            return new NAry(disposables);
        }

        /// <summary>
        /// Disposes all disposables in the group.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        public abstract bool IsDisposed
        {
            get;
        }

        class Binary : StableCompositeDisposable
        {
            int disposedCallCount = -1;
            private volatile IDisposable _disposable1;
            private volatile IDisposable _disposable2;

            public Binary(IDisposable disposable1, IDisposable disposable2)
            {
                _disposable1 = disposable1;
                _disposable2 = disposable2;
            }

            public override bool IsDisposed
            {
                get
                {
                    return disposedCallCount != -1;
                }
            }

            public override void Dispose()
            {
                if (Interlocked.Increment(ref disposedCallCount) == 0)
                {
                    _disposable1.Dispose();
                    _disposable2.Dispose();
                }
            }
        }

        class Trinary : StableCompositeDisposable
        {
            int disposedCallCount = -1;
            private volatile IDisposable _disposable1;
            private volatile IDisposable _disposable2;
            private volatile IDisposable _disposable3;

            public Trinary(IDisposable disposable1, IDisposable disposable2, IDisposable disposable3)
            {
                _disposable1 = disposable1;
                _disposable2 = disposable2;
                _disposable3 = disposable3;
            }

            public override bool IsDisposed
            {
                get
                {
                    return disposedCallCount != -1;
                }
            }

            public override void Dispose()
            {
                if (Interlocked.Increment(ref disposedCallCount) == 0)
                {
                    _disposable1.Dispose();
                    _disposable2.Dispose();
                    _disposable3.Dispose();
                }
            }
        }

        class Quaternary : StableCompositeDisposable
        {
            int disposedCallCount = -1;
            private volatile IDisposable _disposable1;
            private volatile IDisposable _disposable2;
            private volatile IDisposable _disposable3;
            private volatile IDisposable _disposable4;

            public Quaternary(IDisposable disposable1, IDisposable disposable2, IDisposable disposable3, IDisposable disposable4)
            {
                _disposable1 = disposable1;
                _disposable2 = disposable2;
                _disposable3 = disposable3;
                _disposable4 = disposable4;
            }

            public override bool IsDisposed
            {
                get
                {
                    return disposedCallCount != -1;
                }
            }

            public override void Dispose()
            {
                if (Interlocked.Increment(ref disposedCallCount) == 0)
                {
                    _disposable1.Dispose();
                    _disposable2.Dispose();
                    _disposable3.Dispose();
                    _disposable4.Dispose();
                }
            }
        }

        class NAry : StableCompositeDisposable
        {
            int disposedCallCount = -1;
            private volatile List<IDisposable> _disposables;

            public NAry(IDisposable[] disposables)
                : this((IEnumerable<IDisposable>)disposables)
            {
            }

            public NAry(IEnumerable<IDisposable> disposables)
            {
                _disposables = new List<IDisposable>(disposables);

                //
                // Doing this on the list to avoid duplicate enumeration of disposables.
                //
                if (_disposables.Contains(null)) throw new ArgumentException("Disposables can't contains null", "disposables");
            }

            public override bool IsDisposed
            {
                get
                {
                    return disposedCallCount != -1;
                }
            }

            public override void Dispose()
            {
                if (Interlocked.Increment(ref disposedCallCount) == 0)
                {
                    foreach (var d in _disposables)
                    {
                        d.Dispose();
                    }
                }
            }
        }

        class NAryUnsafe : StableCompositeDisposable
        {
            int disposedCallCount = -1;
            private volatile IDisposable[] _disposables;

            public NAryUnsafe(IDisposable[] disposables)
            {
                _disposables = disposables;
            }

            public override bool IsDisposed
            {
                get
                {
                    return disposedCallCount != -1;
                }
            }

            public override void Dispose()
            {
                if (Interlocked.Increment(ref disposedCallCount) == 0)
                {
                    var len = _disposables.Length;
                    for (int i = 0; i < len; i++)
                    {
                        _disposables[i].Dispose();
                    }
                }
            }
        }
    }
}