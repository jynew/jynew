// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.Examples.StateMachines.Brains;
using System;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Weapons
{
    /// <summary>A <see cref="CreatureState"/> which managed the currently equipped <see cref="Weapon"/>.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/weapons">Weapons</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.Weapons/EquipState
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Weapons - Equip State")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(Weapons) + "/" + nameof(EquipState))]
    public sealed class EquipState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private Transform _WeaponHolder;
        [SerializeField] private Weapon _Weapon;

        private Weapon _EquippingWeapon;
        private Action _OnUnequipEnd;

        /************************************************************************************************************************/

        // Called by UI Buttons.
        public Weapon Weapon
        {
            get => _Weapon;
            set
            {
                if (enabled)
                    return;

                _EquippingWeapon = value;
                if (!Creature.StateMachine.TrySetState(this))
                    _EquippingWeapon = _Weapon;
            }
        }

        /************************************************************************************************************************/

        private void Awake()
        {
            _EquippingWeapon = _Weapon;
            _OnUnequipEnd = OnUnequipEnd;
            AttachWeapon();
        }

        /************************************************************************************************************************/

        // This state can only be entered by setting the Weapon property.
        public override bool CanEnterState => _Weapon != _EquippingWeapon;

        /************************************************************************************************************************/

        /// <summary>
        /// Start at the beginning of the sequence by default, but if the previous attack hasn't faded out yet then
        /// perform the next attack instead.
        /// </summary>
        private void OnEnable()
        {
            if (_Weapon.UnequipAnimation.IsValid)
            {
                var state = Creature.Animancer.Play(_Weapon.UnequipAnimation);
                state.Events.OnEnd = _OnUnequipEnd;
            }
            else
            {
                OnUnequipEnd();
            }
        }

        /************************************************************************************************************************/

        private void OnUnequipEnd()
        {
            DetachWeapon();
            _Weapon = _EquippingWeapon;
            AttachWeapon();

            if (_Weapon.EquipAnimation.IsValid)
            {
                var state = Creature.Animancer.Play(_Weapon.EquipAnimation);
                state.Events.OnEnd = Creature.ForceEnterIdleState;
            }
            else
            {
                Creature.StateMachine.ForceSetState(Creature.Idle);
            }
        }

        /************************************************************************************************************************/

        private void AttachWeapon()
        {
            if (_Weapon == null)
                return;

            if (_WeaponHolder != null)
            {
                var transform = _Weapon.transform;
                transform.parent = _WeaponHolder;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
            }

            _Weapon.gameObject.SetActive(true);
        }

        private void DetachWeapon()
        {
            if (_Weapon == null)
                return;

            // It might be more appropriate to reparent inactive weapons to the inventory system if you have one.
            // Or you could even attach them to specific bones on the character and leave them active.
            _Weapon.transform.parent = transform;
            _Weapon.gameObject.SetActive(false);
        }

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            Creature.Rigidbody.velocity = Vector3.zero;
        }

        /************************************************************************************************************************/

        public override bool CanExitState => false;

        /************************************************************************************************************************/
    }
}
