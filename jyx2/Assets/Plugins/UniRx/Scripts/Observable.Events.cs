using System;
using UniRx.Operators;

namespace UniRx
{
    public static partial class Observable
    {
        public static IObservable<EventPattern<TEventArgs>> FromEventPattern<TDelegate, TEventArgs>(Func<EventHandler<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
            where TEventArgs : EventArgs
        {
            return new FromEventPatternObservable<TDelegate, TEventArgs>(conversion, addHandler, removeHandler);
        }

        public static IObservable<Unit> FromEvent<TDelegate>(Func<Action, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
        {
            return new FromEventObservable<TDelegate>(conversion, addHandler, removeHandler);
        }

        public static IObservable<TEventArgs> FromEvent<TDelegate, TEventArgs>(Func<Action<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
        {
            return new FromEventObservable<TDelegate, TEventArgs>(conversion, addHandler, removeHandler);
        }

        public static IObservable<Unit> FromEvent(Action<Action> addHandler, Action<Action> removeHandler)
        {
            return new FromEventObservable(addHandler, removeHandler);
        }

        public static IObservable<T> FromEvent<T>(Action<Action<T>> addHandler, Action<Action<T>> removeHandler)
        {
            return new FromEventObservable_<T>(addHandler, removeHandler);
        }
    }
}