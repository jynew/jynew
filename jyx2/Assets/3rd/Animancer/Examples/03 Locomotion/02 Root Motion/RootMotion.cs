// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using UnityEngine;

namespace Animancer.Examples.Locomotion
{
    /// <summary>Demonstrates how to use Root Motion for some animations but not others.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/locomotion/root-motion">Root Motion</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Locomotion/RootMotion
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Locomotion - Root Motion")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Locomotion) + "/" + nameof(RootMotion))]
    public sealed class RootMotion : MonoBehaviour
    {
        /************************************************************************************************************************/

        /// <summary>
        /// A <see cref="ClipState.Transition"/> with an <see cref="_ApplyRootMotion"/> toggle.
        /// </summary>
        [Serializable]
        public class MotionTransition : ClipState.Transition
        {
            /************************************************************************************************************************/

            [SerializeField, Tooltip("Determines if Root Motion should be enabled when this animation plays")]
            private bool _ApplyRootMotion;

            /************************************************************************************************************************/

            public override void Apply(AnimancerState state)
            {
                base.Apply(state);
                state.Root.Component.Animator.applyRootMotion = _ApplyRootMotion;
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private float _MaxDistance;
        [SerializeField] private MotionTransition[] _Animations;

        private Vector3 _Start;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            _Start = transform.position;
            Play(0);
        }

        /************************************************************************************************************************/

        /// <summary>Plays the animation at the specified `index` in the <see cref="_Animations"/> array.</summary>
        /// <remarks>This method is called by UI Buttons.</remarks>
        public void Play(int index)
        {
            _Animancer.Play(_Animations[index]);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Teleports this object back to its starting location if it moves too far.
        /// </summary>
        private void FixedUpdate()
        {
            if (Vector3.Distance(_Start, transform.position) > _MaxDistance)
                transform.position = _Start;
        }

        /************************************************************************************************************************/

        // These fields determine which object the Root Motion will be applied to.
        // You would normally only have one of these for whichever system you are using to move your characters.
        // But for this example, we have all of them to demonstrate how each could be used.
        [SerializeField] private Transform _MotionTransform;
        [SerializeField] private Rigidbody _MotionRigidbody;
        [SerializeField] private CharacterController _MotionCharacterController;

        /// <summary>
        /// Called when the <see cref="Animator"/> would apply Root Motion. Applies that Root Motion to a different
        /// object instead.
        /// <para></para>
        /// This can be useful if for example the character's <see cref="Rigidbody"/> or
        /// <see cref="CharacterController"/> is on a parent of the <see cref="Animator"/> so that the model is kept
        /// separate from the character's mechanics.
        /// </summary>
        private void OnAnimatorMove()
        {
            if (!_Animancer.Animator.applyRootMotion)
                return;

            if (_MotionTransform != null)
            {
                _MotionTransform.position += _Animancer.Animator.deltaPosition;
                _MotionTransform.rotation *= _Animancer.Animator.deltaRotation;
            }
            else if (_MotionRigidbody != null)
            {
                _MotionRigidbody.MovePosition(_MotionRigidbody.position + _Animancer.Animator.deltaPosition);
                _MotionRigidbody.MoveRotation(_MotionRigidbody.rotation * _Animancer.Animator.deltaRotation);
            }
            else if (_MotionCharacterController != null)
            {
                _MotionCharacterController.Move(_Animancer.Animator.deltaPosition);
                _MotionCharacterController.transform.rotation *= _Animancer.Animator.deltaRotation;
            }
            else
            {
                // If we aren't retargeting, just let Unity apply the Root Motion normally.
                _Animancer.Animator.ApplyBuiltinRootMotion();
            }
        }

        /************************************************************************************************************************/
    }
}
