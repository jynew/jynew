// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System; // require keep for Windows Universal App
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservableMoveTrigger : ObservableTriggerBase, IEventSystemHandler, IMoveHandler
    {
        Subject<AxisEventData> onMove;

        void IMoveHandler.OnMove(AxisEventData eventData)
        {
            if (onMove != null) onMove.OnNext(eventData);
        }

        public IObservable<AxisEventData> OnMoveAsObservable()
        {
            return onMove ?? (onMove = new Subject<AxisEventData>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onMove != null)
            {
                onMove.OnCompleted();
            }
        }
    }
}


#endif