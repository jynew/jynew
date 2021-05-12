// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System; // require keep for Windows Universal App
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservableScrollTrigger : ObservableTriggerBase, IEventSystemHandler, IScrollHandler
    {
        Subject<PointerEventData> onScroll;

        void IScrollHandler.OnScroll(PointerEventData eventData)
        {
            if (onScroll != null) onScroll.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnScrollAsObservable()
        {
            return onScroll ?? (onScroll = new Subject<PointerEventData>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onScroll != null)
            {
                onScroll.OnCompleted();
            }
        }
    }
}


#endif