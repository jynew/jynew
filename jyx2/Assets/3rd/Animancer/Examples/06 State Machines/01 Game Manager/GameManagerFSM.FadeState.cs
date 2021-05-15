// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Animancer.Examples.StateMachines.GameManager
{
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.GameManager/GameManagerFSM
    /// 
    partial class GameManagerFSM
    {
        /************************************************************************************************************************/

        [SerializeField] private FadeState _Fade;

        /************************************************************************************************************************/

        /// <summary>Fading the screen to black then resetting the ball and fading back in.</summary>
        /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.GameManager/FadeState
        /// 
        [Serializable]
        public sealed class FadeState : State
        {
            /************************************************************************************************************************/

            [SerializeField] private Image _Image;
            [SerializeField] private float _Speed = 2;

            private bool _IsFadingOut;

            /************************************************************************************************************************/

            public override void OnEnterState()
            {
                base.OnEnterState();
                _Image.gameObject.SetActive(true);
                _Image.color = new Color(0, 0, 0, 0);
                _IsFadingOut = true;
            }

            /************************************************************************************************************************/

            public override void Update()
            {
                var targetAlpha = _IsFadingOut ? 1 : 0;

                var color = _Image.color;
                color.a = Mathf.MoveTowards(color.a, targetAlpha, _Speed * Time.deltaTime);
                _Image.color = color;

                if (color.a == targetAlpha)// When the fade ends.
                {
                    if (_IsFadingOut)
                    {
                        Instance._Ready.ResetCamera();
                        Instance._Golfer.ReturnToReady();
                        _IsFadingOut = false;
                    }
                    else
                    {
                        Instance._StateMachine.TrySetState(Instance._Ready);
                    }
                }
            }

            /************************************************************************************************************************/

            public override void OnExitState()
            {
                base.OnExitState();
                _Image.gameObject.SetActive(false);
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
    }
}
