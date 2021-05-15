// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //
// Compare to the original script: https://github.com/Unity-Technologies/animation-jobs-samples/blob/master/Assets/animation-jobs-samples/Samples/Scripts/TwoBoneIK/TwoBoneIK.cs

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Jobs
{
    /// <summary>
    /// An example of how to use Animation Jobs in Animancer to apply simple two bone Inverse Kinematics, even to
    /// Generic Rigs which are not supported by Unity's inbuilt IK system.
    /// </summary>
    /// 
    /// <remarks>
    /// This example is based on Unity's
    /// <see href="https://github.com/Unity-Technologies/animation-jobs-samples">Animation Jobs Samples</see>.
    /// <para></para>
    /// This script sets up the job in place of
    /// <see href="https://github.com/Unity-Technologies/animation-jobs-samples/blob/master/Assets/animation-jobs-samples/Samples/Scripts/TwoBoneIK/TwoBoneIK.cs">
    /// TwoBoneIK.cs</see>.
    /// <para></para>
    /// The <see cref="TwoBoneIKJob"/> script is almost identical to the original 
    /// <see href="https://github.com/Unity-Technologies/animation-jobs-samples/blob/master/Assets/animation-jobs-samples/Runtime/AnimationJobs/TwoBoneIKJob.cs">
    /// TwoBoneIKJob.cs</see>.
    /// <para></para>
    /// The <see href="https://learn.unity.com/tutorial/working-with-animation-rigging">Animation Rigging</see> package
    /// has an IK system which is much better than this example.
    /// </remarks>
    /// 
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/jobs/two-bone-ik">Two Bone IK</see></example>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Jobs/TwoBoneIK
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Jobs - Two Bone IK")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Jobs) + "/" + nameof(TwoBoneIK))]
    public class TwoBoneIK : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private Transform _EndBone;
        [SerializeField] private Transform _Target;

        /************************************************************************************************************************/

        private void Awake()
        {
            // Get the bones we want to affect.
            var midBone = _EndBone.parent;
            var topBone = midBone.parent;

            // Create the job and setup its details.
            var twoBoneIKJob = new TwoBoneIKJob();
            twoBoneIKJob.Setup(_Animancer.Animator, topBone, midBone, _EndBone, _Target);

            // Add it to Animancer's output.
            _Animancer.Playable.InsertOutputJob(twoBoneIKJob);
        }

        /************************************************************************************************************************/
    }
}
