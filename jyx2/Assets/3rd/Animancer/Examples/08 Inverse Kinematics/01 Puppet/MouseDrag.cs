// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using UnityEngine;

namespace Animancer.Examples.InverseKinematics
{
    /// <summary>Allows the user to drag any object with a collider around on screen with the mouse.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/ik/puppet">Puppet</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.InverseKinematics/MouseDrag
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Inverse Kinematics - Mouse Drag")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(InverseKinematics) + "/" + nameof(MouseDrag))]
    public sealed class MouseDrag : MonoBehaviour
    {
        /************************************************************************************************************************/

        private Transform _Dragging;
        private float _Distance;

        /************************************************************************************************************************/

        private void Update()
        {
            // On click, do a raycast and grab whatever it hits and calculate how far away it is.
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit))
                {
                    _Dragging = hit.transform;
                    _Distance = Vector3.Distance(_Dragging.position, Camera.main.transform.position);
                }
            }
            // While holding the button, move the object in line with the mouse ray.
            else if (_Dragging != null && Input.GetMouseButton(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                _Dragging.position = Camera.main.transform.position + ray.direction * _Distance;
            }
            else
            {
                _Dragging = null;
            }
        }

        /************************************************************************************************************************/
    }
}
