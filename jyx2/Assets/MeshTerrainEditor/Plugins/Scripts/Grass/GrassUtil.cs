using System.Collections.Generic;
using UnityEngine;

namespace MTE
{
    public class GrassUtil
    {
        #region grass: three quads (star)

        public static void GenerateGrassStarObject(Vector3 position, Quaternion rotation,
            float width, float height, Material material,
            out GameObject grassGameObject, out MeshRenderer grassMeshRenderer, out Mesh grassMesh)
        {
            GameObject obj = new GameObject("GrassStar");
            var meshFilter = obj.AddComponent<MeshFilter>();
            var meshRenderer = obj.AddComponent<MeshRenderer>();

            var prototypeMesh = Resources.Load<Mesh>("Grass/Prototype_GrassStar");
            if (!prototypeMesh)
            {
                Debug.LogError("[MTE] Failed to load \"Grass/Prototype_GrassStar\" as Mesh.");
            }

            var mesh = Object.Instantiate(prototypeMesh);

            var vertices = new List<Vector3>(mesh.vertexCount);
            mesh.GetVertices(vertices);

            //apply width and height
            for (int i = 0; i < vertices.Count; i++)
            {
                var v = vertices[i];
                v.x *= width;
                v.y *= height;
                v.z *= width;
                vertices[i] = v;
            }

            mesh.SetVertices(vertices);

            meshRenderer.sharedMaterial = material;
            meshFilter.sharedMesh = mesh;

            meshRenderer.receiveShadows = true;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            obj.transform.position = position;
            obj.transform.rotation = rotation;

            obj.hideFlags = HideFlags.HideInHierarchy;
            obj.isStatic = true;// set to static for baking lightmap

            //output
            grassGameObject = obj;
            grassMeshRenderer = meshRenderer;
            grassMesh = mesh;
        }

        #endregion

        #region grass: one quad

        public static void GenerateGrassQuadObject(Vector3 position, Quaternion rotation,
            float width, float height, Material material,
            out GameObject grassGameObject, out MeshRenderer grassMeshRenderer, out Mesh grassMesh)
        {
            GameObject obj = new GameObject("GrassQuad");
            var meshFilter = obj.AddComponent<MeshFilter>();
            var meshRenderer = obj.AddComponent<MeshRenderer>();

            var prototypeMesh = Resources.Load<Mesh>("Grass/Prototype_GrassQuad");
            if (!prototypeMesh)
            {
                Debug.LogError("[MTE] Failed to load \"Grass/Prototype_GrassQuad\" as Mesh.");
            }
            var mesh = Object.Instantiate(prototypeMesh);

            //apply width and height
            var vertices = new List<Vector3>();
            mesh.GetVertices(vertices);
            for (int i = 0; i < vertices.Count; i++)
            {
                var v = vertices[i];
                v.x *= width;
                v.y *= height;
                v.z *= width;
                vertices[i] = v;
            }
            mesh.SetVertices(vertices);

            meshRenderer.sharedMaterial = material;
            meshFilter.sharedMesh = mesh;

            meshRenderer.receiveShadows = true;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            obj.transform.position = position;
            obj.transform.rotation = rotation;

            obj.hideFlags = HideFlags.HideInHierarchy;
            obj.isStatic = true;// set to static for baking lightmap

            //output
            grassGameObject = obj;
            grassMeshRenderer = meshRenderer;
            grassMesh = mesh;
        }

        #endregion

    }
}