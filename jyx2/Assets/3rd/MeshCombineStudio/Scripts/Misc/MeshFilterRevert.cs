using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    public class MeshFilterRevert : MonoBehaviour
    {
        // [HideInInspector] 
        public string guid = string.Empty;
        public string meshName;

        public bool DestroyAndReferenceMeshFilter(MeshFilter mf)
        {
#if UNITY_EDITOR
            Mesh m = mf.sharedMesh;
            if (m == null) return false;

            string path = UnityEditor.AssetDatabase.GetAssetPath(m);
            if (path == null || path == string.Empty) return false;
            
            // Debug.Log(path);

            guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
            meshName = m.name;

            mf.sharedMesh = null;
#endif
            return true;
        }

        public void RevertMeshFilter(MeshFilter mf)
        {
#if UNITY_EDITOR
            if (guid == string.Empty) return;

            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            
            var meshes = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);

            for (int i = 0; i < meshes.Length; i++)
            {
                UnityEngine.Object m = meshes[i];

                if (m.GetType() != typeof(Mesh)) continue;

                if (m.name == meshName)
                {
                    mf.sharedMesh = (Mesh)m;
                    break;
                }
            }

            //Debug.Log(path + " " + (m == null));
            //if (m != null) Debug.Log(m.name);
            //mf.sharedMesh = m;

            Methods.Destroy(this);
#endif
        }
    }
}
