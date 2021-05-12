using System;

namespace UniRx
{
    public static partial class Observable
    {
        public static Func<IObservable<TResult>> FromAsyncPattern<TResult>(Func<AsyncCallback, object, IAsyncResult> begin, Func<IAsyncResult, TResult> end)
        {
            return () =>
            {
                var subject = new AsyncSubject<TResult>();
                try
                {
                    begin(iar =>
                    {
                        TResult result;
                        try
                        {
                            result = end(iar);
                        }
                        catch (Exception exception)
                        {
                            subject.OnError(exception);
                            return;
                        }
                        subject.OnNext(result);
                        subject.OnCompleted();
                    }, null);
                }
                catch (Exception exception)
                {
                    return Observable.Throw<TResult>(exception, Scheduler.DefaultSchedulers.AsyncConversions);
                }
                return subject.AsObservable();
            };
        }

        public static Func<T1, IObservable<TResult>> FromAsyncPattern<T1, TResult>(Func<T1, AsyncCallback, object, IAsyncResult> begin, Func<IAsyncResult, TResult> end)
        {
            return x =>
            {
                var subject = new AsyncSubject<TResult>();
                try
                {
                    begin(x, iar =>
                    {
                        TResult result;
                        try
                        {
                            result = end(iar);
                        }
                        catch (Exception exception)
                        {
                            subject.OnError(exception);
                            return;
                        }
                        subject.OnNext(result);
                        subject.OnCompleted();
                    }, null);
                }
                catch (Exception exception)
                {
                    return Observable.Throw<TResult>(exception, Scheduler.DefaultSchedulers.AsyncConversions);
                }
                return subject.AsObservable();
            };
        }

        public static Func<T1, T2, IObservable<TResult>> FromAsyncPattern<T1, T2, TResult>(Func<T1, T2, AsyncCallback, object, IAsyncResult> begin, Func<IAsyncResult, TResult> end)
        {
            return (x, y) =>
            {
                var subject = new AsyncSubject<TResult>();
                try
                {
                    begin(x, y, iar =>
                    {
                        TResult result;
                        try
                        {
                            result = end(iar);
                        }
                        catch (Exception exception)
                        {
                            subject.OnError(exception);
                            return;
                        }
                        subject.OnNext(result);
                        subject.OnCompleted();
                    }, null);
                }
                catch (Exception exception)
                {
                    return Observable.Throw<TResult>(exception, Scheduler.DefaultSchedulers.AsyncConversions);
                }
                return subject.AsObservable();
            };
        }

        public static Func<IObservable<Unit>> FromAsyncPattern(Func<AsyncCallback, object, IAsyncResult> begin, Action<IAsyncResult> end)
        {
            return FromAsyncPattern(begin, iar =>
            {
                end(iar);
                return Unit.Default;
            });
        }

        public static Func<T1, IObservable<Unit>> FromAsyncPattern<T1>(Func<T1, AsyncCallback, object, IAsyncResult> begin, Action<IAsyncResult> end)
        {
            return FromAsyncPattern(begin, iar =>
            {
                end(iar);
                return Unit.Default;
            });
        }

        public static Func<T1, T2, IObservable<Unit>> FromAsyncPattern<T1, T2>(Func<T1, T2, AsyncCallback, object, IAsyncResult> begin, Action<IAsyncResult> end)
        {
            return FromAsyncPattern(begin, iar =>
            {
                end(iar);
                return Unit.Default;
            });
        }
    }
}