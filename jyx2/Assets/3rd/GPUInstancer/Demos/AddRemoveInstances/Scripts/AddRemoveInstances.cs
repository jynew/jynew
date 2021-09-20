using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPUInstancer
{
    public class AddRemoveInstances : MonoBehaviour
    {
        public GPUInstancerPrefab prefab;
        public GPUInstancerPrefabManager prefabManager;

        private Transform parentTransform;
        private int instanceCount;
        private List<GPUInstancerPrefab> instancesList = new List<GPUInstancerPrefab>();
        private List<GPUInstancerPrefab> extraInstancesList = new List<GPUInstancerPrefab>();
        private Toggle addRemoveInstantlyToggle;

        private Canvas guiCanvas;

        private void Awake()
        {
            parentTransform = GameObject.Find("SphereObjects").transform;
            instanceCount = parentTransform.childCount;
            guiCanvas = FindObjectOfType<Canvas>();
            addRemoveInstantlyToggle = GameObject.Find("AddRemoveInstantlyToggle").GetComponent<Toggle>();
        }

        private void Start()
        {
            // No prefab registration or GPU Instancer initialization is necessary since the manager instances the GPU Instancer prefabs automatically.
            // Here we are adding these prefabs to a list to manage add/remove operations later at runtime.
            for (int i = 0; i < instanceCount; i++)
                instancesList.Add(parentTransform.GetChild(i).gameObject.GetComponent<GPUInstancerPrefab>());
        }

        public void AddInstances()
        {
            LockAllButtons();
            StartCoroutine(AddInstancesAtRuntime());
        }

        public void RemoveInstances()
        {
            LockAllButtons();
            StartCoroutine(RemoveInstancesAtRuntime());
        }

        public void AddExtraInstances()
        {
            LockAllButtons();
            StartCoroutine(AddExtraInstancesAtRuntime());
        }

        public void RemoveExtraInstances()
        {
            LockAllButtons();
            StartCoroutine(RemoveExtraInstancesAtRuntime());
        }

        private IEnumerator AddInstancesAtRuntime()
        {
            for (int i = 0; i < instanceCount; i++)
            {
                GPUInstancerPrefab prefabInstance = Instantiate(prefab);
                prefabInstance.transform.localPosition = Random.insideUnitSphere * 10;
                prefabInstance.transform.SetParent(parentTransform);
                if(!prefabInstance.prefabPrototype.addRuntimeHandlerScript)
                    GPUInstancerAPI.AddPrefabInstance(prefabManager, prefabInstance);
                instancesList.Add(prefabInstance);
                if (!addRemoveInstantlyToggle.isOn)
                    yield return new WaitForSeconds(0.001f);
            }

            EnableButton("RemoveInstancesButton");
            if (extraInstancesList.Count == 0)
                EnableButton("AddExtraInstancesButton");
            else
                EnableButton("RemoveExtraInstancesButton");
        }

        private IEnumerator RemoveInstancesAtRuntime()
        {
            for (int i = 0; i < instanceCount; i++)
            {
                if (!instancesList[instancesList.Count - 1].prefabPrototype.addRuntimeHandlerScript)
                    GPUInstancerAPI.RemovePrefabInstance(prefabManager, instancesList[instancesList.Count - 1]);
                Destroy(instancesList[instancesList.Count - 1].gameObject);
                instancesList.RemoveAt(instancesList.Count - 1);
                if (!addRemoveInstantlyToggle.isOn)
                    yield return new WaitForSeconds(0.001f);
            }

            EnableButton("AddInstancesButton");
            if (extraInstancesList.Count == 0)
                EnableButton("AddExtraInstancesButton");
            else
                EnableButton("RemoveExtraInstancesButton");

        }

        private IEnumerator AddExtraInstancesAtRuntime()
        {
            // Extra instance count that can be added at runtime is limited by the "Extra buffer size" property defined for this prefab prototype in the manager.
            for (int i = 0; i < prefab.prefabPrototype.extraBufferSize; i++)
            {
                GPUInstancerPrefab prefabInstance = Instantiate(prefab);
                prefabInstance.transform.localPosition = Random.insideUnitSphere * 5;

                // Move the extra spheres to the outer sphere
                prefabInstance.transform.localPosition = prefabInstance.transform.localPosition.normalized * (prefabInstance.transform.localPosition.magnitude + 10f);

                prefabInstance.transform.SetParent(parentTransform);
                if (!prefabInstance.prefabPrototype.addRuntimeHandlerScript)
                    GPUInstancerAPI.AddPrefabInstance(prefabManager, prefabInstance);
                extraInstancesList.Add(prefabInstance);
                if (!addRemoveInstantlyToggle.isOn)
                    yield return new WaitForSeconds(0.001f);
            }

            EnableButton("RemoveExtraInstancesButton");
            if (instancesList.Count == 0)
                EnableButton("AddInstancesButton");
            else
                EnableButton("RemoveInstancesButton");
        }

        private IEnumerator RemoveExtraInstancesAtRuntime()
        {
            for (int i = 0; i < prefab.prefabPrototype.extraBufferSize; i++)
            {
                if (!extraInstancesList[extraInstancesList.Count - 1].prefabPrototype.addRuntimeHandlerScript)
                    GPUInstancerAPI.RemovePrefabInstance(prefabManager, extraInstancesList[extraInstancesList.Count - 1]);
                Destroy(extraInstancesList[extraInstancesList.Count - 1].gameObject);
                extraInstancesList.RemoveAt(extraInstancesList.Count - 1);
                if (!addRemoveInstantlyToggle.isOn)
                    yield return new WaitForSeconds(0.001f);
            }

            EnableButton("AddExtraInstancesButton");
            if (instancesList.Count == 0)
                EnableButton("AddInstancesButton");
            else
                EnableButton("RemoveInstancesButton");
        }

        private void LockAllButtons()
        {
            foreach (Selectable button in guiCanvas.transform.GetComponentsInChildren<Selectable>())
            {
                if (button != addRemoveInstantlyToggle)
                    button.interactable = false;
            }
        }

        private void EnableButton(string buttonName)
        {
            guiCanvas.transform.GetChild(0).Find(buttonName).GetComponent<Selectable>().interactable = true;
        }
    }
}

