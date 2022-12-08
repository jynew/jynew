using UnityEngine;

namespace Jyx2.InputCore.UI
{
    public class CommonNoticeInputContext : Jyx2Input_UIContext
    {
        private CommonNoticePanel m_NoticePanel;

        private void Awake()
        {
            m_NoticePanel = GetComponent<CommonNoticePanel>();
        }

        public override void OnUpdate()
        {
            if (m_NoticePanel == null)
                return;
            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIClose))
            {
                m_NoticePanel.OnCloseBtnClick();
            }
        }
    }
}
