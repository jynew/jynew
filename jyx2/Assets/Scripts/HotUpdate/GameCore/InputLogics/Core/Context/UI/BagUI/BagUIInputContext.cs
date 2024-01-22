using UnityEngine;

namespace Jyx2.InputCore.UI
{
    public class BagUIInputContext : Jyx2Input_UIContext
    {
        private BagUIPanel m_BagUI;

        private void Awake()
        {
            m_BagUI = GetComponent<BagUIPanel>();
        }

        public override void OnUpdate()
        {
            if (m_BagUI == null)
                return;

            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIClose))
            {
                m_BagUI.OnCloseBtnClick();
            }

            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIConfirm))
            {
                if(m_BagUI.IsUseButtonActive)
                    m_BagUI.OnUseBtnClick();
            }

            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UISwitchLeft))
            {
                m_BagUI.TabLeft();
            }
            else if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UISwitchRight))
            {
                m_BagUI.TabRight();
            }
        }
    }
}
