// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    /// <summary>A <see cref="CreatureBrain"/> which controls the creature using keyboard input.</summary>
    /// <remarks>This class serves the same purpose as <c>PlayerInput</c> from the 3D Game Kit.</remarks>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/animator-controllers/3d-game-kit">3D Game Kit</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.AnimatorControllers.GameKit/KeyboardAndMouseBrain
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Game Kit - Keyboard And Mouse Brain")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(AnimatorControllers) + "." + nameof(GameKit) + "/" + nameof(KeyboardAndMouseBrain))]
    public sealed class KeyboardAndMouseBrain : CreatureBrain
    {
        /************************************************************************************************************************/

        [SerializeField] private CreatureState _Attack;
        [SerializeField] private float _AttackInputTimeOut = 0.5f;

        private StateMachine<CreatureState>.InputBuffer _InputBuffer;

        /************************************************************************************************************************/

        private void Awake()
        {
            _InputBuffer = new StateMachine<CreatureState>.InputBuffer(Creature.StateMachine);
        }

        /************************************************************************************************************************/

        private void Update()
        {
            UpdateMovement();
            UpdateActions();
        }

        /************************************************************************************************************************/

        private void UpdateMovement()
        {
            var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (input == Vector2.zero)
            {
                Movement = Vector3.zero;
                return;
            }

            // Get the camera's forward and right vectors and flatten them onto the XZ plane.
            var camera = Camera.main.transform;

            var forward = camera.forward;
            forward.y = 0;
            forward.Normalize();

            var right = camera.right;
            right.y = 0;
            right.Normalize();

            // Build the movement vector by multiplying the input by those axes.
            Movement =
                right * input.x +
                forward * input.y;
            Movement = Vector3.ClampMagnitude(Movement, 1);
        }

        /************************************************************************************************************************/

        private void UpdateActions()
        {
            // Jump gets priority for better platforming.
            if (Input.GetButtonDown("Jump"))
            {
                Creature.Airborne.TryJump();
            }
            else if (Input.GetButtonUp("Jump"))
            {
                Creature.Airborne.CancelJump();
            }

            if (Input.GetButtonDown("Fire1"))
            {
                _InputBuffer.TrySetState(_Attack, _AttackInputTimeOut);
            }
            else
            {
                _InputBuffer.Update();
            }
        }

        /************************************************************************************************************************/
    }
}
