using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    public class GPUInstancerPrefabRuntimeHandler : MonoBehaviour
    {
        [HideInInspector]
        public GPUInstancerPrefab gpuiPrefab;

        private GPUInstancerPrefabManager _prefabManager;
        private static Dictionary<GPUInstancerPrefabPrototype, GPUInstancerPrefabManager> _managerDictionary;

        private void Awake()
        {
            gpuiPrefab = GetComponent<GPUInstancerPrefab>();
            if (_managerDictionary == null)
            {
                _managerDictionary = new Dictionary<GPUInstancerPrefabPrototype, GPUInstancerPrefabManager>();

                GPUInstancerPrefabManager[] prefabManagers = FindObjectsOfType<GPUInstancerPrefabManager>();
                if (prefabManagers != null && prefabManagers.Length > 0)
                {
                    foreach (GPUInstancerPrefabManager pm in prefabManagers)
                    {
                        foreach (GPUInstancerPrefabPrototype prototype in pm.prototypeList)
                        {
                            if (!_managerDictionary.ContainsKey(prototype))
                                _managerDictionary.Add(prototype, pm);
                        }
                    }
                }
            }
        }

        private void OnEnable()
        {
            if (gpuiPrefab.state == PrefabInstancingState.None)
            {
                if (_prefabManager == null)
                    _prefabManager = GetPrefabManager();
                if (_prefabManager != null)
                {
                    if (!_prefabManager.isInitialized)
                        _prefabManager.InitializeRuntimeDataAndBuffers();
                    _prefabManager.AddPrefabInstance(gpuiPrefab, true);
                }
            }
        }

        private void OnDisable()
        {
            if (gpuiPrefab.state == PrefabInstancingState.Instanced)
            {
                if (_prefabManager == null)
                    _prefabManager = GetPrefabManager();
                if (_prefabManager != null && !_prefabManager.isQuiting)
                    _prefabManager.RemovePrefabInstance(gpuiPrefab, false);
            }
        }

        private GPUInstancerPrefabManager GetPrefabManager()
        {
            GPUInstancerPrefabManager prefabManager = null;
            if (GPUInstancerManager.activeManagerList != null)
            {
                if (!_managerDictionary.TryGetValue(gpuiPrefab.prefabPrototype, out prefabManager))
                {
                    prefabManager = (GPUInstancerPrefabManager)GPUInstancerManager.activeManagerList.Find(manager => manager.prototypeList.Contains(gpuiPrefab.prefabPrototype));
                    if (prefabManager == null)
                    {
                        Debug.LogWarning("Can not find GPUI Prefab Manager for prototype: " + gpuiPrefab.prefabPrototype);
                        return null;
                    }
                    _managerDictionary.Add(gpuiPrefab.prefabPrototype, prefabManager);
                }
                if (prefabManager == null)
                {
                    prefabManager = (GPUInstancerPrefabManager)GPUInstancerManager.activeManagerList.Find(manager => manager.prototypeList.Contains(gpuiPrefab.prefabPrototype));
                    if (prefabManager == null)
                        return null;
                    _managerDictionary[gpuiPrefab.prefabPrototype] = prefabManager;
                }
            }
            return prefabManager;
        }
    }
}
