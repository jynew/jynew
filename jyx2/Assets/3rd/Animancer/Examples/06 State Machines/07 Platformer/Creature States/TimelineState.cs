// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Platformer
{
    /// <summary>
    /// A <see cref="CreatureState"/> that plays a <see cref="UnityEngine.Playables.PlayableAsset"/> (such as a
    /// <see cref="UnityEngine.Timeline.TimelineAsset"/>)
    /// then returns to idle.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/platformer">Platformer</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.Platformer/TimelineState
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Platformer - Timeline State")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(Platformer) + "/" + nameof(TimelineState))]
    public sealed class TimelineState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private PlayableAssetState.Transition _Animation;
        [SerializeField] private bool _DestroyWhenDone;

        /************************************************************************************************************************/

        private void Awake()
        {
            if (_DestroyWhenDone)
            {
                _Animation.Events.OnEnd = () =>
                {
                    Creature.Idle.ForceEnterState();
                    Creature.Animancer.States.Destroy(_Animation);
                    Destroy(this);
                };
            }
            else
            {
                _Animation.Events.OnEnd = Creature.ForceEnterIdleState;
            }
        }

        /************************************************************************************************************************/

        private void OnEnable()
        {
            Creature.Animancer.Play(_Animation);
        }

        /************************************************************************************************************************/
    }
}
