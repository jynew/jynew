// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.StateMachines.Weapons
{
    /// <summary>
    /// Takes the root motion from the <see cref="Animator"/> attached to the same <see cref="GameObject"/> and applies
    /// it to a <see cref="Rigidbody"/> on a different object.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/weapons">Weapons</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.Weapons/RootMotionRedirect
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Weapons - Root Motion Redirect")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(Weapons) + "/" + nameof(RootMotionRedirect))]
    public sealed class RootMotionRedirect : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private Rigidbody _Rigidbody;
        [SerializeField] private Animator _Animator;

        /************************************************************************************************************************/

        private void OnAnimatorMove()
        {
            if (_Animator.applyRootMotion)
            {
                _Rigidbody.MovePosition(_Rigidbody.position + _Animator.deltaPosition);
                _Rigidbody.MoveRotation(_Rigidbody.rotation * _Animator.deltaRotation);
            }
        }

        /************************************************************************************************************************/
    }
}
