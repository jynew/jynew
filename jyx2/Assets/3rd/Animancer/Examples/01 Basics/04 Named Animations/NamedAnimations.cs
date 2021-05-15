// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using UnityEngine;

namespace Animancer.Examples.Basics
{
    /// <summary>
    /// Demonstrates how to use a <see cref="NamedAnimancerComponent"/> to play animations by name.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/basics/named-animations">Named Animations</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Basics/NamedAnimations
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Basics - Named Animations")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Basics) + "/" + nameof(NamedAnimations))]
    public sealed class NamedAnimations : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private NamedAnimancerComponent _Animancer;
        [SerializeField] private AnimationClip _Walk;
        [SerializeField] private AnimationClip _Run;

        /************************************************************************************************************************/
        // Idle.
        /************************************************************************************************************************/

        // Called by a UI Button.
        /// <summary>
        /// Plays the idle animation by name. This requires the animation to already have a state in the
        /// <see cref="NamedAnimancerComponent"/>, which has already been done in this example by adding it to the
        /// <see cref="NamedAnimancerComponent.Animations"/> list in the Inspector.
        /// <para></para>
        /// If it has not been added, this method will simply do nothing.
        /// </summary>
        public void PlayIdle()
        {
            _Animancer.TryPlay("Humanoid-Idle");
        }

        /************************************************************************************************************************/
        // Walk.
        /************************************************************************************************************************/

        // Called by a UI Button.
        /// <summary>
        /// Plays the walk animation by name. Unlike the idle animation, this one has not been added to the
        /// Inspector list so it will not exist and this method will log a warning unless you call
        /// <see cref="InitialiseWalkState"/> first.
        /// </summary>
        public void PlayWalk()
        {
            var state = _Animancer.TryPlay("Humanoid-Walk");

            if (state == null)
                Debug.LogWarning("No state has been registered with 'Humanoid-Walk' as its key yet.");

            // _Animancer.TryPlay(_Walk.name); would also work,
            // but if we are going to use the clip we should really just use _Animancer.Play(_Walk);
        }

        /************************************************************************************************************************/

        // Called by a UI Button.
        /// <summary>
        /// Creates a state for the walk animation so that <see cref="PlayWalk"/> can play it.
        /// </summary>
        /// <remarks>
        /// Calling this method more than once will throw an <see cref="ArgumentException"/> because a state already
        /// exists with the key it's trying to use (the animation's name).
        /// <para></para>
        /// If we wanted to allow repeated calls we could use
        /// <see cref="AnimancerLayer.GetOrCreateState(AnimationClip, bool)"/> instead, which would create a state the
        /// first time then return the same one every time after that.
        /// <para></para>
        /// If we wanted to actually create multiple states for the same animation, we would have to use the optional
        /// `key` parameter to specify a different key for each of them.
        /// </remarks>
        public void InitialiseWalkState()
        {
            _Animancer.States.Create(_Walk);
            Debug.Log("Created a state to play " + _Walk, this);
        }

        /************************************************************************************************************************/
        // Run.
        /************************************************************************************************************************/

        // Called by a UI Button.
        /// <summary>
        /// Plays the run animation using a direct reference to show that the ability to play animations by
        /// name in a <see cref="NamedAnimancerComponent"/> does not prevent it from also using direct references like
        /// the base <see cref="AnimancerComponent"/>.
        /// </summary>
        public void PlayRun()
        {
            _Animancer.Play(_Run);

            // What actually happens internally looks more like this:

            // object key = _Animancer.GetKey(_Run);
            // var state = _Animancer.GetOrCreate(key, _Run);
            // _Animancer.Play(state);

            // The base AnimancerComponent.GetKey returns the AnimationClip to use as its own key, but
            // NamedAnimancerComponent overrides it to instead return the clip's name. This is a bit less
            // efficient, but it allows us to use clips (like we are here) or names (like with the idle)
            // interchangeably.

            // After the 'Run' state has been created, we could do any of the following:
            // _Animancer.GetState(_Run) or GetState("Run").
            // _Animancer.Play(_Run) or Play("Run").
            // Same for CrossFade, and CrossFadeFromStart.
        }

        /************************************************************************************************************************/
    }
}
