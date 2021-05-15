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

        [SerializeField] private ReadyState _Ready;

        /************************************************************************************************************************/

        /// <summary>Waiting for player input to hit the ball.</summary>
        /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.GameManager/ReadyState
        /// 
        [Serializable]
        public sealed class ReadyState : State
        {
            /************************************************************************************************************************/

            [SerializeField] private Vector3 _CameraPosition;
            [SerializeField] private Vector3 _CameraRotation;

            /************************************************************************************************************************/

            public override string DisplayText => "Click to hit the ball";

            /************************************************************************************************************************/

            public override void OnEnterState()
            {
                base.OnEnterState();
                ResetCamera();
                Instance._Golfer.enabled = true;
            }

            /************************************************************************************************************************/

            public void ResetCamera()
            {
                Instance._Camera.position = _CameraPosition;
                Instance._Camera.eulerAngles = _CameraRotation;
            }

            /************************************************************************************************************************/

            public override void Update()
            {
                // The GolfHitController handles its own input now that it's enabled.
                // So we just wait for it to change states.
                if (Instance._Golfer.CurrentState != Events.GolfHitController.State.Ready)
                    Instance._StateMachine.TrySetState(Instance._Action);
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
    }
}
