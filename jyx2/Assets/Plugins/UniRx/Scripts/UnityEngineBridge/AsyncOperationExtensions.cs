using System;
using System.Collections;
using System.Threading;
using UnityEngine;

#if !UniRxLibrary
using ObservableUnity = UniRx.Observable;
#endif

namespace UniRx
{
    public static partial class AsyncOperationExtensions
    {
        /// <summary>
        /// If you needs return value, use AsAsyncOperationObservable instead.
        /// </summary>
        public static IObservable<AsyncOperation> AsObservable(this AsyncOperation asyncOperation, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<AsyncOperation>((observer, cancellation) => AsObservableCore(asyncOperation, observer, progress, cancellation));
        }

        // T: where T : AsyncOperation is ambigious with IObservable<T>.AsObservable
        public static IObservable<T> AsAsyncOperationObservable<T>(this T asyncOperation, IProgress<float> progress = null)
            where T : AsyncOperation
        {
            return ObservableUnity.FromCoroutine<T>((observer, cancellation) => AsObservableCore(asyncOperation, observer, progress, cancellation));
        }

        static IEnumerator AsObservableCore<T>(T asyncOperation, IObserver<T> observer, IProgress<float> reportProgress, CancellationToken cancel)
            where T : AsyncOperation
        {
            if (reportProgress != null)
            {
                while (!asyncOperation.isDone && !cancel.IsCancellationRequested)
                {
                    try
                    {
                        reportProgress.Report(asyncOperation.progress);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        yield break;
                    }
                    yield return null;
                }
            }
            else
            {
                if (!asyncOperation.isDone)
                {
                    yield return asyncOperation;
                }
            }

            if (cancel.IsCancellationRequested) yield break;

            if (reportProgress != null)
            {
                try
                {
                    reportProgress.Report(asyncOperation.progress);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    yield break;
                }
            }

            observer.OnNext(asyncOperation);
            observer.OnCompleted();
        }
    }
}