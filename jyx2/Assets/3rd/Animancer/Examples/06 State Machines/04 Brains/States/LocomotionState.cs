// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.StateMachines.Brains
{
    /// <summary>
    /// A <see cref="CreatureState"/> which moves the creature according to their
    /// <see cref="CreatureBrain.MovementDirection"/>.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/brains">Brains</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.Brains/LocomotionState
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Brains - Locomotion State")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(Brains) + "/" + nameof(LocomotionState))]
    public sealed class LocomotionState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimationClip _Walk;
        [SerializeField] private AnimationClip _Run;

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
            // We will play either the Walk or Run animation.

            // We need to know which animation we are trying to play and which is the other one.
            AnimationClip playAnimation, otherAnimation;

            if (Creature.Brain.IsRunning)
            {
                playAnimation = _Run;
                otherAnimation = _Walk;
            }
            else
            {
                playAnimation = _Walk;
                otherAnimation = _Run;
            }

            // Play the one we want.
            var playState = Creature.Animancer.Play(playAnimation, 0.25f);

            // If the brain wants to move slowly, slow down the animation.
            var speed = Mathf.Min(Creature.Brain.MovementDirection.magnitude, 1);
            playState.Speed = speed;

            // If the other one is still fading out, align their NormalizedTime to ensure they stay at the same
            // relative progress through their walk cycle.
            if (Creature.Animancer.States.TryGet(otherAnimation, out var otherState) &&
                otherState.IsPlaying)
            {
                playState.NormalizedTime = otherState.NormalizedTime;
                otherState.Speed = speed;
            }
        }

        /************************************************************************************************************************/

        private void UpdateTurning()
        {
            // Do not turn if we are not trying to move.
            var movement = Creature.Brain.MovementDirection;
            if (movement == Vector3.zero)
                return;

            // Determine the angle we want to turn towards.
            // Without going into the maths behind it, Atan2 gives us the angle of a vector in radians.
            // So we just feed in the x and z values because we want an angle around the y axis,
            // then convert the result to degrees because Transform.eulerAngles uses degrees.
            var targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;

            // Determine how far we can turn this frame (in degrees).
            var turnDelta = Creature.Stats.TurnSpeed * Time.deltaTime;

            // Get the current rotation, move its y value towards the target, and apply it back to the Transform.
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
    }
}
