using System;
using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;

namespace UniRx
{
    public static partial class DisposableExtensions
    {
        /// <summary>Dispose self on target gameObject has been destroyed. Return value is self disposable.</summary>
        public static T AddTo<T>(this T disposable, GameObject gameObject)
            where T : IDisposable
        {
            if (gameObject == null)
            {
                disposable.Dispose();
                return disposable;
            }

            var trigger = gameObject.GetComponent<ObservableDestroyTrigger>();
            if (trigger == null)
            {
                trigger = gameObject.AddComponent<ObservableDestroyTrigger>();
            }

#pragma warning disable 618

            // If gameObject is deactive, does not raise OnDestroy, watch and invoke trigger.
            if (!trigger.IsActivated && !trigger.IsMonitoredActivate && !trigger.gameObject.activeInHierarchy)
            {
                trigger.IsMonitoredActivate = true;
                MainThreadDispatcher.StartEndOfFrameMicroCoroutine(MonitorTriggerHealth(trigger, gameObject));
            }

#pragma warning restore 618

            trigger.AddDisposableOnDestroy(disposable);
            return disposable;
        }

        static IEnumerator MonitorTriggerHealth(ObservableDestroyTrigger trigger, GameObject targetGameObject)
        {
            while (true)
            {
                yield return null;
                if (trigger.IsActivated) yield break;

                if (targetGameObject == null) // isDestroy
                {
                    trigger.ForceRaiseOnDestroy(); // Force publish OnDestroy
                    yield break;
                }
            }
        }

        /// <summary>Dispose self on target gameObject has been destroyed. Return value is self disposable.</summary>
        public static T AddTo<T>(this T disposable, Component gameObjectComponent)
            where T : IDisposable
        {
            if (gameObjectComponent == null)
            {
                disposable.Dispose();
                return disposable;
            }

            return AddTo(disposable, gameObjectComponent.gameObject);
        }

        /// <summary>
        /// <para>Add disposable(self) to CompositeDisposable(or other ICollection) and Dispose self on target gameObject has been destroyed.</para>
        /// <para>Return value is self disposable.</para>
        /// </summary>
        public static T AddTo<T>(this T disposable, ICollection<IDisposable> container, GameObject gameObject)
            where T : IDisposable
        {
            return disposable.AddTo(container).AddTo(gameObject);
        }

        /// <summary>
        /// <para>Add disposable(self) to CompositeDisposable(or other ICollection) and Dispose self on target gameObject has been destroyed.</para>
        /// <para>Return value is self disposable.</para>
        /// </summary>
        public static T AddTo<T>(this T disposable, ICollection<IDisposable> container, Component gameObjectComponent)
            where T : IDisposable
        {
            return disposable.AddTo(container).AddTo(gameObjectComponent);
        }
    }
}