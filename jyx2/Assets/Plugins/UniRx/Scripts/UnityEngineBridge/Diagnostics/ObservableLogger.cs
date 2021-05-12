using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UniRx.Diagnostics
{
    public class ObservableLogger : IObservable<LogEntry>
    {
        static readonly Subject<LogEntry> logPublisher = new Subject<LogEntry>();

        public static readonly ObservableLogger Listener = new ObservableLogger();

        private ObservableLogger()
        {

        }

        public static Action<LogEntry> RegisterLogger(Logger logger)
        {
            if (logger.Name == null) throw new ArgumentNullException("logger.Name is null");

            return logPublisher.OnNext;
        }

        public IDisposable Subscribe(IObserver<LogEntry> observer)
        {
            return logPublisher.Subscribe(observer);
        }
    }
}