using System;
using System.Collections;
using UniRx.Diagnostics;
using UnityEngine;

namespace UniRx.Examples
{
    public class Sample11_Logger
    {
        // UniRx.Diagnostics.Logger
        // logger is threadsafe, define per class with name.
		static readonly UniRx.Diagnostics.Logger logger = new UniRx.Diagnostics.Logger("Sample11");

        // call once at applicationinit
        public void ApplicationInitialize()
        {
            // Log as Stream, UniRx.Diagnostics.ObservableLogger.Listener is IObservable<LogEntry>
            // You can subscribe and output to any place.
            ObservableLogger.Listener.LogToUnityDebug();

            // for example, filter only Exception and upload to web.
            // (make custom sink(IObserver<EventEntry>) is better to use)
            ObservableLogger.Listener
                .Where(x => x.LogType == LogType.Exception)
                .Subscribe(x =>
                {
                    // ObservableWWW.Post("", null).Subscribe();
                });
        }

        public void Run()
        {
            // Debug is write only DebugBuild.
            logger.Debug("Debug Message");

            // or other logging methods
            logger.Log("Message");
            logger.Exception(new Exception("test exception"));
        }
    }
}