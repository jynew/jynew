// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.InverseKinematics
{
    /// <summary>Demonstrates how to use Unity's Inverse Kinematics (IK) system to move a character's limbs.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/ik/puppet">Puppet</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.InverseKinematics/IKPuppet
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Inverse Kinematics - IK Puppet")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(InverseKinematics) + "/" + nameof(IKPuppet))]
    public sealed class IKPuppet : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private Transform _BodyTarget;
        [SerializeField] private IKPuppetLookTarget _LookTarget;
        [SerializeField] private IKPuppetTarget[] _IKTargets;

        /************************************************************************************************************************/

        private void Awake()
        {
            // Tell Unity that we want it to call OnAnimatorIK for states on this layer:
            _Animancer.Layers[0].ApplyAnimatorIK = true;
        }

        /************************************************************************************************************************/

        private void OnAnimatorIK(int layerIndex)
        {
            _Animancer.Animator.bodyPosition = _BodyTarget.position;
            _Animancer.Animator.bodyRotation = _BodyTarget.rotation;

            _LookTarget.UpdateAnimatorIK(_Animancer.Animator);

            for (int i = 0; i < _IKTargets.Length; i++)
            {
                _IKTargets[i].UpdateAnimatorIK(_Animancer.Animator);
            }
        }

        /************************************************************************************************************************/
    }
}
