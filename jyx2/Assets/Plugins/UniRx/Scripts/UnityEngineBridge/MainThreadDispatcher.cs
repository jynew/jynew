#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
#define SupportCustomYieldInstruction
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UniRx.InternalUtil;
using UnityEngine;

namespace UniRx
{
    public sealed class MainThreadDispatcher : MonoBehaviour
    {
        public enum CullingMode
        {
            /// <summary>
            /// Won't remove any MainThreadDispatchers.
            /// </summary>
            Disabled,

            /// <summary>
            /// Checks if there is an existing MainThreadDispatcher on Awake(). If so, the new dispatcher removes itself.
            /// </summary>
            Self,

            /// <summary>
            /// Search for excess MainThreadDispatchers and removes them all on Awake().
            /// </summary>
            All
        }

        public static CullingMode cullingMode = CullingMode.Self;

#if UNITY_EDITOR

        // In UnityEditor's EditorMode can't instantiate and work MonoBehaviour.Update.
        // EditorThreadDispatcher use EditorApplication.update instead of MonoBehaviour.Update.
        class EditorThreadDispatcher
        {
            static object gate = new object();
            static EditorThreadDispatcher instance;

            public static EditorThreadDispatcher Instance
            {
                get
                {
                    // Activate EditorThreadDispatcher is dangerous, completely Lazy.
                    lock (gate)
                    {
                        if (instance == null)
                        {
                            instance = new EditorThreadDispatcher();
                        }

                        return instance;
                    }
                }
            }

            ThreadSafeQueueWorker editorQueueWorker = new ThreadSafeQueueWorker();

            EditorThreadDispatcher()
            {
                UnityEditor.EditorApplication.update += Update;
            }

            public void Enqueue(Action<object> action, object state)
            {
                editorQueueWorker.Enqueue(action, state);
            }

            public void UnsafeInvoke(Action action)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            public void UnsafeInvoke<T>(Action<T> action, T state)
            {
                try
                {
                    action(state);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            public void PseudoStartCoroutine(IEnumerator routine)
            {
                editorQueueWorker.Enqueue(_ => ConsumeEnumerator(routine), null);
            }

            void Update()
            {
                editorQueueWorker.ExecuteAll(x => Debug.LogException(x));
            }

            void ConsumeEnumerator(IEnumerator routine)
            {
                if (routine.MoveNext())
                {
                    var current = routine.Current;
                    if (current == null)
                    {
                        goto ENQUEUE;
                    }

                    var type = current.GetType();
#if UNITY_2018_3_OR_NEWER
#pragma warning disable CS0618
#endif
                    if (type == typeof(WWW))
                    {
                        var www = (WWW)current;
                        editorQueueWorker.Enqueue(_ => ConsumeEnumerator(UnwrapWaitWWW(www, routine)), null);
                        return;
                    }
#if UNITY_2018_3_OR_NEWER
#pragma warning restore CS0618
#endif
                    else if (type == typeof(AsyncOperation))
                    {
                        var asyncOperation = (AsyncOperation)current;
                        editorQueueWorker.Enqueue(_ => ConsumeEnumerator(UnwrapWaitAsyncOperation(asyncOperation, routine)), null);
                        return;
                    }
                    else if (type == typeof(WaitForSeconds))
                    {
                        var waitForSeconds = (WaitForSeconds)current;
                        var accessor = typeof(WaitForSeconds).GetField("m_Seconds", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic);
                        var second = (float)accessor.GetValue(waitForSeconds);
                        editorQueueWorker.Enqueue(_ => ConsumeEnumerator(UnwrapWaitForSeconds(second, routine)), null);
                        return;
                    }
                    else if (type == typeof(Coroutine))
                    {
                        Debug.Log("Can't wait coroutine on UnityEditor");
                        goto ENQUEUE;
                    }
#if SupportCustomYieldInstruction
                    else if (current is IEnumerator)
                    {
                        var enumerator = (IEnumerator)current;
                        editorQueueWorker.Enqueue(_ => ConsumeEnumerator(UnwrapEnumerator(enumerator, routine)), null);
                        return;
                    }
#endif

                    ENQUEUE:
                    editorQueueWorker.Enqueue(_ => ConsumeEnumerator(routine), null); // next update
                }
            }

#if UNITY_2018_3_OR_NEWER
#pragma warning disable CS0618
#endif
            IEnumerator UnwrapWaitWWW(WWW www, IEnumerator continuation)
            {
                while (!www.isDone)
                {
                    yield return null;
                }
                ConsumeEnumerator(continuation);
            }
#if UNITY_2018_3_OR_NEWER
#pragma warning restore CS0618
#endif

            IEnumerator UnwrapWaitAsyncOperation(AsyncOperation asyncOperation, IEnumerator continuation)
            {
                while (!asyncOperation.isDone)
                {
                    yield return null;
                }
                ConsumeEnumerator(continuation);
            }

            IEnumerator UnwrapWaitForSeconds(float second, IEnumerator continuation)
            {
                var startTime = DateTimeOffset.UtcNow;
                while (true)
                {
                    yield return null;

                    var elapsed = (DateTimeOffset.UtcNow - startTime).TotalSeconds;
                    if (elapsed >= second)
                    {
                        break;
                    }
                };
                ConsumeEnumerator(continuation);
            }

            IEnumerator UnwrapEnumerator(IEnumerator enumerator, IEnumerator continuation)
            {
                while (enumerator.MoveNext())
                {
                    yield return null;
                }
                ConsumeEnumerator(continuation);
            }
        }

#endif

        /// <summary>Dispatch Asyncrhonous action.</summary>
        public static void Post(Action<object> action, object state)
        {
#if UNITY_EDITOR
            if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.Enqueue(action, state); return; }

#endif

            var dispatcher = Instance;
            if (!isQuitting && !object.ReferenceEquals(dispatcher, null))
            {
                dispatcher.queueWorker.Enqueue(action, state);
            }
        }

