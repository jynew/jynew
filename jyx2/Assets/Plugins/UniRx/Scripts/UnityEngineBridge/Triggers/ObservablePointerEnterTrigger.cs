// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System; // require keep for Windows Universal App
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservablePointerEnterTrigger : ObservableTriggerBase, IEventSystemHandler, IPointerEnterHandler
    {
        Subject<PointerEventData> onPointerEnter;

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (onPointerEnter != null) onPointerEnter.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnPointerEnterAsObservable()
        {
            return onPointerEnter ?? (onPointerEnter = new Subject<PointerEventData>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onPointerEnter != null)
            {
                onPointerEnter.OnCompleted();
            }
        }
    }
}


#endif