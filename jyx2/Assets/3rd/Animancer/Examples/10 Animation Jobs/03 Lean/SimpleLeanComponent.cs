// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Jobs
{
    /// <summary>
    /// An example component that demonstrates how to use <see cref="SimpleLean"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// Since <see cref="SimpleLean"/> is not a <see cref="MonoBehaviour"/> component, we need this script to
    /// demonstrate its use. However, in a real project you might simply integrate the contents of this class into one
    /// of your existing classes.
    /// </remarks>
    /// 
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/jobs/lean">Lean</see></example>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Jobs/SimpleLeanComponent
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Jobs - Simple Lean")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Jobs) + "/" + nameof(SimpleLeanComponent))]
    public sealed class SimpleLeanComponent : MonoBehaviour
    {
        /************************************************************************************************************************/
        // Initialisation.
        /************************************************************************************************************************/

        [SerializeField]
        private AnimancerComponent _Animancer;

        private SimpleLean _Lean;

        private void Awake()
        {
            _Lean = new SimpleLean(_Animancer.Playable);
        }

        /************************************************************************************************************************/
        // Usage.
        /************************************************************************************************************************/

        // Set by a UI Slider.
        public float Angle
        {
            get => _Lean.Angle;
            set => _Lean.Angle = value;
        }

        /************************************************************************************************************************/

        [SerializeField]
        private Transform _Axis;

        private void Update()
        {
            _Lean.Axis = _Axis.forward;
        }

        /************************************************************************************************************************/
    }
}
