using UnityEngine;

namespace Jyx2.InputCore.UI
{
    public class GameSettingInputContext : Jyx2Input_UIContext
    {
        private GameSettingsPanel m_SettingPanel;
        
        private void Start()
        {
            m_SettingPanel = GetComponent<GameSettingsPanel>();
        }

        public override void OnUpdate()
        {
            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIClose))
            {
                m_SettingPanel?.OnCloseBtnClick();
            }
            if(Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UISwitchLeft))
            {
                m_SettingPanel.TabLeft();
            }
            else if(Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UISwitchRight))
            {
                m_SettingPanel.TabRight();
            }
        }
    }
}
