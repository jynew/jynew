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

        [SerializeField] private IntroductionState _Introduction;

        /************************************************************************************************************************/

        /// <summary>Camera orbiting the player, waiting for the player to click.</summary>
        /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.GameManager/IntroductionState
        /// 
        [Serializable]
        public sealed class IntroductionState : State
        {
            /************************************************************************************************************************/

            [SerializeField] private float _OrbitSpeed = 45;
            [SerializeField] private float _OrbitRadius = 3;

            /************************************************************************************************************************/

            public override string DisplayText => "Welcome to the Game Manager example\nClick to start playing";

            /************************************************************************************************************************/

            public override void OnEnterState()
            {
                base.OnEnterState();
                Instance._Golfer.EndSwing();
            }

            /************************************************************************************************************************/

            public override void Update()
            {
                var camera = Instance._Camera;

                var euler = camera.eulerAngles;
                euler.y += _OrbitSpeed * Time.deltaTime;
                camera.eulerAngles = euler;

                var lookAt = Instance._Golfer.transform.position;
                lookAt.y += 1;
                camera.position = lookAt - camera.forward * _OrbitRadius;

                if (Input.GetMouseButtonUp(0))
                    Instance._StateMachine.TrySetState(Instance._Ready);
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
    }
}
