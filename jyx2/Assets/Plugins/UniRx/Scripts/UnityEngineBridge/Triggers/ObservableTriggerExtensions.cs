using System; // require keep for Windows Universal App
using UnityEngine;

namespace UniRx.Triggers
{
    // for GameObject
    public static partial class ObservableTriggerExtensions
    {
        #region ObservableAnimatorTrigger

        /// <summary>Callback for setting up animation IK (inverse kinematics).</summary>
        public static IObservable<int> OnAnimatorIKAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<int>();
            return GetOrAddComponent<ObservableAnimatorTrigger>(gameObject).OnAnimatorIKAsObservable();
        }

        /// <summary>Callback for processing animation movements for modifying root motion.</summary>
        public static IObservable<Unit> OnAnimatorMoveAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableAnimatorTrigger>(gameObject).OnAnimatorMoveAsObservable();
        }

        #endregion

        #region ObservableCollision2DTrigger

        /// <summary>Sent when an incoming collider makes contact with this object's collider (2D physics only).</summary>
        public static IObservable<Collision2D> OnCollisionEnter2DAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Collision2D>();
            return GetOrAddComponent<ObservableCollision2DTrigger>(gameObject).OnCollisionEnter2DAsObservable();
        }


        /// <summary>Sent when a collider on another object stops touching this object's collider (2D physics only).</summary>
        public static IObservable<Collision2D> OnCollisionExit2DAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Collision2D>();
            return GetOrAddComponent<ObservableCollision2DTrigger>(gameObject).OnCollisionExit2DAsObservable();
        }

        /// <summary>Sent each frame where a collider on another object is touching this object's collider (2D physics only).</summary>
        public static IObservable<Collision2D> OnCollisionStay2DAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Collision2D>();
            return GetOrAddComponent<ObservableCollision2DTrigger>(gameObject).OnCollisionStay2DAsObservable();
        }

        #endregion

        #region ObservableCollisionTrigger

        /// <summary>OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.</summary>
        public static IObservable<Collision> OnCollisionEnterAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Collision>();
            return GetOrAddComponent<ObservableCollisionTrigger>(gameObject).OnCollisionEnterAsObservable();
        }


        /// <summary>OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.</summary>
        public static IObservable<Collision> OnCollisionExitAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Collision>();
            return GetOrAddComponent<ObservableCollisionTrigger>(gameObject).OnCollisionExitAsObservable();
        }

        /// <summary>OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider.</summary>
        public static IObservable<Collision> OnCollisionStayAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Collision>();
            return GetOrAddComponent<ObservableCollisionTrigger>(gameObject).OnCollisionStayAsObservable();
        }

        #endregion

        #region ObservableDestroyTrigger

        /// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
        public static IObservable<Unit> OnDestroyAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Return(Unit.Default); // send destroy message
            return GetOrAddComponent<ObservableDestroyTrigger>(gameObject).OnDestroyAsObservable();
        }

        #endregion


        #region ObservableEnableTrigger

        /// <summary>This function is called when the object becomes enabled and active.</summary>
        public static IObservable<Unit> OnEnableAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableEnableTrigger>(gameObject).OnEnableAsObservable();
        }

        /// <summary>This function is called when the behaviour becomes disabled () or inactive.</summary>
        public static IObservable<Unit> OnDisableAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableEnableTrigger>(gameObject).OnDisableAsObservable();
        }

        #endregion

        #region ObservableFixedUpdateTrigger

        /// <summary>This function is called every fixed framerate frame, if the MonoBehaviour is enabled.</summary>
        public static IObservable<Unit> FixedUpdateAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableFixedUpdateTrigger>(gameObject).FixedUpdateAsObservable();
        }

        #endregion

        #region ObservableLateUpdateTrigger

        /// <summary>LateUpdate is called every frame, if the Behaviour is enabled.</summary>
        public static IObservable<Unit> LateUpdateAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableLateUpdateTrigger>(gameObject).LateUpdateAsObservable();
        }

        #endregion

#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

        #region ObservableMouseTrigger

        /// <summary>OnMouseDown is called when the user has pressed the mouse button while over the GUIElement or Collider.</summary>
        public static IObservable<Unit> OnMouseDownAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableMouseTrigger>(gameObject).OnMouseDownAsObservable();
        }

        /// <summary>OnMouseDrag is called when the user has clicked on a GUIElement or Collider and is still holding down the mouse.</summary>
        public static IObservable<Unit> OnMouseDragAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableMouseTrigger>(gameObject).OnMouseDragAsObservable();
        }

        /// <summary>OnMouseEnter is called when the mouse entered the GUIElement or Collider.</summary>
        public static IObservable<Unit> OnMouseEnterAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableMouseTrigger>(gameObject).OnMouseEnterAsObservable();
        }

        /// <summary>OnMouseExit is called when the mouse is not any longer over the GUIElement or Collider.</summary>
        public static IObservable<Unit> OnMouseExitAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableMouseTrigger>(gameObject).OnMouseExitAsObservable();
        }

        /// <summary>OnMouseOver is called every frame while the mouse is over the GUIElement or Collider.</summary>
        public static IObservable<Unit> OnMouseOverAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableMouseTrigger>(gameObject).OnMouseOverAsObservable();
        }

        /// <summary>OnMouseUp is called when the user has released the mouse button.</summary>
        public static IObservable<Unit> OnMouseUpAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableMouseTrigger>(gameObject).OnMouseUpAsObservable();
        }

        /// <summary>OnMouseUpAsButton is only called when the mouse is released over the same GUIElement or Collider as it was pressed.</summary>
        public static IObservable<Unit> OnMouseUpAsButtonAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableMouseTrigger>(gameObject).OnMouseUpAsButtonAsObservable();
        }

        #endregion

