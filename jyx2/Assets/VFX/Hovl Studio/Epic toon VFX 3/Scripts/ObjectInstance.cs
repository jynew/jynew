using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInstance : MonoBehaviour
{
    public GameObject prefab;
    public bool Repeating = true;
    public float RepeatTime = 2.0f;
    public float StartTime = 0.0f;
    public float DestroyTimeDelay = 2;

    void Start()
    {
        if (Repeating == true)
        {
            InvokeRepeating("RepeatInstance", StartTime, RepeatTime);
        }
    }

    void Update()
    {

    }

    void RepeatInstance()
    {
        var instance = Instantiate(prefab, transform.position, new Quaternion());
        Destroy(instance, DestroyTimeDelay);
    }
}
