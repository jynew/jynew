using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Jyx2.InputCore.UI
{
    public class GameSettingInputContext : Jyx2Input_UIContext
    {

        [SerializeField]
        [LabelText("选项卡切换后的默认UI选中对象")]
        private GameObject[] m_TabDefaultSelectObjects;

        [SerializeField]
        private SelectButtonGroup m_TabButtonGroup;

        private GameSettingsPanel m_SettingPanel;

        private void Awake()
        {
            m_TabButtonGroup.OnButtonSelect.AddListener(OnTabButtonSelect);
        }

        private void OnDestroy()
        {
            m_TabButtonGroup.OnButtonSelect.RemoveListener(OnTabButtonSelect);
        }
        
        private void Start()
        {
            m_SettingPanel = GetComponent<GameSettingsPanel>();
        }

        private void OnTabButtonSelect(int idx)
        {
            if (m_TabDefaultSelectObjects == null || m_TabDefaultSelectObjects.Length == 0)
                return;
            if (idx < 0 || idx >= m_TabDefaultSelectObjects.Length)
                return;
            CurrentSelect = m_TabDefaultSelectObjects[idx];
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
