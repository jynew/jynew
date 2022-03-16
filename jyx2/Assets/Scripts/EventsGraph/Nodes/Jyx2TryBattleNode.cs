using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("战斗")]
[NodeWidth(170)]
public class Jyx2TryBattleNode : Jyx2BaseNode
{
    [Output] public Node win;
    [Output] public Node lose;

    [Header("战斗ID")]
    public int BattleId;

    [Header("失败直接GAME OVER")]
    public bool loseGameOver;

    private void Reset() {
        name = "战斗";
    }


    protected override string OnPlay()
    {
        bool ret = Jyx2LuaBridge.TryBattle(BattleId);
        return ret ? nameof(win) : nameof(lose);
    }
}
