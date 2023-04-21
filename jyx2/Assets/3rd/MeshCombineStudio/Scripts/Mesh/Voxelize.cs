using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class Voxelize : MonoBehaviour
{
    static readonly byte[] bits = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 };

    static Dictionary<Mesh, VoxelizedMesh> voxelizedLookup = new Dictionary<Mesh, VoxelizedMesh>();

    static List<float> intersectList = new List<float>();

    const byte insideVoxel = 1;
    const byte outsideVoxel = 2;

    public int voxelizeLayer;
    public float voxelResolution;
    public bool voxelize;
    

    public void Update()
    {
        if (voxelize)
        {
            voxelize = false; 
            VoxelizeMesh(transform, voxelResolution, voxelizeLayer);
        }
    }

    public class VoxelizedMesh
    {
        public byte[,,] volume;
        public Bounds bounds;
        public Int3 voxels;
    }

    public struct Int3
    {
        public int x, y, z;

        public Int3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Int3 operator +(Int3 a, Int3 b)
        {
            Int3 v;
            v.x = a.x + b.x;
            v.y = a.y + b.y;
            v.z = a.z + b.z;
            return v;
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ", " + z + ")";
        }
    }

    VoxelizedMesh VoxelizeMesh(Transform t, float voxelResolution, int voxelizeLayer)
    {
        // TODO ray cast from right / left and top/ down
        Physics.queriesHitBackfaces = false;

        MeshRenderer mr = t.GetComponent<MeshRenderer>();
        if (mr == null) return null;

        MeshFilter mf = t.GetComponent<MeshFilter>();
        if (mf == null) return null;

        Mesh mesh = mf.sharedMesh;
        if (mesh == null) return null;

        VoxelizedMesh vm = new VoxelizedMesh();
        voxelizedLookup[mesh] = vm;

        Transform oldParent = t.parent;
        Vector3 oldPos = t.position;
        Quaternion oldRot = t.rotation;
        Vector3 oldScale = t.localScale;

        t.parent = null;
        t.position = Vector3.zero;
        t.rotation = Quaternion.identity;
        t.localScale = Vector3.one;
        int oldLayer = t.gameObject.layer;
        t.gameObject.layer = voxelizeLayer;

        LayerMask voxelizeLayerMask = 1 << voxelizeLayer;

        Bounds bounds = mr.bounds;

        Vector3 size = bounds.size;
        Int3 voxels = new Int3(Mathf.CeilToInt(size.x / voxelResolution), Mathf.CeilToInt(size.y / voxelResolution), Mathf.CeilToInt(size.z / voxelResolution));
        voxels += new Int3(2, 2, 2);
        int voxelsX = Mathf.CeilToInt(voxels.x / 8f);

        vm.voxels = voxels;

        size = new Vector3(voxels.x * voxelResolution, voxels.y * voxelResolution, voxels.z * voxelResolution);
        bounds.size = size;
        vm.bounds = bounds;

        byte[,,] volume = new byte[voxelsX, voxels.y, voxels.z];

        Ray ray = new Ray();
        Ray ray2 = new Ray();

        ray.direction = Vector3.forward;
        ray2.direction = Vector3.back;
        Vector3 pos = bounds.min;
        Vector3 pos2 = pos; 
        pos2.z = bounds.max.z;

        Debug.Log(PrintVector3(mr.bounds.size) + " new size " + PrintVector3(size) + " voxels " + voxels.ToString());
        int voxelCount = 0;

        Vector3 halfVoxel = Vector3.one * voxelResolution * 0.5f;
        Vector3 minBoundsVoxel = pos + halfVoxel;
        // voxels.y = 2;

        try
        {
            for (int x = 0; x < voxels.x; x++)
            {
                int xGrid = x / 8;
                byte bit = bits[x - (xGrid * 8)];

                for (int y = 0; y < voxels.y; y++)
                {
                    Vector3 origin = pos + new Vector3((x + 0.5f) * voxelResolution, (y + 0.5f) * voxelResolution, 0);
                    ray.origin = origin;
                    origin.z = pos2.z;
                    ray2.origin = origin;

                    intersectList.Clear();

                    MultiCast(ray, intersectList, 0.001f, size.z, voxelizeLayerMask);
                    MultiCast(ray2, intersectList, -0.001f, size.z, voxelizeLayerMask);

                    intersectList.Sort();

                    float half = (float)intersectList.Count / 2;
                    if (half != (int)half) { continue; }

                    // Debug.Log(hitInfos.Length + " " + hitInfos2.Length + " " + list.Count);

                    for (int i = 0; i < intersectList.Count; i += 2)
                    {
                        int z1 = Mathf.RoundToInt((intersectList[i] - pos.z) / voxelResolution);
                        int z2 = Mathf.RoundToInt((intersectList[i + 1] - pos.z) / voxelResolution);

                        for (int z = z1; z < z2; z++)
                        {
                            Vector3 voxelPos = new Vector3(x * voxelResolution, y * voxelResolution, z * voxelResolution) + minBoundsVoxel;
                            voxelPos = t.TransformPoint(voxelPos);

                            volume[xGrid, y, z] |= bit;
                            ++voxelCount;
                            //if (!Physics.CheckBox(voxelPos, halfVoxel, Quaternion.identity, voxelizeLayerMask))
                            //{
                            //    volume[xGrid, y, z] |= bit;
                            //    ++voxelCount;
                            //}
                        }
                    }
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        Debug.Log(t.name + " voxels " + voxelCount);

        vm.volume = volume;

        t.gameObject.layer = oldLayer;
        t.parent = oldParent;
        t.position = oldPos;
        t.rotation = oldRot;
        t.localScale = oldScale;

        return vm;
    }

    static string PrintVector3(Vector3 v)
    {
        return "(" + v.x + ", " + v.y + ", " + v.z + ")";
    }

    static void MultiCast(Ray ray, List<float> points, float hitOffset, float maxDistance, LayerMask voxelizeLayerMask)
    {
        RaycastHit hitInfo;

        while (Physics.Raycast(ray, out hitInfo, maxDistance, voxelizeLayerMask))
        {
            points.Add(hitInfo.point.z);

            Vector3 origin = ray.origin;
            ray.origin = new Vector3(origin.x, origin.y, hitInfo.point.z + hitOffset);
        }
    }

    static void Report(VoxelizedMesh vm, float voxelResolution)
    {
        int voxelResolutionX = (int)voxelResolution / 8;

        for (int x = 0; x < voxelResolutionX; x++)
        {
            for (int y = 0; y < voxelResolution; y++)
            {
                for (int z = 0; z < voxelResolution; z++)
                {
                    Debug.Log(vm.volume[x, y, z]);
                }
            }
        }
    }

    //static public void RemoveOverlap(Transform t, MeshCombineJobManager.MeshCombineJob meshCombineJob, MeshCache.SubMeshCache newMeshCache, ref byte[] vertexIsInsideVoxels)
    //{
    //    if (vertexIsInsideVoxels == null) vertexIsInsideVoxels = new byte[65534];

    //    float voxelResolution = meshCombineJob.meshCombiner.voxelResolution;
    //    int voxelizeLayer = meshCombineJob.meshCombiner.voxelizeLayer;

    //    Vector3[] newVertices = newMeshCache.vertices;
    //    int[] newTriangles = newMeshCache.triangles;

    //    List<MeshObject> meshObjects = meshCombineJob.meshObjectsHolder.meshObjects;

    //    int startIndex = meshCombineJob.startIndex;
    //    int endIndex = meshCombineJob.endIndex;

    //    for (int a = startIndex; a < endIndex; a++)
    //    {
    //        MeshObject meshObject = meshObjects[a];
    //        CachedGameObject cachedGO = meshObject.cachedGO;

    //        // Get array of intersections
    //        // Bounds bounds = cachedGO.mr.bounds;
    //        Collider[] colliders = null;//!! Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity, meshCombineJob.meshCombiner.overlapLayerMask);

    //        if (colliders.Length == 0)
    //        {
    //            // Debug.Log("No overlap " + cachedGO.go.name);
    //            continue;
    //        }

    //        // Debug.Log("Overlaps " + colliders.Length);

    //        Transform[] colliderTs = new Transform[colliders.Length];
    //        VoxelizedMesh[] colliderVms = new VoxelizedMesh[colliders.Length];

    //        for (int i = 0; i < colliderVms.Length; i++)
    //        {
    //            colliderTs[i] = colliders[i].transform;

    //            if (colliderTs[i] == cachedGO.t) continue;

    //            MeshFilter mf = colliderTs[i].GetComponent<MeshFilter>();
    //            if (mf == null) continue;

    //            Mesh mesh = mf.sharedMesh;
    //            if (mesh == null) continue;

    //            voxelizedLookup.TryGetValue(mesh, out colliderVms[i]);

    //            if (colliderVms[i] == null) colliderVms[i] = VoxelizeMesh(colliderTs[i], voxelResolution, voxelizeLayer);

    //            // Debug.LogError("Couldn't find voxelized mesh for " + mo.m + " " + child.name);
    //        }

    //        float invVoxelResolution = 1 / voxelResolution;

    //        int startTriangleIndex = meshObject.startNewTriangleIndex;
    //        int endTriangleIndex = meshObject.newTriangleCount + startTriangleIndex;

    //        // Debug.Log("start " + startTriangleIndex + " end " + endTriangleIndex);

    //        for (int i = startTriangleIndex; i < endTriangleIndex; i += 3)
    //        {
    //            bool insideAllVoxels = true;

    //            for (int k = 0; k < 3; k++)
    //            {
    //                int vertexIndex = newTriangles[i + k];
    //                if (vertexIndex == -1) continue;

    //                byte isInsideVoxel = vertexIsInsideVoxels[vertexIndex];

    //                if (isInsideVoxel == 0)
    //                {
    //                    bool inside = false;

    //                    for (int j = 0; j < colliders.Length; j++)
    //                    {
    //                        Transform colliderT = colliderTs[j];
    //                        VoxelizedMesh colliderVm = colliderVms[j];
    //                        if (colliderVm == null) continue;

    //                        Vector3 boundsMin = colliderVm.bounds.min;

    //                        Vector3 vertPos = t.TransformPoint(newVertices[vertexIndex]);

    //                        Vector3 pos = colliderT.InverseTransformPoint(vertPos) - boundsMin;
    //                        Vector3 grid = new Vector3(pos.x * invVoxelResolution, pos.y * invVoxelResolution, pos.z * invVoxelResolution);

    //                        if (grid.x < 0 || grid.x >= colliderVm.voxels.x || grid.y < 0 || grid.y >= colliderVm.voxels.y || grid.z < 0 || grid.z >= colliderVm.voxels.z) continue;

    //                        int xGrid = (int)grid.x;
    //                        int xxGrid = xGrid / 8;
    //                        byte bit = bits[(xGrid - (xxGrid * 8))];

    //                        if ((colliderVm.volume[xxGrid, (int)grid.y, (int)grid.z] & bit) == 0) continue;

    //                        inside = true;
    //                        break;
    //                    }

    //                    vertexIsInsideVoxels[vertexIndex] = isInsideVoxel = (inside ? insideVoxel : outsideVoxel);
    //                }

    //                if (isInsideVoxel == outsideVoxel)
    //                {
    //                    insideAllVoxels = false;
    //                    break;
    //                }
    //            }

    //            if (insideAllVoxels)
    //            {
    //                meshCombineJob.trianglesRemoved += 3;
    //                newTriangles[i] = -1;
    //            }
    //        }
    //    }

    //    Array.Clear(vertexIsInsideVoxels, 0, newMeshCache.vertexCount);

    //    // Debug.Log("Removed " + meshCombineJob.trianglesRemoved);

    //    newMeshCache.triangles = newTriangles;
    //}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        DrawVolume(transform, voxelResolution);

        Gizmos.color = Color.white;
    }

    public void DrawVolume(Transform t, float voxelResolution)
    {
        MeshRenderer mr = t.GetComponent<MeshRenderer>();
        if (mr == null) return;

        MeshFilter mf = t.GetComponent<MeshFilter>();
        if (mf == null) return;

        Mesh m = mf.sharedMesh;
        if (m == null) return;

        VoxelizedMesh vm;
        voxelizedLookup.TryGetValue(m, out vm);
        if (vm == null) return;

        byte[,,] volume = vm.volume;
        if (volume == null) return;

        Vector3 pos = vm.bounds.min;

        Vector3 voxel = t.lossyScale * voxelResolution;
        Vector3 halfVoxel = Vector3.one * voxelResolution * 0.5f;

        Int3 voxels = vm.voxels;
        // Debug.Log(voxels);
        // Debug.Log(volume.Length);

        Gizmos.DrawWireCube(mr.bounds.center, mr.bounds.size);

        for (int x = 0; x < voxels.x; x++)
        {
            int xGrid = x / 8;
            int bit = x - (xGrid * 8);

            for (int y = 0; y < voxels.y; y++)
            {
                for (int z = 0; z < voxels.z; z++)
                {
                    if ((volume[xGrid, y, z] & bits[bit]) > 0)
                    {
                        Vector3 localPos = new Vector3(pos.x + (x * voxelResolution), pos.y + (y * voxelResolution), pos.z + (z * voxelResolution)) + halfVoxel;
                        Gizmos.DrawWireCube(t.TransformPoint(localPos), voxel);
                    }
                }
            }
        }
    }
}
