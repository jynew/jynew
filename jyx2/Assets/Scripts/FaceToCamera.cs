using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceToCamera : MonoBehaviour
{
    private Transform mainCameraTrans;
    void Start()
    {
        mainCameraTrans = Camera.main.transform;
    }
    
    // Update is called once per frame
    void Update()
    {
        this.transform.forward = mainCameraTrans.forward;
    }
}
