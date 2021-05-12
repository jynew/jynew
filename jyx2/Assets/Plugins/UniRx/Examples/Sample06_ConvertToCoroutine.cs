using System;
using System.Collections;
using UnityEngine;

namespace UniRx.Examples
{
    public class Sample06_ConvertToCoroutine : MonoBehaviour
    {
        // convert IObservable to Coroutine
        void Start()
        {
            StartCoroutine(ComplexCoroutineTest());
        }

        IEnumerator ComplexCoroutineTest()
        {
            yield return new WaitForSeconds(1);

            var v = default(int);
            yield return Observable.Range(1, 10).StartAsCoroutine(x => v = x);

            Debug.Log(v); // 10(callback is last value)
            yield return new WaitForSeconds(3);

            yield return Observable.Return(100).StartAsCoroutine(x => v = x);

            Debug.Log(v); // 100
        }

        // Note:ToAwaitableEnumerator/StartAsCoroutine/LazyTask are obsolete way on Unity 5.3
        // You can use ToYieldInstruction.

#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
#if UNITY_2018_3_OR_NEWER
#pragma warning disable CS0618
#endif

        IEnumerator TestNewCustomYieldInstruction()
        {
            // wait Rx Observable.
            yield return Observable.Timer(TimeSpan.FromSeconds(1)).ToYieldInstruction();

            // you can change the scheduler(this is ignore Time.scale)
            yield return Observable.Timer(TimeSpan.FromSeconds(1), Scheduler.MainThreadIgnoreTimeScale).ToYieldInstruction();

            // get return value from ObservableYieldInstruction
            var o = ObservableWWW.Get("http://unity3d.com/").ToYieldInstruction(throwOnError: false);
            yield return o;

            if (o.HasError) { Debug.Log(o.Error.ToString()); }
            if (o.HasResult) { Debug.Log(o.Result); }

            // other sample(wait until transform.position.y >= 100) 
            yield return this.ObserveEveryValueChanged(x => x.transform).FirstOrDefault(x => x.position.y >= 100).ToYieldInstruction();
        }
#if UNITY_2018_3_OR_NEWER
#pragma warning restore CS0618
#endif
#endif

    }
}