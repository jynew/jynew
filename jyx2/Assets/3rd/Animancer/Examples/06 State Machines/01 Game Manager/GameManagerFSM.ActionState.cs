// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using UnityEngine;

namespace Animancer.Examples.StateMachines.GameManager
{
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.GameManager/GameManagerFSM
    /// 
    partial class GameManagerFSM
    {
        /************************************************************************************************************************/

        [SerializeField] private ActionState _Action;

        /************************************************************************************************************************/

        /// <summary>Waiting the the character to hit the ball and the ball to stop.</summary>
        /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.GameManager/ActionState
        /// 
        [Serializable]
        public sealed class ActionState : State
        {
            /************************************************************************************************************************/

            [SerializeField] private float _CameraTurnSpeedFactor = 5;
            [SerializeField] private Rigidbody _Ball;

            /************************************************************************************************************************/

            public override string DisplayText
                => $"Wait for the ball to stop\nCurrent Speed: {_Ball.velocity.magnitude:0.00}m/s";

            /************************************************************************************************************************/

            public override void OnEnterState()
            {
                base.OnEnterState();
                Instance._Golfer.enabled = false;
            }

            /************************************************************************************************************************/

            public override void Update()
            {
                var camera = Instance._Camera;
                var targetRotation = Quaternion.LookRotation(_Ball.position - camera.position);
                camera.rotation = Quaternion.Slerp(camera.rotation, targetRotation, _CameraTurnSpeedFactor * Time.deltaTime);

                Instance._Text.text = DisplayText;

                if (Instance._Golfer.CurrentState == Events.GolfHitController.State.Idle &&
                    _Ball.IsSleeping())
                    Instance._StateMachine.TrySetState(Instance._Fade);
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
    }
}
