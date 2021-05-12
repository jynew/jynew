using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.UI;

// A behaviour that is attached to a playable
public class HitPlayableBehaviour : PlayableBehaviour
{
    //事件
    public List<MapRole> m_HitRoleList;

    public string m_Effect;

    public string m_Voice;

    public int m_DamageCount = 1;

    public bool m_IsEndHit = false;

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
        foreach (var hitRole in m_HitRoleList)
        {
            hitRole.HitEffect(m_Effect, 1f, m_IsEndHit);
            hitRole.HitVoice(m_Voice);
            hitRole.ShowDamage();
        }
        Debug.Log("OnBehaviourPlay Called");
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        // Debug.Log("OnBehaviourPause Called");
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        base.PrepareFrame(playable, info);
        // Debug.Log("PrepareFrame Called");
    }
}
