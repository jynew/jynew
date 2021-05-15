// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.StateMachines.Platformer
{
    /// <summary>
    /// Base class for the various states a <see cref="Platformer.Creature"/> can be in and actions they can perform.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/platformer">Platformer</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.Platformer/CreatureState
    /// 
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(Platformer) + "/" + nameof(CreatureState))]
    public abstract class CreatureState : FSM.StateBehaviour, FSM.IOwnedState<CreatureState>
    {
        /************************************************************************************************************************/

        [SerializeField]
        private Creature _Creature;

        /// <summary>The <see cref="Examples.PlatformerCreature"/> that owns this state.</summary>
        public Creature Creature => _Creature;

        /// <summary>
        /// Sets the <see cref="Creature"/>.
        /// </summary>
        /// <remarks>
        /// This is not a property setter because you shouldn't be casually changing the owner of a state. Usually this
        /// would only be used when adding a state to a creature using a script.
        /// </remarks>
        public void SetCreature(Creature creature) => _Creature = creature;

        /************************************************************************************************************************/

        /// <summary>
        /// The current speed at which this state allows the creature to move.
        /// </summary>
        /// <remarks>
        /// This value is always 0 unless overridden by a child class.
        /// </remarks>
        public virtual float MovementSpeed => 0;

        /************************************************************************************************************************/

        public FSM.StateMachine<CreatureState> OwnerStateMachine => _Creature.StateMachine;

        /************************************************************************************************************************/
    }
}
