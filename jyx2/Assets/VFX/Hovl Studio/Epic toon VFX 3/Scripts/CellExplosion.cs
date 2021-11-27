using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellExplosion : MonoBehaviour
{
    public float minForce;
    public float maxForce;
    public float radious;
    public float timer = 2;
    private float size;

    void Start()
    {
        size = 0.01f / timer;
        Explode();
    }

    public void Explode()
    {
        foreach (Transform t in transform)
        {
            if (t.GetComponent<Rigidbody>() != null && t.GetComponent<Transform>() != null)
            {
                var rb = t.GetComponent<Rigidbody>();
                rb.AddExplosionForce(Random.Range(minForce, maxForce), transform.position, radious);
                StartCoroutine(ChangeSize(t.GetComponent<Transform>()));
            }
        }
    }

    public IEnumerator ChangeSize(Transform tr)
    {
        while (true)
        {
            if (size <= 0)
            {
                yield break;
            }
            else
            {
                tr.transform.localScale = tr.transform.localScale - new Vector3(size, size, size);
                yield return null;
            }
        }
    }
}
