// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System.Collections;
using UnityEngine;

namespace Animancer.Examples.Basics
{
    /// <summary>Plays through a series of animations in a sequence using a coroutine.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/basics/sequence-coroutine">Sequence Coroutine</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Basics/SequenceCoroutine
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Basics - Sequence Coroutine")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Basics) + "/" + nameof(SequenceCoroutine))]
    public sealed class SequenceCoroutine : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private TextMesh _Text;
        [SerializeField] private ClipState.Transition[] _Animations;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            // This script would give a generic IndexOutOfRangeException if we did not set up any of the animations in the
            // Inspector and if we only added one animation it would actually work fine but simply do nothing more than
            // play that animation. So we can use Debug.Assert to ensure that there are at least two animations and give a
            // more helpful error message if there are not.

            // Debug.Assert is like saying "(condition) must be true, so give error (message) if it is not".

            Debug.Assert(_Animations.Length > 1, "Must have more than 1 animation assigned.");

            // If you check the definition of Debug.Assert, you can see that it has a [Conditional("UNITY_ASSERTIONS")]
            // attribute which means that any calls to that method will be automatically removed if that conditional
            // compilation symbol is not defined, as if we had put "#if UNITY_ASSERTIONS" before it and "#endif" after.

            // In the case of "UNITY_ASSERTIONS", it is defined in the Unity Editor and in Development Builds, but not in
            // regular release builds. This allows us to validate our data to catch issues during development without
            // affecting the performance of the released game.

            // Since we have access to the animation details, we can enforce restrictions on them as well. The script
            // would still work fine with a non-looping animation, but since we are using it as an Idle animation, it
            // would probably not be appropriate to just play it once and freeze on the last frame.
            Debug.Assert(_Animations[0].Clip.isLooping, "The first animation should be looping.");

            // With that out of the way, all we really want to do here is play the first animation.
            Play(0);
        }

        /************************************************************************************************************************/

        // Called by a UI Button.
        public void PlaySequence()
        {
            // Multiple instances of the same coroutine can run at the same time, but we do not want that so we stop
            // them any old ones first.
            StopAllCoroutines();
            StartCoroutine(CoroutineAnimationSequence());

            // StartCoroutine returns a Coroutine object which you can store in a field in case you want to stop that
            // specific one later on, but since we only have one we can just stop everything.
        }

        private IEnumerator CoroutineAnimationSequence()
        {
            // For each animation after the first (start at 1 instead of 0).
            for (int i = 1; i < _Animations.Length; i++)
            {
                // Play the animation and wait until it is done.
                var state = Play(i);
                yield return state;

                // We could have done that in one line:
                // yield return Play(i);
                // But having "Play" after "yield" does not look right so we used two to make it clear what we want.
            }

            // Then go back to the first animation (Idle).
            Play(0);
        }

        /************************************************************************************************************************/

        private AnimancerState Play(int index)
        {
            // We want to make sure that the text always shows the name of the current animation, so this method wraps
            // the regular AnimancerComponent.Play and we simply call this instead.

            // If we wanted other scripts to be able to play their own transitions, we could make this method public
            // and give it a ClipState.Transition parameter instead of just an `index`. But this way allows us to call
            // `Play(0)` and `Play(i)` instead of `Play(_Animations[0])` and `Play(_Animations[i])`.

            var animation = _Animations[index];
            _Text.text = animation.Clip.name;
            return _Animancer.Play(animation);
        }

        /************************************************************************************************************************/
    }
}
