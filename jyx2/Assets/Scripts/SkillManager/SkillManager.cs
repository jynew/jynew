using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Hanjiasongshu.ThreeD.XML;
using HanSquirrel.ResourceManager;
using Jyx2;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<SkillManager>();
            return _instance;
        }
    }
    private static SkillManager _instance;

    public Animator m_Source;
    public List<Animator> m_TargetList;
    public Animator battleCamera;
    public Animator m_SlowMotion;

    private string _currentAnimation;
    private Dictionary<string, PlayableBinding> _bindingDic;

    private bool _isFlashEnemyDash = false;

    public PlayableDirector CreateTimeLine(TimelineAsset timeline)
    {
        _isFlashEnemyDash = false;
        //获取Track
        Dictionary<string, TrackAsset> tracks = new Dictionary<string, TrackAsset>();
        foreach (var t in timeline.GetOutputTracks())
        {
            if (!tracks.ContainsKey(t.name))
            {
                tracks[t.name] = t;
            }
        }

        tracks["Source"].muted = true;
        tracks["SourceEffect"].muted = true;
        // tracks["Target"].muted = true;
        tracks["TargetEffect"].muted = true;
        //tracks["CameraShake"].muted = true;

        //获取当前待机动作
        var sourceIdleClip = GetIdleAnimation(m_Source);

        //新增攻击者轨道
        var attackTrack = timeline.CreateTrack<AnimationTrack>(null, $"Source0");
        CloneAttackAnimation(tracks["Source"], attackTrack);

        //追加攻击前动作
        //AddAnimationOnStart(attackTrack, sourceIdleClip);

        //追加攻击后动作
        //AddAnimationOnEnd(attackTrack, sourceIdleClip);

        //加到轨道列
        tracks.Add($"Source0", attackTrack);

        //新增摄像机震动轨道
        //var CameraShake = timeline.CreateTrack<AnimationTrack>(null, $"CameraShake0");
        //CloneAttackAnimation(tracks["CameraShake"], CameraShake);
        //加到摄像机震动轨道列
        //tracks.Add($"CameraShake0", CameraShake);

        //新增攻击特效轨道
        var attackEffectTrack = timeline.CreateTrack<ControlTrack>(null, $"SourceEffect0");
        CloneEffect(tracks["SourceEffect"], attackEffectTrack, m_Source.gameObject);

        //加到轨道列
        tracks.Add($"SourceEffect0", attackEffectTrack);

        //攻击特效轨道
        foreach (var c in tracks["SourceEffect0"].GetClips())
        {
            ControlPlayableAsset asset = (ControlPlayableAsset)c.asset;
            asset.sourceGameObject.defaultValue = m_Source.gameObject;
        }

        //残影事件
        if (tracks.ContainsKey("GhostShadowEvent") && m_Source != null)
        {
            var eventList = tracks["GhostShadowEvent"].GetClips().ToList();
            for (int i = 0; i < eventList.Count; i++)
            {
                if (eventList[i].asset.GetType().Name == "GhostShadowPlayableAsset")
                {
                    GhostShadowPlayableAsset asset = (GhostShadowPlayableAsset)eventList[i].asset;
                    if (asset == null) continue;
                    asset.m_Role.defaultValue = m_Source.GetComponent<MapRole>();
                }
            }
        }

        //Dash事件
        int flashEnemyIndex = 0;
        if (tracks.ContainsKey("DashEvent") && m_Source != null)
        {
            var eventList = tracks["DashEvent"].GetClips().ToList();
            for (int i = 0; i < eventList.Count; i++)
            {
                if (eventList[i].asset.GetType().Name == "DashPlayableAsset")
                {
                    DashPlayableAsset asset = (DashPlayableAsset)eventList[i].asset;
                    if (asset == null) continue;
                    asset.m_Role.defaultValue = m_Source.GetComponent<MapRole>();
                }
                else if (eventList[i].asset.GetType().Name == "FlashEnemyPlayableAsset")
                {
                    FlashEnemyPlayableAsset asset = (FlashEnemyPlayableAsset)eventList[i].asset;
                    if (asset == null) continue;
                    asset.m_Index = flashEnemyIndex++;
                    asset.m_Role.defaultValue = m_Source.GetComponent<MapRole>();
                    _isFlashEnemyDash = true;
                }
                else if (eventList[i].asset.GetType().Name == "JumpBackPlayableAsset")
                {
                    JumpBackPlayableAsset asset = (JumpBackPlayableAsset)eventList[i].asset;
                    if (asset == null) continue;
                    asset.m_Role.defaultValue = m_Source.GetComponent<MapRole>();
                }
            }
        }

        //受击者
        if (tracks.ContainsKey("Target") && m_TargetList.Count > 0)
        {
            var hitList = tracks["Target"].GetClips().ToList();
            for (int i = 0; i < hitList.Count; i++)
            {
                if (hitList[i].asset.GetType().Name != "HitPlayableAsset") continue;
                HitPlayableAsset hitAsset = (HitPlayableAsset)hitList[i].asset;
                if (hitAsset == null) continue;
                hitAsset.m_HitRoleList = new List<ExposedMapRole>();
                if (_isFlashEnemyDash)
                {
                    hitAsset.m_IsEndHit = true;
                    var currentIndex = i % m_TargetList.Count;
                    ExposedMapRole exposed = new ExposedMapRole();
                    exposed.m_MapRole.defaultValue = m_TargetList[currentIndex];
                    hitAsset.m_HitRoleList.Add(exposed);
                }
                else
                {
                    hitAsset.m_DamageCount = hitList.Count;
                    if (i == hitList.Count - 1) hitAsset.m_IsEndHit = true;
                    foreach (var target in m_TargetList)
                    {
                        ExposedMapRole exposed = new ExposedMapRole();
                        // PropertyName id = new PropertyName(exposed);
                        exposed.m_MapRole.defaultValue = target;
                        hitAsset.m_HitRoleList.Add(exposed);
                    }
                }
                // hitAsset.m_HitRoleList = roleList;
            }
        }

        //创建PlayableDirector
        var playableDirector = new GameObject().AddComponent<PlayableDirector>();
        playableDirector.playableAsset = timeline;

        //获取待绑定目标
        _bindingDic = new Dictionary<string, PlayableBinding>(); // PlayableAsset下的所有binding
        foreach (var o in playableDirector.playableAsset.outputs) // 每一个binding，其实就是trackasset和需要动画的模型之间的链接关系
        {
            var trackname = o.streamName;
            _bindingDic.Add(trackname, o); // 每一个binding的名字和binding绑定
        }

        //绑定对象
        playableDirector.SetGenericBinding(_bindingDic["Source0"].sourceObject, m_Source);

        //绑定CameraShake对象
        if (battleCamera != null)
            playableDirector.SetGenericBinding(_bindingDic["CameraShake"].sourceObject, battleCamera.GetComponent<Animator>());
        //绑定Camera Animation对象
        //if(!tracks["CameraAnimation"].isEmpty)
        //playableDirector.SetGenericBinding(_bindingDic["CameraAnimation"].sourceObject, battleCamera.GetComponent<Animator>());
        //绑定SlowMotion对象
        if (m_SlowMotion != null)
            playableDirector.SetGenericBinding(_bindingDic["SlowMotion"].sourceObject, m_SlowMotion.GetComponent<Animator>());

        //绑定角色音效
        playableDirector.SetGenericBinding(_bindingDic["PlayerAudio1"].sourceObject, m_Source);
        playableDirector.SetGenericBinding(_bindingDic["PlayerAudio2"].sourceObject, m_Source);

        return playableDirector;
    }

    public TimelineClip CloneTimeLineClip(TimelineClip clip, TrackAsset track)
    {
        var newClip = track.CreateDefaultClip();
        newClip.clipIn = clip.clipIn;
        newClip.duration = clip.duration;
        newClip.easeInDuration = clip.easeInDuration;
        newClip.easeOutDuration = clip.easeOutDuration;
        newClip.mixInCurve = clip.mixInCurve;
        newClip.mixOutCurve = clip.mixOutCurve;
        newClip.start = clip.start;
        newClip.timeScale = clip.timeScale;
        newClip.asset = clip.asset;
        newClip.blendInCurveMode = clip.blendInCurveMode;
        newClip.blendInDuration = clip.blendInDuration;
        newClip.blendOutCurveMode = clip.blendOutCurveMode;
        newClip.blendOutDuration = clip.blendOutDuration;
        newClip.displayName = clip.displayName;
        return newClip;
    }

    public ControlPlayableAsset CloneControlPlayableAsset(ControlPlayableAsset asset, GameObject target)
    {
        var newAsset = new ControlPlayableAsset
        {
            prefabGameObject = asset.prefabGameObject,
            updateParticle = asset.updateParticle,
            particleRandomSeed = asset.particleRandomSeed,
            updateDirector = asset.updateDirector,
            updateITimeControl = asset.updateITimeControl,
            searchHierarchy = asset.searchHierarchy,
            active = asset.active,
            postPlayback = asset.postPlayback,
            hideFlags = asset.hideFlags,
        };
        newAsset.sourceGameObject.defaultValue = target;
        return newAsset;
    }

    public void Play(RoleInstance source, List<RoleInstance> targets, string skillKey)
    {
        m_Source = source.View.GetComponent<Animator>();
        m_TargetList = new List<Animator>();
        foreach (var t in targets)
        {
            m_TargetList.Add(t.View.GetComponent<Animator>());
        }
        Play(skillKey);
    }

    public void Play(string skillKey)
    {
        Jyx2ResourceHelper.LoadAsset<TimelineAsset>($"{ConStr.SkillPath}{skillKey}.playable", timelineAsset=> {
            var timeline = Instantiate(timelineAsset);
            var playableDirector = CreateTimeLine(timeline);

            playableDirector.Play();
            playableDirector.stopped += delegate
            {
                Destroy(timeline);
                Destroy(playableDirector.gameObject);
            };
        });
    }

    /// <summary>
    /// 获取待机动作
    /// </summary>
    /// <param name="animator"></param>
    /// <returns></returns>
    public AnimationClip GetIdleAnimation(Animator animator)
    {
        return null;
        //var clip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        //if (!clip.name.Contains("Idle"))
        //{
        //    var weaponType = (WeaponEnum)animator.GetInteger("WeaponType");
        //    string weaponTypeStr = weaponType.ToString();
        //    if (weaponTypeStr == "Cudgel")
        //        weaponTypeStr = "Gudgel";
        //    clip = ResourceLoader.LoadAsset<AnimationClip>($"Assets/3D/Animation/Nan/M_{weaponTypeStr}_Idle1.anim");
        //}
        //return clip;
    }

    /// <summary>
    /// 获取受击动作
    /// </summary>
    /// <param name="animator"></param>
    /// <returns></returns>
    public AnimationClip GetHitAnimation(Animator animator)
    {
        return null;
        //var weaponType = (WeaponEnum)animator.GetInteger("WeaponType");
        //string weaponTypeStr = weaponType.ToString();
        //if (weaponTypeStr == "Cudgel")
        //    weaponTypeStr = "Gudgel";
        //var clip = ResourceLoader.LoadAsset<AnimationClip>($"Assets/3D/Animation/Nan/M_{weaponTypeStr}_Hit1.anim");
        //return clip;
    }

    /// <summary>
    /// 克隆攻击动作
    /// </summary>
    public void CloneAttackAnimation(TrackAsset track, AnimationTrack newTrack)
    {
        foreach (var clip in track.GetClips())
        {
            var newClip = CloneTimeLineClip(clip, newTrack);
        }
    }

    /// <summary>
    /// 克隆特效
    /// </summary>
    /// <param name="track"></param>
    /// <param name="newTrack"></param>
    /// <param name="target"></param>
    private void CloneEffect(TrackAsset track, ControlTrack newTrack, GameObject target)
    {
        foreach (var clip in track.GetClips())
        {
            var newClip = CloneTimeLineClip(clip, newTrack);
            var asset = CloneControlPlayableAsset((ControlPlayableAsset)clip.asset, target);
            newClip.asset = asset;
        }
    }

    public void Dash(MapRole source, float duration)
    {
        if (m_TargetList != null && m_TargetList.Count > 0)
        {
            var currentEnemy = m_TargetList[0];
            var relativePos = (currentEnemy.transform.position - source.transform.position).normalized;
            source.LookAtWorldPosInBattle(currentEnemy.transform.position);
            Sequence seq = DOTween.Sequence();
            seq.Append(source.transform.DOMove(currentEnemy.transform.position - relativePos, duration));
        }
    }

    public void JumpBack(MapRole source, float duration)
    {
        var battleBlock = BattleboxHelper.Instance.GetBlockData(source.DataInstance.Pos.X, source.DataInstance.Pos.Y);
        Sequence seq = DOTween.Sequence();
        seq.Append(source.transform.DOMove(battleBlock.WorldPos, duration));
    }

    public void FlashEnemy(MapRole source, float duration, int index = 0)
    {
        if (m_TargetList != null && m_TargetList.Count > 0)
        {
            var currentIndex = index % m_TargetList.Count;
            var currentEnemy = m_TargetList[currentIndex];
            var relativePos = (currentEnemy.transform.position - source.transform.position).normalized;
            Sequence seq = DOTween.Sequence();
            seq.Append(source.transform.DOLookAt(currentEnemy.transform.position, 0.01f));
            seq.Append(source.transform.DOMove(currentEnemy.transform.position + relativePos * 1.5f, duration));
        }
    }
}