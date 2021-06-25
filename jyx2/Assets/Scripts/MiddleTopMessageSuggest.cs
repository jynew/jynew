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
