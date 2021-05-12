using UnityEngine;
using UnityEditor;
using System.Collections;

//Disable Log warnings for assigned but not used variables 
#pragma warning disable 0219
namespace TileWorld
{

    public class TileWorldCreatorEditorSettingsWindow : EditorWindow
    {
        public static TileWorldCreatorEditorSettingsWindow window;
        //static TileWorldConfigurationEditor[] editor;
        static TileWorldCreator twc;
        static GameObject obj;

        public static void InitWindow(GameObject _obj)
        {
            // Get existing open window or if none, make a new one:
            window = (TileWorldCreatorEditorSettingsWindow)EditorWindow.GetWindow(typeof(TileWorldCreatorEditorSettingsWindow));
            twc = _obj.GetComponent<TileWorldCreator>();
            //editor = (TileWorldConfigurationEditor[])Resources.FindObjectsOfTypeAll(typeof(TileWorldConfigurationEditor));
            obj = _obj;
        }


        void OnGUI()
        {
            if (GUILayout.Button("select"))
            {
                if (obj != null)
                {
                    Selection.activeGameObject = obj;
                }
                else
                {
                    TileWorldCreator _c = GameObject.FindObjectOfType<TileWorldCreator>();
                    obj = _c.gameObject;
                    InitWindow(obj);
                }
            }

            if (window != null)
            {
                TileWorldConfigurationEditor.ShowGlobalSettings(twc.configuration);
            }
        }
    }
}
