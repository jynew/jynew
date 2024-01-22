using UnityEngine;

namespace Jyx2.InputCore.UI
{
    public class SelectRolePanelInputContext : Jyx2Input_UIContext
    {
        private SelectRolePanel m_SelectRolePanel;

        private void Awake()
        {
            m_SelectRolePanel = GetComponent<SelectRolePanel>();
        }

        public override void OnUpdate()
        {
            if (m_SelectRolePanel == null)
                return;
            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIConfirm))
            {
                TrySwitchCurrentRoleItem();
            }

            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIClose))
            {
                if(m_SelectRolePanel.IsCancelBtnEnable)
                    m_SelectRolePanel.OnCancelClick();
            }

            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UI_Yes))
            {
                if (m_SelectRolePanel.IsAllSelectBtnEnable)
                    m_SelectRolePanel.OnAllClick();
            }
        }

        private void TrySwitchCurrentRoleItem()
        {
            var curSelect = CurrentSelect;
            if (curSelect == null)
                return;
            var roleItem = curSelect.GetComponent<RoleUIItem>();
            if (roleItem == null)
                return;
            roleItem.SetState(!roleItem.IsSelected);
        }
    }
}
