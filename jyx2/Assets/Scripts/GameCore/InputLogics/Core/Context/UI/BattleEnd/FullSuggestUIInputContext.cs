using UnityEngine;

namespace Jyx2.InputCore.UI
{
    public class FullSuggestUIInputContext : Jyx2Input_UIContext
    {
        private FullSuggestUIPanel m_SuggestUI;

        private void Awake()
        {
            m_SuggestUI = GetComponent<FullSuggestUIPanel>();
        }

        public override void OnUpdate()
        {
            if (m_SuggestUI == null)
                return;
            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UICancel))
            {
                m_SuggestUI.OnBgClick();
            }
        }
    }
}
