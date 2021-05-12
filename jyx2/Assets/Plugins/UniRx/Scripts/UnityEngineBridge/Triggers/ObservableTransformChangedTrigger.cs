// after uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System;
using UnityEngine;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservableTransformChangedTrigger : ObservableTriggerBase
    {
        Subject<Unit> onBeforeTransformParentChanged;

        // Callback sent to the graphic before a Transform parent change occurs
        void OnBeforeTransformParentChanged()
        {
            if (onBeforeTransformParentChanged != null) onBeforeTransformParentChanged.OnNext(Unit.Default);
        }

        /// <summary>Callback sent to the graphic before a Transform parent change occurs.</summary>
        public IObservable<Unit> OnBeforeTransformParentChangedAsObservable()
        {
            return onBeforeTransformParentChanged ?? (onBeforeTransformParentChanged = new Subject<Unit>());
        }

        Subject<Unit> onTransformParentChanged;

        // This function is called when the parent property of the transform of the GameObject has changed
        void OnTransformParentChanged()
        {
            if (onTransformParentChanged != null) onTransformParentChanged.OnNext(Unit.Default);
        }

        /// <summary>This function is called when the parent property of the transform of the GameObject has changed.</summary>
        public IObservable<Unit> OnTransformParentChangedAsObservable()
        {
            return onTransformParentChanged ?? (onTransformParentChanged = new Subject<Unit>());
        }

        Subject<Unit> onTransformChildrenChanged;

        // This function is called when the list of children of the transform of the GameObject has changed
        void OnTransformChildrenChanged()
        {
            if (onTransformChildrenChanged != null) onTransformChildrenChanged.OnNext(Unit.Default);
        }

        /// <summary>This function is called when the list of children of the transform of the GameObject has changed.</summary>
        public IObservable<Unit> OnTransformChildrenChangedAsObservable()
        {
            return onTransformChildrenChanged ?? (onTransformChildrenChanged = new Subject<Unit>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onBeforeTransformParentChanged != null)
            {
                onBeforeTransformParentChanged.OnCompleted();
            }
            if (onTransformParentChanged != null)
            {
                onTransformParentChanged.OnCompleted();
            }
            if (onTransformChildrenChanged != null)
            {
                onTransformChildrenChanged.OnCompleted();
            }
        }
    }
}

#endif