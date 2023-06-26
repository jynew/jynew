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

public class DebugInfoManager : MonoBehaviour
{
    public static void Init()
    {
        var obj = FindObjectOfType<DebugInfoManager>();
        if (obj != null)
            return;

        //否则初始化
        var prefab = Resources.Load<GameObject>("DebugInfoManager");
        var newObj = Instantiate(prefab) as GameObject;
        newObj.name = "[DebugInfoManager]";
        DontDestroyOnLoad(newObj);
    }

    public Text m_FpsText;
    public float fps_updateInterval = 0.5F;

    private double lastInterval;
    private int frames = 0;
    private float fps;
    void CalcFps()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + fps_updateInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;
        }
        m_FpsText.text = string.Format("FPS={0:f2}", fps);
    }

    // Update is called once per frame
    void Update()
    {
        CalcFps();
    }
}
