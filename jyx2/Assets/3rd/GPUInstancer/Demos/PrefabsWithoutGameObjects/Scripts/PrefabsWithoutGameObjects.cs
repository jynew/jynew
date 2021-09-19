using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GPUInstancer
{
    /// <summary>
    /// Example usage of InitializeWithMatrix4x4Array - UpdateVisibilityBufferWithMatrix4x4Array and DefineAndAddVariationFromArray - UpdateVariationFromArray API methods
    /// </summary>
    public class PrefabsWithoutGameObjects : MonoBehaviour
    {
        // reference to the Prefab Manager
        public GPUInstancerPrefabManager prefabManager;
        // reference to the No Game Object prefab
        public GPUInstancerPrefab prefab;
        // size of array and buffers
        public int bufferSize = 10000;

        // UI
        public Button addSphere;
        public Button removeSphere;
        public Button clearSphere;
        public Text sphereCountText;
        public Text positionUpdateFrequencyText;
        public Text scaleUpdateFrequencyText;
        public Text colorUpdateFrequencyText;
        public string bufferName = "colorBuffer";

        // Transform Data Array
        private Matrix4x4[] _matrix4x4Array;

        private int sphereCount = 0;
        private float positionUpdateFrequency = 1;
        private float scaleUpdateFrequency = 1;
        private float colorUpdateFrequency = 1;
        private Vector4[] variationData;

        // Use this for initialization
        private void Awake()
        {
            // min buffersize check
            if (bufferSize < 1000)
                bufferSize = 1000;
            // initialize the array with the max size
            _matrix4x4Array = new Matrix4x4[bufferSize];
            // change the data of the array
            AddMatrix4x4ToArray(1000);

            // initialize the buffers with array
            GPUInstancerAPI.InitializeWithMatrix4x4Array(prefabManager, prefab.prefabPrototype, _matrix4x4Array);
            GPUInstancerAPI.SetInstanceCount(prefabManager, prefab.prefabPrototype, sphereCount);

            // Color variatons
            variationData = new Vector4[bufferSize];
            for (int i = 0; i < bufferSize; i++)
                variationData[i] = Random.ColorHSV();
            GPUInstancerAPI.DefineAndAddVariationFromArray(prefabManager, prefab.prefabPrototype, bufferName, variationData);

            // UI
            CheckButtonsAvailablity();
            // Start Update Positions coroutine
            StartCoroutine(UpdatePositions());

            StartCoroutine(UpdateScale());

            StartCoroutine(UpdateColors());
        }

        // Update 100 random sphere positions
        private IEnumerator UpdatePositions()
        {
            while (true)
            {
                if(sphereCount > 100 && positionUpdateFrequency > 0)
                {
                    Vector3 newPosition;
                    // update 100 positions starting from random index
                    int randomIndex = Random.Range(0, sphereCount - 100);
                    for (int i = randomIndex; i < randomIndex + 100; i++)
                    {
                        newPosition = Random.insideUnitSphere * 20;
                        _matrix4x4Array[i].SetColumn(3, new Vector4(newPosition.x, newPosition.y, newPosition.z, _matrix4x4Array[i][3, 3]));
                    }
                    // set updated transform data
                    GPUInstancerAPI.UpdateVisibilityBufferWithMatrix4x4Array(prefabManager, prefab.prefabPrototype, _matrix4x4Array, randomIndex, randomIndex, 100);
                }
                yield return new WaitForSeconds(1 - positionUpdateFrequency + 0.01f);
            }
        }

        // Update 100 random sphere positions
        private IEnumerator UpdateScale()
        {
            while (true)
            {
                if (sphereCount > 100 && scaleUpdateFrequency > 0)
                {
                    Matrix4x4 instance;
                    Vector3 position;
                    Quaternion rotation;
                    Vector3 newScale;
                    // update 100 positions starting from random index
                    int randomIndex = Random.Range(0, sphereCount - 100);
                    for (int i = randomIndex; i < randomIndex + 100; i++)
                    {
                        instance = _matrix4x4Array[i];
                        position = instance.GetColumn(3);
                        rotation = Quaternion.LookRotation(instance.GetColumn(2), instance.GetColumn(1));
                        newScale = Vector3.one * Random.Range(0.5f, 1.5f);
                        _matrix4x4Array[i] = Matrix4x4.TRS(position, rotation, newScale);
                    }
                    // set updated transform data
                    GPUInstancerAPI.UpdateVisibilityBufferWithMatrix4x4Array(prefabManager, prefab.prefabPrototype, _matrix4x4Array, randomIndex, randomIndex, 100);
                }
                yield return new WaitForSeconds(1 - scaleUpdateFrequency + 0.01f);
            }
        }

        // Update 100 random sphere positions
        private IEnumerator UpdateColors()
        {
            while (true)
            {
                if (sphereCount > 100 && colorUpdateFrequency > 0)
                {
                    // update 100 positions starting from random index
                    int randomIndex = Random.Range(0, sphereCount - 100);
                    for (int i = randomIndex; i < randomIndex + 100; i++)
                        variationData[i] = Random.ColorHSV();
                    // set updated transform data
                    GPUInstancerAPI.UpdateVariationFromArray(prefabManager, prefab.prefabPrototype, bufferName, variationData, randomIndex, randomIndex, 100);
                }
                yield return new WaitForSeconds(1 - colorUpdateFrequency + 0.01f);
            }
        }

        // Add new spheres
        private void AddMatrix4x4ToArray(int instanceCount)
        {
            int start = sphereCount;
            sphereCount += instanceCount;
            for (int i = start; i < sphereCount; i++)
            {
                _matrix4x4Array[i] = Matrix4x4.TRS(Random.insideUnitSphere * 20, Quaternion.identity, Vector3.one * Random.Range(0.5f, 1.5f));
            }
        }

        // remove spheres
        private void RemoveMatrix4x4FromArray(int instanceCount)
        {
            int end = sphereCount;
            sphereCount -= instanceCount;
            for (int i = sphereCount; i < end; i++)
            {
                _matrix4x4Array[i] = Matrix4x4.zero;
            }
        }

        // UI
        private void CheckButtonsAvailablity()
        {
            if (sphereCount + 1000 > bufferSize)
                addSphere.interactable = false;
            else
                addSphere.interactable = true;

            if (sphereCount - 1000 < 0)
                removeSphere.interactable = false;
            else
                removeSphere.interactable = true;

            if (sphereCount <= 0)
                clearSphere.interactable = false;
            else
                clearSphere.interactable = true;

            sphereCountText.text = "Sphere Count: " + sphereCount;
        }

        public void SetPositionUpdateFrequency(float updateInterval)
        {
            this.positionUpdateFrequency = updateInterval;
            if(positionUpdateFrequency <= 0)
                positionUpdateFrequencyText.text = "Updating positions cancelled";
            else
                positionUpdateFrequencyText.text = "Updating positions every " + (1 - positionUpdateFrequency + 0.01f).ToString("0.00") + " seconds";
        }

        public void SetScaleUpdateFrequency(float updateInterval)
        {
            this.scaleUpdateFrequency = updateInterval;
            if (scaleUpdateFrequency <= 0)
                scaleUpdateFrequencyText.text = "Updating scales cancelled";
            else
                scaleUpdateFrequencyText.text = "Updating scales every " + (1 - scaleUpdateFrequency + 0.01f).ToString("0.00") + " seconds";
        }

        public void SetColorUpdateFrequency(float updateInterval)
        {
            this.colorUpdateFrequency = updateInterval;
            if (colorUpdateFrequency <= 0)
                colorUpdateFrequencyText.text = "Updating colors cancelled";
            else
                colorUpdateFrequencyText.text = "Updating colors every " + (1 - colorUpdateFrequency + 0.01f).ToString("0.00") + " seconds";
        }

        public void AddSpheres()
        {
            int previousSphereCount = sphereCount;
            AddMatrix4x4ToArray(1000);
            // UI
            CheckButtonsAvailablity();
            // set updated transform data
            GPUInstancerAPI.UpdateVisibilityBufferWithMatrix4x4Array(prefabManager, prefab.prefabPrototype, _matrix4x4Array, 
                previousSphereCount, previousSphereCount, 1000);
            GPUInstancerAPI.SetInstanceCount(prefabManager, prefab.prefabPrototype, sphereCount);
        }

        public void RemoveSpheres()
        {
            RemoveMatrix4x4FromArray(1000);
            // UI
            CheckButtonsAvailablity();
            // set updated transform data
            GPUInstancerAPI.UpdateVisibilityBufferWithMatrix4x4Array(prefabManager, prefab.prefabPrototype, _matrix4x4Array,
                sphereCount, sphereCount, 1000);
            GPUInstancerAPI.SetInstanceCount(prefabManager, prefab.prefabPrototype, sphereCount);
        }

        public void ClearSpheres()
        {
            sphereCount = 0;
            // UI
            CheckButtonsAvailablity();
            // clear array
            _matrix4x4Array = new Matrix4x4[bufferSize];
            // set updated transform data
            GPUInstancerAPI.UpdateVisibilityBufferWithMatrix4x4Array(prefabManager, prefab.prefabPrototype, _matrix4x4Array);
            GPUInstancerAPI.SetInstanceCount(prefabManager, prefab.prefabPrototype, sphereCount);
        }
    }
}