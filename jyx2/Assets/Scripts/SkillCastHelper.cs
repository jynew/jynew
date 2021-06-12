
using System;
using System.Collections.Generic;
using System.Linq;
using Animancer;
using DG.Tweening;
using HanSquirrel.ResourceManager;
using Jyx2.Middleware;
using HSFrameWork.Common;
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


        Jyx2SkillDisplay GetDisplay()
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
                GameUtil.CallWithDelay(display.AnimaionDelay, Source.Attack);
            }


            //普通特效
            if (!string.IsNullOrEmpty(display.CastEft))
            {
                GameUtil.CallWithDelay(display.CastDelay, DisplayCastEft);
            }

            //格子特效
            if(!string.IsNullOrEmpty(display.BlockEft))
            {
                GameUtil.CallWithDelay(display.BlockDelay, DisplayBlockEft);
            }

            //音效
            if(!string.IsNullOrEmpty(display.AudioEft))
            {
                GameUtil.CallWithDelay(display.AudioEftDelay,ExecuteSoundEffect);
            }
            
            //播放受击动画和飘字
            GameUtil.CallWithDelay(display.HitDelay, ExecuteBeHit);

            //回调
            if(callback != null)
            {
                GameUtil.CallWithDelay(display.Duration, callback);
            }
        }


        /// <summary>
        /// 释放特效
        /// </summary>
        /// <param name="pre"></param>
        /// <param name="time"></param>
        /// <param name="parent"></param>
        /// <param name="callback"></param>
        private void CastEffectAndWaitSkill(GameObject pre, float time, Transform parent, Vector3 offset, Action callback = null)
        {
            if (pre == null) return;

            GameObject obj = GameObject.Instantiate(pre);
            obj.transform.rotation = parent.rotation;
            obj.transform.position = parent.position + offset;
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

            //播放特效
            Jyx2ResourceHelper.LoadPrefab(display.CastEft, prefab=> {

                var duration = HSUnityTools.ParticleSystemLength(prefab.transform);

                Vector3 offset = Vector3.zero;
                if (!string.IsNullOrEmpty(display.CastOffset))
                {
                    offset = UnityTools.StringToVector3(display.CastOffset, ',');
                }
                CastEffectAndWaitSkill(prefab, duration, Source.gameObject.transform, offset); //默认预留三秒
            });
        }


        private void DisplayBlockEft()
        {
            var display = GetDisplay();

            Jyx2ResourceHelper.LoadPrefab(display.BlockEft,prefab=> {

                var blockEftDuration = HSUnityTools.ParticleSystemLength(prefab.transform);

                Vector3 offset = Vector3.zero;
                if (!string.IsNullOrEmpty(display.BlockOffset))
                {
                    offset = UnityTools.StringToVector3(display.BlockOffset, ',');
                }

                //播放特效
                foreach (var block in CoverBlocks)
                {
                    CastEffectAndWaitSkill(prefab, blockEftDuration, block, offset);
                }
            });
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


        private void ExecuteSoundEffect()
        {
            var display = GetDisplay();
            Jyx2ResourceHelper.LoadAsset<AudioClip>(display.AudioEft, soundEffect =>
            {
                if (soundEffect == null)
                    return;

                AudioSource.PlayClipAtPoint(soundEffect, Camera.main.transform.position, 1);
            });
        }

    }
}
