using UnityEditor;
using UnityEngine;

public class RemoveMissingScripts
{
    [MenuItem("Tools/RemoveMissingScripts")]
    public static void StartRemoveMissingScripts()
    {
        FindAll();
    }

    [MenuItem("Assets/清理选中GameObject中丢失的引用.", priority = -100)]
    private static void FindInSelected()
    {
        GameObject[] _go = Selection.gameObjects;

        foreach (GameObject _gameObject in _go)
        {
            FindInGO(_gameObject);
        }
    }

    private static void FindAll()
    {
        string[] _assetsPaths = AssetDatabase.GetAllAssetPaths();

        foreach (string _assetPath in _assetsPaths)
        {
            Object[] _objects = LoadAllAssetsAtPath(_assetPath);

            foreach (Object _o in _objects)
            {
                if (_o != null)
                {
                    if (_o is GameObject _gameObject)
                    {
                        FindInGO(_gameObject);
                    }
                }
            }
        }
    }

    public static Object[] LoadAllAssetsAtPath(string assetPath)
    {
        return typeof(SceneAsset) == AssetDatabase.GetMainAssetTypeAtPath(assetPath) ?
            new[] { AssetDatabase.LoadMainAssetAtPath(assetPath) } :
            AssetDatabase.LoadAllAssetsAtPath(assetPath);
    }

    private static void FindInGO(GameObject _gameObject)
    {
        Component[] _components = _gameObject.GetComponents<Component>();

        bool _flag = false;

        for (int i = 0; i < _components.Length; i++)
        {
            if (_components[i] == null)
            {
                _flag = true;

                break;
            }
        }

        if (_flag)
        {
            RemoveRecursively(_gameObject);
        }

        foreach (Transform _transform in _gameObject.transform)
        {
            FindInGO(_transform.gameObject);
        }
    }

    private static void RemoveRecursively(GameObject _gameObject)
    {
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(_gameObject);

        foreach (Transform _transform in _gameObject.transform)
        {
            RemoveRecursively(_transform.gameObject);
        }
    }
}