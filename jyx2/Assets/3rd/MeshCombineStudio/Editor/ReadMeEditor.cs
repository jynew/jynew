using UnityEngine;
using UnityEditor;
using System.Collections;

namespace MeshCombineStudio
{
    [CustomEditor (typeof(ReadMe))]
    public class ReadMeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ReadMe r = (ReadMe)target;

            Event eventCurrent = Event.current;

            GUI.changed = false;

            if (eventCurrent.control && eventCurrent.shift && eventCurrent.keyCode == KeyCode.E && eventCurrent.type == EventType.KeyDown)
            {
                r.buttonEdit = !r.buttonEdit;
                GUI.changed = true;
            }
            
            GUILayout.Space(5);

            if (r.buttonEdit)
            {
                EditorGUILayout.LabelField("EDIT MODE");
                r.readme = EditorGUILayout.TextArea(r.readme);
            }
            else
            {
                EditorGUILayout.TextArea(r.readme);
            }
          
            if (GUI.changed) EditorUtility.SetDirty(target);
        }
    
    }
}