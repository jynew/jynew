using System;
using System.Collections;
using UnityEngine;

namespace UniRx.Examples
{
    public class Sample10_MainThreadDispatcher
    {
        public void Run()
        {
            // MainThreadDispatcher is heart of Rx and Unity integration

            // StartCoroutine can start coroutine besides MonoBehaviour.
            MainThreadDispatcher.StartCoroutine(TestAsync());

            // We have two way of run coroutine, FromCoroutine or StartCoroutine.
            // StartCoroutine is Unity primitive way and it's awaitable by yield return.
            // FromCoroutine is Rx, it's composable and cancellable by subscription's IDisposable.
            // FromCoroutine's overload can have return value, see:Sample05_ConvertFromCoroutine
            Observable.FromCoroutine(TestAsync).Subscribe();

            // Add Action to MainThreadDispatcher. Action is saved queue, run on next update.
            MainThreadDispatcher.Post(_ => Debug.Log("test"), null);

            // Timebased operations is run on MainThread(as default)
            // All timebased operation(Interval, Timer, Delay, Buffer, etc...)is single thread, thread safe!
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(x => Debug.Log(x));

            // Observable.Start use ThreadPool Scheduler as default.
            // ObserveOnMainThread return to mainthread
            Observable.Start(() => Unit.Default) // asynchronous work
                .ObserveOnMainThread()
                .Subscribe(x => Debug.Log(x));
        }

        IEnumerator TestAsync()
        {
            Debug.Log("a");
            yield return new WaitForSeconds(1);
            Debug.Log("b");
            yield return new WaitForSeconds(1);
            Debug.Log("c");
            yield return new WaitForSeconds(1);
            Debug.Log("d");
        }
    }
}