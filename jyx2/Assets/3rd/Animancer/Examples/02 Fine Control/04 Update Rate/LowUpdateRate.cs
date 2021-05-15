// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.FineControl
{
    /// <summary>Demonstrates how to save some performance by updating Animancer less often.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fine-control/update-rate">Update Rate</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.FineControl/LowUpdateRate
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Fine Control - Low Update Rate")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(FineControl) + "/" + nameof(LowUpdateRate))]
    public sealed class LowUpdateRate : MonoBehaviour
    {
        /************************************************************************************************************************/

        // This script doesn't play any animations.
        // In a real game, you would have other scripts doing that.
        // But for this example, we are just using a NamedAnimancerComponent for its Play Automatically field.

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private float _UpdatesPerSecond = 10;

        private float _LastUpdateTime;

        /************************************************************************************************************************/
        // The DynamicUpdateRate script will enable and disable this script.
        /************************************************************************************************************************/

        private void OnEnable()
        {
            _Animancer.Playable.PauseGraph();
            _LastUpdateTime = Time.time;
        }

        private void OnDisable()
        {

            // This will get called when destroying the object as well (such as when loading a different scene).
            // So we need to make sure the AnimancerComponent still exists and is still initialised.
            if (_Animancer != null && _Animancer.IsPlayableInitialised)
                _Animancer.Playable.UnpauseGraph();
        }

        /************************************************************************************************************************/

        private void Update()
        {
            var time = Time.time;
            var timeSinceLastUpdate = time - _LastUpdateTime;
            if (timeSinceLastUpdate > 1 / _UpdatesPerSecond)
            {
                _Animancer.Evaluate(timeSinceLastUpdate);
                _LastUpdateTime = time;
            }
        }

        /************************************************************************************************************************/
    }
}
