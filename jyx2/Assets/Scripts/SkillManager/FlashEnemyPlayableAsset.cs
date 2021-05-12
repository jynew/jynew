using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[Serializable]
public class FlashEnemyPlayableAsset : PlayableAsset
{
    public ExposedReference<MapRole> m_Role;

    [HideInInspector]
    public int m_Index;

    public override double duration { get { return 0.2f; } }

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var scriptPlayable = ScriptPlayable<FlashEnemyPlayableBehaviour>.Create(graph);
        scriptPlayable.GetBehaviour().m_Role = m_Role.Resolve(graph.GetResolver());
        scriptPlayable.GetBehaviour().m_Index = m_Index;
        return scriptPlayable;
    }
}
