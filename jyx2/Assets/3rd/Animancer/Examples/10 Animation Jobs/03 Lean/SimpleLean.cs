// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;

namespace Animancer.Examples.Jobs
{
    /// <summary>
    /// A wrapper that manages an Animation Job (the <see cref="Job"/> struct nested inside this class) which rotates a
    /// set of bones to allow the character to dynamically lean over independantly of their animations.
    /// </summary>
    /// 
    /// <remarks>
    /// The axis around which the bones are rotated can be set to achieve several different effects:
    /// <list type="number">
    /// <item>The right axis allows bending forwards and backwards.</item>
    /// <item>The up axis allows turning to either side.</item>
    /// <item>The forward axis allows leaning to either side.</item>
    /// </list>
    /// <see cref="https://github.com/KybernetikGames/animancer/issues/48#issuecomment-632336377">
    /// This script is based on an implementation by ted-hou on GitHub.</see>
    /// </remarks>
    /// 
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/jobs/lean">Lean</see></example>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Jobs/SimpleLean
    /// 
    public sealed class SimpleLean : AnimancerJob<SimpleLean.Job>, IDisposable
    {
        /************************************************************************************************************************/
        #region Initialisation
        /************************************************************************************************************************/

        public SimpleLean(AnimancerPlayable animancer, Vector3 axis, NativeArray<TransformStreamHandle> leanBones)
        {
            var animator = animancer.Component.Animator;

            _Job = new Job
            {
                root = animator.BindStreamTransform(animator.transform),
                bones = leanBones,
                axis = axis,
                angle = AnimancerUtilities.CreateNativeReference<float>(),
            };

            CreatePlayable(animancer);

            animancer.Disposables.Add(this);
        }

        /************************************************************************************************************************/

        public SimpleLean(AnimancerPlayable animancer)
            : this(animancer, Vector3.forward, GetDefaultHumanoidLeanBones(animancer.Component.Animator))
        { }

        /************************************************************************************************************************/

        public static NativeArray<TransformStreamHandle> GetDefaultHumanoidLeanBones(Animator animator)
        {
            var leanBones = new NativeArray<TransformStreamHandle>(2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            leanBones[0] = animator.BindStreamTransform(animator.GetBoneTransform(HumanBodyBones.Spine));
            leanBones[1] = animator.BindStreamTransform(animator.GetBoneTransform(HumanBodyBones.Chest));
            return leanBones;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Control
        /************************************************************************************************************************/
        // The Axis probably won't change often so the setter can just get the job data and change it.
        /************************************************************************************************************************/

        public Vector3 Axis
        {
            get => _Job.axis;
            set
            {
                if (_Job.axis == value)
                    return;

                _Job.axis = value;
                _Playable.SetJobData(_Job);
            }
        }

        /************************************************************************************************************************/
        // But since the Angle could change all the time, we can exploit the fact that arrays are actualy references to avoid
        // copying the entire struct out of the job playable then back in every time.
        /************************************************************************************************************************/

        public float Angle
        {
            get => _Job.angle[0];
            set => _Job.angle[0] = value;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Clean Up
        /************************************************************************************************************************/

        void IDisposable.Dispose() => Dispose();

        /// <summary>Cleans up the <see cref="NativeArray{T}"/>s.</summary>
        /// <remarks>Called by <see cref="AnimancerPlayable.OnPlayableDestroy"/>.</remarks>
        private void Dispose()
        {
            if (_Job.angle.IsCreated)
                _Job.angle.Dispose();

            if (_Job.bones.IsCreated)
                _Job.bones.Dispose();
        }

        /// <summary>Destroys the <see cref="_Playable"/> and restores the graph connection it was intercepting.</summary>
        public override void Destroy()
        {
            Dispose();
            base.Destroy();
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Job
        /************************************************************************************************************************/

        /// <summary>An <see cref="IAnimationJob"/> that applies a lean effect to an <see cref="AnimationStream"/>.</summary>
        /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/jobs/lean">Lean</see></example>
        public struct Job : IAnimationJob
        {
            /************************************************************************************************************************/

            public TransformStreamHandle root;
            public NativeArray<TransformStreamHandle> bones;
            public Vector3 axis;
            public NativeArray<float> angle;

            /************************************************************************************************************************/

            public void ProcessRootMotion(AnimationStream stream) { }

            /************************************************************************************************************************/

            public void ProcessAnimation(AnimationStream stream)
            {
                var angle = this.angle[0] / bones.Length;
                var worldAxis = root.GetRotation(stream) * axis;
                var offset = Quaternion.AngleAxis(angle, worldAxis);

                for (int i = bones.Length - 1; i >= 0; i--)
                {
                    var bone = bones[i];
                    bone.SetRotation(stream, offset * bone.GetRotation(stream));
                }
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}
