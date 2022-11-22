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
            var cheatCommands = new[] { KeyCode.B, KeyCode.A, KeyCode.B, KeyCode.E, KeyCode.R, KeyCode.U, KeyCode.T, KeyCode.H };
            var keys = new HashSet<KeyCode>(cheatCommands);
            var cheatEvent = MakeKeycodeStream(keys).Buffer(cheatCommands.Length, 1)
                                                     .Where(xs => xs.SequenceEqual(cheatCommands));
            cheatEvent.Subscribe(s => {
                _mainMenu.DoGeneratePlayerRole(true);
            }).AddTo(this);
        }

        private IObservable<KeyCode> MakeKeycodeStream(IEnumerable<KeyCode> keys)
        {
            var inputs = keys.Select(key => this.UpdateAsObservable()
                                                .Where(_ => Input.GetKeyDown(key))
                                                .Select(_ => key));
            var result = Observable.Merge(inputs);
#if UNITY_EDITOR  
            result.Subscribe(k => Debug.Log(k))
                   .AddTo(this);
#endif
            return result;
        }

        
        private void InitControllerCheatInput()
        {
            var cheatCommands = new[] { "Up", "Up", "Down", "Down", "Left", "Right", "Left", "Right", "B", "A" , "B", "A" };
            var up = this.UpdateAsObservable().Where(_ => Jyx2_Input.IsUIMoveUp()).Select(_ => "Up");
            var down = this.UpdateAsObservable().Where(_ => Jyx2_Input.IsUIMoveDown()).Select(_ => "Down");
            var left = this.UpdateAsObservable().Where(_ => Jyx2_Input.IsUIMoveLeft()).Select(_ => "Left");
            var right = this.UpdateAsObservable().Where(_ => Jyx2_Input.IsUIMoveRight()).Select(_ => "Right");
            var a = this.UpdateAsObservable().Where(_ => Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UI_Yes)).Select(_ => "A");
            var b = this.UpdateAsObservable().Where(_ => Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UI_No)).Select(_ => "B");
            var inputs = new[] { up, down, left, right, a, b };
            Observable.Merge(inputs)
                      .Buffer(cheatCommands.Length, 1)
                      .Where(xs => xs.SequenceEqual(cheatCommands))
                      .Subscribe(s =>
                      {
                          _mainMenu.DoGeneratePlayerRole(true);
                      }).AddTo(this);
#if UNITY_EDITOR
            foreach(var input in inputs)
            {
                input.Subscribe(k => Debug.Log(k))
                     .AddTo(this);
            }
#endif
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
