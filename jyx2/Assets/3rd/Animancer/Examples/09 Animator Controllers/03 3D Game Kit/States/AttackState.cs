// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;
using UnityEngine.Events;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    /// <summary>A <see cref="CreatureState"/> which plays a series of "attack" animations.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/animator-controllers/3d-game-kit/attack">3D Game Kit/Attack</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.AnimatorControllers.GameKit/AttackState
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Game Kit - Attack State")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(AnimatorControllers) + "." + nameof(GameKit) + "/" + nameof(AttackState))]
    public sealed class AttackState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private float _TurnSpeed = 400;
        [SerializeField] private UnityEvent _SetWeaponOwner;// See the Read Me.
        [SerializeField] private UnityEvent _OnStart;// See the Read Me.
        [SerializeField] private UnityEvent _OnEnd;// See the Read Me.
        [SerializeField] private ClipState.Transition[] _Animations;

        private int _AttackIndex = int.MaxValue;
        private ClipState.Transition _Attack;

        /************************************************************************************************************************/

        private void Awake()
        {
            _SetWeaponOwner.Invoke();
        }

        /************************************************************************************************************************/

        public override bool CanEnterState => Creature.IsGrounded;

        /************************************************************************************************************************/

        /// <summary>
        /// Start at the beginning of the sequence by default, but if the previous attack hasn't faded out yet then
        /// perform the next attack instead.
        /// </summary>
        private void OnEnable()
        {
            if (_AttackIndex >= _Animations.Length - 1 ||
                _Animations[_AttackIndex].State.Weight == 0)
            {
                _AttackIndex = 0;
            }
            else
            {
                _AttackIndex++;
            }

            _Attack = _Animations[_AttackIndex];
            Creature.Animancer.Play(_Attack);
            Creature.ForwardSpeed = 0;
            _OnStart.Invoke();
        }

        /************************************************************************************************************************/

        private void OnDisable()
        {
            _OnEnd.Invoke();
        }

        /************************************************************************************************************************/

        public override bool FullMovementControl => false;

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            if (Creature.CheckMotionState())
                return;

            Creature.TurnTowards(Creature.Brain.Movement, _TurnSpeed);
        }

        /************************************************************************************************************************/

        // Use the End Event time to determine when this state is alowed to exit.

        // We cannot simply have this method return false and set the End Event to call Creature.CheckMotionState
        // because it uses TrySetState (instead of ForceSetState) which would be prevented if this returned false.

        // And we cannot have this method return true because that would allow other actions like jumping in the
        // middle of an attack.

        public override bool CanExitState
            => _Attack.State.NormalizedTime >= _Attack.State.Events.NormalizedEndTime;

        /************************************************************************************************************************/
    }
}
