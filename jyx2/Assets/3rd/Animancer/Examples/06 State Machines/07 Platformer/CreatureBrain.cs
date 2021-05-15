// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.StateMachines.Platformer
{
    /// <summary>
    /// Base class for any kind of <see cref="Platformer.Creature"/> controller - local, network, AI, replay, etc.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/platformer">Platformer</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.Platformer/CreatureBrain
    /// 
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(Platformer) + "/" + nameof(CreatureBrain))]
    public abstract class CreatureBrain : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private Creature _Creature;
        public Creature Creature => _Creature;

        /************************************************************************************************************************/

        public float MovementDirection { get; protected set; }

        public bool IsRunning { get; protected set; }

        /************************************************************************************************************************/
    }
}
