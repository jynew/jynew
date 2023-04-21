using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This feature will be added in the next update

namespace MeshCombineStudio
{
    public class RemoveGeometryBelowTerrain : MonoBehaviour
    {
        int totalTriangles;
        int removeTriangles;
        int skippedObjects;

        public List<Transform> terrains = new List<Transform>();
        public List<Transform> meshTerrains = new List<Transform>();

        public Bounds[] terrainBounds, meshBounds;

        Terrain[] terrainComponents;
        Terrain[] terrainArray;
        Bounds[] terrainBoundsArray; //, meshBoundsArray;

        MeshRenderer[] mrs;
        Mesh[] meshTerrainComponents;
        Mesh[] meshArray;

        public bool runOnStart;

        private void Start()
        {
            if (runOnStart)
            {
                Remove(gameObject);
            }
        }

        public void Remove(GameObject go)
        {
            MeshFilter[] mfs = go.GetComponentsInChildren<MeshFilter>(true);
            totalTriangles = 0;
            removeTriangles = 0;
            skippedObjects = 0;

            for (int i = 0; i < mfs.Length; i++)
            {
                RemoveMesh(mfs[i].transform, mfs[i].mesh);
            }

            Debug.Log("Removeable " + removeTriangles + " total " + totalTriangles + " improvement " + (((float)removeTriangles / totalTriangles) * 100).ToString("F2"));
            Debug.Log("Skipped Objects " + skippedObjects);
        }

        public void RemoveMesh(Transform t, Mesh mesh)
        {
            if (mesh == null) return;
            if (!(IsMeshUnderTerrain(t, mesh))) { ++skippedObjects; return; }

            Vector3[] vertices = mesh.vertices;
            List<int> newTriangles = new List<int>();

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                newTriangles.AddRange(mesh.GetTriangles(i));
                int length = newTriangles.Count;
                RemoveTriangles(t, newTriangles, vertices);

                if (newTriangles.Count < length) mesh.SetTriangles(newTriangles.ToArray(), i);
            }
        }

        public bool IsMeshUnderTerrain(Transform t, Mesh mesh)
        {
            Bounds bounds = mesh.bounds;
            bounds.center += t.position;

            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            //Vector3 center = bounds.center;

            Vector2 delta = new Vector2(max.x - min.x, max.z - min.z);

            for (float z = 0; z < 1; z += 0.125f)
            {
                for (float x = 0; x < 1; x += 0.125f)
                {
                    Vector3 p = new Vector3(min.x + (x * delta.x), min.y, min.z + (z * delta.y));
                    float height = 0;// terrainColliderCam.GetHeight(p);
                    if (p.y < height) return true;
                }
            }

            return false;
        }

        public void GetTerrainComponents()
        {
            terrainComponents = new Terrain[terrains.Count];

            for (int i = 0; i < terrains.Count; i++)
            {
                Terrain terrain = terrains[i].GetComponent<Terrain>();
                terrainComponents[i] = terrain;
            }
        }

        public void GetMeshRenderersAndComponents()
        {
            mrs = new MeshRenderer[meshTerrains.Count];
            meshTerrainComponents = new Mesh[meshTerrains.Count];

            for (int i = 0; i < meshTerrains.Count; i++)
            {
                mrs[i] = meshTerrains[i].GetComponent<MeshRenderer>();
                MeshFilter mf = meshTerrains[i].GetComponent<MeshFilter>();
                meshTerrainComponents[i] = mf.sharedMesh;
            }
        }

        public void CreateTerrainBounds()
        {
            terrainBounds = new Bounds[terrainComponents.Length];

            for (int i = 0; i < terrainBounds.Length; i++)
            {
                terrainBounds[i] = new Bounds();
                terrainBounds[i].min = terrains[i].position;
                terrainBounds[i].max = terrainBounds[i].min + terrainComponents[i].terrainData.size;
            }

            meshBounds = new Bounds[meshTerrains.Count];

            for (int i = 0; i < meshTerrains.Count; i++)
            {
                meshBounds[i] = mrs[i].bounds;
            }
        }

