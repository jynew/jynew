// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Creatures
{
    /// <summary>A state for a <see cref="Creature"/> which simply plays an animation.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/creatures">Creatures</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.Creatures/CreatureState
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Creatures - Creature State")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(Creatures) + "/" + nameof(CreatureState))]
    public class CreatureState : StateBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private Creature _Creature;
        [SerializeField] private AnimationClip _Animation;

        /************************************************************************************************************************/

        /// <summary>
        /// Plays the animation and if it is not looping it returns the <see cref="Creature"/> to Idle afterwards.
        /// </summary>
        private void OnEnable()
        {
            var state = _Creature.Animancer.Play(_Animation, 0.25f);
            if (!_Animation.isLooping)
                state.Events.OnEnd = _Creature.ForceIdleState;
        }

        /************************************************************************************************************************/
    }
}
