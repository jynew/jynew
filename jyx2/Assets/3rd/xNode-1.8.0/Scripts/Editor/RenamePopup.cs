using UnityEditor;
using UnityEngine;

namespace XNodeEditor {
    /// <summary> Utility for renaming assets </summary>
    public class RenamePopup : EditorWindow {
        private const string inputControlName = "nameInput";

        public static RenamePopup current { get; private set; }
        public Object target;
        public string input;

        private bool firstFrame = true;

        /// <summary> Show a rename popup for an asset at mouse position. Will trigger reimport of the asset on apply.
        public static RenamePopup Show(Object target, float width = 200) {
            RenamePopup window = EditorWindow.GetWindow<RenamePopup>(true, "Rename " + target.name, true);
            if (current != null) current.Close();
            current = window;
            window.target = target;
            window.input = target.name;
            window.minSize = new Vector2(100, 44);
            window.position = new Rect(0, 0, width, 44);
            window.UpdatePositionToMouse();
            return window;
        }

        private void UpdatePositionToMouse() {
            if (Event.current == null) return;
            Vector3 mousePoint = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            Rect pos = position;
            pos.x = mousePoint.x - position.width * 0.5f;
            pos.y = mousePoint.y - 10;
            position = pos;
        }

        private void OnLostFocus() {
            // Make the popup close on lose focus
            Close();
        }

        private void OnGUI() {
            if (firstFrame) {
                UpdatePositionToMouse();
                firstFrame = false;
            }
            GUI.SetNextControlName(inputControlName);
            input = EditorGUILayout.TextField(input);
            EditorGUI.FocusTextInControl(inputControlName);
            Event e = Event.current;
            // If input is empty, revert name to default instead
            if (input == null || input.Trim() == "") {
                if (GUILayout.Button("Revert to default") || (e.isKey && e.keyCode == KeyCode.Return)) {
                    target.name = NodeEditorUtilities.NodeDefaultName(target.GetType());
                    NodeEditor.GetEditor((XNode.Node)target, NodeEditorWindow.current).OnRename();
                    AssetDatabase.SetMainObject((target as XNode.Node).graph, AssetDatabase.GetAssetPath(target));
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
                    Close();
                    target.TriggerOnValidate();
                }
            }
            // Rename asset to input text
            else {
                if (GUILayout.Button("Apply") || (e.isKey && e.keyCode == KeyCode.Return)) {
                    target.name = input;
                    NodeEditor.GetEditor((XNode.Node)target, NodeEditorWindow.current).OnRename();
                    AssetDatabase.SetMainObject((target as XNode.Node).graph, AssetDatabase.GetAssetPath(target));
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
                    Close();
                    target.TriggerOnValidate();
                }
            }

            if (e.isKey && e.keyCode == KeyCode.Escape) {
                Close();
            }
        }

        private void OnDestroy() {
            EditorGUIUtility.editingTextField = false;
        }
    }
}