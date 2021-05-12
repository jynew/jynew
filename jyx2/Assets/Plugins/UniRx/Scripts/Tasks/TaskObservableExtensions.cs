// this code is borrowed from RxOfficial(rx.codeplex.com) and modified

#if (NET_4_6 || NET_STANDARD_2_0)

using System;
using System.Threading.Tasks;
using System.Threading;

namespace UniRx
{
    /// <summary>
    /// Provides a set of static methods for converting tasks to observable sequences.
    /// </summary>
    public static class TaskObservableExtensions
    {
        /// <summary>
        /// Returns an observable sequence that signals when the task completes.
        /// </summary>
        /// <param name="task">Task to convert to an observable sequence.</param>
        /// <returns>An observable sequence that produces a unit value when the task completes, or propagates the exception produced by the task.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is null.</exception>
        /// <remarks>If the specified task object supports cancellation, consider using <see cref="Observable.FromAsync(Func{CancellationToken, Task})"/> instead.</remarks>
        public static IObservable<Unit> ToObservable(this Task task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return ToObservableImpl(task, null);
        }

        /// <summary>
        /// Returns an observable sequence that signals when the task completes.
        /// </summary>
        /// <param name="task">Task to convert to an observable sequence.</param>
        /// <param name="scheduler">Scheduler on which to notify observers about completion, cancellation or failure.</param>
        /// <returns>An observable sequence that produces a unit value when the task completes, or propagates the exception produced by the task.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is null or <paramref name="scheduler"/> is null.</exception>
        /// <remarks>If the specified task object supports cancellation, consider using <see cref="Observable.FromAsync(Func{CancellationToken, Task})"/> instead.</remarks>
        public static IObservable<Unit> ToObservable(this Task task, IScheduler scheduler)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (scheduler == null)
                throw new ArgumentNullException("scheduler");

            return ToObservableImpl(task, scheduler);
        }

        private static IObservable<Unit> ToObservableImpl(Task task, IScheduler scheduler)
        {
            var res = default(IObservable<Unit>);

            if (task.IsCompleted)
            {
                scheduler = scheduler ?? Scheduler.Immediate;

                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        res = Observable.Return<Unit>(Unit.Default, scheduler);
                        break;
                    case TaskStatus.Faulted:
                        res = Observable.Throw<Unit>(task.Exception.InnerException, scheduler);
                        break;
                    case TaskStatus.Canceled:
                        res = Observable.Throw<Unit>(new TaskCanceledException(task), scheduler);
                        break;
                }
            }
            else
            {
                //
                // Separate method to avoid closure in synchronous completion case.
                //
                res = ToObservableSlow(task, scheduler);
            }

