using UnityEditor;
using UnityEngine;

namespace ThreeEyedGames
{
    public class DecalUtilityWindow : EditorWindow
    {
        protected GameObject _instance = null;

        [MenuItem("Window/Decalicious Utility")]
        static void Init()
        {
            DecalUtilityWindow window = GetWindow<DecalUtilityWindow>("Decal Utility", true);
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Select a GameObject, then click the button to grab it. " +
                "Move the scene view camera until you're happy.", MessageType.Info);
            if (_instance == null)
            {
                if (GUILayout.Button("Grab object..."))
                    _instance = Selection.activeGameObject;
            }
            else
            {
                if (GUILayout.Button("I'm happy"))
                    _instance = null;
            }
            EditorGUILayout.EndVertical();
        }

        void OnInspectorUpdate()
        {
            if (!SceneView.lastActiveSceneView || !SceneView.lastActiveSceneView.camera)
                return;

            if (!_instance)
                return;

            Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(Vector2.one * 0.5f);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                _instance.transform.position = hit.point;
                _instance.transform.up = hit.normal;
            }
        }
    }
}
