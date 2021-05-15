// Copyright Unity Technologies 2019 // https://github.com/Unity-Technologies/animation-jobs-samples //
// The original file can be downloaded from https://github.com/Unity-Technologies/animation-jobs-samples/blob/master/Assets/animation-jobs-samples/Runtime/AnimationJobs/DampingJob.cs
// This file has been modified:
// - Moved into the Animancer.Examples.Jobs namespace.
// - Removed the contents of ProcessRootMotion since it is unnecessary.

#pragma warning disable IDE0054 // Use compound assignment

using Unity.Collections;
using UnityEngine;
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Animations;
#else
using UnityEngine.Experimental.Animations;
#endif

namespace Animancer.Examples.Jobs
{
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/jobs/damping">Damping</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Jobs/DampingJob
    /// 
    public struct DampingJob : IAnimationJob
    {
        public TransformStreamHandle rootHandle;
        public NativeArray<TransformStreamHandle> jointHandles;
        public NativeArray<Vector3> localPositions;
        public NativeArray<Quaternion> localRotations;
        public NativeArray<Vector3> positions;
        public NativeArray<Vector3> velocities;

        /// <summary>
        /// Transfer the root position and rotation through the graph.
        /// </summary>
        /// <param name="stream">The animation stream</param>
        public void ProcessRootMotion(AnimationStream stream)
        {
            // This was in the original sample, but it causes problems if the character is a child of a moving object.
            // There is no need for this method to do anything in order to support root motion.

            //// Get root position and rotation.
            //var rootPosition = rootHandle.GetPosition(stream);
            //var rootRotation = rootHandle.GetRotation(stream);

            //// The root always follow the given position and rotation.
            //rootHandle.SetPosition(stream, rootPosition);
            //rootHandle.SetRotation(stream, rootRotation);
        }

        /// <summary>
        /// Procedurally generate the joints rotation.
        /// </summary>
        /// <param name="stream">The animation stream</param>
        public void ProcessAnimation(AnimationStream stream)
        {
            if (jointHandles.Length < 2)
                return;

            ComputeDampedPositions(stream);
            ComputeJointLocalRotations(stream);
        }

        /// <summary>
        /// Compute the new global positions of the joints.
        ///
        /// The position of the first joint is driven by the root's position, and
        /// then the other joints positions are recomputed in order to follow their
        /// initial local positions, smoothly.
        ///
        /// Algorithm breakdown:
        ///     1. Compute the target position;
        ///     2. Damp this target position based on the current position;
        ///     3. Constrain the damped position to the joint initial length;
        ///     4. Iterate on the next joint.
        /// </summary>
        /// <param name="stream">The animation stream</param>
        private void ComputeDampedPositions(AnimationStream stream)
        {
            // Get root position and rotation.
            var rootPosition = rootHandle.GetPosition(stream);
            var rootRotation = rootHandle.GetRotation(stream);

            // The first non-root joint follows the root position,
            // but its rotation is damped (see ComputeJointLocalRotations).
            var parentPosition = rootPosition + rootRotation * localPositions[0];
            var parentRotation = rootRotation * localRotations[0];
            positions[0] = parentPosition;
            for (var i = 1; i < jointHandles.Length; ++i)
            {
                // The target position is the global position, without damping.
                var newPosition = parentPosition + (parentRotation * localPositions[i]);

                // Apply damping on this target.
                var velocity = velocities[i];
                newPosition = Vector3.SmoothDamp(positions[i], newPosition, ref velocity, 0.15f, Mathf.Infinity, stream.deltaTime);

                // Apply constraint: keep original length between joints.
                newPosition = parentPosition + (newPosition - parentPosition).normalized * localPositions[i].magnitude;

                // Save new velocity and position for next frame.
                velocities[i] = velocity;
                positions[i] = newPosition;

                // Current joint is now the parent of the next joint.
                parentPosition = newPosition;
                parentRotation = parentRotation * localRotations[i];
            }
        }

        /// <summary>
        /// Compute the new local rotations of the joints.
        ///
        /// Based on the global positions computed in ComputeDampedPositions,
        /// recompute the local rotation of each joint.
        ///
        /// Algorithm breakdown:
        ///     1. Compute the rotation between the current and new directions of the joint;
        ///     2. Apply this rotation on the current joint rotation;
        ///     3. Compute the local rotation and set it in the stream;
        ///     4. Iterate on the next joint.
        /// </summary>
        /// <param name="stream">The animation stream</param>
        private void ComputeJointLocalRotations(AnimationStream stream)
        {
            var parentRotation = rootHandle.GetRotation(stream);
            for (var i = 0; i < jointHandles.Length - 1; ++i)
            {
                // Get the current joint rotation.
                var rotation = parentRotation * localRotations[i];

                // Get the current joint direction.
                var direction = (rotation * localPositions[i + 1]).normalized;

                // Get the wanted joint direction.
                var newDirection = (positions[i + 1] - positions[i]).normalized;

                // Compute the rotation from the current direction to the new direction.
                var currentToNewRotation = Quaternion.FromToRotation(direction, newDirection);

                // Pre-rotate the current rotation, to get the new global rotation.
                rotation = currentToNewRotation * rotation;

                // Set the new local rotation.
                var newLocalRotation = Quaternion.Inverse(parentRotation) * rotation;
                jointHandles[i].SetLocalRotation(stream, newLocalRotation);

                // Set the new parent for the next joint.
                parentRotation = rotation;
            }
        }
    }
}
