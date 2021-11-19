using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ES3Internal;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Reflection;
using System;
#endif

public class ES3ReferenceMgr : ES3ReferenceMgrBase
{
#if UNITY_EDITOR
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void RefreshDependencies(bool isEnteringPlayMode = false)
    {
        ES3ReferenceMgrBase.isEnteringPlayMode = isEnteringPlayMode;

        // If we're not adding all prefabs to the manager, clear the prefabs array each time we refresh the dependencies.
        if (!ES3Settings.defaultSettingsScriptableObject.addAllPrefabsToManager)
            prefabs.Clear();

        // This will get the dependencies for all GameObjects and Components from the active scene.
        AddDependencies(this.gameObject.scene.GetRootGameObjects());
        AddPrefabsToManager();
        RemoveNullOrInvalidValues();

        ES3ReferenceMgrBase.isEnteringPlayMode = false;
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void Optimize()
    {
        var dependencies = CollectDependencies(this.gameObject.scene.GetRootGameObjects());
        var notDependenciesOfScene = new HashSet<UnityEngine.Object>();

        foreach (var kvp in idRef)
            if (!dependencies.Contains(kvp.Value))
                notDependenciesOfScene.Add(kvp.Value);

        foreach (var obj in notDependenciesOfScene)
        {
            Remove(obj);
        }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void AddDependencies(UnityEngine.Object[] objs)
    {
        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];

            if (obj.name == "Easy Save 3 Manager")
                continue;

            var dependencies = CollectDependencies(obj);

            foreach (var dependency in dependencies)
            {
                if (dependency != null)
                {
                    Add(dependency);

                    // Add the prefab if it's referenced by this scene.
                    if (dependency.GetType() == typeof(ES3Prefab))
                        AddPrefabToManager((ES3Prefab)dependency);
                }
            }
        }

        Undo.RecordObject(this, "Update Easy Save 3 Reference List");
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void AddDependencies(UnityEngine.Object obj)
    {
        AddDependencies(new UnityEngine.Object[] { obj });
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void GeneratePrefabReferences()
    {
        AddPrefabsToManager();
        foreach (var es3Prefab in prefabs)
            es3Prefab.GeneratePrefabReferences();
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void AddPrefabsToManager()
    {
        if (ES3Settings.defaultSettingsScriptableObject.addAllPrefabsToManager)
        {
            // Clear any null values. This isn't necessary if we're not adding all prefabs to manager as the list is cleared each time.
            if (this.prefabs.RemoveAll(item => item == null) > 0)
                Undo.RecordObject(this, "Update Easy Save 3 Reference List");

            foreach (var es3Prefab in Resources.FindObjectsOfTypeAll<ES3Prefab>())
                AddPrefabToManager(es3Prefab);
        }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    private void AddPrefabToManager(ES3Prefab es3Prefab)
    {
            try
            {
                if (es3Prefab != null && EditorUtility.IsPersistent(es3Prefab))
                    if(AddPrefab(es3Prefab))
                        Undo.RecordObject(this, "Update Easy Save 3 Reference List");
                es3Prefab.GeneratePrefabReferences();
            }
            catch { }
    }
#endif
}
