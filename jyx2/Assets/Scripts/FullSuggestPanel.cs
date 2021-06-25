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
using UnityEngine.UI;

public class FullSuggestPanel : MonoBehaviour {

    public Text suggestText;

    System.Action _callback;

    public void Show(string msg, System.Action callback)
    {
        Time.timeScale = 0;
        suggestText.text = msg;
        _callback = callback;
        gameObject.SetActive(true);
    }

    public void OnClicked()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        if (_callback != null)
            _callback();
    }
    
}
