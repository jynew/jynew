using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ES3ReferenceMgr))]
[System.Serializable]
public class ES3ReferenceMgrEditor : Editor
{
    private bool isDraggingOver = false;
    private bool openReferences = false;

    private ES3ReferenceMgr _mgr = null;
    private ES3ReferenceMgr mgr
    {
        get
        {
            if (_mgr == null)
                _mgr = (ES3ReferenceMgr)serializedObject.targetObject;
            return _mgr;
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("This allows Easy Save to maintain references to objects in your scene.\n\nIt is automatically updated when you enter Playmode or build your project.", MessageType.Info);

        if (EditorGUILayout.Foldout(openReferences, "References") != openReferences)
        {
            openReferences = !openReferences;
            if (openReferences == true)
                openReferences = EditorUtility.DisplayDialog("Are you sure?", "Opening this list will display every reference in the manager, which for larger projects can cause the Editor to freeze\n\nIt is strongly recommended that you save your project before continuing.", "Open References", "Cancel");
        }

        // Make foldout drag-and-drop enabled for objects.
        if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
        {
            Event evt = Event.current;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    isDraggingOver = true;
                    break;
                case EventType.DragExited:
                    isDraggingOver = false;
                    break;
            }

            if (isDraggingOver)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    Undo.RecordObject(mgr, "Add References to Easy Save 3 Reference List");
                    foreach (UnityEngine.Object obj in DragAndDrop.objectReferences)
                        mgr.Add(obj);
                    // Return now because otherwise we'll change the GUI during an event which doesn't allow it.
                    return;
                }
            }
        }

        if (openReferences)
        {
            EditorGUI.indentLevel++;

            foreach (var kvp in mgr.idRef)
            {
                EditorGUILayout.BeginHorizontal();

                var value = EditorGUILayout.ObjectField(kvp.Value, typeof(UnityEngine.Object), true);
                var key = EditorGUILayout.LongField(kvp.Key);

                EditorGUILayout.EndHorizontal();

                if (value != kvp.Value || key != kvp.Key)
                {
                    Undo.RecordObject(mgr, "Change Easy Save 3 References");
                    // If we're deleting a value, delete it.
                    if (value == null)
                        mgr.Remove(key);
                    // Else, update the ID.
                    else
                        mgr.ChangeId(kvp.Key, key);
                    // Break, as removing or changing Dictionary items will make the foreach out of sync.
                    break;
                }
            }

            EditorGUI.indentLevel--;
        }

        mgr.openPrefabs = EditorGUILayout.Foldout(mgr.openPrefabs, "ES3Prefabs");
        if (mgr.openPrefabs)
        {
            EditorGUI.indentLevel++;

            foreach (var prefab in mgr.prefabs)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.ObjectField(prefab, typeof(UnityEngine.Object), true);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.LabelField("Reference count", mgr.refId.Count.ToString());
        EditorGUILayout.LabelField("Prefab count", mgr.prefabs.Count.ToString());

        if (GUILayout.Button("Refresh"))
        {
            mgr.RefreshDependencies();
        }

        if (GUILayout.Button("Optimize"))
        {
            mgr.Optimize();
        }
    }

    [MenuItem("GameObject/Easy Save 3/Add Reference(s) to Manager", false, 33)]
    [MenuItem("Assets/Easy Save 3/Add Reference(s) to Manager", false, 33)]
    public static void AddReferenceToManager()
    {
        var mgr = ES3ReferenceMgr.Current;
        if (mgr == null)
        {
            EditorUtility.DisplayDialog("Could not add reference to manager", "This object could not be added to the reference manager because no reference manager exists in this scene. To create one, go to Assets > Easy Save 3 > Add Manager to Scene", "Ok");
            return;
        }

        if (Selection.objects == null || Selection.objects.Length == 0)
            return;

        Undo.RecordObject(mgr, "Update Easy Save 3 Reference Manager");

        foreach (var obj in Selection.objects)
        {
            if (obj == null)
                continue;

            if (obj.GetType() == typeof(GameObject))
            {
                var go = (GameObject)obj;
                if (ES3EditorUtility.IsPrefabInAssets(go) && go.GetComponent<ES3Internal.ES3Prefab>() != null)
                    mgr.AddPrefab(go.GetComponent<ES3Internal.ES3Prefab>());
            }

            ((ES3ReferenceMgr)mgr).AddDependencies(obj);
        }
    }

    [MenuItem("GameObject/Easy Save 3/Add Reference(s) to Manager", true, 33)]
    [MenuItem("Assets/Easy Save 3/Add Reference(s) to Manager", true, 33)]
    private static bool CanAddReferenceToManager()
    {
        return Selection.objects != null && Selection.objects.Length > 0 && ES3ReferenceMgr.Current != null;
    }

    [MenuItem("GameObject/Easy Save 3/Add Manager to Scene", false, 33)]
    [MenuItem("Assets/Easy Save 3/Add Manager to Scene", false, 33)]
    [MenuItem("Tools/Easy Save 3/Add Manager to Scene", false, 150)]
    public static void EnableForScene()
    {
        if(!SceneManager.GetActiveScene().isLoaded)
            EditorUtility.DisplayDialog("Could not add manager to scene", "Could not add Easy Save 3 Manager to scene because there is not currently a scene open.", "Ok");
        Selection.activeObject = ES3Postprocessor.AddManagerToScene();
    }

    [MenuItem("GameObject/Easy Save 3/Add Manager to Scene", true, 33)]
    [MenuItem("Assets/Easy Save 3/Add Manager to Scene", true, 33)]
    [MenuItem("Tools/Easy Save 3/Add Manager to Scene", true, 150)]
    private static bool CanEnableForScene()
    {
        return ES3ReferenceMgr.Current == null;
    }
}
