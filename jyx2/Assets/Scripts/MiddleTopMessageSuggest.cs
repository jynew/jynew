using System.Collections;
using System.Collections.Generic;
using HSFrameWork.Common;
using UnityEngine;
using UnityEngine.UI;

public class MiddleTopMessageSuggest : MonoBehaviour
{

    public Text text;

    public void Show(string msg)
    {
        text.text = msg;
        this.gameObject.SetActive(true);

        HSUtilsEx.CallWithDelay(this, Hide, 1f);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