        public void MakeIntersectLists(Bounds bounds)
        {
            List<Terrain> terrainList = new List<Terrain>();
            List<Mesh> meshList = new List<Mesh>();
            List<Bounds> terrainBoundsList = new List<Bounds>();
            List<Bounds> meshBoundsList = new List<Bounds>();

            Vector3[] pos = new Vector3[8];
            Vector3 size = bounds.size;

            pos[0] = bounds.min;
            pos[1] = pos[0] + new Vector3(size.x, 0, 0);
            pos[2] = pos[0] + new Vector3(0, 0, size.z);
            pos[3] = pos[0] + new Vector3(size.x, 0, size.z);
            pos[4] = pos[0] + new Vector3(0, size.y, 0);
            pos[5] = pos[0] + new Vector3(size.x, size.y, 0);
            pos[6] = pos[0] + new Vector3(0, size.y, size.z);
            pos[7] = pos[0] + size;

            for (int i = 0; i < 8; i++)
            {
                int index = InterectTerrain(pos[i]);
                if (index != -1)
                {
                    terrainList.Add(terrainArray[index]);
                    terrainBoundsList.Add(terrainBounds[index]);
                }

                index = InterectMesh(pos[i]);
                if (index != -1)
                {
                    meshList.Add(meshArray[index]);
                    meshBoundsList.Add(meshBounds[index]);
                }
            }

            terrainArray = terrainList.ToArray();
            meshArray = meshList.ToArray();
            terrainBoundsArray = terrainBoundsList.ToArray();
            // meshBoundsArray = meshBoundsList.ToArray();
        }

        public int InterectTerrain(Vector3 pos)
        {
            for (int i = 0; i < terrainBounds.Length; i++)
            {
                if (terrainBounds[i].Contains(pos)) return i;
            }

            return -1;
        }

        public int InterectMesh(Vector3 pos)
        {
            for (int i = 0; i < meshBounds.Length; i++)
            {
                if (meshBounds[i].Contains(pos)) return i;
            }

            return -1;
        }

        // Ray ray = new Ray(Vector3.zero, Vector3.down);

        public float GetTerrainHeight(Vector3 pos)
        {
            int index = -1;

            for (int i = 0; i < terrainArray.Length; i++)
            {
                if (terrainBoundsArray[i].Contains(pos)) { index = i; break; }
            }

            if (index != -1)
            {
                return terrainArray[index].SampleHeight(pos);
            }
            return Mathf.Infinity;
        }

        public void RemoveTriangles(Transform t, List<int> newTriangles, Vector3[] vertices)
        {
            bool[] verticeIsBelow = new bool[vertices.Length];

            Vector3 pos = Vector3.zero;
            float height = 0;

            for (int j = 0; j < newTriangles.Count; j += 3)
            {
                ++totalTriangles;
                int verticeIndex = newTriangles[j];
                bool isBelow = verticeIsBelow[verticeIndex];

                if (!isBelow)
                {
                    pos = t.TransformPoint(vertices[verticeIndex]);
                    height = GetTerrainHeight(pos);
                    isBelow = pos.y < height;
                }

                if (isBelow)
                {
                    verticeIsBelow[verticeIndex] = true;
                    verticeIndex = newTriangles[j + 1];
                    isBelow = verticeIsBelow[verticeIndex];
                    if (!isBelow)
                    {
                        pos = t.TransformPoint(vertices[verticeIndex]);
                        height = GetTerrainHeight(pos);
                        isBelow = pos.y < height;
                    }
                    if (isBelow)
                    {
                        verticeIsBelow[verticeIndex] = true;
                        verticeIndex = newTriangles[j + 2];
                        isBelow = verticeIsBelow[verticeIndex];
                        if (!isBelow)
                        {
                            pos = t.TransformPoint(vertices[verticeIndex]);
                            height = GetTerrainHeight(pos);
                            isBelow = pos.y < height;
                        }
                        if (isBelow)
                        {
                            verticeIsBelow[verticeIndex] = true;
                            ++removeTriangles;
                            newTriangles.RemoveAt(j + 2);
                            newTriangles.RemoveAt(j + 1);
                            newTriangles.RemoveAt(j);

                            if (j + 3 < newTriangles.Count) j -= 3;
                        }
                    }
                }
            }
        }
    }
}