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
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.UI;

[System.Serializable]
public class TestPlayableAsset : PlayableAsset
{
    public ExposedReference<GameObject> ShowNumberText;
    private Text text;
    public int startNum;
    public UnityEvent e;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var scriptPlayable = ScriptPlayable<TestPlayableBehaviour>.Create(graph);
        //从ExposedReference中获取我们需要的控件
        text = ShowNumberText.Resolve(graph.GetResolver()).GetComponent<Text>();
        //对指定的PlayableBehaviour中的属性进行赋值
        scriptPlayable.GetBehaviour().ShowNumberText = text;
        scriptPlayable.GetBehaviour().StartNum = startNum;
        return scriptPlayable;
    }
}
