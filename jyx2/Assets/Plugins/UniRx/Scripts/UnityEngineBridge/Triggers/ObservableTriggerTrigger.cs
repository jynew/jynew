using System; // require keep for Windows Universal App
using UnityEngine;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservableTriggerTrigger : ObservableTriggerBase
    {
        Subject<Collider> onTriggerEnter;

        /// <summary>OnTriggerEnter is called when the Collider other enters the trigger.</summary>
        void OnTriggerEnter(Collider other)
        {
            if (onTriggerEnter != null) onTriggerEnter.OnNext(other);
        }

        /// <summary>OnTriggerEnter is called when the Collider other enters the trigger.</summary>
        public IObservable<Collider> OnTriggerEnterAsObservable()
        {
            return onTriggerEnter ?? (onTriggerEnter = new Subject<Collider>());
        }

        Subject<Collider> onTriggerExit;

        /// <summary>OnTriggerExit is called when the Collider other has stopped touching the trigger.</summary>
        void OnTriggerExit(Collider other)
        {
            if (onTriggerExit != null) onTriggerExit.OnNext(other);
        }

        /// <summary>OnTriggerExit is called when the Collider other has stopped touching the trigger.</summary>
        public IObservable<Collider> OnTriggerExitAsObservable()
        {
            return onTriggerExit ?? (onTriggerExit = new Subject<Collider>());
        }

        Subject<Collider> onTriggerStay;

        /// <summary>OnTriggerStay is called once per frame for every Collider other that is touching the trigger.</summary>
        void OnTriggerStay(Collider other)
        {
            if (onTriggerStay != null) onTriggerStay.OnNext(other);
        }

        /// <summary>OnTriggerStay is called once per frame for every Collider other that is touching the trigger.</summary>
        public IObservable<Collider> OnTriggerStayAsObservable()
        {
            return onTriggerStay ?? (onTriggerStay = new Subject<Collider>());
        }
        
        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onTriggerEnter != null)
            {
                onTriggerEnter.OnCompleted();
            }
            if (onTriggerExit != null)
            {
                onTriggerExit.OnCompleted();
            }
            if (onTriggerStay != null)
            {
                onTriggerStay.OnCompleted();
            }
        }
    }
}