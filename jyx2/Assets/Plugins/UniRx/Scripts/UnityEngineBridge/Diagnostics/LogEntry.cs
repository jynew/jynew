using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UniRx.Diagnostics
{
    public struct LogEntry
    {
        // requires
        public string LoggerName { get; private set; }
        public LogType LogType { get; private set; }
        public string Message { get; private set; }
        public DateTime Timestamp { get; private set; }

        // options

        /// <summary>[Optional]</summary>
        public UnityEngine.Object Context { get; private set; }
        /// <summary>[Optional]</summary>
        public Exception Exception { get; private set; }
        /// <summary>[Optional]</summary>
        public string StackTrace { get; private set; }
        /// <summary>[Optional]</summary>
        public object State { get; private set; }

        public LogEntry(string loggerName, LogType logType, DateTime timestamp, string message, UnityEngine.Object context = null, Exception exception = null, string stackTrace = null, object state = null)
            : this()
        {
            this.LoggerName = loggerName;
            this.LogType = logType;
            this.Timestamp = timestamp;
            this.Message = message;
            this.Context = context;
            this.Exception = exception;
            this.StackTrace = stackTrace;
            this.State = state;
        }

        public override string ToString()
        {
            var plusEx = (Exception != null) ? (Environment.NewLine + Exception.ToString()) : "";
            return "[" + Timestamp.ToString() + "]"
                + "[" + LoggerName + "]"
                + "[" + LogType.ToString() + "]"
                + Message
                + plusEx;
        }
    }
}
