using UnityEditor;
using UnityEngine;

namespace GameObjectBrush {

    /// <summary>
    /// Class that is responsible for the addition of new brushes to the list of brushObjects in the main editor windo class: "GameObjectBrushEditor"
    /// </summary>
    public class StringPopupWindow : EditorWindow {

        public static StringPopupWindow instance;

        public EditorWindow parent;
        public string value = "";
        public string windoName = "";
        public string valueLabel = "";
        public CreateInstance OnSubmit;

        public new delegate BrushCollection CreateInstance(string name);

        //initialize the popup window
        public static void Init(EditorWindow parent, CreateInstance OnSubmit, string defaultValue, string windowName, string valueLabel) {
            //check if a window is already open
            if (instance != null) {
                return;
            }


            //create window
            instance = ScriptableObject.CreateInstance<StringPopupWindow>();

            //cache the reference to the parent for later repaint
            instance.parent = parent;
            instance.value = defaultValue;
            instance.valueLabel = valueLabel;
            instance.OnSubmit = OnSubmit;


            //calculate window position (center of the parent window)
            float x = parent.position.x + (parent.position.width - 350) * 0.5f;
            float y = parent.position.y + (parent.position.height - 75) * 0.5f;
            instance.position = new Rect(x, y, 350, 75);

            //show window as "utility"
            instance.ShowUtility();
        }

        /// <summary>
        /// Creates the gui when called
        /// </summary>
        void OnGUI() {
            //create the "title" label
            EditorGUILayout.LabelField("Add GameObject to Brushes", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            //label field to indicare weather the brush collection already exists or not
            if (BrushCollection.CheckIfBrushCollectionAlreadyExists(value)) {
                GUILayout.Label("Brush Collection with the same name already exists!", EditorStyles.boldLabel);
            }

            //create the object field for the gameobject
            string prevValue = value;
            value = EditorGUILayout.TextField(valueLabel, value);

            //remove discouraged chars from the name, that could cause errors
            if (value != prevValue) {
                value = value.Replace("\\", "").Replace("/", "").Replace(".", "");
            }

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
            if (GUILayout.Button("Submit") && !BrushCollection.CheckIfBrushCollectionAlreadyExists(value)) {
                OnSubmit(value);
                parent.Repaint();
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