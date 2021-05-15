// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.FineControl
{
    /// <summary>
    /// An <see cref="IInteractable"/> door which toggles between open and closed when something interacts with it.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fine-control/doors">Doors</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.FineControl/Door
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Fine Control - Door")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(FineControl) + "/" + nameof(Door))]
    [SelectionBase]
    public sealed class Door : MonoBehaviour, IInteractable
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private AnimationClip _Open;

        [SerializeField, Range(0, 1)]
        private float _Openness;

        /************************************************************************************************************************/

        private void Awake()
        {
            // Apply the starting state and pause the graph.
            var state = _Animancer.Play(_Open);
            state.NormalizedTime = _Openness;
            _Animancer.Evaluate();
            _Animancer.Playable.PauseGraph();

            // And also pause it whenever the animation finishes to save performance.
            state.Events.OnEnd = _Animancer.Playable.PauseGraph;

            // Normally the OnEnd event would be cleared whenever we play a new animation, but since there is only one
            // animation in this example we just leave it playing and pause/unpause the graph instead.
        }

        /************************************************************************************************************************/

        /// <summary>[<see cref="IInteractable"/>]
        /// Toggles this door between open and closed.
        /// </summary>
        public void Interact()
        {
            // Get the state to set its speed (or we could have just kept the state from Awake).
            var state = _Animancer.States[_Open];

            // If currently near closed, play the animation forwards.
            if (_Openness < 0.5f)
            {
                state.Speed = 1;
                _Openness = 1;
            }
            else// Otherwise play it backwards.
            {
                state.Speed = -1;
                _Openness = 0;
            }

            // And make sure the graph is playing.
            _Animancer.Playable.UnpauseGraph();
        }

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <summary>[Editor-Only] Applies the starting openness value to the door in Edit Mode.</summary>
        /// <remarks>
        /// Called by the Unity Editor in Edit Mode whenever an instance of this script is loaded or a value is changed
        /// in the Inspector.
        /// </remarks>
        private void OnValidate()
        {
            if (_Animancer == null || _Open == null)
                return;

            // Delay for a frame. Otherwise Unity gives an error after recompiling scripts.
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (_Animancer == null || _Open == null)
                    return;

                Awake();
            };
        }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
    }
}
