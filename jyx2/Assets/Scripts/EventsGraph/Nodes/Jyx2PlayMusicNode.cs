using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("音乐音效/播放音乐")]
[NodeWidth(150)]
public class Jyx2PlayMusicNode : Jyx2SimpleNode
{
    [Header("音乐id")]
    public int musicId;
    
    private void Reset() {
        name = "播放音乐";
    }

    protected override void DoExecute()
    {
        Jyx2LuaBridge.PlayMusic(musicId);
    }
}
