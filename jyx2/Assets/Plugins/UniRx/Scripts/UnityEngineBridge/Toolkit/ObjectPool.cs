#if UNITY_5_3_OR_NEWER

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace UniRx.Toolkit
{
    /// <summary>
    /// Bass class of ObjectPool.
    /// </summary>
    public abstract class ObjectPool<T> : IDisposable
        where T : UnityEngine.Component
    {
        bool isDisposed = false;
        Queue<T> q;

        /// <summary>
        /// Limit of instace count.
        /// </summary>
        protected int MaxPoolCount
        {
            get
            {
                return int.MaxValue;
            }
        }

        /// <summary>
        /// Create instance when needed.
        /// </summary>
        protected abstract T CreateInstance();

        /// <summary>
        /// Called before return to pool, useful for set active object(it is default behavior).
        /// </summary>
        protected virtual void OnBeforeRent(T instance)
        {
            instance.gameObject.SetActive(true);
        }

        /// <summary>
        /// Called before return to pool, useful for set inactive object(it is default behavior).
        /// </summary>
        protected virtual void OnBeforeReturn(T instance)
        {
            instance.gameObject.SetActive(false);
        }

        /// <summary>
        /// Called when clear or disposed, useful for destroy instance or other finalize method.
        /// </summary>
        protected virtual void OnClear(T instance)
        {
            if (instance == null) return;

            var go = instance.gameObject;
            if (go == null) return;
            UnityEngine.Object.Destroy(go);
        }

        /// <summary>
        /// Current pooled object count.
        /// </summary>
        public int Count
        {
            get
            {
                if (q == null) return 0;
                return q.Count;
            }
        }

        /// <summary>
        /// Get instance from pool.
        /// </summary>
        public T Rent()
        {
            if (isDisposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
            if (q == null) q = new Queue<T>();

            var instance = (q.Count > 0)
                ? q.Dequeue()
                : CreateInstance();

            OnBeforeRent(instance);
            return instance;
        }

        /// <summary>
        /// Return instance to pool.
        /// </summary>
        public void Return(T instance)
        {
            if (isDisposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
            if (instance == null) throw new ArgumentNullException("instance");

            if (q == null) q = new Queue<T>();

            if ((q.Count + 1) == MaxPoolCount)
            {
                throw new InvalidOperationException("Reached Max PoolSize");
            }

            OnBeforeReturn(instance);
            q.Enqueue(instance);
        }

        /// <summary>
        /// Clear pool.
        /// </summary>
        public void Clear(bool callOnBeforeRent = false)
        {
            if (q == null) return;
            while (q.Count != 0)
            {
                var instance = q.Dequeue();
                if (callOnBeforeRent)
                {
                    OnBeforeRent(instance);
                }
                OnClear(instance);
            }
        }

        /// <summary>
        /// Trim pool instances. 
        /// </summary>
        /// <param name="instanceCountRatio">0.0f = clear all ~ 1.0f = live all.</param>
        /// <param name="minSize">Min pool count.</param>
        /// <param name="callOnBeforeRent">If true, call OnBeforeRent before OnClear.</param>
        public void Shrink(float instanceCountRatio, int minSize, bool callOnBeforeRent = false)
        {
            if (q == null) return;

            if (instanceCountRatio <= 0) instanceCountRatio = 0;
            if (instanceCountRatio >= 1.0f) instanceCountRatio = 1.0f;

            var size = (int)(q.Count * instanceCountRatio);
            size = Math.Max(minSize, size);

            while (q.Count > size)
            {
                var instance = q.Dequeue();
                if (callOnBeforeRent)
                {
                    OnBeforeRent(instance);
                }
                OnClear(instance);
            }
        }

        /// <summary>
        /// If needs shrink pool frequently, start check timer.
        /// </summary>
        /// <param name="checkInterval">Interval of call Shrink.</param>
        /// <param name="instanceCountRatio">0.0f = clearAll ~ 1.0f = live all.</param>
        /// <param name="minSize">Min pool count.</param>
        /// <param name="callOnBeforeRent">If true, call OnBeforeRent before OnClear.</param>
        public IDisposable StartShrinkTimer(TimeSpan checkInterval, float instanceCountRatio, int minSize, bool callOnBeforeRent = false)
        {
            return Observable.Interval(checkInterval)
                .TakeWhile(_ => !isDisposed)
                .Subscribe(_ =>
                {
                    Shrink(instanceCountRatio, minSize, callOnBeforeRent);
                });
        }

        /// <summary>
        /// Fill pool before rent operation.
        /// </summary>
        /// <param name="preloadCount">Pool instance count.</param>
        /// <param name="threshold">Create count per frame.</param>
        public IObservable<Unit> PreloadAsync(int preloadCount, int threshold)
        {
            if (q == null) q = new Queue<T>(preloadCount);

            return Observable.FromMicroCoroutine<Unit>((observer, cancel) => PreloadCore(preloadCount, threshold, observer, cancel));
        }

        IEnumerator PreloadCore(int preloadCount, int threshold, IObserver<Unit> observer, CancellationToken cancellationToken)
        {
            while (Count < preloadCount && !cancellationToken.IsCancellationRequested)
            {
                var requireCount = preloadCount - Count;
                if (requireCount <= 0) break;

                var createCount = Math.Min(requireCount, threshold);

                for (int i = 0; i < createCount; i++)
                {
                    try
                    {
                        var instance = CreateInstance();
                        Return(instance);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        yield break;
                    }
                }
                yield return null; // next frame.
            }

            observer.OnNext(Unit.Default);
            observer.OnCompleted();
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    Clear(false);
                }

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }

    /// <summary>
    /// Bass class of ObjectPool. If needs asynchronous initialization, use this instead of standard ObjectPool.
    /// </summary>
    public abstract class AsyncObjectPool<T> : IDisposable
        where T : UnityEngine.Component
    {
        bool isDisposed = false;
        Queue<T> q;

        /// <summary>
        /// Limit of instace count.
        /// </summary>
        protected int MaxPoolCount
        {
            get
            {
                return int.MaxValue;
            }
        }

        /// <summary>
        /// Create instance when needed.
        /// </summary>
        protected abstract IObservable<T> CreateInstanceAsync();

        /// <summary>
        /// Called before return to pool, useful for set active object(it is default behavior).
        /// </summary>
        protected virtual void OnBeforeRent(T instance)
        {
            instance.gameObject.SetActive(true);
        }

        /// <summary>
        /// Called before return to pool, useful for set inactive object(it is default behavior).
        /// </summary>
        protected virtual void OnBeforeReturn(T instance)
        {
            instance.gameObject.SetActive(false);
        }

        /// <summary>
        /// Called when clear or disposed, useful for destroy instance or other finalize method.
        /// </summary>
        protected virtual void OnClear(T instance)
        {
            if (instance == null) return;

            var go = instance.gameObject;
            if (go == null) return;
            UnityEngine.Object.Destroy(go);
        }

        /// <summary>
        /// Current pooled object count.
        /// </summary>
        public int Count
        {
            get
            {
                if (q == null) return 0;
                return q.Count;
            }
        }

        /// <summary>
        /// Get instance from pool.
        /// </summary>
        public IObservable<T> RentAsync()
        {
            if (isDisposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
            if (q == null) q = new Queue<T>();

            if (q.Count > 0)
            {
                var instance = q.Dequeue();
                OnBeforeRent(instance);
                return Observable.Return(instance);
            }
            else
            {
                var instance = CreateInstanceAsync();
                return instance.Do(x => OnBeforeRent(x));
            }
        }

        /// <summary>
        /// Return instance to pool.
        /// </summary>
        public void Return(T instance)
        {
            if (isDisposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
            if (instance == null) throw new ArgumentNullException("instance");

            if (q == null) q = new Queue<T>();

            if ((q.Count + 1) == MaxPoolCount)
            {
                throw new InvalidOperationException("Reached Max PoolSize");
            }

            OnBeforeReturn(instance);
            q.Enqueue(instance);
        }

        /// <summary>
        /// Trim pool instances. 
        /// </summary>
        /// <param name="instanceCountRatio">0.0f = clear all ~ 1.0f = live all.</param>
        /// <param name="minSize">Min pool count.</param>
        /// <param name="callOnBeforeRent">If true, call OnBeforeRent before OnClear.</param>
        public void Shrink(float instanceCountRatio, int minSize, bool callOnBeforeRent = false)
        {
            if (q == null) return;

            if (instanceCountRatio <= 0) instanceCountRatio = 0;
            if (instanceCountRatio >= 1.0f) instanceCountRatio = 1.0f;

            var size = (int)(q.Count * instanceCountRatio);
            size = Math.Max(minSize, size);

            while (q.Count > size)
            {
                var instance = q.Dequeue();
                if (callOnBeforeRent)
                {
                    OnBeforeRent(instance);
                }
                OnClear(instance);
            }
        }

        /// <summary>
        /// If needs shrink pool frequently, start check timer.
        /// </summary>
        /// <param name="checkInterval">Interval of call Shrink.</param>
        /// <param name="instanceCountRatio">0.0f = clearAll ~ 1.0f = live all.</param>
        /// <param name="minSize">Min pool count.</param>
        /// <param name="callOnBeforeRent">If true, call OnBeforeRent before OnClear.</param>
        public IDisposable StartShrinkTimer(TimeSpan checkInterval, float instanceCountRatio, int minSize, bool callOnBeforeRent = false)
        {
            return Observable.Interval(checkInterval)
                .TakeWhile(_ => !isDisposed)
                .Subscribe(_ =>
                {
                    Shrink(instanceCountRatio, minSize, callOnBeforeRent);
                });
        }

        /// <summary>
        /// Clear pool.
        /// </summary>
        public void Clear(bool callOnBeforeRent = false)
        {
            if (q == null) return;
            while (q.Count != 0)
            {
                var instance = q.Dequeue();
                if (callOnBeforeRent)
                {
                    OnBeforeRent(instance);
                }
                OnClear(instance);
            }
        }

        /// <summary>
        /// Fill pool before rent operation.
        /// </summary>
        /// <param name="preloadCount">Pool instance count.</param>
        /// <param name="threshold">Create count per frame.</param>
        public IObservable<Unit> PreloadAsync(int preloadCount, int threshold)
        {
            if (q == null) q = new Queue<T>(preloadCount);

            return Observable.FromMicroCoroutine<Unit>((observer, cancel) => PreloadCore(preloadCount, threshold, observer, cancel));
        }

        IEnumerator PreloadCore(int preloadCount, int threshold, IObserver<Unit> observer, CancellationToken cancellationToken)
        {
            while (Count < preloadCount && !cancellationToken.IsCancellationRequested)
            {
                var requireCount = preloadCount - Count;
                if (requireCount <= 0) break;

                var createCount = Math.Min(requireCount, threshold);

                var loaders = new IObservable<Unit>[createCount];
                for (int i = 0; i < createCount; i++)
                {
                    var instanceFuture = CreateInstanceAsync();
                    loaders[i] = instanceFuture.ForEachAsync(x => Return(x));
                }

                var awaiter = Observable.WhenAll(loaders).ToYieldInstruction(false, cancellationToken);
                while (!(awaiter.HasResult || awaiter.IsCanceled || awaiter.HasError))
                {
                    yield return null;
                }

                if (awaiter.HasError)
                {
                    observer.OnError(awaiter.Error);
                    yield break;
                }
                else if (awaiter.IsCanceled)
                {
                    yield break; // end.
                }
            }

            observer.OnNext(Unit.Default);
            observer.OnCompleted();
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    Clear(false);
                }

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}

#endif