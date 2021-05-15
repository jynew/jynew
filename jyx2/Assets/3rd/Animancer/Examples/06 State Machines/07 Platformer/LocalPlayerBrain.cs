// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.StateMachines.Platformer
{
    /// <summary>A brain for creatures controlled by local input (keyboard and mouse).</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/platformer">Platformer</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.Platformer/LocalPlayerBrain
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Platformer - Local Player Brain")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(Platformer) + "/" + nameof(LocalPlayerBrain))]
    public sealed class LocalPlayerBrain : CreatureBrain
    {
        /************************************************************************************************************************/

        [SerializeField] private CreatureState _Attack;
        [SerializeField] private AdvancedJumpState _Jump;

        /************************************************************************************************************************/

        private void Update()
        {
            if (Input.GetButtonUp("Fire1"))// Left Click by default.
                Creature.StateMachine.TrySetState(_Attack);

            if (Input.GetButtonDown("Jump"))// Space by default.
                Creature.StateMachine.TrySetState(_Jump);

            if (Input.GetButtonUp("Jump"))
                _Jump.IsHolding = false;

            // GetAxisRaw rather than GetAxis because we don't want any smoothing.
            MovementDirection = Input.GetAxisRaw("Horizontal");// A and D or Arrow Keys by default.

            IsRunning = Input.GetButton("Fire3");// Left Shift by default.
        }

        /************************************************************************************************************************/
    }
}
