using System;
using Cysharp.Threading.Tasks;
using Jyx2;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace TimelineExtend
{
    public class StoryPlayable : PlayableBehaviour
    {
        public bool m_IsAutoTalk;
        public bool m_JumpToEndWhenFinished;

        public double m_CustomClipStart;
        public double m_CustomClipEnd;
        
        public string m_DialogSets;
        
        private PlayableGraph graph;
           
        private bool isEnter = false;
        private bool isLeave = false;
        private bool pass = false;
        private const float FRAME_DELTA_TIME = 1f / 30;
        public override void OnPlayableCreate(Playable playable)
        {
            graph = playable.GetGraph();
        }
        
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!isEnter)
            {
                isEnter = true;
                Enter();
            }

            if (!isLeave)
            {
                if (playable.GetTime() >= (playable.GetDuration() - FRAME_DELTA_TIME))
                {
                    isLeave = true;
                    Leave();
                }    
            }
        }
        async UniTask Enter()
        {
            _multiLineStorySet = m_DialogSets.Split('\n');
            if (_multiLineStorySet.Length == 0)
            {
                Next();
                return;
            }
                
            for (int i = 0; i < _multiLineStorySet.Length; ++i)
            {
                _multiLineStorySet[i] = _multiLineStorySet[i].Trim('\n').Trim('\r');
            }

            _multiLineCurrentIndex = 0;
            while (_multiLineCurrentIndex != _multiLineStorySet.Length)
            {
                await PlaySingleLine(_multiLineStorySet[_multiLineCurrentIndex]);
                _multiLineCurrentIndex++;
            }

            Next();  
            if (m_JumpToEndWhenFinished)
            {
                graph.GetRootPlayable(0).SetTime(m_CustomClipEnd);
            }
        }

        private string[] _multiLineStorySet;

        private int _multiLineCurrentIndex = 0;

        async UniTask PlaySingleLine(string lineStr)
        {
            if (string.IsNullOrEmpty(lineStr))
                return;

            if (lineStr.StartsWith("Talk="))
            {
                var tmp = lineStr.Replace("Talk=", "").Split(',');
                var roleId = int.Parse(tmp[0]);
                var content = tmp[1];
                
                if (!string.IsNullOrEmpty(content))
                {
                    StoryEngine.BlockPlayerControl = true;
                    await Jyx2_UIManager.Instance.ShowUIAsync(nameof(ChatUIPanel), ChatType.RoleId, roleId, content, 0, new Action(() =>
                    {
                        StoryEngine.BlockPlayerControl = false;
                    }));
                }
            }
        }

        void Next()
        {
            pass = true;
            Resume();
        }

        void Leave()
        {
            if (!pass && !m_IsAutoTalk)
            {
                Pause();
            }

            if (m_IsAutoTalk)
            {
                Jyx2_UIManager.Instance.HideUI(nameof(ChatUIPanel));
            }
        }
        
        void Reset()
        {
            pass = false; //reset pass
            isEnter = false;
            isLeave = false;
        }


        void Pause()
        {
            graph.GetRootPlayable(0).SetSpeed(0);
        }

        void Resume()
        {
            if (graph.GetRootPlayable(0).GetSpeed() == 0)
            {
                graph.GetRootPlayable(0).SetSpeed(1);    
            }
        }
    }
}
