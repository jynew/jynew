// Copyright Unity Technologies 2019 // https://github.com/Unity-Technologies/animation-jobs-samples //
// This file has not been modified other to put it in the Animancer.Examples.Jobs namespace and add this message and the comment on the struct.

using UnityEngine;

#if UNITY_2019_3_OR_NEWER
using UnityEngine.Animations;
#else
using UnityEngine.Experimental.Animations;
#endif

namespace Animancer.Examples.Jobs
{
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/jobs/two-bone-ik">Two Bone IK</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Jobs/TwoBoneIKJob
    /// 
    public struct TwoBoneIKJob : IAnimationJob
    {
        public TransformSceneHandle effector;

        public TransformStreamHandle top;
        public TransformStreamHandle mid;
        public TransformStreamHandle low;

        public void Setup(Animator animator, Transform topX, Transform midX, Transform lowX, Transform effectorX)
        {
            top = animator.BindStreamTransform(topX);
            mid = animator.BindStreamTransform(midX);
            low = animator.BindStreamTransform(lowX);

            effector = animator.BindSceneTransform(effectorX);
        }

        public void ProcessRootMotion(AnimationStream stream)
        {
        }

        public void ProcessAnimation(AnimationStream stream)
        {
            Solve(stream, top, mid, low, effector);
        }

        /// <summary>
        /// Returns the angle needed between v1 and v2 so that their extremities are
        /// spaced with a specific length.
        /// </summary>
        /// <returns>The angle between v1 and v2.</returns>
        /// <param name="aLen">The desired length between the extremities of v1 and v2.</param>
        /// <param name="v1">First triangle edge.</param>
        /// <param name="v2">Second triangle edge.</param>
        private static float TriangleAngle(float aLen, Vector3 v1, Vector3 v2)
        {
            float aLen1 = v1.magnitude;
            float aLen2 = v2.magnitude;
            float c = Mathf.Clamp((aLen1 * aLen1 + aLen2 * aLen2 - aLen * aLen) / (aLen1 * aLen2) / 2.0f, -1.0f, 1.0f);
            return Mathf.Acos(c);
        }

        private static void Solve(AnimationStream stream, TransformStreamHandle topHandle, TransformStreamHandle midHandle, TransformStreamHandle lowHandle, TransformSceneHandle effectorHandle)
        {
            Quaternion aRotation = topHandle.GetRotation(stream);
            Quaternion bRotation = midHandle.GetRotation(stream);
            Quaternion eRotation = effectorHandle.GetRotation(stream);

            Vector3 aPosition = topHandle.GetPosition(stream);
            Vector3 bPosition = midHandle.GetPosition(stream);
            Vector3 cPosition = lowHandle.GetPosition(stream);
            Vector3 ePosition = effectorHandle.GetPosition(stream);

            Vector3 ab = bPosition - aPosition;
            Vector3 bc = cPosition - bPosition;
            Vector3 ac = cPosition - aPosition;
            Vector3 ae = ePosition - aPosition;

            float abcAngle = TriangleAngle(ac.magnitude, ab, bc);
            float abeAngle = TriangleAngle(ae.magnitude, ab, bc);
            float angle = (abcAngle - abeAngle) * Mathf.Rad2Deg;
            Vector3 axis = Vector3.Cross(ab, bc).normalized;

            Quaternion fromToRotation = Quaternion.AngleAxis(angle, axis);

            Quaternion worldQ = fromToRotation * bRotation;
            midHandle.SetRotation(stream, worldQ);

            cPosition = lowHandle.GetPosition(stream);
            ac = cPosition - aPosition;
            Quaternion fromTo = Quaternion.FromToRotation(ac, ae);
            topHandle.SetRotation(stream, fromTo * aRotation);

            lowHandle.SetRotation(stream, eRotation);
        }
    }
}
