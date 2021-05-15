// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;
using UnityEngine.UI;

namespace Animancer.Examples.StateMachines.GameManager
{
    /// <summary>A game manager that uses a <see cref="StateMachine{TState}"/>.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/game-manager">Game Manager</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.GameManager/GameManagerFSM
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Game Manager - FSM")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(GameManager) + "/" + nameof(GameManagerFSM))]
    public sealed partial class GameManagerFSM : MonoBehaviour
    {
        /************************************************************************************************************************/

        public static GameManagerFSM Instance { get; private set; }

        [SerializeField] private Transform _Camera;
        [SerializeField] private Text _Text;
        [SerializeField] private Events.GolfHitController _Golfer;

        private StateMachine<State> _StateMachine;

        /************************************************************************************************************************/

        private void Awake()
        {
            Debug.Assert(Instance == null, $"The {nameof(GameManagerFSM)}.{nameof(Instance)} is already assigned.");
            Instance = this;

            _StateMachine = new StateMachine<State>(_Introduction);

            if (FindObjectOfType<GameManagerEnum>() != null)
                Debug.LogError(
                    $"Both the {nameof(GameManagerEnum)} and {nameof(GameManagerFSM)} are active. Exit Play Mode and disable one of them.");
        }

        /************************************************************************************************************************/

        private void Update()
        {
            _StateMachine.CurrentState.Update();
        }

        /************************************************************************************************************************/
    }
}
