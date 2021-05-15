// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Platformer
{
    /// <summary>
    /// A centralised group of references to the common parts of a creature and a state machine for their actions.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/platformer">Platformer</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.Platformer/Creature
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Platformer - Creature")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(Platformer) + "/" + nameof(Creature))]
    [DefaultExecutionOrder(-5000)]// Initialise the State Machine early.
    public sealed class Creature : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private AnimancerComponent _Animancer;
        public AnimancerComponent Animancer => _Animancer;

        [SerializeField]
        private SpriteRenderer _Renderer;
        public SpriteRenderer Renderer => _Renderer;

        [SerializeField]
        private CreatureBrain _Brain;
        public CreatureBrain Brain => _Brain;

        [SerializeField]
        private Rigidbody2D _Rigidbody;
        public Rigidbody2D Rigidbody => _Rigidbody;

        [SerializeField]
        private GroundDetector _GroundDetector;
        public GroundDetector GroundDetector => _GroundDetector;

        [SerializeField]
        private Health _Health;
        public Health Health => _Health;

        [SerializeField]
        private CreatureState _Introduction;
        public CreatureState Introduction => _Introduction;

        [SerializeField]
        private CreatureState _Idle;
        public CreatureState Idle => _Idle;

        // Stats.
        // Mana.
        // Pathfinding.
        // Etc.
        // Anything common to most creatures.

        /************************************************************************************************************************/

        /// <summary>
        /// The Finite State Machine that manages the actions of this creature.
        /// </summary>
        public FSM.StateMachine<CreatureState> StateMachine { get; private set; }

        /// <summary>
        /// Forces the <see cref="StateMachine"/> to return to the <see cref="Idle"/> state.
        /// </summary>
        public Action ForceEnterIdleState { get; private set; }

        /************************************************************************************************************************/

        // Note that this class has a [DefaultExecutionOrder] attribute to ensure that this method runs before any
        // other components that might want to access it.
        private void Awake()
        {
            ForceEnterIdleState = () => StateMachine.ForceSetState(_Idle);

            StateMachine = new FSM.StateMachine<CreatureState>(_Introduction);
        }

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            var speed = StateMachine.CurrentState.MovementSpeed * _Brain.MovementDirection;
            _Rigidbody.velocity = new Vector2(speed, _Rigidbody.velocity.y);

            // The sprites face right by default, so flip the X axis when moving left.
            if (speed != 0)
                _Renderer.flipX = _Brain.MovementDirection < 0;
        }

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <summary>[Editor-Only] Displays the current state at the bottom of the Inspector.</summary>
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
