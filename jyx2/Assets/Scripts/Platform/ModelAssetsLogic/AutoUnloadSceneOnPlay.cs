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
using UnityEngine.SceneManagement;

public class AutoUnloadSceneOnPlay : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(UnloadSceneAsync(gameObject.scene.name));
    }
    
    public IEnumerator UnloadSceneAsync(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        if (!scene.isLoaded)
        {
            Debug.Log("Scene is not loaded : " + sceneName);
            yield break;
        }

        AsyncOperation async = SceneManager.UnloadSceneAsync(scene);
        yield return async;
    }
}
