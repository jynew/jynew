using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    public class ColorVariations : MonoBehaviour
    {

        // The reference to the Prototype (the prefab itself can be assigned here since the GPUI Prototype component lives on the Prefab).
        public GPUInstancerPrefab prefab;

        // The reference to the active Prefab Manager in the scene.
        public GPUInstancerPrefabManager prefabManager;

        // The count of instances that will be generated.
        public int instances = 1000;

        // The name of the buffer. Must be the same with the StructuredBuffer in the shader that the Mateiral will use. See: "ColorVariationShader_GPUI.shader".
        public string bufferName = "colorBuffer";

        // The List to hold the instances that will be generated.
        private List<GPUInstancerPrefab> goList;

        void Start()
        {
            goList = new List<GPUInstancerPrefab>();

            // Define the buffer to the Prefab Manager.
            if (prefabManager != null && prefabManager.isActiveAndEnabled)
            {
                GPUInstancerAPI.DefinePrototypeVariationBuffer<Vector4>(prefabManager, prefab.prefabPrototype, bufferName);
            }

            // Generate instances inside a radius.
            for (int i = 0; i < instances; i++)
            {
                GPUInstancerPrefab prefabInstance = Instantiate(prefab);
                prefabInstance.transform.localPosition = Random.insideUnitSphere * 20;
                prefabInstance.transform.SetParent(transform);
                goList.Add(prefabInstance);

                // Register the variation buffer for this instance.
                prefabInstance.AddVariation(bufferName, (Vector4)Random.ColorHSV());
            }

            // Register the generated instances to the manager and initialize the manager.
            if (prefabManager != null && prefabManager.isActiveAndEnabled)
            {
                GPUInstancerAPI.RegisterPrefabInstanceList(prefabManager, goList);
                GPUInstancerAPI.InitializeGPUInstancer(prefabManager);
            }
        }

        void Update()
        {
            // Update the variation buffer with a random set of colors every frame, thus changing instance colors per instance every frame.
            GPUInstancerAPI.UpdateVariation(prefabManager, goList[Random.Range(0, goList.Count)], bufferName, (Vector4)Random.ColorHSV());
        }
    }
}


