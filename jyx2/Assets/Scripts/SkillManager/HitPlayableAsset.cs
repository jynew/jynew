using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[Serializable]
public class HitPlayableAsset : PlayableAsset
{
    public List<ExposedMapRole> m_HitRoleList;

    [FilePath(ParentFolder = "Assets/Effects/Prefabs", Extensions = "prefab", UseBackslashes = true)]
    public string m_Effect;

    [FilePath(ParentFolder = "Assets/BuildSource/SoundEffect")]
    public string m_Voice;

    [HideInInspector]
    public int m_DamageCount = 1;

    [HideInInspector]
    public bool m_IsEndHit = false;

    public override double duration { get { return 0.2f; } }

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var scriptPlayable = ScriptPlayable<HitPlayableBehaviour>.Create(graph);
        scriptPlayable.GetBehaviour().m_HitRoleList = new List<MapRole>();
        foreach (var exposed in m_HitRoleList)
        {
            if (exposed == null) continue;
            var mapRole = exposed.m_MapRole.Resolve(graph.GetResolver());
            if (mapRole == null)
            {
                var g = (Animator)exposed.m_MapRole.defaultValue;
                mapRole = g.GetComponent<MapRole>();
            }
            scriptPlayable.GetBehaviour().m_HitRoleList.Add(mapRole);
        }
        scriptPlayable.GetBehaviour().m_Effect = m_Effect;
        scriptPlayable.GetBehaviour().m_Voice = m_Voice;
        scriptPlayable.GetBehaviour().m_DamageCount = m_DamageCount;
        scriptPlayable.GetBehaviour().m_IsEndHit = m_IsEndHit;
        return scriptPlayable;
    }
}
