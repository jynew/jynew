using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    public class AstroidGenerator : MonoBehaviour
    {
        [Range(0, 200000)]
        public int count = 50000;
        
        public List<GPUInstancerPrefab> asteroidObjects = new List<GPUInstancerPrefab>();
        public GPUInstancerPrefabManager prefabManager;
        public Transform centerTransform;

        private List<GPUInstancerPrefab> asteroidInstances = new List<GPUInstancerPrefab>();
        private int instantiatedCount;
        private Vector3 center;
        private Vector3 allocatedPos;
        private Quaternion allocatedRot;
        private Vector3 allocatedLocalEulerRot;
        private Vector3 allocatedLocalScale;
        private GPUInstancerPrefab allocatedGO;
        private GameObject goParent;
        private float allocatedLocalScaleFactor;
        private int columnSize;
        private int columnSpace = 3;

        private void Awake()
        {
            instantiatedCount = 0;
            center = centerTransform.position;
            allocatedPos = Vector3.zero;
            allocatedRot = Quaternion.identity;
            allocatedLocalEulerRot = Vector3.zero;
            allocatedLocalScale = Vector3.one;
            allocatedLocalScaleFactor = 1f;

            goParent = new GameObject("Asteroids");
            goParent.transform.position = center;
            goParent.transform.parent = gameObject.transform;

            columnSize = count < 5000 ? 1 : count / 2500;
            
            int firstPassColumnSize = count % columnSize > 0 ? columnSize - 1 : columnSize;

            asteroidInstances.Clear();

            for (int h = 0; h < firstPassColumnSize; h++)
            {
                for (int i = 0; i < Mathf.FloorToInt((float)count / columnSize); i++)
                {
                    asteroidInstances.Add(InstantiateInCircle(center, h));
                }
            }

            if (firstPassColumnSize != columnSize)
            {
                for (int i = 0; i < count - (Mathf.FloorToInt((float)count / columnSize) * firstPassColumnSize); i++)
                {
                    asteroidInstances.Add(InstantiateInCircle(center, columnSize));
                }
            }
            Debug.Log("Instantiated " + instantiatedCount + " objects.");
        }

        private void Start()
        {
            if (prefabManager != null && prefabManager.gameObject.activeSelf && prefabManager.enabled)
            {
                GPUInstancerAPI.RegisterPrefabInstanceList(prefabManager, asteroidInstances);
                GPUInstancerAPI.InitializeGPUInstancer(prefabManager);
            }
        }

        private void SetRandomPosInCircle(Vector3 center, int column, float radius)
        {
            float ang = Random.value * 360;

            allocatedPos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            allocatedPos.y = center.y - (column * (float)columnSpace / 2) + (column * columnSpace) + Random.Range(0f, 1f);
            allocatedPos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        }

        private GPUInstancerPrefab InstantiateInCircle(Vector3 center, int column)
        {
            SetRandomPosInCircle(center, column - Mathf.FloorToInt(columnSize / 2f), Random.Range(80.0f, 150f));
            allocatedRot = Quaternion.FromToRotation(Vector3.forward, center - allocatedPos);
            allocatedGO = Instantiate(asteroidObjects[Random.Range(0, asteroidObjects.Count)], allocatedPos, allocatedRot);
            allocatedGO.transform.parent = goParent.transform;

            allocatedLocalEulerRot.x = Random.Range(-180f, 180f);
            allocatedLocalEulerRot.y = Random.Range(-180f, 180f);
            allocatedLocalEulerRot.z = Random.Range(-180f, 180f);
            allocatedGO.transform.localRotation = Quaternion.Euler(allocatedLocalEulerRot);

            allocatedLocalScaleFactor = Random.Range(0.3f, 1.2f);
            allocatedLocalScale.x = allocatedLocalScaleFactor;
            allocatedLocalScale.y = allocatedLocalScaleFactor;
            allocatedLocalScale.z = allocatedLocalScaleFactor;
            allocatedGO.transform.localScale = allocatedLocalScale;

            instantiatedCount++;

            return allocatedGO;
        }
    }
}
