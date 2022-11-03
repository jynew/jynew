using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Jyx2.InputCore.UI
{
    public class RandomPropetyInputContext : Jyx2Input_UIContext
    {
        private GameMainMenu _mainMenu;
        
        private void Start()
        {
            _mainMenu = GetComponentInParent<GameMainMenu>();
            InitKeyboardCheatInput();
            InitControllerCheatInput();
        }
        
        private void InitKeyboardCheatInput()
        {
            var cheatCommands = new[] { KeyCode.B, KeyCode.A, KeyCode.B, KeyCode.Y, KeyCode.R, KeyCode.U, KeyCode.T, KeyCode.H };
            var keys = new HashSet<KeyCode>(cheatCommands);
            var cheatEvent = MakeKeycodeStream(keys).Buffer(cheatCommands.Length, 1)
                                                     .Where(xs => xs.SequenceEqual(cheatCommands));
            cheatEvent.Subscribe(s => {
                _mainMenu.DoGeneratePlayerRole(true);
            }).AddTo(this);
        }

        private IObservable<KeyCode> MakeKeycodeStream(IEnumerable<KeyCode> keys)
        {

            var result = Observable.Merge(
                keys.Select(
                            key => this.UpdateAsObservable()
                                       .Where(_ => Input.GetKeyDown(key))
                                       .Select(_ => key)));
#if UNITY_EDITOR  
            result.Subscribe(k => Debug.Log(k))
                   .AddTo(this);
#endif
            return result;
        }

        
        private void InitControllerCheatInput()
        {
            
        }

        public override void OnUpdate()
        {
            if (_mainMenu == null)
                return;
            if(Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIConfirm))
            {
                _mainMenu.OnCreateRoleYesClick();
            }
            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UICancel))
            {
                _mainMenu.OnCreateRoleNoClick();
            }
        }
    }
}
