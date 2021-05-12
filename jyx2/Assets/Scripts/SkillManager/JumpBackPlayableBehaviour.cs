using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.UI;

// A behaviour that is attached to a playable
public class JumpBackPlayableBehaviour : PlayableBehaviour
{
    //事件
    public MapRole m_Role;

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
        float speed = Convert.ToSingle(playable.GetDuration());
        SkillManager.Instance.JumpBack(m_Role, speed);
        Debug.Log("OnBehaviourPlay Called");
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        Debug.Log("OnBehaviourPause Called");
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        base.PrepareFrame(playable, info);
        // Debug.Log("PrepareFrame Called");
    }
}
