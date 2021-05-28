using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jyx2;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CreateAssetMenu]
public class SceneObjectFindEditor : Sirenix.OdinInspector.SerializedScriptableObject
{
    public GameObject prefab;
    public float X=-1;
    public float Y=-1;
    public float Z=-1;

    [FolderPath] public string scenePath;

    [Button("Find")]
    private void Find()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(scenePath);
        foreach (var fileInfo in directoryInfo.GetFiles("*.unity", SearchOption.AllDirectories))
        {
            var scene = EditorSceneManager.OpenScene(fileInfo.FullName);
            var level = scene.GetRootGameObjects().FirstOrDefault(x => x.name == "Level");
            if (!level)
            {
                Debug.Log($"{scene.name} 没有Level");
                continue;
            }

            var arr = level.GetComponentsInChildren<MapChestInteract>();
            if (arr == null) continue;
            bool save = false;
            foreach (var mapChest in arr)
            {
                if (GetPath(prefab) == GetPath(mapChest.gameObject))
                {
                    Debug.Log(mapChest.name);
                    var trans = mapChest.transform;
                    var angles =trans.localEulerAngles;
                    if (X != -1)
                        angles.x = X;
                    if (Y != -1)
                        angles.y += Y;
                    if (Z != -1)
                        angles.z = Z;

                    trans.localEulerAngles = angles;
                    save = true;
                }
            }

            EditorSceneManager.SaveScene(scene);
        }
    }

    private string GetPath(GameObject gameObject)
    {
        // Project中的Prefab是Asset不是Instance
        if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(gameObject))
        {
            // 预制体资源就是自身
            return UnityEditor.AssetDatabase.GetAssetPath(gameObject);
        }

        // Scene中的Prefab Instance是Instance不是Asset
        if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(gameObject))
        {
            // 获取预制体资源
            var prefabAsset = UnityEditor.PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);
            return UnityEditor.AssetDatabase.GetAssetPath(prefabAsset);
        }

        // PrefabMode中的GameObject既不是Instance也不是Asset
        var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
        if (prefabStage != null)
        {
            // 预制体资源：prefabAsset = prefabStage.prefabContentsRoot
            return prefabStage.prefabAssetPath;
        }

        return null;
    }
}