using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jyx2;

public class GameStart : MonoBehaviour
{
    void Start()
    {
#if UNITY_EDITOR

#else
        //运行时，需要手动调用
        BeforeSceneLoad.ColdBind();
#endif
        Jyx2_UIManager.Instance.GameStart();
    }
}
