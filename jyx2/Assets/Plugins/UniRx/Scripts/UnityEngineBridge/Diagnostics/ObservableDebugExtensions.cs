using System;

namespace UniRx.Diagnostics
{
    public static class ObservableDebugExtensions
    {
        /// <summary>
        /// Debug helper of observbale stream. Works for only DEBUG symbol.
        /// </summary>
        public static IObservable<T> Debug<T>(this IObservable<T> source, string label = null)
        {
#if DEBUG
            var l = (label == null) ? "" : "[" + label + "]";
            return source.Materialize()
                .Do(x => UnityEngine.Debug.Log(l + x.ToString()))
                .Dematerialize()
                .DoOnCancel(() => UnityEngine.Debug.Log(l + "OnCancel"))
                .DoOnSubscribe(() => UnityEngine.Debug.Log(l + "OnSubscribe"));

#else
            return source;
#endif
        }

        /// <summary>
        /// Debug helper of observbale stream. Works for only DEBUG symbol.
        /// </summary>
        public static IObservable<T> Debug<T>(this IObservable<T> source, UniRx.Diagnostics.Logger logger)
        {
#if DEBUG
            return source.Materialize()
                .Do(x => logger.Debug(x.ToString()))
                .Dematerialize()
                .DoOnCancel(() => logger.Debug("OnCancel"))
                .DoOnSubscribe(() => logger.Debug("OnSubscribe"));

#else
            return source;
#endif
        }
    }
}