            return res;
        }

        private static IObservable<Unit> ToObservableSlow(Task task, IScheduler scheduler)
        {
            var subject = new AsyncSubject<Unit>();

            var options = GetTaskContinuationOptions(scheduler);

            task.ContinueWith(t => ToObservableDone(task, subject), options);

            return ToObservableResult(subject, scheduler);
        }

        private static void ToObservableDone(Task task, IObserver<Unit> subject)
        {
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                    subject.OnNext(Unit.Default);
                    subject.OnCompleted();
                    break;
                case TaskStatus.Faulted:
                    subject.OnError(task.Exception.InnerException);
                    break;
                case TaskStatus.Canceled:
                    subject.OnError(new TaskCanceledException(task));
                    break;
            }
        }

        /// <summary>
        /// Returns an observable sequence that propagates the result of the task.
        /// </summary>
        /// <typeparam name="TResult">The type of the result produced by the task.</typeparam>
        /// <param name="task">Task to convert to an observable sequence.</param>
        /// <returns>An observable sequence that produces the task's result, or propagates the exception produced by the task.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is null.</exception>
        /// <remarks>If the specified task object supports cancellation, consider using <see cref="Observable.FromAsync{TResult}(Func{CancellationToken, Task{TResult}})"/> instead.</remarks>
        public static IObservable<TResult> ToObservable<TResult>(this Task<TResult> task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return ToObservableImpl(task, null);
        }

        /// <summary>
        /// Returns an observable sequence that propagates the result of the task.
        /// </summary>
        /// <typeparam name="TResult">The type of the result produced by the task.</typeparam>
        /// <param name="task">Task to convert to an observable sequence.</param>
        /// <param name="scheduler">Scheduler on which to notify observers about completion, cancellation or failure.</param>
        /// <returns>An observable sequence that produces the task's result, or propagates the exception produced by the task.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is null or <paramref name="scheduler"/> is null.</exception>
        /// <remarks>If the specified task object supports cancellation, consider using <see cref="Observable.FromAsync{TResult}(Func{CancellationToken, Task{TResult}})"/> instead.</remarks>
        public static IObservable<TResult> ToObservable<TResult>(this Task<TResult> task, IScheduler scheduler)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (scheduler == null)
                throw new ArgumentNullException("scheduler");

            return ToObservableImpl(task, scheduler);
        }

        private static IObservable<TResult> ToObservableImpl<TResult>(Task<TResult> task, IScheduler scheduler)
        {
            var res = default(IObservable<TResult>);

            if (task.IsCompleted)
            {
                scheduler = scheduler ?? Scheduler.Immediate;

                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        res = Observable.Return<TResult>(task.Result, scheduler);
                        break;
                    case TaskStatus.Faulted:
                        res = Observable.Throw<TResult>(task.Exception.InnerException, scheduler);
                        break;
                    case TaskStatus.Canceled:
                        res = Observable.Throw<TResult>(new TaskCanceledException(task), scheduler);
                        break;
                }
            }
            else
            {
                //
                // Separate method to avoid closure in synchronous completion case.
                //
                res = ToObservableSlow(task, scheduler);
            }

            return res;
        }

        private static IObservable<TResult> ToObservableSlow<TResult>(Task<TResult> task, IScheduler scheduler)
        {
            var subject = new AsyncSubject<TResult>();

            var options = GetTaskContinuationOptions(scheduler);

            task.ContinueWith(t => ToObservableDone(task, subject), options);

            return ToObservableResult(subject, scheduler);
        }

        private static void ToObservableDone<TResult>(Task<TResult> task, IObserver<TResult> subject)
        {
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                    subject.OnNext(task.Result);
                    subject.OnCompleted();
                    break;
                case TaskStatus.Faulted:
                    subject.OnError(task.Exception.InnerException);
                    break;
                case TaskStatus.Canceled:
                    subject.OnError(new TaskCanceledException(task));
                    break;
            }
        }

        private static TaskContinuationOptions GetTaskContinuationOptions(IScheduler scheduler)
        {
            var options = TaskContinuationOptions.None;

            if (scheduler != null)
            {
                //
                // We explicitly don't special-case the immediate scheduler here. If the user asks for a
                // synchronous completion, we'll try our best. However, there's no guarantee due to the
                // internal stack probing in the TPL, which may cause asynchronous completion on a thread
                // pool thread in order to avoid stack overflows. Therefore we can only attempt to be more
                // efficient in the case where the user specified a scheduler, hence we know that the
                // continuation will trigger a scheduling operation. In case of the immediate scheduler,
                // it really becomes "immediate scheduling" wherever the TPL decided to run the continuation,
                // i.e. not necessarily where the task was completed from.
                //
                options |= TaskContinuationOptions.ExecuteSynchronously;
            }

            return options;
        }

        private static IObservable<TResult> ToObservableResult<TResult>(AsyncSubject<TResult> subject, IScheduler scheduler)
        {
            if (scheduler != null)
            {
                return subject.ObserveOn(scheduler);
            }
            else
            {
                return subject.AsObservable();
            }
        }

        /// <summary>
        /// Returns a task that will receive the last value or the exception produced by the observable sequence.
        /// </summary>
        /// <typeparam name="TResult">The type of the elements in the source sequence.</typeparam>
        /// <param name="observable">Observable sequence to convert to a task.</param>
        /// <returns>A task that will receive the last element or the exception produced by the observable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="observable"/> is null.</exception>
        public static Task<TResult> ToTask<TResult>(this IObservable<TResult> observable)
        {
            if (observable == null)
                throw new ArgumentNullException("observable");

            return observable.ToTask(new CancellationToken(), null);
        }

        /// <summary>
        /// Returns a task that will receive the last value or the exception produced by the observable sequence.
        /// </summary>
        /// <typeparam name="TResult">The type of the elements in the source sequence.</typeparam>
        /// <param name="observable">Observable sequence to convert to a task.</param>
        /// <param name="state">The state to use as the underlying task's AsyncState.</param>
        /// <returns>A task that will receive the last element or the exception produced by the observable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="observable"/> is null.</exception>
        public static Task<TResult> ToTask<TResult>(this IObservable<TResult> observable, object state)
        {
            if (observable == null)
                throw new ArgumentNullException("observable");

            return observable.ToTask(new CancellationToken(), state);
        }

        /// <summary>
        /// Returns a task that will receive the last value or the exception produced by the observable sequence.
        /// </summary>
        /// <typeparam name="TResult">The type of the elements in the source sequence.</typeparam>
        /// <param name="observable">Observable sequence to convert to a task.</param>
        /// <param name="cancellationToken">Cancellation token that can be used to cancel the task, causing unsubscription from the observable sequence.</param>
        /// <returns>A task that will receive the last element or the exception produced by the observable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="observable"/> is null.</exception>
        public static Task<TResult> ToTask<TResult>(this IObservable<TResult> observable, CancellationToken cancellationToken)
        {
            if (observable == null)
                throw new ArgumentNullException("observable");

            return observable.ToTask(cancellationToken, null);
        }

        /// <summary>
        /// Returns a task that will receive the last value or the exception produced by the observable sequence.
        /// </summary>
        /// <typeparam name="TResult">The type of the elements in the source sequence.</typeparam>
        /// <param name="observable">Observable sequence to convert to a task.</param>
        /// <param name="cancellationToken">Cancellation token that can be used to cancel the task, causing unsubscription from the observable sequence.</param>
        /// <param name="state">The state to use as the underlying task's AsyncState.</param>
        /// <returns>A task that will receive the last element or the exception produced by the observable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="observable"/> is null.</exception>
        public static Task<TResult> ToTask<TResult>(this IObservable<TResult> observable, CancellationToken cancellationToken, object state)
        {
            if (observable == null)
                throw new ArgumentNullException("observable");

            var hasValue = false;
            var lastValue = default(TResult);

            var tcs = new TaskCompletionSource<TResult>(state);

            var disposable = new SingleAssignmentDisposable();

            var ctr = default(CancellationTokenRegistration);

            if (cancellationToken.CanBeCanceled)
            {
                ctr = cancellationToken.Register(() =>
                {
                    disposable.Dispose();
                    tcs.TrySetCanceled(cancellationToken);
                });
            }

            var taskCompletionObserver = Observer.Create<TResult>(
                value =>
                {
                    hasValue = true;
                    lastValue = value;
                },
                ex =>
                {
                    tcs.TrySetException(ex);

                    ctr.Dispose(); // no null-check needed (struct)
                    disposable.Dispose();
                },
                () =>
                {
                    if (hasValue)
                        tcs.TrySetResult(lastValue);
                    else
                        tcs.TrySetException(new InvalidOperationException("Strings_Linq.NO_ELEMENTS"));

                    ctr.Dispose(); // no null-check needed (struct)
                    disposable.Dispose();
                }
            );

            //
            // Subtle race condition: if the source completes before we reach the line below, the SingleAssigmentDisposable
            // will already have been disposed. Upon assignment, the disposable resource being set will be disposed on the
            // spot, which may throw an exception. (Similar to TFS 487142)
            //
            try
            {
                //
                // [OK] Use of unsafe Subscribe: we're catching the exception here to set the TaskCompletionSource.
                //
                // Notice we could use a safe subscription to route errors through OnError, but we still need the
                // exception handling logic here for the reason explained above. We cannot afford to throw here
                // and as a result never set the TaskCompletionSource, so we tunnel everything through here.
                //
                disposable.Disposable = observable.Subscribe/*Unsafe*/(taskCompletionObserver);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }
    }
}
#endif
