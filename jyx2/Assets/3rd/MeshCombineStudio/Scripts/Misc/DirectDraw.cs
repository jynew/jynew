using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    public class DirectDraw : MonoBehaviour
    {

        MeshRenderer[] mrs;
        Mesh[] meshes;
        Material[] mats;
        Vector3[] positions;
        Quaternion[] rotations;

        private void Awake()
        {
            mrs = GetComponentsInChildren<MeshRenderer>(false);
            SetMeshRenderersEnabled(false);

            meshes = new Mesh[mrs.Length];
            mats = new Material[mrs.Length];
            positions = new Vector3[mrs.Length];
            rotations = new Quaternion[mrs.Length];

            for (int i = 0; i < mrs.Length; i++)
            {
                MeshFilter mf = mrs[i].GetComponent<MeshFilter>();
                meshes[i] = mf.sharedMesh;
                mats[i] = mrs[i].sharedMaterial;
                positions[i] = mrs[i].transform.position;
                rotations[i] = mrs[i].transform.rotation;
            }
        }

        void SetMeshRenderersEnabled(bool enabled)
        {
            for (int i = 0; i < mrs.Length; i++) mrs[i].enabled = enabled;
        }

        private void Update()
        {
            for (int i = 0; i < mrs.Length; i++)
            {
                Graphics.DrawMesh(meshes[i], positions[i], rotations[i], mats[i], 0);
            }
        }
    }
}