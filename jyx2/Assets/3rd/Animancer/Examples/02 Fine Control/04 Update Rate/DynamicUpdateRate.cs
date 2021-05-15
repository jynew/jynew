// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.FineControl
{
    /// <summary>
    /// Demonstrates how to save some performance by updating Animancer at a lower frequency when the character is far
    /// away from the camera.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fine-control/update-rate">Update Rate</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.FineControl/DynamicUpdateRate
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Fine Control - Dynamic Update Rate")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(FineControl) + "/" + nameof(DynamicUpdateRate))]
    public sealed class DynamicUpdateRate : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private LowUpdateRate _LowUpdateRate;
        [SerializeField] private TextMesh _TextMesh;
        [SerializeField] private float _SlowUpdateDistance = 5;

        private Transform _Camera;

        /************************************************************************************************************************/

        private void Awake()
        {
            // Finding the Camera.main is a slow operation so we don't want to repeat it every update.
            _Camera = Camera.main.transform;
        }

        /************************************************************************************************************************/

        private void Update()
        {
            // Compare the squared distance to the camera with the squared threshold.
            // This is more efficient than calculating the distance because it avoids the square root calculation.
            var offset = _Camera.position - transform.position;
            var squaredDistance = offset.sqrMagnitude;

            // enabled = true if the distance is larger.
            // enabled = false if the distance is smaller
            _LowUpdateRate.enabled = squaredDistance > _SlowUpdateDistance * _SlowUpdateDistance;

            // For the sake of this example, use a TextMesh to show the current details.
            var distance = Mathf.Sqrt(squaredDistance);
            var updating = _LowUpdateRate.enabled ? "Slowly" : "Normally";
            _TextMesh.text = $"Distance {distance}\nUpdating {updating}\n\nDynamic Rate";
        }

        /************************************************************************************************************************/
    }
}
