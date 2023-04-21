using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    public class SimpleMove : MonoBehaviour
    {
        public Vector3 rotDirMulti = Vector3.one;
        public float moveMulti = 50;
        public float rotMulti = 50;

        Vector3 dir;
        float t;

        void Start()
        {
            dir = Random.onUnitSphere;
            t = Random.value * moveMulti;
        }

        void Update()
        {
            float v = Mathf.Sin(Time.time + t) * moveMulti;

            if (moveMulti != 0) transform.Translate(dir * v * Time.deltaTime, Space.World);

            transform.Rotate(Vector3.Scale(dir, rotDirMulti) * Time.deltaTime * rotMulti, Space.Self);
        }
    }
}
