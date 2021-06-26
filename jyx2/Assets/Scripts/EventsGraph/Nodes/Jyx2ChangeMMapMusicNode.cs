using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("音乐音效/修改大地图音乐")]
[NodeWidth(150)]
public class Jyx2ChangeMMapMusicNode : Jyx2SimpleNode
{
    public int MusicId;
    
    private void Reset() {
        name = "修改大地图音乐";
    }

    protected override void DoExecute()
    {
        Jyx2LuaBridge.ChangeMMapMusic(MusicId);
    }
}
