using System;
using UnityEngine;

namespace UniRx.Examples
{
    public class Sample04_ConvertFromUnityCallback : MonoBehaviour
    {
        // This is about log but more reliable log sample => Sample11_Logger

        private class LogCallback
        {
            public string Condition;
            public string StackTrace;
            public UnityEngine.LogType LogType;
        }

        static class LogHelper
        {
            // If static register callback, use Subject for event branching.

#if (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7)                    
            static Subject<LogCallback> subject;

            public static IObservable<LogCallback> LogCallbackAsObservable()
            {
                if (subject == null)
                {
                    subject = new Subject<LogCallback>();

                    // Publish to Subject in callback


                    UnityEngine.Application.RegisterLogCallback((condition, stackTrace, type) =>
                    {
                        subject.OnNext(new LogCallback { Condition = condition, StackTrace = stackTrace, LogType = type });
                    });
                }

                return subject.AsObservable();
            }

#else
            // If standard evetns, you can use Observable.FromEvent.

            public static IObservable<LogCallback> LogCallbackAsObservable()
            {
                return Observable.FromEvent<Application.LogCallback, LogCallback>(
                    h => (condition, stackTrace, type) => h(new LogCallback { Condition = condition, StackTrace = stackTrace, LogType = type }),
                    h => Application.logMessageReceived += h, h => Application.logMessageReceived -= h);
            }
#endif
        }

        void Awake()
        {
            // method is separatable and composable
            LogHelper.LogCallbackAsObservable()
                .Where(x => x.LogType == LogType.Warning)
                .Subscribe(x => Debug.Log(x));

            LogHelper.LogCallbackAsObservable()
                .Where(x => x.LogType == LogType.Error)
                .Subscribe(x => Debug.Log(x));
        }
    }
}