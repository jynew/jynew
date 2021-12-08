using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    List<GameObject> originObjects;
    // Start is called before the first frame update
    void Start()
    {
        originObjects = new List<GameObject>();
        List<CombineInstance> combineInstances = new List<CombineInstance>();
        MeshFilter[] meshFilters = transform.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter meshFilter in meshFilters)
        {
            // TODO ：可添加一些筛选
            originObjects.Add(meshFilter.gameObject);
            for (int sub = 0; sub < meshFilter.sharedMesh.subMeshCount; sub++)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = meshFilter.sharedMesh;
                ci.subMeshIndex = sub;
                ci.transform = meshFilter.transform.localToWorldMatrix;
                combineInstances.Add(ci);
            }
        }

        GameObject combinedObject = new GameObject();
        combinedObject.transform.parent = transform;
        combinedObject.transform.position = Vector3.zero;
        combinedObject.name = "Combined Render Object(Generated)";

        MeshFilter tempFilter = combinedObject.GetComponent<MeshFilter>();
        if (!tempFilter)
        {
            tempFilter = combinedObject.AddComponent<MeshFilter>();
        }

        tempFilter.sharedMesh = new Mesh();

        // 合并网格
        tempFilter.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        MeshRenderer tempRenderer = combinedObject.GetComponent<MeshRenderer>();
        if (!tempRenderer)
        {
            tempRenderer = combinedObject.AddComponent<MeshRenderer>();
        }

        MeshRenderer meshRenderer = transform.GetComponentInChildren<MeshRenderer>();

        tempRenderer.sharedMaterial = meshRenderer.sharedMaterial;

        for(int i = 0; i < originObjects.Count; i++)
        {
            originObjects[i].GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void OnDestroy()
    {
        originObjects.Clear();
    }

}
