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

using Jyx2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{

    /// <summary>
    /// 载入场景
    /// </summary>
    /// <param name="sceneAsset">为null则返回主菜单</param>
    /// <returns></returns>
    public static async UniTask Create(AssetReference sceneAsset)
    {
        var loadingPanel = Jyx2ResourceHelper.CreatePrefabInstance("Assets/Prefabs/LoadingPanelCanvas.prefab").GetComponent<LoadingPanel>();
        GameObject.DontDestroyOnLoad(loadingPanel);
        await loadingPanel.LoadLevel(sceneAsset);
    }

    public Text m_LoadingText;

    private async UniTask LoadLevel(AssetReference sceneAsset)
    {
        Jyx2_UIManager.Instance.CloseAllUI();
        gameObject.SetActive(true);
        await UniTask.DelayFrame(1);
        await UniTask.WaitForEndOfFrame(); //否则BattleHelper还没有初始化
        
        //返回主菜单
        if (sceneAsset == null)
        {
            var handle = SceneManager.LoadSceneAsync(GameConst.DefaultMainMenuScene);
            while (!handle.isDone)
            {
                m_LoadingText.text = "载入中... " + (int)(handle.progress * 100) + "%";
                await UniTask.WaitForEndOfFrame();
            }
        }
        //切换场景
        else 
        {
            var async = Addressables.LoadSceneAsync(sceneAsset);
        
            while (!async.IsDone)
            {
                m_LoadingText.text = "载入中... " + (int)(async.PercentComplete * 100) + "%";
                await UniTask.WaitForEndOfFrame();
            }

            GameRuntimeData.Instance.CheckCompass(); //TODO，改成eventListener
        }
        
        Destroy(gameObject);
    }
}
