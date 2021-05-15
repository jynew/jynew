// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;

namespace Animancer.Examples.Jobs
{
    /// <summary>An example of how to use Animation Jobs in Animancer to apply physics based damping to certain bones.</summary>
    /// 
    /// <remarks>
    /// This example is based on Unity's
    /// <see href="https://github.com/Unity-Technologies/animation-jobs-samples">Animation Jobs Samples</see>.
    /// <para></para>
    /// This script sets up the job in place of
    /// <see href="https://github.com/Unity-Technologies/animation-jobs-samples/blob/master/Assets/animation-jobs-samples/Samples/Scripts/Damping/Damping.cs">
    /// Damping.cs</see>.
    /// <para></para>
    /// The <see cref="DampingJob"/> script is almost identical to the original 
    /// <see href="https://github.com/Unity-Technologies/animation-jobs-samples/blob/master/Assets/animation-jobs-samples/Runtime/AnimationJobs/DampingJob.cs">
    /// DampingJob.cs</see>.
    /// <para></para>
    /// The <see href="https://learn.unity.com/tutorial/working-with-animation-rigging">Animation Rigging</see> package
    /// has a damping system which is likely better than this example.
    /// </remarks>
    /// 
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/jobs/damping">Damping</see></example>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Jobs/Damping
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Jobs - Damping")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Jobs) + "/" + nameof(Damping))]
    public class Damping : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private Transform _EndBone;
        [SerializeField] private int _BoneCount = 1;

        /************************************************************************************************************************/

        private void Awake()
        {
            // Create the job and initialise all its arrays.
            // They are all Persistent because we want them to last for the full lifetime of the job.
            // Most of them can use UninitializedMemory which is faster because we will be immediately filling them.
            // But the velocities will use the default ClearMemory to make sure all the values start at zero.

            // Since we are about to use these values several times, we can shorten the following lines a bit by using constants:
            const Allocator Persistent = Allocator.Persistent;
            const NativeArrayOptions UninitializedMemory = NativeArrayOptions.UninitializedMemory;

            var job = new DampingJob()
            {
                jointHandles = new NativeArray<TransformStreamHandle>(_BoneCount, Persistent, UninitializedMemory),
                localPositions = new NativeArray<Vector3>(_BoneCount, Persistent, UninitializedMemory),
                localRotations = new NativeArray<Quaternion>(_BoneCount, Persistent, UninitializedMemory),
                positions = new NativeArray<Vector3>(_BoneCount, Persistent, UninitializedMemory),
                velocities = new NativeArray<Vector3>(_BoneCount, Persistent),
            };

            // Initialise the contents of the arrays for each bone.
            var animator = _Animancer.Animator;
            var bone = _EndBone;
            for (int i = _BoneCount - 1; i >= 0; i--)
            {
                job.jointHandles[i] = animator.BindStreamTransform(bone);
                job.localPositions[i] = bone.localPosition;
                job.localRotations[i] = bone.localRotation;
                job.positions[i] = bone.position;

                bone = bone.parent;
            }

            job.rootHandle = animator.BindStreamTransform(bone);

            // Add the job to Animancer's output.
            _Animancer.Playable.InsertOutputJob(job);

            // Make sure Animancer disposes the Native Arrays when it is destroyed so we don't leak memory.
            // If we were writing our own job rather than just using the sample, we could have it implement the
            // IDisposable interface to dispose its arrays so that we would only have to call ...Add(_Job); here.
            _Animancer.Playable.Disposables.Add(job.jointHandles);
            _Animancer.Playable.Disposables.Add(job.localPositions);
            _Animancer.Playable.Disposables.Add(job.localRotations);
            _Animancer.Playable.Disposables.Add(job.positions);
            _Animancer.Playable.Disposables.Add(job.velocities);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Ensures that the <see cref="_BoneCount"/> is positive and not larger than the number of bones between the
        /// <see cref="_EndBone"/> and the <see cref="Animator"/>.
        /// </summary>
        /// <remarks>
        /// Called by the Unity Editor in Edit Mode whenever an instance of this script is loaded or a value is changed
        /// in the Inspector.
        /// </remarks>
        private void OnValidate()
        {
            if (_BoneCount < 1)
            {
                _BoneCount = 1;
            }
            else if (_EndBone != null && _Animancer != null && _Animancer.Animator != null)
            {
                var root = _Animancer.Animator.transform;

                var bone = _EndBone;
                for (int i = 0; i < _BoneCount; i++)
                {
                    bone = bone.parent;
                    if (bone == root)
                    {
                        _BoneCount = i + 1;
                        break;
                    }
                    else if (bone == null)
                    {
                        _EndBone = null;
                        Debug.LogWarning("The End Bone must be a child of the Animator.");
                        break;
                    }
                }
            }
        }

        /************************************************************************************************************************/
#if !UNITY_2019_1_OR_NEWER
        /************************************************************************************************************************/

        private static bool _HasLoggedUnityVersionWarning;

        private void Start()
        {
            if (!_HasLoggedUnityVersionWarning && !_Animancer.Animator.isHuman)
            {
                _HasLoggedUnityVersionWarning = true;
                Debug.LogWarning("A bug in Unity versions older than 2019.1 prevents the Damping system from working on Generic Rigs." +
                    " The DampingJob relies on world positions but TransformStreamHandle.GetPosition returns local positions.", this);
            }
        }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
    }
}
