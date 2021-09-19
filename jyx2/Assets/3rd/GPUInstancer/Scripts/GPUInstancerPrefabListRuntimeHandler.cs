using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace GPUInstancer
{
    public class GPUInstancerPrefabListRuntimeHandler : MonoBehaviour
    {
        public GPUInstancerPrefabManager prefabManager;
        private IEnumerable<GPUInstancerPrefab> _gpuiPrefabs;
        private bool _isIntancesAdded;
        public bool runInThreads = true;

        private void OnEnable()
        {
            if (prefabManager == null)
                return;

            if (!prefabManager.prototypeList.All(p => ((GPUInstancerPrefabPrototype)p).meshRenderersDisabled))
            {
                Debug.LogWarning("GPUInstancerPrefabListRuntimeHandler can not run in Threads while Mesh Renderers are enabled on the prefabs. Disabling threading...");
                runInThreads = false;
            }

            _gpuiPrefabs = gameObject.GetComponentsInChildren<GPUInstancerPrefab>(true);

            if (_gpuiPrefabs != null && _gpuiPrefabs.Count() > 0)
            {
                _isIntancesAdded = true;
                if (runInThreads)
                {
                    foreach (GPUInstancerPrefab pi in _gpuiPrefabs)
                    {
                        // save transform data before threading
                        pi.GetLocalToWorldMatrix(true);
                    }
                    ParameterizedThreadStart addPrefabInstancesAsync = new ParameterizedThreadStart(AddPrefabInstancesAsync);
                    Thread addPrefabInstancesAsyncThread = new Thread(addPrefabInstancesAsync);
                    addPrefabInstancesAsyncThread.IsBackground = true;
                    prefabManager.threadStartQueue.Enqueue(new GPUInstancerManager.GPUIThreadData() { thread = addPrefabInstancesAsyncThread, parameter = _gpuiPrefabs });
                }
                else
                    AddPrefabInstancesAsync(_gpuiPrefabs);
            }
        }

        private void OnDisable()
        {
            _isIntancesAdded = false;
            if (prefabManager == null)
                return;

            if (_gpuiPrefabs != null && _gpuiPrefabs.Count() > 0)
            {
                if (runInThreads)
                {
                    ParameterizedThreadStart removePrefabInstancesAsync = new ParameterizedThreadStart(RemovePrefabInstancesAsync);
                    Thread removePrefabInstancesAsyncThread = new Thread(removePrefabInstancesAsync);
                    removePrefabInstancesAsyncThread.IsBackground = true;
                    prefabManager.threadStartQueue.Enqueue(new GPUInstancerManager.GPUIThreadData() { thread = removePrefabInstancesAsyncThread, parameter = _gpuiPrefabs });
                }
                else
                    RemovePrefabInstancesAsync(_gpuiPrefabs);
            }
            _gpuiPrefabs = null;
        }

        public void AddPrefabInstancesAsync(object param)
        {
            try
            {
                prefabManager.AddPrefabInstances((IEnumerable<GPUInstancerPrefab>)param, runInThreads);
            }
            catch (Exception e)
            {
                if (runInThreads)
                {
                    prefabManager.threadException = e;
                    prefabManager.threadQueue.Enqueue(prefabManager.LogThreadException);
                }
                else
                    Debug.LogException(e);
            }
        }

        public void RemovePrefabInstancesAsync(object param)
        {
            try
            {
                prefabManager.RemovePrefabInstances((IEnumerable<GPUInstancerPrefab>)param, runInThreads);
            }
            catch (Exception e)
            {
                if (runInThreads)
                {
                    prefabManager.threadException = e;
                    prefabManager.threadQueue.Enqueue(prefabManager.LogThreadException);
                }
                else
                    Debug.LogException(e);
            }
        }

        public void SetManager(GPUInstancerPrefabManager prefabManager)
        {
            if (_isIntancesAdded)
                OnDisable();
            this.prefabManager = prefabManager;
            if (isActiveAndEnabled)
                OnEnable();
        }
    }
}
