// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0414 // Field is assigned but its value is never used.
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.InverseKinematics
{
    /// <summary>
    /// Demonstrates how to use Unity's Inverse Kinematics (IK) system to adjust a character's feet according to the
    /// terrain they are moving over.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/ik/uneven-ground">Uneven Ground</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.InverseKinematics/RaycastFootIK
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Inverse Kinematics - Raycast Foot IK")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(InverseKinematics) + "/" + nameof(RaycastFootIK))]
    public sealed class RaycastFootIK : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private AnimationClip _Animation;
        [SerializeField] private float _RaycastOriginY = 0.5f;
        [SerializeField] private float _RaycastEndY = -0.2f;

        /************************************************************************************************************************/

        private Transform _LeftFoot;
        private Transform _RightFoot;

        private AnimatedFloat _FootWeights;

        /************************************************************************************************************************/

        /// <summary>Public property for a UI Toggle to enable or disable the IK.</summary>
        public bool ApplyAnimatorIK
        {
            get => _Animancer.Layers[0].ApplyAnimatorIK;
            set => _Animancer.Layers[0].ApplyAnimatorIK = value;
        }

        /************************************************************************************************************************/

        private void Awake()
        {
            _LeftFoot = _Animancer.Animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            _RightFoot = _Animancer.Animator.GetBoneTransform(HumanBodyBones.RightFoot);

            _FootWeights = new AnimatedFloat(_Animancer, "LeftFootIK", "RightFootIK");

            _Animancer.Play(_Animation);

            // Tell Unity that OnAnimatorIK needs to be called every frame.
            ApplyAnimatorIK = true;

        }

        /************************************************************************************************************************/

        // Note that due to limitations in the Playables API, Unity will always call this method with layerIndex = 0.
        private void OnAnimatorIK(int layerIndex)
        {
            // _FootWeights[0] is the first property we specified in Awake: "LeftFootIK".
            // _FootWeights[1] is the second property we specified in Awake: "RightFootIK".
            UpdateFootIK(_LeftFoot, AvatarIKGoal.LeftFoot, _FootWeights[0], _Animancer.Animator.leftFeetBottomHeight);
            UpdateFootIK(_RightFoot, AvatarIKGoal.RightFoot, _FootWeights[1], _Animancer.Animator.rightFeetBottomHeight);
        }

        /************************************************************************************************************************/

        private void UpdateFootIK(Transform footTransform, AvatarIKGoal goal, float weight, float footBottomHeight)
        {
            var animator = _Animancer.Animator;
            animator.SetIKPositionWeight(goal, weight);
            animator.SetIKRotationWeight(goal, weight);

            if (weight == 0)
                return;

            // Get the local up direction of the foot.
            var rotation = animator.GetIKRotation(goal);
            var localUp = rotation * Vector3.up;

            var position = footTransform.position;
            position += localUp * _RaycastOriginY;

            var distance = _RaycastOriginY - _RaycastEndY;

            if (Physics.Raycast(position, -localUp, out var hit, distance))
            {
                // Use the hit point as the desired position.
                position = hit.point;
                position += localUp * footBottomHeight;
                animator.SetIKPosition(goal, position);

                // Use the hit normal to calculate the desired rotation.
                var rotAxis = Vector3.Cross(localUp, hit.normal);
                var angle = Vector3.Angle(localUp, hit.normal);
                rotation = Quaternion.AngleAxis(angle, rotAxis) * rotation;

                animator.SetIKRotation(goal, rotation);
            }
            else// Otherwise simply stretch the leg out to the end of the ray.
            {
                position += localUp * (footBottomHeight - distance);
                animator.SetIKPosition(goal, position);
            }
        }

        /************************************************************************************************************************/
    }
}
