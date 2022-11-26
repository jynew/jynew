using UnityEngine;

namespace Jyx2.InputCore.UI
{
    public class ShopUIInputContext : Jyx2Input_UIContext
    {
        private ShopUIPanel m_ShopUI;

        private void Awake()
        {
            m_ShopUI = GetComponent<ShopUIPanel>();
        }

        public override void OnUpdate()
        {
            if (m_ShopUI == null)
                return;
            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIConfirm))
            {
                m_ShopUI.OnConfirmClick();
            }

            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIClose))
            {
                m_ShopUI.OnCloseClick();
            }
            
            if(Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UISwitchLeft))
            {
                m_ShopUI.CurSelectItem?.OnReduceBtnClick();
            }
            else if(Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UISwitchRight))
            {
                m_ShopUI.CurSelectItem?.OnAddBtnClick();
            }
        }
    }
}
