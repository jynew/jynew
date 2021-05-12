#if (NET_4_6 || NET_STANDARD_2_0)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UniRx
{
    public static partial class Observable
    {
        /// <summary>
        /// Gets an awaiter that returns the last value of the observable sequence or throws an exception if the sequence is empty.
        /// This operation subscribes to the observable sequence, making it hot.
        /// </summary>
        /// <param name="source">Source sequence to await.</param>
        public static AsyncSubject<TSource> GetAwaiter<TSource>(this IObservable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            
            return RunAsync(source, CancellationToken.None);
        }

        /// <summary>
        /// Gets an awaiter that returns the last value of the observable sequence or throws an exception if the sequence is empty.
        /// This operation subscribes to the observable sequence, making it hot.
        /// </summary>
        /// <param name="source">Source sequence to await.</param>
        public static AsyncSubject<TSource> GetAwaiter<TSource>(this IConnectableObservable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return RunAsync(source, CancellationToken.None);
        }

        /// <summary>
        /// Gets an awaiter that returns the last value of the observable sequence or throws an exception if the sequence is empty.
        /// This operation subscribes to the observable sequence, making it hot.
        /// </summary>
        /// <param name="source">Source sequence to await.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static AsyncSubject<TSource> GetAwaiter<TSource>(this IObservable<TSource> source, CancellationToken cancellationToken)
        {
            if (source == null) throw new ArgumentNullException("source");

            return RunAsync(source, cancellationToken);
        }

        /// <summary>
        /// Gets an awaiter that returns the last value of the observable sequence or throws an exception if the sequence is empty.
        /// This operation subscribes to the observable sequence, making it hot.
        /// </summary>
        /// <param name="source">Source sequence to await.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static AsyncSubject<TSource> GetAwaiter<TSource>(this IConnectableObservable<TSource> source, CancellationToken cancellationToken)
        {
            if (source == null) throw new ArgumentNullException("source");

            return RunAsync(source, cancellationToken);
        }

        static AsyncSubject<TSource> RunAsync<TSource>(IObservable<TSource> source, CancellationToken cancellationToken)
        {
            var s = new AsyncSubject<TSource>();

            if (cancellationToken.IsCancellationRequested)
            {
                return Cancel(s, cancellationToken);
            }

            var d = source.Subscribe(s);

            if (cancellationToken.CanBeCanceled)
            {
                RegisterCancelation(s, d, cancellationToken);
            }

            return s;
        }

        static AsyncSubject<TSource> RunAsync<TSource>(IConnectableObservable<TSource> source, CancellationToken cancellationToken)
        {
            var s = new AsyncSubject<TSource>();

            if (cancellationToken.IsCancellationRequested)
            {
                return Cancel(s, cancellationToken);
            }

            var d = source.Subscribe(s);
            var c = source.Connect();

            if (cancellationToken.CanBeCanceled)
            {
                RegisterCancelation(s, StableCompositeDisposable.Create(d, c), cancellationToken);
            }

            return s;
        }

        static AsyncSubject<T> Cancel<T>(AsyncSubject<T> subject, CancellationToken cancellationToken)
        {
            subject.OnError(new OperationCanceledException(cancellationToken));
            return subject;
        }

        static void RegisterCancelation<T>(AsyncSubject<T> subject, IDisposable subscription, CancellationToken token)
        {
            //
            // Separate method used to avoid heap allocation of closure when no cancellation is needed,
            // e.g. when CancellationToken.None is provided to the RunAsync overloads.
            //

            var ctr = token.Register(() =>
            {
                subscription.Dispose();
                Cancel(subject, token);
            });

            //
            // No null-check for ctr is needed:
            //
            // - CancellationTokenRegistration is a struct
            // - Registration will succeed 99% of the time, no warranting an attempt to avoid spurious Subscribe calls
            //
            subject.Subscribe(Stubs<T>.Ignore, _ => ctr.Dispose(), ctr.Dispose);
        }
    }
}

#endif