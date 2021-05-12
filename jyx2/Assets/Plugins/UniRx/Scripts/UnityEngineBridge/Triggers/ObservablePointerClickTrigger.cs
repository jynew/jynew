// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System; // require keep for Windows Universal App
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservablePointerClickTrigger : ObservableTriggerBase, IEventSystemHandler, IPointerClickHandler
    {
        Subject<PointerEventData> onPointerClick;

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (onPointerClick != null) onPointerClick.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnPointerClickAsObservable()
        {
            return onPointerClick ?? (onPointerClick = new Subject<PointerEventData>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onPointerClick != null)
            {
                onPointerClick.OnCompleted();
            }
        }
    }
}


#endif