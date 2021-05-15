// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;
using UnityEngine.Events;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    /// <summary>A <see cref="CreatureState"/> which plays an "airborne" animation.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/animator-controllers/3d-game-kit/airborne">3D Game Kit/Airborne</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.AnimatorControllers.GameKit/AirborneState
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Game Kit - Airborne State")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(AnimatorControllers) + "." + nameof(GameKit) + "/" + nameof(AirborneState))]
    public sealed class AirborneState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private LinearMixerState.Transition _Animations;
        [SerializeField] private float _JumpSpeed = 10;
        [SerializeField] private float _JumpAbortSpeed = 10;
        [SerializeField] private float _TurnSpeedProportion = 5.4f;
        [SerializeField] private LandingState _LandingState;
        [SerializeField] private UnityEvent _PlayAudio;// See the Read Me.

        private bool _IsJumping;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            _IsJumping = false;
            Creature.Animancer.Play(_Animations);
        }

        /************************************************************************************************************************/

        public override bool StickToGround => false;

        /************************************************************************************************************************/

        /// <summary>
        /// The airborne animations do not have root motion, so we just let the brain determine which way to go.
        /// </summary>
        public override Vector3 RootMotion => Creature.Brain.Movement * (Creature.ForwardSpeed * Time.deltaTime);

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            // When you jump, do not start checking if you have landed until you stop going up.
            if (_IsJumping)
            {
                if (Creature.VerticalSpeed <= 0)
                    _IsJumping = false;
            }
            else
            {
                // If we have a landing state, try to enter it.
                if (_LandingState != null)
                {
                    if (Creature.StateMachine.TrySetState(_LandingState))
                        return;
                }
                else// Otherwise check the default transitions to Idle or Locomotion.
                {
                    if (Creature.CheckMotionState())
                        return;
                }

                // If the jump was cancelled but we are still going up, apply some extra downwards acceleration in
                // addition to the regular graivty applied in Creature.OnAnimatorMove.
                if (Creature.VerticalSpeed > 0)
                    Creature.VerticalSpeed -= _JumpAbortSpeed * Time.deltaTime;
            }

            _Animations.State.Parameter = Creature.VerticalSpeed;

            Creature.UpdateSpeedControl();

            var input = Creature.Brain.Movement;

            // Since we do not have quick turn animations like the LocomotionState, we just increase the turn speed
            // when the direction we want to go is further away from the direction we are currently facing.
            var turnSpeed = Vector3.Angle(Creature.transform.forward, input) * (1f / 180) *
                _TurnSpeedProportion *
                Creature.CurrentTurnSpeed;

            Creature.TurnTowards(input, turnSpeed);
        }

        /************************************************************************************************************************/

        public bool TryJump()
        {
            // We did not override CanEnterState to check if the Creature is grounded because this state is also used
            // if you walk off a ledge, so instead we check that condition here when specifically attempting to jump.
            if (Creature.IsGrounded &&
                Creature.StateMachine.TryResetState(this))
            {
                // Entering this state would have called OnEnable.

                _IsJumping = true;
                Creature.VerticalSpeed = _JumpSpeed;

                // In the 3D Game Kit the jump sound is actually triggered whenever you have a positive VerticalSpeed
                // when you become airborne, which could happen if you go up a ramp for example.
                _PlayAudio.Invoke();

                return true;
            }

            return false;
        }

        /************************************************************************************************************************/

        public void CancelJump() => _IsJumping = false;

        /************************************************************************************************************************/
    }
}
