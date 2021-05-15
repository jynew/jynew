// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using Animancer.Examples.StateMachines.Brains;
using UnityEngine;

namespace Animancer.Examples.AnimatorControllers
{
    /// <summary>
    /// A <see cref="CreatureState"/> which moves the creature according to their
    /// <see cref="CreatureBrain.MovementDirection"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// This class is very similar to <see cref="StateMachines.Brains.LocomotionState"/>, except that it manages a
    /// Blend Tree instead of individual <see cref="AnimationClip"/>s.
    /// </remarks>
    /// 
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/animator-controllers/mini-game">Hybrid Mini Game</see></example>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.AnimatorControllers/LocomotionState
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Hybrid - Locomotion State")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(AnimatorControllers) + "/" + nameof(LocomotionState))]
    public sealed class LocomotionState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private float _Acceleration = 3;

        private float _MoveBlend;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            Animancer.PlayController();
            _MoveBlend = 0;
        }

        /************************************************************************************************************************/

        private void Update()
        {
            UpdateAnimation();
            UpdateTurning();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// This method is similar to the "PlayMove" method in <see cref="Locomotion.IdleAndWalkAndRun"/>, but instead
        /// of checking <see cref="Input"/> to determine whether or not to run we are checking if the
        /// <see cref="Creature.Brain"/> says it wants to run.
        /// </summary>
        private void UpdateAnimation()
        {
            float targetBlend;
            if (Creature.Brain.MovementDirection == Vector3.zero)
                targetBlend = 0;
            else if (Creature.Brain.IsRunning)
                targetBlend = 1;
            else
                targetBlend = 0.5f;

            _MoveBlend = Mathf.MoveTowards(_MoveBlend, targetBlend, _Acceleration * Time.deltaTime);
            Animancer.SetFloat("MoveBlend", _MoveBlend);
        }

        /************************************************************************************************************************/

        /// <remarks>
        /// This method is identical to the same method in <see cref="StateMachines.Brains.LocomotionState"/>.
        /// </remarks>
        private void UpdateTurning()
        {
            var movement = Creature.Brain.MovementDirection;
            if (movement == Vector3.zero)
                return;

            var targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
            var turnDelta = Creature.Stats.TurnSpeed * Time.deltaTime;

            var transform = Creature.Animancer.transform;
            var eulerAngles = transform.eulerAngles;
            eulerAngles.y = Mathf.MoveTowardsAngle(eulerAngles.y, targetAngle, turnDelta);
            transform.eulerAngles = eulerAngles;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Constantly moves the creature according to the <see cref="CreatureBrain.MovementDirection"/>.
        /// </summary>
        /// <remarks>
        /// This method is kept simple for the sake of demonstrating the animation system in this example.
        /// In a real game you would usually not set the velocity directly, but would instead use
        /// <see cref="Rigidbody.AddForce"/> to avoid interfering with collisions and other forces.
        /// <para></para>
        /// This method is identical to the same method in <see cref="StateMachines.Brains.LocomotionState"/>.
        /// </remarks>
        private void FixedUpdate()
        {
            // Get the desired direction, remove any vertical component, and limit the magnitude to 1 or less.
            // Otherwise a brain could make the creature travel at any speed by setting a longer vector.
            var direction = Creature.Brain.MovementDirection;
            direction.y = 0;
            direction = Vector3.ClampMagnitude(direction, 1);

            var speed = Creature.Stats.GetMoveSpeed(Creature.Brain.IsRunning);

            Creature.Rigidbody.velocity = direction * speed;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Normally the <see cref="Creature"/> class would have a reference to the specific type of
        /// <see cref="AnimancerComponent"/> we want, but for the sake of reusing code from the earlier example, we
        /// just use a type cast here.
        /// </summary>
        private HybridAnimancerComponent Animancer => (HybridAnimancerComponent)Creature.Animancer;

        /************************************************************************************************************************/
    }
}
