using UnityEngine;

namespace Jyx2.InputCore.UI
{
    public class SystemUIPanelInputContext : Jyx2Input_UIContext
    {

        public override void OnUpdate()
        {
            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIClose))
            {
                Jyx2_UIManager.Instance.HideUI(nameof(SystemUIPanel));
            }
        }
    }
}
