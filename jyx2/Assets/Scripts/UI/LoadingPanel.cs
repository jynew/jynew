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
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using i18n.TranslatorDef;
using Jyx2;
using Jyx2.MOD;
using Jyx2.ResourceManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadingPanel : MonoBehaviour
{
    public static void Show(UniTask task)
    {
        if (_curLoadingPanel != null)
        {
            Debug.LogError("错误：前一个LoadingPanel还未关闭");
            return;
        }
        _curLoadingPanel = Jyx2ResourceHelper.CreatePrefabInstance("LoadingPanelCanvas").GetComponent<LoadingPanel>();
        DontDestroyOnLoad(_curLoadingPanel);
    }

    public static void Hide()
    {
        if (_curLoadingPanel != null)
            Destroy(_curLoadingPanel.gameObject);
    }

    private static LoadingPanel _curLoadingPanel = null; 
    
    /// <summary>
    /// 载入场景
    /// </summary>
    /// <param name="scenePath">为null则返回主菜单</param>
    /// <returns></returns>
    public static async UniTask Create(string scenePath)
    {
        var loadingPanel = Jyx2ResourceHelper.CreatePrefabInstance("LoadingPanelCanvas").GetComponent<LoadingPanel>();
        GameObject.DontDestroyOnLoad(loadingPanel);
        await loadingPanel.LoadLevel(scenePath);
        Destroy(loadingPanel.gameObject);
    }

    public Text m_LoadingText;

    public async UniTask LoadLevel(string scenePath)
    {
        Jyx2_UIManager.Instance.HideAllUI();
        gameObject.SetActive(true);
        await UniTask.DelayFrame(1);
        await UniTask.WaitForEndOfFrame(); //否则BattleHelper还没有初始化

        //返回主菜单
        if (scenePath == null)
        {
            var handle = SceneManager.LoadSceneAsync(GameConst.DefaultMainMenuScene);
            while (!handle.isDone)
            {
                m_LoadingText.text = "载入中…… ".GetContent(nameof(LoadingPanel)) + (int)(handle.progress * 100) + "%";
                await UniTask.WaitForEndOfFrame();
            }
        }
        //切换场景
        else
        {
            m_LoadingText.text = "载入中... ";
            await ResLoader.LoadScene(scenePath);
        }
    }
}
