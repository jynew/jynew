// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System; // require keep for Windows Universal App
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservablePointerDownTrigger : ObservableTriggerBase, IEventSystemHandler, IPointerDownHandler
    {
        Subject<PointerEventData> onPointerDown;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (onPointerDown != null) onPointerDown.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnPointerDownAsObservable()
        {
            return onPointerDown ?? (onPointerDown = new Subject<PointerEventData>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onPointerDown != null)
            {
                onPointerDown.OnCompleted();
            }
        }
    }
}


#endif