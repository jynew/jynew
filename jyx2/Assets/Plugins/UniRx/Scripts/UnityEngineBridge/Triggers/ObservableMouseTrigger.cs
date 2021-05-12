#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

using System; // require keep for Windows Universal App
using UnityEngine;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservableMouseTrigger : ObservableTriggerBase
    {
        Subject<Unit> onMouseDown;

        /// <summary>OnMouseDown is called when the user has pressed the mouse button while over the GUIElement or Collider.</summary>
         void OnMouseDown()
        {
            if (onMouseDown != null) onMouseDown.OnNext(Unit.Default);
        }

        /// <summary>OnMouseDown is called when the user has pressed the mouse button while over the GUIElement or Collider.</summary>
        public IObservable<Unit> OnMouseDownAsObservable()
        {
            return onMouseDown ?? (onMouseDown = new Subject<Unit>());
        }

        Subject<Unit> onMouseDrag;

        /// <summary>OnMouseDrag is called when the user has clicked on a GUIElement or Collider and is still holding down the mouse.</summary>
         void OnMouseDrag()
        {
            if (onMouseDrag != null) onMouseDrag.OnNext(Unit.Default);
        }

        /// <summary>OnMouseDrag is called when the user has clicked on a GUIElement or Collider and is still holding down the mouse.</summary>
        public IObservable<Unit> OnMouseDragAsObservable()
        {
            return onMouseDrag ?? (onMouseDrag = new Subject<Unit>());
        }

        Subject<Unit> onMouseEnter;

        /// <summary>OnMouseEnter is called when the mouse entered the GUIElement or Collider.</summary>
         void OnMouseEnter()
        {
            if (onMouseEnter != null) onMouseEnter.OnNext(Unit.Default);
        }

        /// <summary>OnMouseEnter is called when the mouse entered the GUIElement or Collider.</summary>
        public IObservable<Unit> OnMouseEnterAsObservable()
        {
            return onMouseEnter ?? (onMouseEnter = new Subject<Unit>());
        }

        Subject<Unit> onMouseExit;

        /// <summary>OnMouseExit is called when the mouse is not any longer over the GUIElement or Collider.</summary>
         void OnMouseExit()
        {
            if (onMouseExit != null) onMouseExit.OnNext(Unit.Default);
        }

        /// <summary>OnMouseExit is called when the mouse is not any longer over the GUIElement or Collider.</summary>
        public IObservable<Unit> OnMouseExitAsObservable()
        {
            return onMouseExit ?? (onMouseExit = new Subject<Unit>());
        }

        Subject<Unit> onMouseOver;

        /// <summary>OnMouseOver is called every frame while the mouse is over the GUIElement or Collider.</summary>
         void OnMouseOver()
        {
            if (onMouseOver != null) onMouseOver.OnNext(Unit.Default);
        }

        /// <summary>OnMouseOver is called every frame while the mouse is over the GUIElement or Collider.</summary>
        public IObservable<Unit> OnMouseOverAsObservable()
        {
            return onMouseOver ?? (onMouseOver = new Subject<Unit>());
        }

        Subject<Unit> onMouseUp;

        /// <summary>OnMouseUp is called when the user has released the mouse button.</summary>
         void OnMouseUp()
        {
            if (onMouseUp != null) onMouseUp.OnNext(Unit.Default);
        }

        /// <summary>OnMouseUp is called when the user has released the mouse button.</summary>
        public IObservable<Unit> OnMouseUpAsObservable()
        {
            return onMouseUp ?? (onMouseUp = new Subject<Unit>());
        }

        Subject<Unit> onMouseUpAsButton;

        /// <summary>OnMouseUpAsButton is only called when the mouse is released over the same GUIElement or Collider as it was pressed.</summary>
         void OnMouseUpAsButton()
        {
            if (onMouseUpAsButton != null) onMouseUpAsButton.OnNext(Unit.Default);
        }

        /// <summary>OnMouseUpAsButton is only called when the mouse is released over the same GUIElement or Collider as it was pressed.</summary>
        public IObservable<Unit> OnMouseUpAsButtonAsObservable()
        {
            return onMouseUpAsButton ?? (onMouseUpAsButton = new Subject<Unit>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onMouseDown != null)
            {
                onMouseDown.OnCompleted();
            }
            if (onMouseDrag != null)
            {
                onMouseDrag.OnCompleted();
            }
            if (onMouseEnter != null)
            {
                onMouseEnter.OnCompleted();
            }
            if (onMouseExit != null)
            {
                onMouseExit.OnCompleted();
            }
            if (onMouseOver != null)
            {
                onMouseOver.OnCompleted();
            }
            if (onMouseUp != null)
            {
                onMouseUp.OnCompleted();
            }
            if (onMouseUpAsButton != null)
            {
                onMouseUpAsButton.OnCompleted();
            }
        }
    }
}

#endif