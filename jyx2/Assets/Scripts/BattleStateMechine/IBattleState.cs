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
