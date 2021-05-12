using System;

namespace UniRx.InternalUtil
{
    public class ThreadSafeQueueWorker
    {
        const int MaxArrayLength = 0X7FEFFFFF;
        const int InitialSize = 16;

        object gate = new object();
        bool dequing = false;

        int actionListCount = 0;
        Action<object>[] actionList = new Action<object>[InitialSize];
        object[] actionStates = new object[InitialSize];

        int waitingListCount = 0;
        Action<object>[] waitingList = new Action<object>[InitialSize];
        object[] waitingStates = new object[InitialSize];

        public void Enqueue(Action<object> action, object state)
        {
            lock (gate)
            {
                if (dequing)
                {
                    // Ensure Capacity
                    if (waitingList.Length == waitingListCount)
                    {
                        var newLength = waitingListCount * 2;
                        if ((uint)newLength > MaxArrayLength) newLength = MaxArrayLength;

                        var newArray = new Action<object>[newLength];
                        var newArrayState = new object[newLength];
                        Array.Copy(waitingList, newArray, waitingListCount);
                        Array.Copy(waitingStates, newArrayState, waitingListCount);
                        waitingList = newArray;
                        waitingStates = newArrayState;
                    }
                    waitingList[waitingListCount] = action;
                    waitingStates[waitingListCount] = state;
                    waitingListCount++;
                }
                else
                {
                    // Ensure Capacity
                    if (actionList.Length == actionListCount)
                    {
                        var newLength = actionListCount * 2;
                        if ((uint)newLength > MaxArrayLength) newLength = MaxArrayLength;

                        var newArray = new Action<object>[newLength];
                        var newArrayState = new object[newLength];
                        Array.Copy(actionList, newArray, actionListCount);
                        Array.Copy(actionStates, newArrayState, actionListCount);
                        actionList = newArray;
                        actionStates = newArrayState;
                    }
                    actionList[actionListCount] = action;
                    actionStates[actionListCount] = state;
                    actionListCount++;
                }
            }
        }

        public void ExecuteAll(Action<Exception> unhandledExceptionCallback)
        {
            lock (gate)
            {
                if (actionListCount == 0) return;

                dequing = true;
            }

            for (int i = 0; i < actionListCount; i++)
            {
                var action = actionList[i];
                var state = actionStates[i];
                try
                {
                    action(state);
                }
                catch (Exception ex)
                {
                    unhandledExceptionCallback(ex);
                }
                finally
                {
                    // Clear
                    actionList[i] = null;
                    actionStates[i] = null;
                }
            }

            lock (gate)
            {
                dequing = false;

                var swapTempActionList = actionList;
                var swapTempActionStates = actionStates;

                actionListCount = waitingListCount;
                actionList = waitingList;
                actionStates = waitingStates;

                waitingListCount = 0;
                waitingList = swapTempActionList;
                waitingStates = swapTempActionStates;
            }
        }
    }
}