using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ListMeshVertCount : MonoBehaviour {

    public bool includeInActive;
    public bool listVertCount;
    
	void Update()
    {
        if (listVertCount)
        {
            listVertCount = false;
            ListVertCount();
        }
    }

    void ListVertCount()
    {
        MeshFilter[] mfs = GetComponentsInChildren<MeshFilter>(includeInActive);

        int vertCount = 0;
        int triangleCount = 0;

        for (int i = 0; i < mfs.Length; i++)
        {
            Mesh m = mfs[i].sharedMesh;
            if (m == null) continue;
            vertCount += m.vertexCount;
            triangleCount += m.triangles.Length;
        }

        Debug.Log(gameObject.name + " Vertices " + vertCount + "  Triangles " + triangleCount);
    }
}
