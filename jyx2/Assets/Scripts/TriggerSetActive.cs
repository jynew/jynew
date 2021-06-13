using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSetActive : MonoBehaviour
{
    public GameObject[] target;
    public bool actived;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        foreach (var t in target)
        {
            //t.transform.Find("YWML2_Fire").gameObject.SetActive(true);
            t.SetActive(actived);
        }
        if (other.transform.Find("YWML2_Fire"))
        {
            other.transform.Find("YWML2_Fire").gameObject.SetActive(true);
        }
    }
}
