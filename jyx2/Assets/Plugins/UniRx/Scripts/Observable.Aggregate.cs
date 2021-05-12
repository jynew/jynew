using System;
using System.Collections.Generic;
using System.Text;
using UniRx.Operators;

namespace UniRx
{
    public static partial class Observable
    {
        public static IObservable<TSource> Scan<TSource>(this IObservable<TSource> source, Func<TSource, TSource, TSource> accumulator)
        {
            return new ScanObservable<TSource>(source, accumulator);
        }

        public static IObservable<TAccumulate> Scan<TSource, TAccumulate>(this IObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
        {
            return new ScanObservable<TSource, TAccumulate>(source, seed, accumulator);
        }

        public static IObservable<TSource> Aggregate<TSource>(this IObservable<TSource> source, Func<TSource, TSource, TSource> accumulator)
        {
            return new AggregateObservable<TSource>(source, accumulator);
        }

        public static IObservable<TAccumulate> Aggregate<TSource, TAccumulate>(this IObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
        {
            return new AggregateObservable<TSource, TAccumulate>(source, seed, accumulator);
        }

        public static IObservable<TResult> Aggregate<TSource, TAccumulate, TResult>(this IObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, Func<TAccumulate, TResult> resultSelector)
        {
            return new AggregateObservable<TSource, TAccumulate, TResult>(source, seed, accumulator, resultSelector);
        }
    }
}
