/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Animancer;
using DG.Tweening;
using HanSquirrel.ResourceManager;
using Jyx2.Middleware;
using HSFrameWork.Common;
using SkillEffect;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Jyx2
{
    public interface ISkillCastTarget
    {
        Animator GetAnimator();
        HybridAnimancerComponent GetAnimancer();
        GameObject gameObject { get; }
        
        /// <summary>
        /// 待机
        /// </summary>
        void Idle();
        
        /// <summary>
        /// 播放受击动作
        /// </summary>
        void BeHit();
        
        /// <summary>
        /// 播放掉血
        /// </summary>
        void ShowDamage();
    }
    

    /// <summary>
    /// 技能释放逻辑
    /// </summary>
    public class SkillCastHelper
    {
        public Jyx2AnimationBattleRole Source;
        public IEnumerable<Jyx2AnimationBattleRole> Targets;
        public IEnumerable<Transform> CoverBlocks;
        public BattleZhaoshiInstance Zhaoshi;


        Jyx2SkillDisplayAsset GetDisplay()
        {
            return Zhaoshi.Data.GetDisplay();
        }


        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="forceChangeWeapon">是否强行更换武器，一般仅用于技能编辑时看效果</param>
        public void Play(Action callback = null)
        {
            var display = GetDisplay();
            if(display == null)
            {
                Debug.LogError($"招式{Zhaoshi.Key}没有配置Display!");
                if (callback != null) callback();
                return;
            }

            if (Source != null)
            {
                Source.CurDisplay = display;
                GameUtil.CallWithDelay(display.animationDelay, Source.Attack);
            }


            //普通特效
            if (display.partilePrefab != null)
            {
                GameUtil.CallWithDelay(display.particleDelay, DisplayCastEft);
            }

            //格子特效
            if(display.blockPartilePrefab != null)
            {
                DisplayBlockEft();
            }

            //音效
            if(display.audio != null)
            {
                GameUtil.CallWithDelay(display.audioDelay, () => ExecuteSoundEffect(display.audio));
            }

            //音效2
            if (display.audio2 != null)
            {
                GameUtil.CallWithDelay(display.audioDelay2, () => ExecuteSoundEffect(display.audio2));
            }
            
            //播放受击动画和飘字
            GameUtil.CallWithDelay(display.behitDelay, ExecuteBeHit);

            //残影
            if (display.isGhostShadowOn)
            {
                var ghostShadow = GameUtil.GetOrAddComponent<GhostShadow>(Source.transform);
                ghostShadow.m_fDuration = 15;
                ghostShadow.m_fInterval = 0.3f;
                ghostShadow.m_fIntension = 0.4f;
                ghostShadow.m_Color = display.ghostShadowColor;
                ghostShadow.m_bOpenGhost = true;
                GameUtil.CallWithDelay(display.duration, () =>
                {
                    ghostShadow.m_bOpenGhost = false;
                });
            }
            
            //回调
            if(callback != null)
            {
                GameUtil.CallWithDelay(display.duration, callback);
            }
        }


        /// <summary>
        /// 释放特效
        /// </summary>
        /// <param name="pre"></param>
        /// <param name="time"></param>
        /// <param name="parent"></param>
        /// <param name="callback"></param>
        private void CastEffectAndWaitSkill(GameObject pre, float time, Transform parent, Vector3 offset, float scale, Action callback = null)
        {
            if (pre == null) return;
            if (time == 0) time = 2f; //jyx2 修复有的特效无法获取时长的问题

            GameObject obj = GameObject.Instantiate(pre);
            obj.transform.rotation = parent.rotation;
            obj.transform.position = parent.position + offset;
            if (Math.Abs(scale - 1) > 0.001)
            {
                var scaleComponent = obj.AddComponent<ScaleParticles>();
                scaleComponent.ScaleSize = scale;
            }
            
            //修复：将所有的技能特效LOOPING去掉
            foreach (var p in obj.GetComponentsInChildren<ParticleSystem>())
            {
                if (!p.main.loop) continue;
                var m = p.main;
                m.loop = true;
            }
            
            Observable.Timer(TimeSpan.FromSeconds(time))
            .Subscribe(ms =>
            {
                GameObject.Destroy(obj);
                callback?.Invoke();
            });
        }

        private void DisplayCastEft()
        {
            var display = GetDisplay();
            var prefab = display.partilePrefab;
            var duration = HSUnityTools.ParticleSystemLength(prefab.transform);
            var scale = display.particleScale;
            Vector3 offset = display.partileOffset;
            CastEffectAndWaitSkill(prefab, duration, Source.gameObject.transform, offset, scale); //默认预留三秒
        }


        private void DisplayBlockEft()
        {
            var display = GetDisplay();
            var prefab = display.blockPartilePrefab;

            var blockEftDuration = HSUnityTools.ParticleSystemLength(prefab.transform);

            //播放特效
            foreach (var block in CoverBlocks)
            {
                GameUtil.CallWithDelay(display.blockParticleDelay, () =>
                {
                    CastEffectAndWaitSkill(
                        prefab,
                        blockEftDuration,
                        block,
                        display.blockPartileOffset,
                        display.blockParticleScale);
                });
                
                
                //补充特效
                if (display.blockPartilePrefabAdd != null)
                {
                    GameUtil.CallWithDelay(display.blockParticleDelayAdd, () =>
                    {
                        CastEffectAndWaitSkill(
                            display.blockPartilePrefabAdd,
                            display.bloackParticleAddDuration,
                            block,
                            display.blockPartileOffsetAdd,
                            display.blockParticleScaleAdd);
                    });
                }
            }

        }

        /// <summary>
        /// 对象受击
        /// </summary>
        private void ExecuteBeHit()
        {
            //播放对象受击
            foreach (var target in Targets)
            {
                target.BeHit();
                //平均分配，每次hit显示掉一次血
                target.ShowDamage();

                target.MarkHpBarIsDirty();
            }
        }


        private void ExecuteSoundEffect(AudioClip clip)
        {
            var soundEffect = clip;
            if (soundEffect == null)
                return;

            AudioSource.PlayClipAtPoint(soundEffect, Camera.main.transform.position, 1);
        }
    }
}
