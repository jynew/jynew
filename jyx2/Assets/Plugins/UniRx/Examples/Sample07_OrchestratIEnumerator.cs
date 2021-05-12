#pragma warning disable 0168
#pragma warning disable 0219

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UniRx.Examples
{
    public class Sample07_OrchestratIEnumerator : MonoBehaviour
    {
        // two coroutines
        IEnumerator AsyncA()
        {
            Debug.Log("a start");
            yield return new WaitForSeconds(3);
            Debug.Log("a end");
        }

        IEnumerator AsyncB()
        {
            Debug.Log("b start");
            yield return new WaitForEndOfFrame();
            Debug.Log("b end");
        }

        void Start()
        {
            // after completed AsyncA, run AsyncB as continuous routine.
            // UniRx expands SelectMany(IEnumerator) as SelectMany(IEnumerator.ToObservable())
            var cancel = Observable.FromCoroutine(AsyncA)
                .SelectMany(AsyncB)
                .Subscribe();

            // If you want to stop Coroutine(as cancel), call subscription.Dispose()
            // cancel.Dispose();
        }
    }
}

#pragma warning restore 0219
#pragma warning restore 0168