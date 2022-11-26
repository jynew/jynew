using UnityEngine;

namespace Jyx2.InputCore.UI
{
    public class XiakeUIInputContext : Jyx2Input_UIContext
    {
        private XiakeUIPanel m_XiakeUI;

        private void Awake()
        {
            m_XiakeUI = GetComponent<XiakeUIPanel>();
        }

        public override void OnUpdate()
        {
            if (m_XiakeUI == null)
                return;

            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIClose))
            {
                m_XiakeUI.OnBackClick();
            }

            if(Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UISwitchLeft))
            {
                m_XiakeUI.TabLeft();
            }
            else if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UISwitchRight))
            {
                m_XiakeUI.TabRight();
            }
        }
    }
}
