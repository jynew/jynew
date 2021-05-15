// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.FineControl
{
    /// <summary>
    /// Demonstrates how to play a single "Wake Up" animation forwards to wake up and backwards to go back to sleep.
    /// </summary>
    /// 
    /// <remarks>
    /// This is an abstract class which is inherited by <see cref="SpiderBotSimple"/> and
    /// <see cref="Locomotion.SpiderBotAdvanced"/>, meaning that you cannot attach this script to an object (because it
    /// would be useless on its own) and both of those scripts get to share its functionality without needing to copy
    /// the same methods into each of them.
    /// </remarks>
    /// 
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fine-control/spider-bot">Spider Bot</see></example>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.FineControl/SpiderBot
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Fine Control - Spider Bot")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(FineControl) + "/" + nameof(SpiderBot))]
    public abstract class SpiderBot : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private AnimancerComponent _Animancer;
        public AnimancerComponent Animancer => _Animancer;

        [SerializeField] private ClipState.Transition _WakeUp;
        [SerializeField] private ClipState.Transition _Sleep;

        private bool _WasMoving;

        /************************************************************************************************************************/

        protected abstract bool IsMoving { get; }

        protected abstract ITransition MovementAnimation { get; }

        /************************************************************************************************************************/

        protected virtual void Awake()
        {
            // Start paused at the beginning of the animation.
            _Animancer.Play(_WakeUp);
            _Animancer.Evaluate();
            _Animancer.Playable.PauseGraph();

            // Initialise the OnEnd events here so we don't allocate garbage every time they are used.
            _WakeUp.Events.OnEnd = () => _Animancer.Play(MovementAnimation);
            _Sleep.Events.OnEnd = _Animancer.Playable.PauseGraph;
        }

        /************************************************************************************************************************/

        protected virtual void Update()
        {
            if (IsMoving)
            {
                if (!_WasMoving)
                {
                    _WasMoving = true;

                    // Make sure the graph is unpaused (because we pause it when going back to sleep).
                    _Animancer.Playable.UnpauseGraph();
                    _Animancer.Play(_WakeUp);
                }
            }
            else
            {
                if (_WasMoving)
                {
                    _WasMoving = false;

                    var state = _Animancer.Play(_Sleep);

                    // If it was past the last frame, skip back to the last frame now that it is playing backwards.
                    // Otherwise just play backwards from the current time.
                    if (state.NormalizedTime > 1)
                        state.NormalizedTime = 1;

                    // If we did not initialise the OnEnd event in Awake, we could set it here:
                    // state.OnEnd = _Animancer.Playable.PauseGraph;
                }
            }
        }

        /************************************************************************************************************************/
    }
}
