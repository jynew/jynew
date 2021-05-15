// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Basics
{
    /// <summary>Demonstrates the differences between various ways of playing and fading between animations.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/basics/playing-and-fading">Playing and Fading</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Basics/PlayingAndFading
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Basics - Playing and Fading")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Basics) + "/" + nameof(PlayingAndFading))]
    public sealed class PlayingAndFading : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private AnimationClip _Idle;
        [SerializeField] private AnimationClip _Action;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            _Animancer.Play(_Idle);
        }

        /************************************************************************************************************************/

        // Called by a UI Button.
        public void Play()
        {
            // Immediately snap from the previous pose to the start of the action.

            _Animancer.Play(_Action);

            // If the action was already playing, it will continue from its current time.

            // When the animation ends it will freeze in its final pose.
        }

        // Called by a UI Button.
        public void PlayThenIdle()
        {
            // Play then return to the Idle animation when the action is finished.

            _Animancer.Play(_Action).Events.OnEnd = () => _Animancer.Play(_Idle);

            // Here we are using a "Lambda Expression" to declare the callback method inside the current method.
            // In this case we could have assigned OnEnable to the event since it does the same thing:
            // _Animancer.Play(_Action).Events.OnEnd = OnEnable;
            // But that is not always so convenient and "OnEnable" is not really an appropriate name for what it does.

            // We could have used multiple lines like so:
            // var state = _Animancer.Play(_Action);
            // state.Events.OnEnd = () => _Animancer.Play(_Idle);
            // But since we only want to do one thing with the state, we can just do it on one line.

            // Note that the events are all automatically cleared whenever a new animation is played.
            // This ensures that the above Play method will not have to worry about any of these other methods that
            // might have set their own events.
        }

        // Called by a UI Button.
        public void PlayFromStart()
        {
            // Play and make sure it is at the start instead of allowing it to continue from its current time.

            _Animancer.Play(_Action).Time = 0;
        }

        // Called by a UI Button.
        public void PlayFromStartThenIdle()
        {
            // Combine both of the above.

            var state = _Animancer.Play(_Action);
            state.Time = 0;
            state.Events.OnEnd = () => _Animancer.Play(_Idle);
        }

        /************************************************************************************************************************/

        // Called by a UI Button.
        public void CrossFade()
        {
            // Smoothly transition to the action over 0.25 seconds.

            _Animancer.Play(_Action, 0.25f);
        }

        // Called by a UI Button.
        public void CrossFadeThenIdle()
        {
            // Same as PlayThenIdle, but since the line is getting a bit long we can split it.
            // Note how the first line does not have a semicolon at the end.

            _Animancer.Play(_Action, 0.25f)
                .Events.OnEnd = () => _Animancer.Play(_Idle, 0.25f);
        }

        // Called by a UI Button.
        public void BadCrossFadeFromStart()
        {
            // Unlike PlayFromStart, setting the Time is not good when cross fading because it prevents a smooth
            // transition from the previous pose into the new animation.

            _Animancer.Play(_Action, 0.25f).Time = 0;
        }

        // Called by a UI Button.
        public void GoodCrossFadeFromStart()
        {
            // Instead, we can use FadeMode.FromStart to ensure that it is smooth.
            // See the documentation of FadeMode.FromStart for details.

            _Animancer.Play(_Action, 0.25f, FadeMode.FromStart);
        }

        // Called by a UI Button.
        public void GoodCrossFadeFromStartThenIdle()
        {
            // For completeness, combine both of the above.

            _Animancer.Play(_Action, 0.25f, FadeMode.FromStart)
                .Events.OnEnd = () => _Animancer.Play(_Idle, 0.25f);
        }

        /************************************************************************************************************************/
    }
}
