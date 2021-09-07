using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LocalPositionEditorMono : MonoBehaviour
{
    public Vector3 position;
    public Vector3 rotaion;
    public Vector3 scale;
    
    [Button]
    void ApplyPosition()
    {
        
    }
    [Button]
    void GetRoation()
    {
        rotaion = transform.localEulerAngles;
    }
    [Button]
    void ApplyRoation()
    {
        transform.localRotation=Quaternion.Euler(rotaion);
    }
}
