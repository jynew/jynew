using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("战斗")]
[NodeWidth(256)]
public class Jyx2TryBattleNode : Node
{
    [Input] public Node prev;
    [Output] public Node win;
    [Output] public Node lose;

    [Header("战斗ID")]
    public int BattleId;
    
    private void Reset() {
        name = "战斗";
    }
}
