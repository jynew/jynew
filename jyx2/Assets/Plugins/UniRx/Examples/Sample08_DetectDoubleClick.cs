using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UniRx.Examples
{
    public class Sample08_DetectDoubleClick : MonoBehaviour
    {
        void Start()
        {
            // Global event handling is very useful.
            // UniRx can handle there events.
            // Observable.EveryUpdate/EveryFixedUpdate/EveryEndOfFrame
            // Observable.EveryApplicationFocus/EveryApplicationPause
            // Observable.OnceApplicationQuit

            // This DoubleCLick Sample is from
            // The introduction to Reactive Programming you've been missing
            // https://gist.github.com/staltz/868e7e9bc2a7b8c1f754

            var clickStream = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0));

            clickStream.Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(250)))
                .Where(xs => xs.Count >= 2)
                .Subscribe(xs => Debug.Log("DoubleClick Detected! Count:" + xs.Count));
        }
    }
}