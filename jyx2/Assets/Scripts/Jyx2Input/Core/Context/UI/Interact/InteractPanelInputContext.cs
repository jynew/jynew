using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace  Jyx2.InputCore.UI
{
    /// <summary>
    /// 这个输入比较特殊 独立于其他输入上下文
    /// </summary>
    public class InteractPanelInputContext: Jyx2Input_UIContext
    {
        private InteractUIPanel m_InteractPanel;

        void Awake()
        {
            m_InteractPanel = GetComponent<InteractUIPanel>();
        }

        protected override void OnEnable()
        {
            //if(IsPlayerControlable)
                CurrentSelect = null;
        }

        protected override void OnDisable()
        {
            
        }


        bool IsPlayerControlable
        {
            get
            {
                var player = LevelMaster.Instance?.GetPlayer();
                if (player == null)
                    return false;
                return player.CanControlPlayer;
            }
        }

        public override void OnUpdate()
        {
            if (m_InteractPanel == null)
                return;
            if (!IsPlayerControlable)
                return;
            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.Interact1) && m_InteractPanel.IsButtonAvailable(0))
            {
                m_InteractPanel.OnBtnClick(0);
            }
            else if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.Interact2) && m_InteractPanel.IsButtonAvailable(1))
            {
                m_InteractPanel.OnBtnClick(1);
            }
        }

        void Update()
        {
            OnUpdate();
        }
    }
}
