using UnityEngine;
using UnityEditor;
using System.Collections;

//Disable Log warnings for assigned but not used variables 
#pragma warning disable 0219
namespace TileWorld
{
    public class TileWorldCreatorEditorPresetsWindow : EditorWindow
    {
        public static TileWorldCreatorEditorPresetsWindow window;
        //static TileWorldConfiguration[] editor;
        static TileWorldCreator twc;
        static GameObject obj;

        Vector2 scrollPosition = Vector2.zero;

        public static void InitWindow(GameObject _obj)
        {
            // Get existing open window or if none, make a new one:
            window = (TileWorldCreatorEditorPresetsWindow)EditorWindow.GetWindow(typeof(TileWorldCreatorEditorPresetsWindow));
            twc = _obj.GetComponent<TileWorldCreator>();
            //editor = (TileWorldConfiguration[])Resources.FindObjectsOfTypeAll(typeof(TileWorldConfiguration));
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
                int _tilesets = 0;
                for (int tp = 0; tp < twc.configuration.presets.Count; tp++)
                {
                    _tilesets += twc.configuration.presets[tp].tiles.Count;
                }

                scrollPosition = GUI.BeginScrollView(new Rect(0, 25, Screen.width, Screen.height - 50), scrollPosition, new Rect(0, 20, Screen.width - 50, (_tilesets * 295f) + 10f));
                TileWorldConfigurationEditor.ShowPresetSettings(twc.configuration);
                GUI.EndScrollView();
            }
        }
    }
}
