using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx.InternalUtil
{
    /// <summary>
    /// Simple supports(only yield return null) lightweight, threadsafe coroutine dispatcher.
    /// </summary>
    public class MicroCoroutine
    {
        const int InitialSize = 16;

        readonly object runningAndQueueLock = new object();
        readonly object arrayLock = new object();
        readonly Action<Exception> unhandledExceptionCallback;

        int tail = 0;
        bool running = false;
        IEnumerator[] coroutines = new IEnumerator[InitialSize];
        Queue<IEnumerator> waitQueue = new Queue<IEnumerator>();

        public MicroCoroutine(Action<Exception> unhandledExceptionCallback)
        {
            this.unhandledExceptionCallback = unhandledExceptionCallback;
        }

        public void AddCoroutine(IEnumerator enumerator)
        {
            lock (runningAndQueueLock)
            {
                if (running)
                {
                    waitQueue.Enqueue(enumerator);
                    return;
                }
            }

            // worst case at multi threading, wait lock until finish Run() but it is super rarely.
            lock (arrayLock)
            {
                // Ensure Capacity
                if (coroutines.Length == tail)
                {
                    Array.Resize(ref coroutines, checked(tail * 2));
                }
                coroutines[tail++] = enumerator;
            }
        }

        public void Run()
        {
            lock (runningAndQueueLock)
            {
                running = true;
            }

            lock (arrayLock)
            {
                var j = tail - 1;

                // eliminate array-bound check for i
                for (int i = 0; i < coroutines.Length; i++)
                {
                    var coroutine = coroutines[i];
                    if (coroutine != null)
                    {
                        try
                        {
                            if (!coroutine.MoveNext())
                            {
                                coroutines[i] = null;
                            }
                            else
                            {
#if UNITY_EDITOR
                                // validation only on Editor.
                                if (coroutine.Current != null)
                                {
                                    UnityEngine.Debug.LogWarning("MicroCoroutine supports only yield return null. return value = " + coroutine.Current);
                                }
#endif

                                continue; // next i 
                            }
                        }
                        catch (Exception ex)
                        {
                            coroutines[i] = null;
                            try
                            {
                                unhandledExceptionCallback(ex);
                            }
                            catch { }
                        }
                    }

                    // find null, loop from tail
                    while (i < j)
                    {
                        var fromTail = coroutines[j];
                        if (fromTail != null)
                        {
                            try
                            {
                                if (!fromTail.MoveNext())
                                {
                                    coroutines[j] = null;
                                    j--;
                                    continue; // next j
                                }
                                else
                                {
#if UNITY_EDITOR
                                    // validation only on Editor.
                                    if (fromTail.Current != null)
                                    {
                                        UnityEngine.Debug.LogWarning("MicroCoroutine supports only yield return null. return value = " + coroutine.Current);
                                    }
#endif

                                    // swap
                                    coroutines[i] = fromTail;
                                    coroutines[j] = null;
                                    j--;
                                    goto NEXT_LOOP; // next i
                                }
                            }
                            catch (Exception ex)
                            {
                                coroutines[j] = null;
                                j--;
                                try
                                {
                                    unhandledExceptionCallback(ex);
                                }
                                catch { }
                                continue; // next j
                            }
                        }
                        else
                        {
                            j--;
                        }
                    }

                    tail = i; // loop end
                    break; // LOOP END

                    NEXT_LOOP:
                    continue;
                }


                lock (runningAndQueueLock)
                {
                    running = false;
                    while (waitQueue.Count != 0)
                    {
                        if (coroutines.Length == tail)
                        {
                            Array.Resize(ref coroutines, checked(tail * 2));
                        }
                        coroutines[tail++] = waitQueue.Dequeue();
                    }
                }
            }
        }
    }
}