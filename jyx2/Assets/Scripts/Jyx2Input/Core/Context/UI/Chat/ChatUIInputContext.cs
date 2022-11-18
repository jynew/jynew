using UnityEngine;

namespace Jyx2.InputCore.UI
{
    public class ChatUIInputContext : Jyx2Input_UIContext
    {
        private ChatUIPanel m_ChatUI;

        private void Awake()
        {
            m_ChatUI = GetComponent<ChatUIPanel>();
        }

        public override void OnUpdate()
        {
            if (m_ChatUI == null)
                return;
        }
    }
}
