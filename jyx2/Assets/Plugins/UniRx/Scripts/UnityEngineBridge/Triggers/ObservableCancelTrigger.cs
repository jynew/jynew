// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System; // require keep for Windows Universal App
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservableCancelTrigger : ObservableTriggerBase, IEventSystemHandler, ICancelHandler
    {
        Subject<BaseEventData> onCancel;

        void ICancelHandler.OnCancel(BaseEventData eventData)
        {
            if (onCancel != null) onCancel.OnNext(eventData);
        }

        public IObservable<BaseEventData> OnCancelAsObservable()
        {
            return onCancel ?? (onCancel = new Subject<BaseEventData>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onCancel != null)
            {
                onCancel.OnCompleted();
            }
        }
    }
}


#endif