using UnityEngine;
using UnityEditor;

public class RemoveMissingScriptsRecursively : EditorWindow
{
    [MenuItem("Tools/丢失脚本删除工具")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(RemoveMissingScriptsRecursively));
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Remove Missing Scripts in selected GameObjects"))
        {
            RemoveInSelected();
        }
    }
    private static void RemoveInSelected()
    {
        GameObject[] go = Selection.gameObjects;
        foreach (GameObject g in go)
        {
            RemoveRecursively(g);
        }
    }

    private static void RemoveRecursively(GameObject g)
    {
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(g);

        foreach (Transform childT in g.transform)
        {
            RemoveRecursively(childT.gameObject);
        }
    }
}