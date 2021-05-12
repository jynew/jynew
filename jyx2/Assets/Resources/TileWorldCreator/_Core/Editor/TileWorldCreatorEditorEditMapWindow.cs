using UnityEngine;
using UnityEditor;
using System.Collections;

//Disable Log warnings for assigned but not used variables 
#pragma warning disable 0219
namespace TileWorld
{
    public class TileWorldCreatorEditorEditMapWindow : EditorWindow
    {

        public static TileWorldCreatorEditorEditMapWindow window;
        static TileWorldCreatorEditor[] editor;
        static GameObject obj;

        public static void InitWindow(GameObject _obj)
        {
            // Get existing open window or if none, make a new one:
            window = (TileWorldCreatorEditorEditMapWindow)EditorWindow.GetWindow(typeof(TileWorldCreatorEditorEditMapWindow));
            editor = (TileWorldCreatorEditor[])Resources.FindObjectsOfTypeAll(typeof(TileWorldCreatorEditor));
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
                editor[0].ShowEdit();
            }
        }
    }
}
