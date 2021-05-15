// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Basics
{
    /// <summary>
    /// Starts with an idle animation and performs an action when the user clicks the mouse, then returns to idle.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/basics/quick-play">Quick Play</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Basics/PlayAnimationOnClick
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Basics - Play Animation On Click")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Basics) + "/" + nameof(PlayAnimationOnClick))]
    public sealed class PlayAnimationOnClick : MonoBehaviour
    {
        /************************************************************************************************************************/

        // Without Animancer, you would reference an Animator component to control animations.
        // But with Animancer, you reference an AnimancerComponent instead.
        [SerializeField] private AnimancerComponent _Animancer;

        // Without Animancer, you would create an Animator Controller to define animation states and transitions.
        // But with Animancer, you can directly reference the AnimationClips you want to play.
        [SerializeField] private AnimationClip _Idle;
        [SerializeField] private AnimationClip _Action;

        /************************************************************************************************************************/

        /// <summary>On startup, play the idle animation.</summary>
        private void OnEnable()
        {
            // On startup, play the idle animation.
            _Animancer.Play(_Idle);
        }

        /************************************************************************************************************************/

        private void Update()
        {
            // Every update, check if the user has clicked the left mouse button (mouse button 0).
            if (Input.GetMouseButtonDown(0))
            {
                // If they have, then play the action animation.
                var state = _Animancer.Play(_Action);

                // The Play method returns the AnimancerState which manages that animation so you can access and
                // control various details, for example:
                // state.Time = 1;// Skip 1 second into the animation.
                // state.NormalizedTime = 0.5f;// Skip halfway into the animation.
                // state.Speed = 2;// Play the animation twice as fast.

                // In this case, we just want it to call the OnActionEnd method (see below) when the animation ends.
                state.Events.OnEnd = OnActionEnd;
            }
        }

        /************************************************************************************************************************/

        private void OnActionEnd()
        {
            // Now that the action is done, go back to idle. But instead of snapping to the new animation instantly,
            // tell it to fade gradually over 0.25 seconds so that it transitions smoothly.
            _Animancer.Play(_Idle, 0.25f);
        }

        /************************************************************************************************************************/
    }
}
