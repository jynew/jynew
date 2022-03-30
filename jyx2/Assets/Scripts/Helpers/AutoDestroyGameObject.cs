using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyGameObject : MonoBehaviour
{
    public float m_DestroyInSeconds = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AutoDestroy());
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(m_DestroyInSeconds);
        Destroy(this.gameObject);
    }
}
