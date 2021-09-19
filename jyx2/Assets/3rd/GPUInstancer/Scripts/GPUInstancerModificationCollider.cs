using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    public class GPUInstancerModificationCollider : MonoBehaviour
    {
        public GPUInstancerPrefabManager prefabManager;

        private List<GPUInstancerPrefab> _enteredInstances;
        private Collider _collider;

        private void Awake()
        {
            _enteredInstances = new List<GPUInstancerPrefab>();
            _collider = GetComponent<Collider>();

            if(prefabManager == null)
                prefabManager = FindObjectOfType<GPUInstancerPrefabManager>();

            if (prefabManager != null)
                prefabManager.AddModificationCollider(this);
            else
                Debug.LogWarning("GPUInstancerModificationCollider does not have a GPUInstancerPrefabManager defined.");
        }

        private void Update()
        {
            if (prefabManager != null && prefabManager.isActiveAndEnabled)
            {
                Rigidbody rb;
                for (int i = 0; i < _enteredInstances.Count; i++)
                {
                    if (!IsInsideCollider(_enteredInstances[i]))
                    {
                        rb = _enteredInstances[i].GetComponent<Rigidbody>();
                        if (rb != null && !rb.IsSleeping())
                            continue;
                        GPUInstancerAPI.EnableInstancingForInstance(prefabManager, _enteredInstances[i]);
                        _enteredInstances.Remove(_enteredInstances[i]);
                        i--;
                    }
                    else if (_enteredInstances[i].state != PrefabInstancingState.Disabled)
                        prefabManager.DisableIntancingForInstance(_enteredInstances[i]);
                }
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (prefabManager != null && prefabManager.isActiveAndEnabled && collider.gameObject)
            {
                GPUInstancerPrefab prefabInstance = collider.gameObject.GetComponent<GPUInstancerPrefab>();
                if(prefabInstance != null && prefabInstance.prefabPrototype.enableRuntimeModifications && prefabInstance.state != PrefabInstancingState.Disabled)
                {
                    prefabManager.DisableIntancingForInstance(prefabInstance);
                    _enteredInstances.Add(prefabInstance);
                }
            }
        }

        //private void OnTriggerExit(Collider collider)
        //{
        //    if (GPUInstancerPrefabManager.Instance != null &&
        //        collider.gameObject &&
        //        collider.gameObject.GetComponent<GPUInstancerPrefab>() &&
        //        collider.gameObject.GetComponent<GPUInstancerPrefab>().prefabPrototype.enableRuntimeModifications &&
        //        !gameObject.GetComponent<Collider>().bounds.Intersects(collider.bounds))
        //    {
        //        GPUInstancerPrefabManager.Instance.AddInstance(collider.gameObject.GetComponent<GPUInstancerPrefab>());
        //    }
        //}

        public bool IsInsideCollider(GPUInstancerPrefab prefabInstance)
        {
            Collider instanceCollider = prefabInstance.GetComponent<Collider>();
            if (instanceCollider == null)
                return false;
            else
                return _collider.bounds.Intersects(instanceCollider.bounds);
        }

        public void AddEnteredInstance(GPUInstancerPrefab prefabInstance)
        {
            _enteredInstances.Add(prefabInstance);
        }

        public int GetEnteredInstanceCount()
        {
            return _enteredInstances.Count;
        }
    }
}
