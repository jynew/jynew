// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.AnimatorControllers
{
    /// <summary>Demonstrates how to play Animator Controllers alongside Animancer.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/animator-controllers">Animator Controllers</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.AnimatorControllers/HybridBasics
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Animator Controllers - Hybrid Basics")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(AnimatorControllers) + "/" + nameof(HybridBasics))]
    public sealed class HybridBasics : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private AnimationClip _SeparateAnimation;

        /************************************************************************************************************************/

        private static readonly int MoveParameterID = Animator.StringToHash("Move");

        // Called by a UI Toggle.
        public void SetMove(bool move)
        {
            // Call SetBool on the HybridAnimancerComponent:
            if (_Animancer is HybridAnimancerComponent hybrid)
                hybrid.SetBool(MoveParameterID, move);
            else// Or on the Animator:
                _Animancer.Animator.SetBool(MoveParameterID, move);
        }

        /************************************************************************************************************************/

        // Called by a UI Button.
        public void PlaySeparateAnimation()
        {
            _Animancer.Play(_SeparateAnimation);
        }

        /************************************************************************************************************************/

        // Called by a UI Button.
        public void PlayAnimatorController()
        {
            // Play the Animator Controller on the HybridAnimancerComponent:
            if (_Animancer is HybridAnimancerComponent hybrid)
                hybrid.Play(hybrid.Controller, 0);
            else// Or Stop the AnimancerComponent to let the native Animator Controller resume control:
                _Animancer.Stop();
        }

        /************************************************************************************************************************/

        // Called by a UI Button.
        public void FadeSeparateAnimation()
        {
            _Animancer.Play(_SeparateAnimation, 0.25f);
        }

        /************************************************************************************************************************/

        // Called by a UI Button.
        public void FadeAnimatorController()
        {
            // Play the Animator Controller on the HybridAnimancerComponent:
            if (_Animancer is HybridAnimancerComponent hybrid)
                hybrid.PlayController();
            else// Or fade out the Animancer Layer to let the native Animator Controller resume control:
                _Animancer.Layers[0].StartFade(0, 0.25f);
        }

        /************************************************************************************************************************/
    }
}
