using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EnableChildrenMeshRenderers : MonoBehaviour
{
    public bool execute;

    void Update()
    {
        if (execute)
        {
            execute = false;
            Execute();
        }
    }

    void Execute()
    {
        var mrs = GetComponentsInChildren<MeshRenderer>();

        foreach (var mr in mrs) mr.enabled = true;
    }
}
