using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

namespace TimelineExtend
{
    public class StoryControlAsset : PlayableAsset
    {
        [LabelText("是否自动对话，勾选则不需要点鼠标跳过")]
        public bool m_IsAutoTalk;

        [InfoBox("格式：以Talk=id,content)来定义，每行一个，例如\n\n" +
                 "Talk=0,你好啊\n" + 
                 "Talk=1,我很好\n")]
        [Multiline(15)]
        public string m_DialogSets;

        [LabelText("对话完成直接跳转到结束")]
        public bool m_JumpToEndWhenFinished = true;

        [HideInInspector]
        public double m_CustomClipStart;

        [HideInInspector]
        public double m_CustomClipEnd;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            StoryPlayable template = new StoryPlayable()
            {
                m_IsAutoTalk = m_IsAutoTalk,
                m_DialogSets = m_DialogSets,
                m_JumpToEndWhenFinished = m_JumpToEndWhenFinished,
                m_CustomClipStart = m_CustomClipStart,
                m_CustomClipEnd = m_CustomClipEnd,
            };
            
            return ScriptPlayable<StoryPlayable>.Create(graph, template);
        }
    }
}
