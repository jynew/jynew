using MOD;
using MOD.UI;
using UnityEngine;

namespace Jyx2.InputCore.UI
{
    public class ModPanelNewInputContext : Jyx2Input_UIContext
    {

        private ModPanelNew _ModPanel;

        private void Awake()
        {
            _ModPanel = GetComponent<ModPanelNew>();
        }

        public override void OnUpdate()
        {
            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIClose))
            {
                _ModPanel?.OnCloseBtnClick();
            }
        }
    }
}
