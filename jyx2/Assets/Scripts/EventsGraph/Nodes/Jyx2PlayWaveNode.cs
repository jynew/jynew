using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("音乐音效/播放音效")]
[NodeWidth(150)]
public class Jyx2PlayWaveNode : Jyx2SimpleNode
{
    public int WaveId;
    
    private void Reset() {
        name = "播放音效";
    }

    protected override void DoExecute()
    {
        Jyx2LuaBridge.PlayWave(WaveId);
    }
}
