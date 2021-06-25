/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using Jyx2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IBattleState
{
    public abstract void OnEnterState();
    /// <summary>
    /// 如果状态机本来就在这个状态 说明需要刷新下这个状态
    /// </summary>
    public virtual void RefreshState() { }
    public abstract void OnLeaveState();
    public virtual void OnUpdate() { }
}