        /// <summary>Dispatch Synchronous action if possible.</summary>
        public static void Send(Action<object> action, object state)
        {
#if UNITY_EDITOR
            if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.Enqueue(action, state); return; }
#endif

            if (mainThreadToken != null)
            {
                try
                {
                    action(state);
                }
                catch (Exception ex)
                {
                    var dispatcher = MainThreadDispatcher.Instance;
                    if (dispatcher != null)
                    {
                        dispatcher.unhandledExceptionCallback(ex);
                    }
                }
            }
            else
            {
                Post(action, state);
            }
        }

        /// <summary>Run Synchronous action.</summary>
        public static void UnsafeSend(Action action)
        {
#if UNITY_EDITOR
            if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.UnsafeInvoke(action); return; }
#endif

            try
            {
                action();
            }
            catch (Exception ex)
            {
                var dispatcher = MainThreadDispatcher.Instance;
                if (dispatcher != null)
                {
                    dispatcher.unhandledExceptionCallback(ex);
                }
            }
        }

        /// <summary>Run Synchronous action.</summary>
        public static void UnsafeSend<T>(Action<T> action, T state)
        {
#if UNITY_EDITOR
            if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.UnsafeInvoke(action, state); return; }
#endif

            try
            {
                action(state);
            }
            catch (Exception ex)
            {
                var dispatcher = MainThreadDispatcher.Instance;
                if (dispatcher != null)
                {
                    dispatcher.unhandledExceptionCallback(ex);
                }
            }
        }

        /// <summary>ThreadSafe StartCoroutine.</summary>
        public static void SendStartCoroutine(IEnumerator routine)
        {
            if (mainThreadToken != null)
            {
                StartCoroutine(routine);
            }
            else
            {
#if UNITY_EDITOR
                // call from other thread
                if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.PseudoStartCoroutine(routine); return; }
#endif

                var dispatcher = Instance;
                if (!isQuitting && !object.ReferenceEquals(dispatcher, null))
                {
                    dispatcher.queueWorker.Enqueue(_ =>
                    {
                        var dispacher2 = Instance;
                        if (dispacher2 != null)
                        {
                            (dispacher2 as MonoBehaviour).StartCoroutine(routine);
                        }
                    }, null);
                }
            }
        }

        public static void StartUpdateMicroCoroutine(IEnumerator routine)
        {
#if UNITY_EDITOR
            if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.PseudoStartCoroutine(routine); return; }
#endif

            var dispatcher = Instance;
            if (dispatcher != null)
            {
                dispatcher.updateMicroCoroutine.AddCoroutine(routine);
            }
        }

        public static void StartFixedUpdateMicroCoroutine(IEnumerator routine)
        {
#if UNITY_EDITOR
            if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.PseudoStartCoroutine(routine); return; }