#endif

        #region ObservableTrigger2DTrigger

        /// <summary>Sent when another object enters a trigger collider attached to this object (2D physics only).</summary>
        public static IObservable<Collider2D> OnTriggerEnter2DAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Collider2D>();
            return GetOrAddComponent<ObservableTrigger2DTrigger>(gameObject).OnTriggerEnter2DAsObservable();
        }


        /// <summary>Sent when another object leaves a trigger collider attached to this object (2D physics only).</summary>
        public static IObservable<Collider2D> OnTriggerExit2DAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Collider2D>();
            return GetOrAddComponent<ObservableTrigger2DTrigger>(gameObject).OnTriggerExit2DAsObservable();
        }

        /// <summary>Sent each frame where another object is within a trigger collider attached to this object (2D physics only).</summary>
        public static IObservable<Collider2D> OnTriggerStay2DAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Collider2D>();
            return GetOrAddComponent<ObservableTrigger2DTrigger>(gameObject).OnTriggerStay2DAsObservable();
        }

        #endregion

        #region ObservableTriggerTrigger

        /// <summary>OnTriggerEnter is called when the Collider other enters the trigger.</summary>
        public static IObservable<Collider> OnTriggerEnterAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Collider>();
            return GetOrAddComponent<ObservableTriggerTrigger>(gameObject).OnTriggerEnterAsObservable();
        }


        /// <summary>OnTriggerExit is called when the Collider other has stopped touching the trigger.</summary>
        public static IObservable<Collider> OnTriggerExitAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Collider>();
            return GetOrAddComponent<ObservableTriggerTrigger>(gameObject).OnTriggerExitAsObservable();
        }

        /// <summary>OnTriggerStay is called once per frame for every Collider other that is touching the trigger.</summary>
        public static IObservable<Collider> OnTriggerStayAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Collider>();
            return GetOrAddComponent<ObservableTriggerTrigger>(gameObject).OnTriggerStayAsObservable();
        }

        #endregion

        #region ObservableUpdateTrigger

        /// <summary>Update is called every frame, if the MonoBehaviour is enabled.</summary>
        public static IObservable<Unit> UpdateAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableUpdateTrigger>(gameObject).UpdateAsObservable();
        }

        #endregion

        #region ObservableVisibleTrigger

        /// <summary>OnBecameInvisible is called when the renderer is no longer visible by any camera.</summary>
        public static IObservable<Unit> OnBecameInvisibleAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableVisibleTrigger>(gameObject).OnBecameInvisibleAsObservable();
        }

        /// <summary>OnBecameVisible is called when the renderer became visible by any camera.</summary>
        public static IObservable<Unit> OnBecameVisibleAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableVisibleTrigger>(gameObject).OnBecameVisibleAsObservable();
        }

        #endregion

#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

        #region ObservableTransformChangedTrigger

        /// <summary>Callback sent to the graphic before a Transform parent change occurs.</summary>
        public static IObservable<Unit> OnBeforeTransformParentChangedAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableTransformChangedTrigger>(gameObject).OnBeforeTransformParentChangedAsObservable();
        }

        /// <summary>This function is called when the parent property of the transform of the GameObject has changed.</summary>
        public static IObservable<Unit> OnTransformParentChangedAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableTransformChangedTrigger>(gameObject).OnTransformParentChangedAsObservable();
        }

        /// <summary>This function is called when the list of children of the transform of the GameObject has changed.</summary>
        public static IObservable<Unit> OnTransformChildrenChangedAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableTransformChangedTrigger>(gameObject).OnTransformChildrenChangedAsObservable();
        }

        #endregion

        #region ObservableCanvasGroupChangedTrigger

        /// <summary>Callback that is sent if the canvas group is changed.</summary>
        public static IObservable<Unit> OnCanvasGroupChangedAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableCanvasGroupChangedTrigger>(gameObject).OnCanvasGroupChangedAsObservable();
        }

        #endregion

        #region ObservableRectTransformTrigger

        /// <summary>Callback that is sent if an associated RectTransform has it's dimensions changed.</summary>
        public static IObservable<Unit> OnRectTransformDimensionsChangeAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableRectTransformTrigger>(gameObject).OnRectTransformDimensionsChangeAsObservable();
        }

        /// <summary>Callback that is sent if an associated RectTransform is removed.</summary>
        public static IObservable<Unit> OnRectTransformRemovedAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableRectTransformTrigger>(gameObject).OnRectTransformRemovedAsObservable();
        }

        #endregion
#endif

        #region ObservableParticleTrigger

        /// <summary>OnParticleCollision is called when a particle hits a collider.</summary>
        public static IObservable<GameObject> OnParticleCollisionAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<GameObject>();
            return GetOrAddComponent<ObservableParticleTrigger>(gameObject).OnParticleCollisionAsObservable();
        }

#if UNITY_5_4_OR_NEWER

        /// <summary>OnParticleTrigger is called when any particles in a particle system meet the conditions in the trigger module.</summary>
        public static IObservable<Unit> OnParticleTriggerAsObservable(this GameObject gameObject)
        {
            if (gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableParticleTrigger>(gameObject).OnParticleTriggerAsObservable();
        }

#endif

        #endregion


        static T GetOrAddComponent<T>(GameObject gameObject)
            where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }
}
