using UnityEngine;

namespace Jyx2.InputCore.UI
{
    public class SavePanelInputContext : Jyx2Input_UIContext
    {
        private SavePanel m_SavePanel;

        private void Start()
        {
            m_SavePanel = GetComponent<SavePanel>();
        }

        public override void OnUpdate()
        {
            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIClose))
            {
                m_SavePanel?.OnBackClick();
            }
        }
    }
}
