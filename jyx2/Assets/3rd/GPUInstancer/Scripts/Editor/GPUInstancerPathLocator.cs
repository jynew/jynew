using UnityEditor;
using UnityEngine;

namespace GPUInstancer
{
    //[CreateAssetMenu(menuName = "GPU Instancer/Path Locator")]
    public class GPUInstancerPathLocator : ScriptableObject
    {
    }

    [CustomEditor(typeof(GPUInstancerPathLocator))]
    public class GPUInstancerPathLocatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUIStyle style = new GUIStyle();
            style.richText = true;
            style.wordWrap = true;
            GUILayout.Label("<size=20>Do not delete or edit this file.</size>\n\nThis file will be used to locate the GPUInstancer folder where your settings and prototype information will be kept.", style);

            string path = AssetDatabase.GetAssetPath(target);
            string currentGuid = AssetDatabase.AssetPathToGUID(path);
            if (GPUInstancerConstants.DEFAULT_PATH_GUID != currentGuid)
            {
                GUILayout.Label("\n\n<color=red><size=14><b>File GUID does not match the expected GUID!</b></size></color>", style);
                GUILayout.Label("\n" + GPUInstancerConstants.DEFAULT_PATH_GUID + " [Expected GUID]\n" + currentGuid + " [Current GUID]", style);
            }
        }
    }
}
