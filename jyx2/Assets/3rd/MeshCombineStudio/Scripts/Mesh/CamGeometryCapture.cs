using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    public class CamGeometryCapture : MonoBehaviour
    {
        public ComputeShader computeDepthToArray;
        public Int2 resolution = new Int2(1024, 1024);
        // public bool capture;
        // public float radius = 0.5f;

        public Camera cam;
        public Transform t;

        public RenderTexture rtCapture;

        float[] heights;
        
        Bounds bounds;

        float maxSize;

        public void Init()
        {
            if (t != null) return;

            t = transform;

            cam = GetComponent<Camera>();

            cam.aspect = 1;
            cam.orthographic = true;
        }

        void OnDestroy()
        {
            DisposeRTCapture();
        }

        //void Update()
        //{
        //    if (capture)
        //    {
        //        capture = false;
        //        Capture(mr.bounds, collisionMask.value, direction, resolution);
        //    }
        //}

        void DisposeRenderTexture(ref RenderTexture rt)
        {
            if (rt == null) return;

            rt.Release();

            #if UNITY_EDITOR
            DestroyImmediate(rt);
            #else
            Destroy(rt);    
            #endif

            rt = null;
        }

        public void DisposeRTCapture()
        {
            cam.targetTexture = null;
            DisposeRenderTexture(ref rtCapture);
        }

        public void RemoveTrianglesBelowSurface(Transform t, MeshCombineJobManager.MeshCombineJob meshCombineJob, MeshCache.SubMeshCache newMeshCache, ref byte[] vertexIsBelow)
        {
            if (vertexIsBelow == null) vertexIsBelow = new byte[65534];

            Vector3 pos = Vector3.zero;
            int layerMask = meshCombineJob.meshCombiner.surfaceLayerMask;
            // float rayHeight = meshCombineJob.meshCombiner.maxSurfaceHeight;

            Vector3[] newVertices = newMeshCache.vertices;
            int[] newTriangles = newMeshCache.triangles;

            FastList<MeshObject> meshObjects = meshCombineJob.meshObjectsHolder.meshObjects;

            int startIndex = meshCombineJob.startIndex;
            int endIndex = meshCombineJob.endIndex;

            const byte belowSurface = 1, aboveSurface = 2;

            for (int i = startIndex; i < endIndex; i++)
            {
                MeshObject meshObject = meshObjects.items[i];

                Capture(meshObject.cachedGO.mr.bounds, layerMask, new Vector3(0, -1, 0), new Int2(1024, 1024));

                int startTriangleIndex = meshObject.startNewTriangleIndex;
                int endTriangleIndex = meshObject.newTriangleCount + startTriangleIndex;

                // Debug.Log("startIndex " + startIndex + " triangle " + startTriangleIndex + " - " + endTriangleIndex);

                for (int j = startTriangleIndex; j < endTriangleIndex; j += 3)
                {
                    bool isAboveSurface = false;

                    for (int k = 0; k < 3; k++)
                    {
                        int vertexIndex = newTriangles[j + k];
                        if (vertexIndex == -1) continue;

                        byte isBelow = vertexIsBelow[vertexIndex];

                        if (isBelow == 0)
                        {
                            pos = t.TransformPoint(newVertices[vertexIndex]);

                            float height = GetHeight(pos);

                            isBelow = pos.y < height ? belowSurface : aboveSurface;
                            vertexIsBelow[vertexIndex] = isBelow;

                            if (pos.y < height)
                            {
                                vertexIsBelow[vertexIndex] = isBelow = belowSurface;
                            }
                            else
                            {
                                vertexIsBelow[vertexIndex] = isBelow = aboveSurface;
                            } 
                        }

                        if (isBelow != belowSurface) { isAboveSurface = true; break; }
                    }

                    if (!isAboveSurface)
                    {
                        meshCombineJob.trianglesRemoved += 3;
                        newTriangles[j] = -1;
                    }
                }
            }

            Array.Clear(vertexIsBelow, 0, newVertices.Length);
        }

        public void Capture(Bounds bounds, int collisionMask, Vector3 direction, Int2 resolution)
        {
            if (rtCapture == null || rtCapture.width != resolution.x || rtCapture.height != resolution.y)
            {
                if (rtCapture != null) DisposeRTCapture();

                rtCapture = new RenderTexture(resolution.x, resolution.y, 16, RenderTextureFormat.Depth, RenderTextureReadWrite.Linear);
            }

            bounds.size *= 1.1f;

            this.bounds = bounds;

            cam.targetTexture = rtCapture;
            cam.cullingMask = collisionMask;

            SetCamera(direction);

            cam.Render();

            int heightsLength = resolution.x * resolution.y;

            ComputeBuffer heightBuffer = new ComputeBuffer(heightsLength, 4);

            computeDepthToArray.SetTexture(0, "rtDepth", rtCapture);
            computeDepthToArray.SetBuffer(0, "heightBuffer", heightBuffer);
            computeDepthToArray.SetInt("resolution", resolution.x);
            computeDepthToArray.SetFloat("captureHeight", t.position.y);
            computeDepthToArray.SetFloat("distance", bounds.size.y + 256);
            computeDepthToArray.SetInt("direction", direction.y == 1 ? 1 : -1);

            computeDepthToArray.Dispatch(0, Mathf.CeilToInt(resolution.x / 8), Mathf.CeilToInt(resolution.y / 8), 1);

            if (heights == null || heights.Length != heightsLength) heights = new float[heightsLength];

            heightBuffer.GetData(heights);

            // Debug.Log(bounds.size.x + " " + bounds.size.y + " " + bounds.size.z);

            heightBuffer.Dispose();
        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.red;

        //    Mesh m = mf.sharedMesh;
        //    Vector3[] vertices = m.vertices;
        //    // Vector3 startPos = new Vector3(t.position.x, 0, t.position.z) - new Vector3(maxSize / 2, 0, maxSize / 2);

        //    for (int i = 0; i < vertices.Length; i++)
        //    {
        //        Vector3 pos = mf.transform.TransformPoint(vertices[i]);
        //        pos.y = GetHeight(pos);
        //        Gizmos.DrawSphere(pos, radius);
        //    }
        //}

        public void SetCamera(Vector3 direction)
        {
            if (direction == new Vector3(0, 1, 0))
            {
                t.position = bounds.center - new Vector3(0, bounds.extents.y + 256, 0);
            }
            else if (direction == new Vector3(0, -1, 0))
            {
                t.position = bounds.center + new Vector3(0, bounds.extents.y + 256, 0);
            }

            t.forward = direction;

            maxSize = bounds.size.x;

            if (bounds.size.z > maxSize) maxSize = bounds.size.z;

            cam.orthographicSize = maxSize / 2;

            cam.nearClipPlane = 0;
            cam.farClipPlane = (bounds.size.y + 256);
        }

        public float GetHeight(Vector3 pos)
        {
            pos -= bounds.min;
            pos.x += (maxSize - bounds.size.x) / 2;
            pos.z += (maxSize - bounds.size.z) / 2;

            float xx = maxSize / (resolution.x);
            float yy = maxSize / (resolution.y);

            float x = (int)(pos.x / xx);
            float y = (int)(pos.z / yy);

            if (x > resolution.x - 2 || x < 0 || y > resolution.y - 2 || y < 0)
            {
                Debug.Log("Out of bounds " + x + " " + y);
                return 0;
            }

            int intX = (int)x;
            int intY = (int)y;
            float lerpValue = x - intX;

            float height0 = heights[intX + (intY * resolution.y)];
            float height1 = heights[intX + 1 + (intY * resolution.y)];

            float heightx1 = Mathf.Lerp(height0, height1, lerpValue);

            height0 = heights[intX + ((intY + 1) * resolution.y)];
            height1 = heights[intX + 1 + ((intY + 1) * resolution.y)];

            float heightx2 = Mathf.Lerp(height0, height1, lerpValue);

            lerpValue = y - intY;

            return Mathf.Lerp(heightx1, heightx2, lerpValue);
        }
    }
}