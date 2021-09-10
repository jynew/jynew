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
using HanSquirrel.ResourceManager;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{

    public static void Create(string level,Action callback)
    {
        var loadingPanel = Jyx2ResourceHelper.CreatePrefabInstance("Assets/Prefabs/LoadingPanelCanvas.prefab").GetComponent<LoadingPanel>();
        loadingPanel.LoadLevel(level, callback);
        GameObject.DontDestroyOnLoad(loadingPanel);
    }

    public Text m_LoadingText;

    public void LoadLevel(string levelKey, Action callback)
    {
        Jyx2_UIManager.Instance.CloseAllUI();
        this.gameObject.SetActive(true);
        StartCoroutine(DoLoadLevel(levelKey, callback));
    }

    IEnumerator DoLoadLevel(string levelKey, Action callback)
    {
        yield return 0; //否则BattleHelper还没有初始化

        //如果是返回主菜单
        if (levelKey.Equals(GameConst.DefaultMainMenuScene))
        {
            var loadAsync = SceneManager.LoadSceneAsync(levelKey);
            
            while (!loadAsync.isDone)
            {
                m_LoadingText.text = "载入中... " + (int)(loadAsync.progress * 100) + "%";
                yield return 0;
            }
            if (callback != null)
                callback();
            GameObject.Destroy(this.gameObject);
        }
        else //否则动态载入场景
        {
            string level = levelKey.Contains("&") ? levelKey.Split('&')[0] : levelKey;
            string command = levelKey.Contains("&") ? levelKey.Split('&')[1] : "";
            //var async = SceneManager.LoadSceneAsync(level);

            //苟且写法
            string scenePath = "";
            if (level.Contains("Battle"))
            {
                scenePath = $"Assets/Jyx2BattleScene/{level}.unity";
            }
            else
            {
                scenePath = $"Assets/Jyx2Scenes/{level}.unity";
            }
        
            var async = Addressables.LoadSceneAsync(scenePath);
        
            while (!async.IsDone)
            {
                m_LoadingText.text = "载入中... " + (int)(async.PercentComplete * 100) + "%";
                yield return 0;
            }
            if (!string.IsNullOrEmpty(command))
            {
                StoryEngine.Instance.ExecuteCommand(command, null);
            }
            yield return 0;

            //Jyx2_UIManager.Instance.ShowMainUI(levelKey);
            if (callback != null)
                callback();

            GameObject.Destroy(this.gameObject);
        }
    }
}
