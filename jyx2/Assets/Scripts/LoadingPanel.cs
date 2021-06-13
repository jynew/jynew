using System;
using System.Collections;
using System.Collections.Generic;
using HanSquirrel.ResourceManager;
using UnityEngine;
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

        string level = levelKey.Contains("&") ? levelKey.Split('&')[0] : levelKey;
        string command = levelKey.Contains("&") ? levelKey.Split('&')[1] : "";
        var async = SceneManager.LoadSceneAsync(level);
        while (!async.isDone)
        {
            m_LoadingText.text = "载入中... " + (int)(async.progress * 100) + "%";
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
