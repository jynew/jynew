// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.Examples.StateMachines.Brains;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Weapons
{
    /// <summary>A <see cref="CreatureState"/> which can perform <see cref="Weapon.AttackAnimations"/> in sequence.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/weapons">Weapons</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.Weapons/AttackState
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Weapons - Attack State")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(Weapons) + "/" + nameof(AttackState))]
    public sealed class AttackState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private EquipState _Equipment;

        private int _AttackIndex = int.MaxValue;

        /************************************************************************************************************************/

        /// <summary>
        /// Start at the beginning of the sequence by default, but if the previous attack has not faded out yet then
        /// perform the next attack instead.
        /// </summary>
        private void OnEnable()
        {
            Creature.Animancer.Animator.applyRootMotion = true;

            if (ShouldRestartCombo())
            {
                _AttackIndex = 0;
            }
            else
            {
                _AttackIndex++;
            }

            var animation = _Equipment.Weapon.AttackAnimations[_AttackIndex];
            var state = Creature.Animancer.Play(animation);
            state.Events.OnEnd = Creature.ForceEnterIdleState;
        }

        /************************************************************************************************************************/

        private bool ShouldRestartCombo()
        {
            var attackAnimations = _Equipment.Weapon.AttackAnimations;

            if (_AttackIndex >= attackAnimations.Length - 1)
                return true;

            var state = attackAnimations[_AttackIndex].State;
            if (state == null ||
                state.Weight == 0)
                return true;

            return false;
        }

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            Creature.Rigidbody.velocity = Vector3.zero;
        }

        /************************************************************************************************************************/

        public override bool CanExitState => false;

        /************************************************************************************************************************/

        private void OnDisable()
        {
            Creature.Animancer.Animator.applyRootMotion = false;
        }

        /************************************************************************************************************************/
    }
}
