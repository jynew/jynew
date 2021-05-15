// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using UnityEngine;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    /// <summary>The numerical details of a <see cref="Creature"/>.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/animator-controllers/3d-game-kit">3D Game Kit</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.AnimatorControllers.GameKit/CreatureStats
    /// 
    [Serializable]
    public sealed class CreatureStats
    {
        /************************************************************************************************************************/

        [SerializeField]
        private float _MaxSpeed = 8;
        public float MaxSpeed => _MaxSpeed;

        [SerializeField]
        private float _Acceleration = 20;
        public float Acceleration => _Acceleration;

        [SerializeField]
        private float _Deceleration = 25;
        public float Deceleration => _Deceleration;

        [SerializeField]
        private float _MinTurnSpeed = 400;
        public float MinTurnSpeed => _MinTurnSpeed;

        [SerializeField]
        private float _MaxTurnSpeed = 1200;
        public float MaxTurnSpeed => _MaxTurnSpeed;

        [SerializeField]
        private float _Gravity = 20;
        public float Gravity => _Gravity;

        [SerializeField]
        private float _StickingGravityProportion = 0.3f;
        public float StickingGravityProportion => _StickingGravityProportion;

        /************************************************************************************************************************/
    }
}
