/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using System;
using System.IO;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Jyx2;
using Jyx2.MOD;
using UnityEngine.SceneManagement;
// 翻译插件
using EZ4i18n;

public class GameStart : MonoBehaviour
{
    public CanvasGroup introPanel;

    private void Awake()
    {
        Translator.SetDefaultLang("中文"); //设置默认语言为中文，对应 中文.txt
        //设置语言文件读取路径
        var langPath = Path.Join(Application.streamingAssetsPath, "Language");
        Translator.SetLangPath(langPath);
        //获取翻译器单例，同时也是初始化翻译器的一个过程
        Translator.GetInstance();
    }

    private void Start()
    {
        StartAsync().Forget();
    }

    private async UniTask StartAsync()
    {
        introPanel.gameObject.SetActive(true);

        introPanel.alpha = 0;
        await introPanel.DOFade(1, 1f).SetEase(Ease.Linear);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        await introPanel.DOFade(0, 1f).SetEase(Ease.Linear).OnComplete(() => { Destroy(introPanel.gameObject); });

        await MODManager.Init();
        BeforeSceneLoad.ColdBind();
        SceneManager.LoadScene("0_MainMenu");
    }
}