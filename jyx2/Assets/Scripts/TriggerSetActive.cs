/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
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
