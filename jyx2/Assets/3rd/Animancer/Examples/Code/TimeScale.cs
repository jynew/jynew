// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using UnityEngine;

namespace Animancer.Examples
{
    /// <summary>A simple Inspector slider to control <see cref="Time.timeScale"/>.</summary>
    /// <remarks>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/examples/basics/scene-setup#time-scale">Time Scale</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples/TimeScale
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Time Scale")]
    [HelpURL(Strings.DocsURLs.APIDocumentation + "." + nameof(Examples) + "/" + nameof(TimeScale))]
    public sealed class TimeScale : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField, Range(0, 1)]
        private float _Value = 0.5f;

        public float Value
        {
            get => _Value;
            set
            {
                _Value = value;

#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                    return;
#endif

                Time.timeScale = _Value;
            }
        }

        /************************************************************************************************************************/

        private void Awake()
        {
            Value = _Value;
        }

        /************************************************************************************************************************/

        private void OnValidate()
        {
            Value = _Value;
        }

        /************************************************************************************************************************/
    }
}
