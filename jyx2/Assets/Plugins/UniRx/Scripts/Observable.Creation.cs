using System;
using System.Collections.Generic;
using UniRx.Operators;

namespace UniRx
{
    public static partial class Observable
    {
        /// <summary>
        /// Create anonymous observable. Observer has exception durability. This is recommended for make operator and event like generator. 
        /// </summary>
        public static IObservable<T> Create<T>(Func<IObserver<T>, IDisposable> subscribe)
        {
            if (subscribe == null) throw new ArgumentNullException("subscribe");

            return new CreateObservable<T>(subscribe);
        }

        /// <summary>
        /// Create anonymous observable. Observer has exception durability. This is recommended for make operator and event like generator(HotObservable). 
        /// </summary>
        public static IObservable<T> Create<T>(Func<IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
        {
            if (subscribe == null) throw new ArgumentNullException("subscribe");

            return new CreateObservable<T>(subscribe, isRequiredSubscribeOnCurrentThread);
        }

        /// <summary>
        /// Create anonymous observable. Observer has exception durability. This is recommended for make operator and event like generator. 
        /// </summary>
        public static IObservable<T> CreateWithState<T, TState>(TState state, Func<TState, IObserver<T>, IDisposable> subscribe)
        {
            if (subscribe == null) throw new ArgumentNullException("subscribe");

            return new CreateObservable<T, TState>(state, subscribe);
        }

        /// <summary>
        /// Create anonymous observable. Observer has exception durability. This is recommended for make operator and event like generator(HotObservable). 
        /// </summary>
        public static IObservable<T> CreateWithState<T, TState>(TState state, Func<TState, IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
        {
            if (subscribe == null) throw new ArgumentNullException("subscribe");

            return new CreateObservable<T, TState>(state, subscribe, isRequiredSubscribeOnCurrentThread);
        }

        /// <summary>
        /// Create anonymous observable. Safe means auto detach when error raised in onNext pipeline. This is recommended for make generator (ColdObservable).
        /// </summary>
        public static IObservable<T> CreateSafe<T>(Func<IObserver<T>, IDisposable> subscribe)
        {
            if (subscribe == null) throw new ArgumentNullException("subscribe");

            return new CreateSafeObservable<T>(subscribe);
        }

        /// <summary>
        /// Create anonymous observable. Safe means auto detach when error raised in onNext pipeline. This is recommended for make generator (ColdObservable).
        /// </summary>
        public static IObservable<T> CreateSafe<T>(Func<IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
        {
            if (subscribe == null) throw new ArgumentNullException("subscribe");

            return new CreateSafeObservable<T>(subscribe, isRequiredSubscribeOnCurrentThread);
        }

        /// <summary>
        /// Empty Observable. Returns only OnCompleted.
        /// </summary>
        public static IObservable<T> Empty<T>()
        {
            return Empty<T>(Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        /// <summary>
        /// Empty Observable. Returns only OnCompleted on specified scheduler.
        /// </summary>
        public static IObservable<T> Empty<T>(IScheduler scheduler)
        {
            if (scheduler == Scheduler.Immediate)
            {
                return ImmutableEmptyObservable<T>.Instance;
            }
            else
            {
                return new EmptyObservable<T>(scheduler);
            }
        }

        /// <summary>
        /// Empty Observable. Returns only OnCompleted. witness is for type inference.
        /// </summary>
        public static IObservable<T> Empty<T>(T witness)
        {
            return Empty<T>(Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        /// <summary>
        /// Empty Observable. Returns only OnCompleted on specified scheduler. witness is for type inference.
        /// </summary>
        public static IObservable<T> Empty<T>(IScheduler scheduler, T witness)
        {
            return Empty<T>(scheduler);
        }

        /// <summary>
        /// Non-Terminating Observable. It's no returns, never finish.
        /// </summary>
        public static IObservable<T> Never<T>()
        {
            return ImmutableNeverObservable<T>.Instance;
        }

        /// <summary>
        /// Non-Terminating Observable. It's no returns, never finish. witness is for type inference.
        /// </summary>
        public static IObservable<T> Never<T>(T witness)
        {
            return ImmutableNeverObservable<T>.Instance;
        }

        /// <summary>
        /// Return single sequence Immediately.
        /// </summary>
        public static IObservable<T> Return<T>(T value)
        {
            return Return<T>(value, Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        /// <summary>
        /// Return single sequence on specified scheduler.
        /// </summary>
        public static IObservable<T> Return<T>(T value, IScheduler scheduler)
        {
            if (scheduler == Scheduler.Immediate)
            {
                return new ImmediateReturnObservable<T>(value);
            }
            else
            {
                return new ReturnObservable<T>(value, scheduler);
            }
        }

        /// <summary>
        /// Return single sequence Immediately, optimized for Unit(no allocate memory).
        /// </summary>
        public static IObservable<Unit> Return(Unit value)
        {
            return ImmutableReturnUnitObservable.Instance;
        }

        /// <summary>
        /// Return single sequence Immediately, optimized for Boolean(no allocate memory).
        /// </summary>
        public static IObservable<bool> Return(bool value)
        {
            return (value == true)
                ? (IObservable<bool>)ImmutableReturnTrueObservable.Instance
                : (IObservable<bool>)ImmutableReturnFalseObservable.Instance;
        }

        /// <summary>
        /// Return single sequence Immediately, optimized for Int32.
        /// </summary>
        public static IObservable<Int32> Return(int value)
        {
            return ImmutableReturnInt32Observable.GetInt32Observable(value);
        }

        /// <summary>
        /// Same as Observable.Return(Unit.Default); but no allocate memory.
        /// </summary>
        public static IObservable<Unit> ReturnUnit()
        {
            return ImmutableReturnUnitObservable.Instance;
        }

        /// <summary>
        /// Empty Observable. Returns only onError.
        /// </summary>
        public static IObservable<T> Throw<T>(Exception error)
        {
            return Throw<T>(error, Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        /// <summary>
        /// Empty Observable. Returns only onError. witness if for Type inference.
        /// </summary>
        public static IObservable<T> Throw<T>(Exception error, T witness)
        {
            return Throw<T>(error, Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        /// <summary>
        /// Empty Observable. Returns only onError on specified scheduler.
        /// </summary>
        public static IObservable<T> Throw<T>(Exception error, IScheduler scheduler)
        {
            return new ThrowObservable<T>(error, scheduler);
        }

        /// <summary>
        /// Empty Observable. Returns only onError on specified scheduler. witness if for Type inference.
        /// </summary>
        public static IObservable<T> Throw<T>(Exception error, IScheduler scheduler, T witness)
        {
            return Throw<T>(error, scheduler);
        }

        public static IObservable<int> Range(int start, int count)
        {
            return Range(start, count, Scheduler.DefaultSchedulers.Iteration);
        }

        public static IObservable<int> Range(int start, int count, IScheduler scheduler)
        {
            return new RangeObservable(start, count, scheduler);
        }

        public static IObservable<T> Repeat<T>(T value)
        {
            return Repeat(value, Scheduler.DefaultSchedulers.Iteration);
        }

        public static IObservable<T> Repeat<T>(T value, IScheduler scheduler)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            return new RepeatObservable<T>(value, null, scheduler);
        }

        public static IObservable<T> Repeat<T>(T value, int repeatCount)
        {
            return Repeat(value, repeatCount, Scheduler.DefaultSchedulers.Iteration);
        }

        public static IObservable<T> Repeat<T>(T value, int repeatCount, IScheduler scheduler)
        {
            if (repeatCount < 0) throw new ArgumentOutOfRangeException("repeatCount");
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            return new RepeatObservable<T>(value, repeatCount, scheduler);
        }

        public static IObservable<T> Repeat<T>(this IObservable<T> source)
        {
            return RepeatInfinite(source).Concat();
        }

        static IEnumerable<IObservable<T>> RepeatInfinite<T>(IObservable<T> source)
        {
            while (true)
            {
                yield return source;
            }
        }

        /// <summary>
        /// Same as Repeat() but if arriving contiguous "OnComplete" Repeat stops.
        /// </summary>
        public static IObservable<T> RepeatSafe<T>(this IObservable<T> source)
        {
            return new RepeatSafeObservable<T>(RepeatInfinite(source), source.IsRequiredSubscribeOnCurrentThread());
        }

        public static IObservable<T> Defer<T>(Func<IObservable<T>> observableFactory)
        {
            return new DeferObservable<T>(observableFactory);
        }

        public static IObservable<T> Start<T>(Func<T> function)
        {
            return new StartObservable<T>(function, null, Scheduler.DefaultSchedulers.AsyncConversions);
        }

        public static IObservable<T> Start<T>(Func<T> function, TimeSpan timeSpan)
        {
            return new StartObservable<T>(function, timeSpan, Scheduler.DefaultSchedulers.AsyncConversions);
        }

        public static IObservable<T> Start<T>(Func<T> function, IScheduler scheduler)
        {
            return new StartObservable<T>(function, null, scheduler);
        }

        public static IObservable<T> Start<T>(Func<T> function, TimeSpan timeSpan, IScheduler scheduler)
        {
            return new StartObservable<T>(function, timeSpan, scheduler);
        }

        public static IObservable<Unit> Start(Action action)
        {
            return new StartObservable<Unit>(action, null, Scheduler.DefaultSchedulers.AsyncConversions);
        }

        public static IObservable<Unit> Start(Action action, TimeSpan timeSpan)
        {
            return new StartObservable<Unit>(action, timeSpan, Scheduler.DefaultSchedulers.AsyncConversions);
        }

        public static IObservable<Unit> Start(Action action, IScheduler scheduler)
        {
            return new StartObservable<Unit>(action, null, scheduler);
        }

        public static IObservable<Unit> Start(Action action, TimeSpan timeSpan, IScheduler scheduler)
        {
            return new StartObservable<Unit>(action, timeSpan, scheduler);
        }

        public static Func<IObservable<T>> ToAsync<T>(Func<T> function)
        {
            return ToAsync(function, Scheduler.DefaultSchedulers.AsyncConversions);
        }

        public static Func<IObservable<T>> ToAsync<T>(Func<T> function, IScheduler scheduler)
        {
            return () =>
            {
                var subject = new AsyncSubject<T>();

                scheduler.Schedule(() =>
                {
                    var result = default(T);
                    try
                    {
                        result = function();
                    }
                    catch (Exception exception)
                    {
                        subject.OnError(exception);
                        return;
                    }
                    subject.OnNext(result);
                    subject.OnCompleted();
                });

                return subject.AsObservable();
            };
        }

        public static Func<IObservable<Unit>> ToAsync(Action action)
        {
            return ToAsync(action, Scheduler.DefaultSchedulers.AsyncConversions);
        }

        public static Func<IObservable<Unit>> ToAsync(Action action, IScheduler scheduler)
        {
            return () =>
            {
                var subject = new AsyncSubject<Unit>();

                scheduler.Schedule(() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception exception)
                    {
                        subject.OnError(exception);
                        return;
                    }
                    subject.OnNext(Unit.Default);
                    subject.OnCompleted();
                });

                return subject.AsObservable();
            };
        }
    }
}