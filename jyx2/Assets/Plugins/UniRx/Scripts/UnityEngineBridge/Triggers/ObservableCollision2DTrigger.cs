using System; // require keep for Windows Universal App
using UnityEngine;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservableCollision2DTrigger : ObservableTriggerBase
    {
        Subject<Collision2D> onCollisionEnter2D;

        /// <summary>Sent when an incoming collider makes contact with this object's collider (2D physics only).</summary>
         void OnCollisionEnter2D(Collision2D coll)
        {
            if (onCollisionEnter2D != null) onCollisionEnter2D.OnNext(coll);
        }

        /// <summary>Sent when an incoming collider makes contact with this object's collider (2D physics only).</summary>
        public IObservable<Collision2D> OnCollisionEnter2DAsObservable()
        {
            return onCollisionEnter2D ?? (onCollisionEnter2D = new Subject<Collision2D>());
        }

        Subject<Collision2D> onCollisionExit2D;

        /// <summary>Sent when a collider on another object stops touching this object's collider (2D physics only).</summary>
         void OnCollisionExit2D(Collision2D coll)
        {
            if (onCollisionExit2D != null) onCollisionExit2D.OnNext(coll);
        }

        /// <summary>Sent when a collider on another object stops touching this object's collider (2D physics only).</summary>
        public IObservable<Collision2D> OnCollisionExit2DAsObservable()
        {
            return onCollisionExit2D ?? (onCollisionExit2D = new Subject<Collision2D>());
        }

        Subject<Collision2D> onCollisionStay2D;

        /// <summary>Sent each frame where a collider on another object is touching this object's collider (2D physics only).</summary>
         void OnCollisionStay2D(Collision2D coll)
        {
            if (onCollisionStay2D != null) onCollisionStay2D.OnNext(coll);
        }

        /// <summary>Sent each frame where a collider on another object is touching this object's collider (2D physics only).</summary>
        public IObservable<Collision2D> OnCollisionStay2DAsObservable()
        {
            return onCollisionStay2D ?? (onCollisionStay2D = new Subject<Collision2D>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onCollisionEnter2D != null)
            {
                onCollisionEnter2D.OnCompleted();
            }
            if (onCollisionExit2D != null)
            {
                onCollisionExit2D.OnCompleted();
            }
            if (onCollisionStay2D != null)
            {
                onCollisionStay2D.OnCompleted();
            }
        }
    }
}