// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using System;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Creatures
{
    /// <summary>
    /// A centralised group of references to the common parts of a creature and a state machine for their actions.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/creatures">Creatures</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.Creatures/Creature
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Creatures - Creature")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(Creatures) + "/" + nameof(Creature))]
    [DefaultExecutionOrder(-5000)]// Initialise the State Machine early.
    public sealed class Creature : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private AnimancerComponent _Animancer;
        public AnimancerComponent Animancer => _Animancer;

        [SerializeField]
        private CreatureState _Idle;
        public CreatureState Idle => _Idle;

        // Rigidbody.
        // Ground Detector.
        // Stats.
        // Health and Mana.
        // Pathfinding.
        // Etc.
        // Anything common to most creatures.

        /************************************************************************************************************************/

        /// <summary>
        /// The Finite State Machine that manages the actions of this creature.
        /// </summary>
        public StateMachine<CreatureState> StateMachine { get; private set; }

        /// <summary>
        /// Forces the <see cref="StateMachine"/> to return to the <see cref="Idle"/> state.
        /// </summary>
        public Action ForceIdleState { get; private set; }

        /************************************************************************************************************************/

        private void Awake()
        {
            // Note that this class has a [DefaultExecutionOrder] attribute to ensure that this method runs before any
            // other components that might want to access it.

            ForceIdleState = () => StateMachine.ForceSetState(_Idle);

            StateMachine = new StateMachine<CreatureState>(_Idle);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Calls <see cref="StateMachine{TState}.TrySetState"/>. Normally you would just access the
        /// <see cref="StateMachine"/> directly. This method only exists to be called by UI buttons.
        /// </summary>
        public void TrySetState(CreatureState state) => StateMachine.TrySetState(state);

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// Inspector Gadgets Pro calls this method after drawing the regular Inspector GUI, allowing this script to
        /// display its current state in Play Mode.
        /// </summary>
        /// <remarks>
        /// <see cref="https://kybernetik.com.au/inspector-gadgets/pro">Inspector Gadgets Pro</see> allows you to
        /// easily customise the Inspector without writing a full custom Inspector class by simply adding a method with
        /// this name. Without Inspector Gadgets, this method will do nothing.
        /// </remarks>
        private void AfterInspectorGUI()
        {
            if (UnityEditor.EditorApplication.isPlaying)
            {
                var enabled = GUI.enabled;
                GUI.enabled = false;
                UnityEditor.EditorGUILayout.ObjectField("Current State", StateMachine.CurrentState, typeof(CreatureState), true);
                GUI.enabled = enabled;
            }
        }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
    }
}
