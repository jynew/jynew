using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameObjectBrush {

    /// <summary>
    /// Class that is responsible for the addition of new brushes to the list of brushObjects in the main editor windo class: "GameObjectBrushEditor"
    /// </summary>
    public class AddObjectPopup : EditorWindow {

        private static string windowName = "Add brush";

        private GameObject obj2Add;
        public List<BrushObject> brushes;
        public EditorWindow parent;

        public static AddObjectPopup instance;

        //initialize the popup window
        public static void Init(List<BrushObject> brushes, EditorWindow parent) {
            //check if a window is already open
            if (instance != null) {
                return;
            }


            //create window
            instance = ScriptableObject.CreateInstance<AddObjectPopup>();

            //cache the brushes from the main editor window for later use
            instance.brushes = brushes;
            //cache the reference to the parent for later repaint
            instance.parent = parent;

            //calculate window position (center of the parent window)
            float x = parent.position.x + (parent.position.width - 350) * 0.5f;
            float y = parent.position.y + (parent.position.height - 75) * 0.5f;
            instance.position = new Rect(x, y, 350, 75);

            //show window as "utility"
            instance.ShowUtility();
            instance.name = windowName;
        }

        /// <summary>
        /// Creates the gui when called
        /// </summary>
        void OnGUI() {
            //create the "title" label
            EditorGUILayout.LabelField("Add GameObject to Brushes", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            //create the object field for the gameobject
            obj2Add = (GameObject) EditorGUILayout.ObjectField("GameObject", obj2Add, typeof(GameObject), false);

            //make sure we have some nice (?) spacing and all button next to each other (horizontally)
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            //close the popup
            GUI.backgroundColor = GameObjectBrushEditor.red;
            if (GUILayout.Button("Cancel")) {
                this.Close();
            }

            //Adds the gameobject to the brushes from the main window and closes the popup
            GUI.backgroundColor = GameObjectBrushEditor.green;
            if (GUILayout.Button("Add")) {
                if (obj2Add != null) {
                    brushes.Add(new BrushObject(obj2Add));
                    parent.Repaint();
                }
                this.Close();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void OnDestroy() {
            if (instance == this) {
                instance = null; //set instance to null
            }
        }
    }
}