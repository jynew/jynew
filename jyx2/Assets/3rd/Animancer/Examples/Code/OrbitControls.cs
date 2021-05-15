// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using UnityEngine;

namespace Animancer.Examples
{
    /// <summary>Simple mouse controls for orbiting the camera around a focal point.</summary>
    /// <remarks>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/examples/basics/scene-setup#orbit-controls">Orbit Controls</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples/OrbitControls
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Orbit Controls")]
    [HelpURL(Strings.DocsURLs.APIDocumentation + "." + nameof(Examples) + "/" + nameof(OrbitControls))]
#if UNITY_2019_1_OR_NEWER
    [ExecuteAlways]
#else
    [ExecuteInEditMode]
#endif
    public sealed class OrbitControls : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private Vector3 _FocalPoint = new Vector3(0, 1, 0);
        [SerializeField] private MouseButton _MouseButton = MouseButton.Right;
        [SerializeField] private Vector3 _Sensitivity = new Vector3(15, -10, -0.1f);

        private float _Distance;

        /************************************************************************************************************************/

        public enum MouseButton
        {
            Automatic = -1,
            Left = 0,
            Right = 1,
            Middle = 2,
        }

        /************************************************************************************************************************/

        private void Awake()
        {
            _Distance = Vector3.Distance(_FocalPoint, transform.position);

            transform.LookAt(_FocalPoint);
        }

        /************************************************************************************************************************/

        private void Update()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                transform.LookAt(_FocalPoint);
                return;
            }
#endif

            if (_MouseButton == MouseButton.Automatic || Input.GetMouseButton((int)_MouseButton))
            {
                var movement = new Vector2(
                    Input.GetAxis("Mouse X"),
                    Input.GetAxis("Mouse Y"));

                if (movement != Vector2.zero)
                {
                    var euler = transform.localEulerAngles;
                    euler.y += movement.x * _Sensitivity.x;
                    euler.x += movement.y * _Sensitivity.y;
                    if (euler.x > 180)
                        euler.x -= 360;
                    euler.x = Mathf.Clamp(euler.x, -80, 80);
                    transform.localEulerAngles = euler;
                }
            }

            var zoom = Input.mouseScrollDelta.y * _Sensitivity.z;
            if (zoom != 0 &&
                Input.mousePosition.x >= 0 && Input.mousePosition.x <= Screen.width &&
                Input.mousePosition.y >= 0 && Input.mousePosition.y <= Screen.height)
            {
                _Distance *= 1 + zoom;
            }

            // Always update position even with no input in case the target is moving.
            UpdatePosition();
        }

        /************************************************************************************************************************/

        private void UpdatePosition()
        {
            transform.position = _FocalPoint - transform.forward * _Distance;
        }

        /************************************************************************************************************************/

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.5f, 1, 0.5f, 1);
            Gizmos.DrawLine(transform.position, _FocalPoint);
        }

        /************************************************************************************************************************/
    }
}
