using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[TrackClipType(typeof(HitPlayableAsset), false)]
[TrackClipType(typeof(GhostShadowPlayableAsset), false)]
[TrackClipType(typeof(DashPlayableAsset), false)]
[TrackClipType(typeof(JumpBackPlayableAsset), false)]
[TrackClipType(typeof(FlashEnemyPlayableAsset), false)]
public class SkillEventTrack : TrackAsset
{
    public SkillEventTrack(){}


    // public ExposedReference<GameObject> HitRole;

    // public override double duration { get{ return 0.01f; } }

    // // Factory method that generates a playable based on this asset
    // public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    // {
    //     var scriptPlayable = ScriptPlayable<HitPlayableBehaviour>.Create(graph);
    //     return scriptPlayable;
    // }
}
