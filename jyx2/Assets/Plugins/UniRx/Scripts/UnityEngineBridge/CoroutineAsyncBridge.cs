#if (NET_4_6 || NET_STANDARD_2_0)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UniRx
{
    public class CoroutineAsyncBridge : INotifyCompletion
    {
        Action continuation;
        public bool IsCompleted { get; private set; }

        CoroutineAsyncBridge()
        {
            IsCompleted = false;
        }

        public static CoroutineAsyncBridge Start<T>(T awaitTarget)
        {
            var bridge = new CoroutineAsyncBridge();
            MainThreadDispatcher.StartCoroutine(bridge.Run(awaitTarget));
            return bridge;
        }

        IEnumerator Run<T>(T target)
        {
            yield return target;
            IsCompleted = true;
            continuation();
        }

        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }

        public void GetResult()
        {
            if (!IsCompleted) throw new InvalidOperationException("coroutine not yet completed");
        }
    }

    public class CoroutineAsyncBridge<T> : INotifyCompletion
    {
        readonly T result;
        Action continuation;
        public bool IsCompleted { get; private set; }

        CoroutineAsyncBridge(T result)
        {
            IsCompleted = false;
            this.result = result;
        }

        public static CoroutineAsyncBridge<T> Start(T awaitTarget)
        {
            var bridge = new CoroutineAsyncBridge<T>(awaitTarget);
            MainThreadDispatcher.StartCoroutine(bridge.Run(awaitTarget));
            return bridge;
        }

        IEnumerator Run(T target)
        {
            yield return target;
            IsCompleted = true;
            continuation();
        }

        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }

        public T GetResult()
        {
            if (!IsCompleted) throw new InvalidOperationException("coroutine not yet completed");
            return result;
        }
    }

    public static class CoroutineAsyncExtensions
    {
        public static CoroutineAsyncBridge GetAwaiter(this Coroutine coroutine)
        {
            return CoroutineAsyncBridge.Start(coroutine);
        }

#if !(CSHARP_7_OR_LATER || (UNITY_2018_3_OR_NEWER && (NET_STANDARD_2_0 || NET_4_6)))

        // should use UniRx.Async in C# 7.0

#if UNITY_2018_3_OR_NEWER
#pragma warning disable CS0618
#endif
        public static CoroutineAsyncBridge<WWW> GetAwaiter(this WWW www)
        {
            return CoroutineAsyncBridge<WWW>.Start(www);
        }
#if UNITY_2018_3_OR_NEWER
#pragma warning restore CS0618
#endif



        public static CoroutineAsyncBridge<AsyncOperation> GetAwaiter(this AsyncOperation asyncOperation)
        {
            return CoroutineAsyncBridge<AsyncOperation>.Start(asyncOperation);
        }

        public static CoroutineAsyncBridge GetAwaiter(this IEnumerator coroutine)
        {
            return CoroutineAsyncBridge.Start(coroutine);
        }

#endif
    }
}

#endif