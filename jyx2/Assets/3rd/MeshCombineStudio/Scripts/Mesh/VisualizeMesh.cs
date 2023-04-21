using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizeMesh : MonoBehaviour
{
    public float sphereRadius = 0.05f;

    MeshFilter mf;
    Mesh m;

    void OnDrawGizmosSelected()
    {
        if (!mf) mf = GetComponent<MeshFilter>();
        if (!mf) return;

        if (!m) m = mf.sharedMesh;
        if (!m) return;

        Vector3[] verts = m.vertices;
        Vector3[] normals = m.normals;
        Vector4[] tangents = m.tangents;

        Matrix4x4 mt = transform.localToWorldMatrix;
        Matrix4x4 mn = mt.inverse.transpose;

        for (int i = 0; i < verts.Length; i++)
        {
            Gizmos.color = Color.green;
            Vector3 pos = mt.MultiplyPoint3x4(verts[i]);
            Gizmos.DrawSphere(pos, sphereRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pos, pos + mn.MultiplyVector(normals[i]) * 0.5f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, pos + mt.MultiplyVector(new Vector3(tangents[i].x, tangents[i].y, tangents[i].z)) * 0.5f);
        }
    }
}
