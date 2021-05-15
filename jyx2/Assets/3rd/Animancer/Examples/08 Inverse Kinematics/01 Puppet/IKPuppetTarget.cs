// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.InverseKinematics
{
    /// <summary>An object for one of a character's limbs to aim at using Inverse Kinematics (IK).</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/ik/puppet">Puppet</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.InverseKinematics/IKPuppetTarget
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Inverse Kinematics - IK Puppet Target")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(InverseKinematics) + "/" + nameof(IKPuppetTarget))]
    public sealed class IKPuppetTarget : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private AvatarIKGoal _Type;
        [SerializeField, Range(0, 1)] private float _PositionWeight = 1;
        [SerializeField, Range(0, 1)] private float _RotationWeight = 0;

        /************************************************************************************************************************/

        public void UpdateAnimatorIK(Animator animator)
        {
            animator.SetIKPositionWeight(_Type, _PositionWeight);
            animator.SetIKRotationWeight(_Type, _RotationWeight);

            animator.SetIKPosition(_Type, transform.position);
            animator.SetIKRotation(_Type, transform.rotation);
        }

        /************************************************************************************************************************/
    }
}
