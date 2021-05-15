// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.StateMachines.Platformer
{
    /// <summary>A more complex <see cref="JumpState"/> that allows you to hold the button down to jump higher.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/platformer">Platformer</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.Platformer/AdvancedJumpState
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Platformer - Advanced Jump State")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(Platformer) + "/" + nameof(AdvancedJumpState))]
    public sealed class AdvancedJumpState : JumpState
    {
        /************************************************************************************************************************/

        [SerializeField] private float _HoldForce = 20;
        [SerializeField] private float _HoldDuration = 0.25f;

        public bool IsHolding { get; set; }

        /************************************************************************************************************************/

        protected override void OnEnable()
        {
            base.OnEnable();
            IsHolding = true;
        }

        /************************************************************************************************************************/

        private void OnDisable()
        {
            IsHolding = false;
        }

        /************************************************************************************************************************/

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (IsHolding && AnimancerState.Time <= _HoldDuration)
                Creature.Rigidbody.velocity += new Vector2(0, _HoldForce * Time.deltaTime);
        }

        /************************************************************************************************************************/
    }
}
