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
using Jyx2;
using UnityEngine;

/// <summary>
/// 随着游戏存档进行调整
/// </summary>
public class FixWithGameRuntime : MonoBehaviour
{
    public enum FixTypeEnum
    {
        Move,
    }

    public FixTypeEnum FixType = FixTypeEnum.Move;
    public string Flag;
    public Transform MoveTo;

    public void Reload()
    {
        var runtime = GameRuntimeData.Instance;
        if (runtime != null)
        {
            if (runtime.KeyExist(Flag))
            {
                if (MoveTo == null)
                {
                    Debug.LogError("FixWithGameRuntime 未定义的移动目标：");
                }
                else
                {
                    transform.position = MoveTo.position;
                    transform.rotation = MoveTo.rotation;
                }
            }
        }
    }
}
