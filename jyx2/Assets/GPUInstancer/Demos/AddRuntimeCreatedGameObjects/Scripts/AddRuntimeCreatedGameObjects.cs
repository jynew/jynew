using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    public class AddRuntimeCreatedGameObjects : MonoBehaviour
    {
        // Reference to the Prefab Manager in scene (no prefabs need to be defined on it)
        public GPUInstancerPrefabManager prefabManager;
        public Material material;

        private List<GameObject> instanceList;
        private GPUInstancerPrefabPrototype prototype;
        private GameObject prototypeGameObject;

        private void Start()
        {
            // Create a GameObject to create a prototype from
            prototypeGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);

            // Set material of the GameObject
            SetMaterial();

            // Create a list to keep track of instances
            instanceList = new List<GameObject>();

            // Add the original to the instanceList
            instanceList.Add(prototypeGameObject);

            // Your instantiation logic. Uses the prototype GameObject to identify the newly generated prototype.
            // This example just instantiates objects at random positions inside a sphere with radius of 20;
            // Replace this with how you want to generate your instances.
            for (int i = 0; i < 1000; i++)
                instanceList.Add(Instantiate(prototypeGameObject, UnityEngine.Random.insideUnitSphere * 20, Quaternion.identity));

            // Define the prototype
            prototype = GPUInstancerAPI.DefineGameObjectAsPrefabPrototypeAtRuntime(prefabManager, prototypeGameObject);

            // Make changes in the prototype settings
            prototype.enableRuntimeModifications = true;
            prototype.autoUpdateTransformData = true;

            // Add the prototype instances
            GPUInstancerAPI.AddInstancesToPrefabPrototypeAtRuntime(prefabManager, prototype, instanceList);

            // Start Coroutine to change instances over time
            StartCoroutine(AddRemoveAtRuntime());
        }

        IEnumerator AddRemoveAtRuntime()
        {
            while (true)
            {
                // Loop through primitives
                foreach (PrimitiveType primitiveType in Enum.GetValues(typeof(PrimitiveType)))
                {
                    yield return new WaitForSeconds(3);

                    // Remove runtime generated prototype definition
                    GPUInstancerAPI.RemovePrototypeAtRuntime(prefabManager, prototype);
                    // Clear the instances
                    ClearInstances();

                    yield return new WaitForSeconds(1);

                    // Create a GameObject to create a prototype from
                    prototypeGameObject = GameObject.CreatePrimitive(primitiveType);

                    // Set material of the GameObject
                    SetMaterial();

                    // Add the original to the instanceList
                    instanceList.Add(prototypeGameObject);

                    // Define the prototype
                    prototype = GPUInstancerAPI.DefineGameObjectAsPrefabPrototypeAtRuntime(prefabManager, prototypeGameObject);

                    // Create 1000 new instances
                    for (int i = 0; i < 1000; i++)
                        instanceList.Add(Instantiate(prototypeGameObject, UnityEngine.Random.insideUnitSphere * 20, Quaternion.identity));

                    // Add instances to manager
                    GPUInstancerAPI.AddInstancesToPrefabPrototypeAtRuntime(prefabManager, prototype, instanceList);

                    yield return new WaitForSeconds(1);
                }
            }
        }

        // Destroy GameObjects and clear the list
        public void ClearInstances()
        {
            foreach (GameObject go in instanceList)
                Destroy(go);
            instanceList.Clear();
        }

        public void SetMaterial()
        {
            prototypeGameObject.GetComponent<MeshRenderer>().sharedMaterial = material;
        }
    }
}

