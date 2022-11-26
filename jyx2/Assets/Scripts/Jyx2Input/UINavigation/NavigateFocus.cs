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
            if(NavigateUtil.IsNavigateInputLastFrame())
            {
                NavigateUtil.TryFocusInScrollRect(this);
            }
#endif
        }
    }
}
