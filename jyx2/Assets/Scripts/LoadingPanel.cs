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
    }

    public Text m_LoadingText;

    private async UniTask LoadLevel(string scenePath)
    {
        Jyx2_UIManager.Instance.CloseAllUI();
        gameObject.SetActive(true);
        await UniTask.DelayFrame(1);
        await UniTask.WaitForEndOfFrame(); //否则BattleHelper还没有初始化

        //返回主菜单
        if (scenePath == null)
        {
            var handle = SceneManager.LoadSceneAsync(GameConst.DefaultMainMenuScene);
            while (!handle.isDone)
            {
                //---------------------------------------------------------------------------
                //m_LoadingText.text = "载入中... " + (int)(handle.progress * 100) + "%";
                //---------------------------------------------------------------------------
                //特定位置的翻译【载入中文本显示】
                //---------------------------------------------------------------------------
                m_LoadingText.text = "载入中…… ".GetContent(nameof(LoadingPanel)) + (int)(handle.progress * 100) + "%";
                //---------------------------------------------------------------------------
                //---------------------------------------------------------------------------
                await UniTask.WaitForEndOfFrame();
            }
        }
        //切换场景
        else
        {
            m_LoadingText.text = "载入中... ";
            await ResLoader.LoadScene(scenePath);


            /*if (MODLoader.Remap.ContainsKey(scenePath))
            {
                var assetBundleItem = MODLoader.Remap[scenePath];
                var handle = SceneManager.LoadSceneAsync(assetBundleItem.Name);
                while (!handle.isDone)
                {
                    //---------------------------------------------------------------------------
                    //m_LoadingText.text = "载入中... " + (int)(handle.progress * 100) + "%";
                    //---------------------------------------------------------------------------
                    //特定位置的翻译【载入中文本显示】
                    //---------------------------------------------------------------------------
                    m_LoadingText.text =
                        "载入中…… ".GetContent(nameof(LoadingPanel)) + (int) (handle.progress * 100) + "%";
                    //---------------------------------------------------------------------------
                    //---------------------------------------------------------------------------
                    await UniTask.WaitForEndOfFrame();
                }
            }
            else
            {
                var async = Addressables.LoadSceneAsync(scenePath);
                while (!async.IsDone)
                {
                    //---------------------------------------------------------------------------
                    //m_LoadingText.text = "载入中... " + (int)(async.PercentComplete * 100) + "%";
                    //---------------------------------------------------------------------------
                    //特定位置的翻译【载入中文本显示】
                    //---------------------------------------------------------------------------
                    m_LoadingText.text = "载入中…… ".GetContent(nameof(LoadingPanel)) +
                                         (int) (async.PercentComplete * 100) + "%";
                    //---------------------------------------------------------------------------
                    //---------------------------------------------------------------------------
                    await UniTask.WaitForEndOfFrame();
                }
            }*/
        }

        Destroy(gameObject);
    }
}
