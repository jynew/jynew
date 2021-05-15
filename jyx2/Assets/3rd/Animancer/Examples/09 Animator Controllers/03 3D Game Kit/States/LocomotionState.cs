// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;
using UnityEngine.Events;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    /// <summary>
    /// A <see cref="CreatureState"/> which moves the creature according to their
    /// <see cref="CreatureBrain.Movement"/>.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/animator-controllers/3d-game-kit/locomotion">3D Game Kit/Locomotion</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.AnimatorControllers.GameKit/LocomotionState
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Game Kit - Locomotion State")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(AnimatorControllers) + "." + nameof(GameKit) + "/" + nameof(LocomotionState))]
    public sealed class LocomotionState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private LinearMixerState.Transition _LocomotionMixer;
        [SerializeField] private ClipState.Transition _QuickTurnLeft;
        [SerializeField] private ClipState.Transition _QuickTurnRight;
        [SerializeField] private float _QuickTurnMoveSpeed = 2;
        [SerializeField] private float _QuickTurnAngle = 145;

        private AnimatedFloat _FootFall;

        /************************************************************************************************************************/

        private void Awake()
        {
            _QuickTurnLeft.Events.OnEnd =
                _QuickTurnRight.Events.OnEnd =
                () => Creature.Animancer.Play(_LocomotionMixer);

            _FootFall = new AnimatedFloat(Creature.Animancer, "FootFall");
        }

        /************************************************************************************************************************/

        public override bool CanEnterState => Creature.IsGrounded;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            Creature.Animancer.Play(_LocomotionMixer);
        }

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            if (Creature.CheckMotionState())
                return;

            Creature.UpdateSpeedControl();
            _LocomotionMixer.State.Parameter = Creature.ForwardSpeed;

            UpdateRotation();
            UpdateAudio();
        }

        /************************************************************************************************************************/

        private void UpdateRotation()
        {
            // If the default locomotion state is not active we must be performing a quick turn.
            // Those animations use root motion to perform the turn so we don't want any scripted rotation during them.
            if (!_LocomotionMixer.State.IsActive)
                return;

            if (!Creature.GetTurnAngles(Creature.Brain.Movement, out var currentAngle, out var targetAngle))
                return;

            // Check if we should play a quick turn animation:

            // If we are moving fast enough.
            if (Creature.ForwardSpeed > _QuickTurnMoveSpeed)
            {
                // And turning sharp enough.
                var deltaAngle = Mathf.DeltaAngle(currentAngle, targetAngle);
                if (Mathf.Abs(deltaAngle) > _QuickTurnAngle)
                {
                    // Determine which way we are turning.
                    var turn = deltaAngle < 0 ? _QuickTurnLeft : _QuickTurnRight;

                    // Make sure the desired turn is not already active so we don't keep using it repeatedly.
                    if (turn.State == null || turn.State.Weight == 0)
                    {
                        Creature.Animancer.Play(turn);

                        // Now that we are quick turning, we don't want to apply the scripted turning below.
                        return;
                    }
                }
            }

            Creature.TurnTowards(currentAngle, targetAngle, Creature.CurrentTurnSpeed);
        }

        /************************************************************************************************************************/

        [SerializeField] private UnityEvent _PlayFootstepAudio;// See the Read Me.
        private bool _CanPlayAudio;
        private bool _IsPlayingAudio;

        /// <remarks>
        /// This is the same logic used for locomotion audio in the original PlayerController.
        /// </remarks>
        private void UpdateAudio()
        {
            const float Threshold = 0.01f;

            var footFallCurve = _FootFall.Value;
            if (footFallCurve > Threshold && !_IsPlayingAudio && _CanPlayAudio)
            {
                _IsPlayingAudio = true;
                _CanPlayAudio = false;

                // The full 3D Game Kit has different footstep sounds depending on the ground material and your speed
                // so it calls RandomAudioPlayer.PlayRandomClip with those parameters:
                //_FootstepAudio.PlayRandomClip(Creature.GroundMaterial, Creature.ForwardSpeed < 4 ? 0 : 1);

                // Unfortunately UnityEvents cannot call methods with multiple parameters (UltEvents can), but it does
                // not realy matter because the 3D Game Kit Lite only has one set of footstep sounds anyway.

                _PlayFootstepAudio.Invoke();
            }
            else if (_IsPlayingAudio)
            {
                _IsPlayingAudio = false;
            }
            else if (footFallCurve < Threshold && !_CanPlayAudio)
            {
                _CanPlayAudio = true;
            }
        }

        /************************************************************************************************************************/
    }
}
