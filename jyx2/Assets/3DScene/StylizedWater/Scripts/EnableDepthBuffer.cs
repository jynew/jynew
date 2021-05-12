// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StylizedWater
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class EnableDepthBuffer : MonoBehaviour
    {
        private Camera cam;
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Set();
        }
#endif

        void OnEnable()
        {
            Set();
        }

        void Set()
        {
            if (!cam) cam = GetComponent<Camera>();

            cam.depthTextureMode |= DepthTextureMode.Depth;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EnableDepthBuffer))]
    public class EnableDepthBufferInspector : Editor
    {

        override public void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("\nWhen using Forward rendering, this component will active the depth buffer on this camera.\n\nThis is necessary for the depth and intersection effects of the water to work.\n", MessageType.Info);
        }
    }
#endif
}
