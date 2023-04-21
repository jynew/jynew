using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    [ExecuteInEditMode]
    public class GarbageCollectMesh : MonoBehaviour
    {
        public Mesh mesh;
        
        void OnDestroy()
        {
            if (mesh != null)
            {
                #if UNITY_EDITOR
                    DestroyImmediate(mesh);
                #else
                    Destroy(mesh);
                #endif
            }
            // Debug.Log("Destroy Mesh");
        }
    }
}
