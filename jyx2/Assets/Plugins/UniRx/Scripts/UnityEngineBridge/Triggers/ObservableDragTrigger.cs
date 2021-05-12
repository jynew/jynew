// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System; // require keep for Windows Universal App
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservableDragTrigger : ObservableTriggerBase, IEventSystemHandler, IDragHandler
    {
        Subject<PointerEventData> onDrag;

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (onDrag != null) onDrag.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnDragAsObservable()
        {
            return onDrag ?? (onDrag = new Subject<PointerEventData>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onDrag != null)
            {
                onDrag.OnCompleted();
            }
        }
    }
}


#endif