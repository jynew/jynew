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
            if (_mainMenu == null)
                return;
            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIClose))
            {
                if (!_mainMenu.IsNameInputFocused)
                    _mainMenu.OnBackBtnClicked();
            }
            else if(Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIConfirm))
            {
                if (!_mainMenu.IsNameInputFocused)
                    _mainMenu.OnCreateBtnClicked();
            }
        }
    }
}
