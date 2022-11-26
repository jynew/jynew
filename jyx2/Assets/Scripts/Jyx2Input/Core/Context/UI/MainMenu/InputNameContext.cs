using UnityEngine;

namespace Jyx2.InputCore.UI
{
    public class InputNameContext : Jyx2Input_UIContext
    {
        private GameMainMenu _mainMenu;

        private void Awake()
        {
            _mainMenu = GetComponentInParent<GameMainMenu>();
        }

        public override void OnUpdate()
        {
            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIClose))
            {
                _mainMenu?.OnBackBtnClicked();
            }
            else if(Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIConfirm))
            {
                _mainMenu?.OnCreateBtnClicked();
            }
        }
    }
}
