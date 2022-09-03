using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineExtend
{
    [TrackClipType(typeof(StoryControlAsset))]
    public class StoryControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            foreach (var clip in GetClips())
            {
                var customClip = clip.asset as StoryControlAsset;
                if (customClip != null)
                {
                    customClip.m_CustomClipStart = clip.start;
                    customClip.m_CustomClipEnd = clip.end;
                }
            }
            
            return ScriptPlayable<StoryPlayable>.Create (graph, inputCount);
        }
    }
}
