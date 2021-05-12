using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UniRx.Diagnostics
{
    public class UnityDebugSink : IObserver<LogEntry>
    {
        public void OnCompleted()
        {
            // do nothing
        }

        public void OnError(Exception error)
        {
            // do nothing
        }

        public void OnNext(LogEntry value)
        {
            // avoid multithread exception.
            // (value.Context == null) can only be called from the main thread.
            var ctx = (System.Object)value.Context;

            switch (value.LogType)
            {
                case LogType.Error:
                    if (ctx == null)
                    {
                        Debug.LogError(value.Message);
                    }
                    else
                    {
                        Debug.LogError(value.Message, value.Context);
                    }
                    break;
                case LogType.Exception:
                    if (ctx == null)
                    {
                        Debug.LogException(value.Exception);
                    }
                    else
                    {
                        Debug.LogException(value.Exception, value.Context);
                    }
                    break;
                case LogType.Log:
                    if (ctx == null)
                    {
                        Debug.Log(value.Message);
                    }
                    else
                    {
                        Debug.Log(value.Message, value.Context);
                    }
                    break;
                case LogType.Warning:
                    if (ctx == null)
                    {
                        Debug.LogWarning(value.Message);
                    }
                    else
                    {
                        Debug.LogWarning(value.Message, value.Context);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}