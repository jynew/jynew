using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    [ExecuteInEditMode]
    public class ObjectSpawner : MonoBehaviour
    {
        public GameObject[] objects;

        public Vector3 spawnArea = new Vector3(512, 512, 512);
        public float density = 0.5f;
        public Vector2 scaleRange = new Vector2(0.5f, 2f);
        public Vector3 rotationRange = new Vector3(5, 360, 5);
        public Vector2 heightRange = new Vector2(0, 1);

        public float scaleMulti = 1;

        public float metersBetweenSpawning = 2;
        public bool spawnInRuntime;
        public bool spawn;
        public bool deleteChildren;


        Transform t;

        private void Awake()
        {
            t = transform;

            if (spawnInRuntime && Application.isPlaying)
            {
                Spawn();
            }
        }

        private void Update()
        {
            if (spawn)
            {
                spawn = false;
                Spawn();
            }
            if (deleteChildren)
            {
                deleteChildren = false;
                DeleteChildren();
            }
        }

        public void DeleteChildren()
        {
            Transform[] transforms = GetComponentsInChildren<Transform>();

            for (int i = 0; i < transforms.Length; i++)
            {
                if (t != transforms[i] && transforms[i] != null) DestroyImmediate(transforms[i].gameObject);
            }

#if UNITY_EDITOR
            if (!Application.isPlaying) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
        }

        void SetObjectsActive(bool active)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(active);
            }
        }

        public void Spawn()
        {
            SetObjectsActive(true);
            Bounds bounds = new Bounds();
            bounds.center = transform.position;
            bounds.size = spawnArea;

            float xStart = bounds.min.x;
            float xEnd = bounds.max.x;
            float yStart = bounds.min.y;
            float yEnd = bounds.max.y;
            float zStart = bounds.min.z;
            float zEnd = bounds.max.z;

            int objectCount = objects.Length;
            float halfRes = metersBetweenSpawning * 0.5f;
            float heightOffset = transform.lossyScale.y * 0.5f;
            int count = 0;

            for (float z = zStart; z < zEnd; z += metersBetweenSpawning)
            {
                for (float x = xStart; x < xEnd; x += metersBetweenSpawning)
                {
                    for (float y = yStart; y < yEnd; y += metersBetweenSpawning)
                    {
                        int index = Random.Range(0, objectCount);
                        float spawnValue = Random.value;
                        if (spawnValue < density)
                        {
                            Vector3 pos = new Vector3(x + Random.Range(-halfRes, halfRes), yStart + (Random.Range(0, bounds.size.y) * Random.Range(heightRange.x, heightRange.y)), z + Random.Range(-halfRes, halfRes));
                            if (pos.x < xStart || pos.x > xEnd || pos.y < yStart || pos.y > yEnd || pos.z < zStart || pos.z > zEnd) continue;
                            pos.y += heightOffset;
                            Vector3 eulerAngles = new Vector3(Random.Range(0, rotationRange.x), Random.Range(0, rotationRange.y), Random.Range(0, rotationRange.z));
                            GameObject go = (GameObject)Instantiate(objects[index], pos, Quaternion.Euler(eulerAngles));
                            float scale = Random.Range(scaleRange.x, scaleRange.y) * scaleMulti;
                            go.transform.localScale = new Vector3(scale, scale, scale);
                            go.transform.parent = t;
                            ++count;
                        }
                    }
                }
            }

            SetObjectsActive(false);

#if UNITY_EDITOR
            if (!Application.isPlaying) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif

            Debug.Log("Spawned " + count);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position + new Vector3(0, 0, 0), spawnArea);
        }
    }
}