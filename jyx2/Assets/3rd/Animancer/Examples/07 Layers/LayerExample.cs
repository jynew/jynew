// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Layers
{
    /// <summary>Demonstrates how to use layers to play multiple animations at once on different body parts.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/layers">Layers</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Layers/LayerExample
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Layers - Layer Example")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Layers) + "/" + nameof(LayerExample))]
    public sealed class LayerExample : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _BasicAnimancer;
        [SerializeField] private AnimancerComponent _LayeredAnimancer;

        [SerializeField] private AnimationClip _Idle;
        [SerializeField] private AnimationClip _Run;
        [SerializeField] private AnimationClip _Action;

        [SerializeField] private AvatarMask _ActionMask;

        /************************************************************************************************************************/

        private const int BaseLayer = 0;
        private const int ActionLayer = 1;

        private const float FadeDuration = 0.25f;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            // Idle on default layer 0.
            _BasicAnimancer.Play(_Idle);
            _LayeredAnimancer.Play(_Idle);

            // Set the mask for layer 1 (this automatically creates the layer).
            _LayeredAnimancer.Layers[ActionLayer].SetMask(_ActionMask);

            // Since we set a mask it will use the name of the mask in the Inspector by default. But we can also
            // replace it with a custom name. Either way, layer names are only used in the Inspector and any calls to
            // this method will be compiled out of runtime builds.
            _LayeredAnimancer.Layers[ActionLayer].SetDebugName("Action Layer");
        }

        /************************************************************************************************************************/

        private bool _IsRunning;

        public void ToggleRunning()
        {
            // Swap between true and false.
            _IsRunning = !_IsRunning;

            // Determine which animation to play.
            var animation = _IsRunning ? _Run : _Idle;

            // Play it.
            _BasicAnimancer.Play(animation, FadeDuration);
            _LayeredAnimancer.Play(animation, FadeDuration);
        }

        /************************************************************************************************************************/

        public void PerformAction()
        {
            // Basic.
            var state = _BasicAnimancer.Play(_Action, FadeDuration);
            state.Events.OnEnd = () => _BasicAnimancer.Play(_IsRunning ? _Run : _Idle, FadeDuration);

            // Layered.

            // When running, perform the action on the ActionLayer (1) then fade that layer back out.
            if (_IsRunning)
            {
                state = _LayeredAnimancer.Layers[ActionLayer].Play(_Action, FadeDuration, FadeMode.FromStart);
                state.Events.OnEnd = () => _LayeredAnimancer.Layers[ActionLayer].StartFade(0, FadeDuration);
            }
            else// Otherwise perform the action on the BaseLayer (0) then return to idle.
            {
                state = _LayeredAnimancer.Layers[BaseLayer].Play(_Action, FadeDuration, FadeMode.FromStart);
                state.Events.OnEnd = () => _LayeredAnimancer.Play(_Idle, FadeDuration);
            }
        }

        /************************************************************************************************************************/
    }
}
