using Jyx2.InputCore;
using Jyx2.MOD;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Jyx2.UINavigation
{
    public class NavigateFocus : MonoBehaviour, ISelectHandler
    {
        public void OnSelect(BaseEventData eventData)
        {
#if UNITY_ANDROID || UNITY_IPHONE
            return;
#else
            if(IsNavigateInputLastFrame())
            {
                NavigateUtil.TryFocusInScrollRect(this);
            }
#endif
        }

        private bool IsNavigateInputLastFrame()
        {
            var controller = Jyx2_Input.GetLastActiveController();
            if (controller == null)
                return false;
            if (!controller.isConnected)
                return false;
            if (controller.type == Rewired.ControllerType.Mouse)
                return false;
            return true;
        }
    }
}
