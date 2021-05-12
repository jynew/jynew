// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System; // require keep for Windows Universal App
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservablePointerUpTrigger : ObservableTriggerBase, IEventSystemHandler, IPointerUpHandler
    {
        Subject<PointerEventData> onPointerUp;

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (onPointerUp != null) onPointerUp.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnPointerUpAsObservable()
        {
            return onPointerUp ?? (onPointerUp = new Subject<PointerEventData>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onPointerUp != null)
            {
                onPointerUp.OnCompleted();
            }
        }
    }
}


#endif