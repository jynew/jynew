using UnityEngine;

namespace TapSDK.UI
{
    [System.Serializable]
    public struct BasePanelConfig
    {
        /// <summary>
        /// animation effect related to opening and closing
        /// </summary>
        public EAnimationMode animationType;

        public BasePanelConfig(EAnimationMode animationMode = EAnimationMode.None)
        {
            animationType = animationMode;
        }
    }
}