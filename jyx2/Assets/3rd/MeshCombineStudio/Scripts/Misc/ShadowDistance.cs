using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowDistance : MonoBehaviour
{
    public float shadowDistance = 2000;

    void Awake()
    {
        QualitySettings.shadowDistance = shadowDistance;    
    }
}
