#if false
using UnityEditor;
using UnityEngine;

namespace MTE
{
    [UnityEditor.CustomEditor(typeof(RuntimeTextureArrayLoader))]
    public class RuntimeTextureArrayLoaderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Load"))
            {
                var loader = target as RuntimeTextureArrayLoader;
                System.Diagnostics.Debug.Assert(loader);
                loader.LoadInEditor();
            }
            if (GUILayout.Button("Unload"))
            {
                var loader = target as RuntimeTextureArrayLoader;
                System.Diagnostics.Debug.Assert(loader);
                loader.UnloadInEditor();
            }
        }
    }
}
#endif