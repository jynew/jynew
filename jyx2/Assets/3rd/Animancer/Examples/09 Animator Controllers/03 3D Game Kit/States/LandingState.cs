// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;
using UnityEngine.Events;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    /// <summary>A <see cref="CreatureState"/> which plays a "landing on the ground" animation.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/animator-controllers/3d-game-kit/landing">3D Game Kit/Landing</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.AnimatorControllers.GameKit/LandingState
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Game Kit - Landing State")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(AnimatorControllers) + "." + nameof(GameKit) + "/" + nameof(LandingState))]
    public sealed class LandingState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private MixerState.Transition2D _SoftLanding;
        [SerializeField] private ClipState.Transition _HardLanding;
        [SerializeField] private float _HardLandingForwardSpeed = 5;
        [SerializeField] private float _HardLandingVerticalSpeed = -10;
        [SerializeField] private UnityEvent _PlayAudio;// See the Read Me.

        private bool _IsSoftLanding;

        /************************************************************************************************************************/

        private void Awake()
        {
            _SoftLanding.Events.OnEnd =
                _HardLanding.Events.OnEnd =
                () => Creature.CheckMotionState();
        }

        /************************************************************************************************************************/

        public override bool CanEnterState => Creature.IsGrounded;

        /************************************************************************************************************************/

        /// <summary>
        /// Performs either a hard or soft landing depending on the current speed (both horizontal and vertical).
        /// </summary>
        private void OnEnable()
        {
            Creature.ForwardSpeed = Creature.DesiredForwardSpeed;

            if (Creature.VerticalSpeed <= _HardLandingVerticalSpeed &&
                Creature.ForwardSpeed >= _HardLandingForwardSpeed)
            {
                _IsSoftLanding = false;
                Creature.Animancer.Play(_HardLanding);
            }
            else
            {
                _IsSoftLanding = true;
                Creature.Animancer.Play(_SoftLanding);
                _SoftLanding.State.Parameter = new Vector2(Creature.ForwardSpeed, Creature.VerticalSpeed);
            }

            // The landing sounds in the full 3D Game Kit have different variations based on the ground material, just
            // like the footstep sounds as discussed in the LocomotionState.

            _PlayAudio.Invoke();
        }

        /************************************************************************************************************************/

        public override bool FullMovementControl => _IsSoftLanding;

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            if (!Creature.IsGrounded &&
                Creature.StateMachine.TrySetState(Creature.Airborne))
                return;

            Creature.UpdateSpeedControl();

            if (_IsSoftLanding)
            {
                // Update the horizontal speed but keep the initial vertical speed from when you first landed.
                var parameter = _SoftLanding.State.Parameter;
                parameter.x = Creature.ForwardSpeed;
                _SoftLanding.State.Parameter = parameter;
            }
        }

        /************************************************************************************************************************/
    }
}
