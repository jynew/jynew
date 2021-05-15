// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Locomotion
{
    /// <summary>
    /// An example of how you can use a <see cref="LinearMixerState"/> to mix a set of animations based on a
    /// <see cref="Speed"/> parameter.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/locomotion/linear-blending">Linear Blending</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Locomotion/LinearMixerLocomotion
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Locomotion - Linear Mixer Locomotion")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Locomotion) + "/" + nameof(LinearMixerLocomotion))]
    public sealed class LinearMixerLocomotion : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private LinearMixerTransition _Mixer;

        private LinearMixerState _State;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            _Animancer.Play(_Mixer);
            _State = _Mixer.Transition.State;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Set by a <see cref="UnityEngine.Events.UnityEvent"/> on a <see cref="UnityEngine.UI.Slider"/>.
        /// </summary>
        public float Speed
        {
            get => _State.Parameter;
            set => _State.Parameter = value;
        }

        /************************************************************************************************************************/
    }
}
