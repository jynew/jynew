using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jyx2
{
    public interface ISettingChildPanel
    {
        void ApplySetting();

        void SetVisibility(bool isVisible);
    }
    

    public class GameSettingsPanel : Jyx2_UIBase
    {
        [SerializeField]
        private Jyx2_UIBase[] m_ChildPanels = new Jyx2_UIBase[0];

        [SerializeField]
        private Button m_CloseBtn;

        [SerializeField]
        private SelectButtonGroup m_BtnGroup;

        // Start is called before the first frame update
        void Start()
        {
            m_CloseBtn.onClick.AddListener(OnCloseBtnClick);
            m_BtnGroup.OnButtonSelect.AddListener(OnTabButtonSelect);
            m_BtnGroup.SelectIndex(0);
        }

        private void OnDestroy()
        {
            m_CloseBtn.onClick.RemoveListener(OnCloseBtnClick);
            m_BtnGroup.OnButtonSelect.RemoveListener(OnTabButtonSelect);
        }

        protected override void OnCreate()
        {

        }

        public void OnCloseBtnClick()
        {
            foreach(var panel in m_ChildPanels)
            {
                if(panel is ISettingChildPanel settingPanel)
                {
                    settingPanel.ApplySetting();
                }
            }
            Jyx2_UIManager.Instance.HideUI(nameof(GameSettingsPanel));
        }

        private void OnTabButtonSelect(int idx)
        {
            for(int i = 0; i< m_ChildPanels.Length; ++i)
            {
                if (m_ChildPanels[i] is ISettingChildPanel settingPanel)
                {
                    settingPanel.SetVisibility(idx == i);
                }
            }
        }

        public void TabLeft()
        {
            m_BtnGroup.SelectIndex(m_BtnGroup.CurButtonIndex - 1);
        }

        public void TabRight()
        {
            m_BtnGroup.SelectIndex(m_BtnGroup.CurButtonIndex + 1);
        }
    }
}
