using System;
using System.Collections.Generic;
using UniRx.Operators;

namespace UniRx
{
    public static partial class Observable
    {
        public static IObservable<T> AsObservable<T>(this IObservable<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            // optimize, don't double wrap
            if (source is UniRx.Operators.AsObservableObservable<T>)
            {
                return source;
            }

            return new AsObservableObservable<T>(source);
        }

        public static IObservable<T> ToObservable<T>(this IEnumerable<T> source)
        {
            return ToObservable(source, Scheduler.DefaultSchedulers.Iteration);
        }

        public static IObservable<T> ToObservable<T>(this IEnumerable<T> source, IScheduler scheduler)
        {
            return new ToObservableObservable<T>(source, scheduler);
        }

        public static IObservable<TResult> Cast<TSource, TResult>(this IObservable<TSource> source)
        {
            return new CastObservable<TSource, TResult>(source);
        }

        /// <summary>
        /// witness is for type inference.
        /// </summary>
        public static IObservable<TResult> Cast<TSource, TResult>(this IObservable<TSource> source, TResult witness)
        {
            return new CastObservable<TSource, TResult>(source);
        }

        public static IObservable<TResult> OfType<TSource, TResult>(this IObservable<TSource> source)
        {
            return new OfTypeObservable<TSource, TResult>(source);
        }

        /// <summary>
        /// witness is for type inference.
        /// </summary>
        public static IObservable<TResult> OfType<TSource, TResult>(this IObservable<TSource> source, TResult witness)
        {
            return new OfTypeObservable<TSource, TResult>(source);
        }

        /// <summary>
        /// Converting .Select(_ => Unit.Default) sequence.
        /// </summary>
        public static IObservable<Unit> AsUnitObservable<T>(this IObservable<T> source)
        {
            return new AsUnitObservableObservable<T>(source);
        }

        /// <summary>
        /// Same as LastOrDefault().AsUnitObservable().
        /// </summary>
        public static IObservable<Unit> AsSingleUnitObservable<T>(this IObservable<T> source)
        {
            return new AsSingleUnitObservableObservable<T>(source);
        }
    }
}