#endif

            var dispatcher = Instance;
            if (dispatcher != null)
            {
                dispatcher.fixedUpdateMicroCoroutine.AddCoroutine(routine);
            }
        }

        public static void StartEndOfFrameMicroCoroutine(IEnumerator routine)
        {
#if UNITY_EDITOR
            if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.PseudoStartCoroutine(routine); return; }
#endif

            var dispatcher = Instance;
            if (dispatcher != null)
            {
                dispatcher.endOfFrameMicroCoroutine.AddCoroutine(routine);
            }
        }

        new public static Coroutine StartCoroutine(IEnumerator routine)
        {
#if UNITY_EDITOR
            if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.PseudoStartCoroutine(routine); return null; }
#endif

            var dispatcher = Instance;
            if (dispatcher != null)
            {
                return (dispatcher as MonoBehaviour).StartCoroutine(routine);
            }
            else
            {
                return null;
            }
        }

        public static void RegisterUnhandledExceptionCallback(Action<Exception> exceptionCallback)
        {
            if (exceptionCallback == null)
            {
                // do nothing
                Instance.unhandledExceptionCallback = Stubs<Exception>.Ignore;
            }
            else
            {
                Instance.unhandledExceptionCallback = exceptionCallback;
            }
        }

        ThreadSafeQueueWorker queueWorker = new ThreadSafeQueueWorker();
        Action<Exception> unhandledExceptionCallback = ex => Debug.LogException(ex); // default

        MicroCoroutine updateMicroCoroutine = null;
        MicroCoroutine fixedUpdateMicroCoroutine = null;
        MicroCoroutine endOfFrameMicroCoroutine = null;

        static MainThreadDispatcher instance;
        static bool initialized;
        static bool isQuitting = false;

        public static string InstanceName
        {
            get
            {
                if (instance == null)
                {
                    throw new NullReferenceException("MainThreadDispatcher is not initialized.");
                }
                return instance.name;
            }
        }

        public static bool IsInitialized
        {
            get { return initialized && instance != null; }
        }

        [ThreadStatic]
        static object mainThreadToken;

        static MainThreadDispatcher Instance
        {
            get
            {
                Initialize();
                return instance;
            }
        }

        public static void Initialize()
        {
            if (!initialized)
            {
#if UNITY_EDITOR
                // Don't try to add a GameObject when the scene is not playing. Only valid in the Editor, EditorView.
                if (!ScenePlaybackDetector.IsPlaying) return;
#endif
                MainThreadDispatcher dispatcher = null;

                try
                {
                    dispatcher = GameObject.FindObjectOfType<MainThreadDispatcher>();
                }
                catch
                {
                    // Throw exception when calling from a worker thread.
                    var ex = new Exception("UniRx requires a MainThreadDispatcher component created on the main thread. Make sure it is added to the scene before calling UniRx from a worker thread.");
                    UnityEngine.Debug.LogException(ex);
                    throw ex;
                }

                if (isQuitting)
                {
                    // don't create new instance after quitting
                    // avoid "Some objects were not cleaned up when closing the scene find target" error.
                    return;
                }

                if (dispatcher == null)
                {
                    // awake call immediately from UnityEngine
                    new GameObject("MainThreadDispatcher").AddComponent<MainThreadDispatcher>();
                }
                else
                {
                    dispatcher.Awake(); // force awake
                }
            }
        }

        public static bool IsInMainThread
        {
            get
            {
                return (mainThreadToken != null);
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                mainThreadToken = new object();
                initialized = true;

                updateMicroCoroutine = new MicroCoroutine(ex => unhandledExceptionCallback(ex));
                fixedUpdateMicroCoroutine = new MicroCoroutine(ex => unhandledExceptionCallback(ex));
                endOfFrameMicroCoroutine = new MicroCoroutine(ex => unhandledExceptionCallback(ex));

                StartCoroutine(RunUpdateMicroCoroutine());
                StartCoroutine(RunFixedUpdateMicroCoroutine());
                StartCoroutine(RunEndOfFrameMicroCoroutine());

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (this != instance)
                {
                    if (cullingMode == CullingMode.Self)
                    {
                        // Try to destroy this dispatcher if there's already one in the scene.
                        Debug.LogWarning("There is already a MainThreadDispatcher in the scene. Removing myself...");
                        DestroyDispatcher(this);
                    }
                    else if (cullingMode == CullingMode.All)
                    {
                        Debug.LogWarning("There is already a MainThreadDispatcher in the scene. Cleaning up all excess dispatchers...");
                        CullAllExcessDispatchers();
                    }
                    else
                    {
                        Debug.LogWarning("There is already a MainThreadDispatcher in the scene.");
                    }
                }
            }
        }

        IEnumerator RunUpdateMicroCoroutine()
        {
            while (true)
            {
                yield return null;
                updateMicroCoroutine.Run();
            }
        }

        IEnumerator RunFixedUpdateMicroCoroutine()
        {
            while (true)
            {
                yield return YieldInstructionCache.WaitForFixedUpdate;
                fixedUpdateMicroCoroutine.Run();
            }
        }

        IEnumerator RunEndOfFrameMicroCoroutine()
        {
            while (true)
            {
                yield return YieldInstructionCache.WaitForEndOfFrame;
                endOfFrameMicroCoroutine.Run();
            }
        }

        static void DestroyDispatcher(MainThreadDispatcher aDispatcher)
        {
            if (aDispatcher != instance)
            {
                // Try to remove game object if it's empty
                var components = aDispatcher.gameObject.GetComponents<Component>();
                if (aDispatcher.gameObject.transform.childCount == 0 && components.Length == 2)
                {
                    if (components[0] is Transform && components[1] is MainThreadDispatcher)
                    {
                        Destroy(aDispatcher.gameObject);
                    }
                }
                else
                {
                    // Remove component
                    MonoBehaviour.Destroy(aDispatcher);
                }
            }
        }

        public static void CullAllExcessDispatchers()
        {
            var dispatchers = GameObject.FindObjectsOfType<MainThreadDispatcher>();
            for (int i = 0; i < dispatchers.Length; i++)
            {
                DestroyDispatcher(dispatchers[i]);
            }
        }

        void OnDestroy()
        {
            if (instance == this)
            {
                instance = GameObject.FindObjectOfType<MainThreadDispatcher>();
                initialized = instance != null;

                /*
                // Although `this` still refers to a gameObject, it won't be found.
                var foundDispatcher = GameObject.FindObjectOfType<MainThreadDispatcher>();

                if (foundDispatcher != null)
                {
                    // select another game object
                    Debug.Log("new instance: " + foundDispatcher.name);
                    instance = foundDispatcher;
                    initialized = true;
                }
                */
            }
        }

        void Update()
        {
            if (update != null)
            {
                try
                {
                    update.OnNext(Unit.Default);
                }
                catch (Exception ex)
                {
                    unhandledExceptionCallback(ex);
                }
            }
            queueWorker.ExecuteAll(unhandledExceptionCallback);
        }

        // for Lifecycle Management

        Subject<Unit> update;

        public static IObservable<Unit> UpdateAsObservable()
        {
            return Instance.update ?? (Instance.update = new Subject<Unit>());
        }

        Subject<Unit> lateUpdate;

        void LateUpdate()
        {
            if (lateUpdate != null) lateUpdate.OnNext(Unit.Default);
        }

        public static IObservable<Unit> LateUpdateAsObservable()
        {
            return Instance.lateUpdate ?? (Instance.lateUpdate = new Subject<Unit>());
        }

        Subject<bool> onApplicationFocus;

        void OnApplicationFocus(bool focus)
        {
            if (onApplicationFocus != null) onApplicationFocus.OnNext(focus);
        }

        public static IObservable<bool> OnApplicationFocusAsObservable()
        {
            return Instance.onApplicationFocus ?? (Instance.onApplicationFocus = new Subject<bool>());
        }

        Subject<bool> onApplicationPause;

        void OnApplicationPause(bool pause)
        {
            if (onApplicationPause != null) onApplicationPause.OnNext(pause);
        }

        public static IObservable<bool> OnApplicationPauseAsObservable()
        {
            return Instance.onApplicationPause ?? (Instance.onApplicationPause = new Subject<bool>());
        }

        Subject<Unit> onApplicationQuit;

        void OnApplicationQuit()
        {
            isQuitting = true;
            if (onApplicationQuit != null) onApplicationQuit.OnNext(Unit.Default);
        }

        public static IObservable<Unit> OnApplicationQuitAsObservable()
        {
            return Instance.onApplicationQuit ?? (Instance.onApplicationQuit = new Subject<Unit>());
        }
    }
}