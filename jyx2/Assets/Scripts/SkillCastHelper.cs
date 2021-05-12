
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HanSquirrel.ResourceManager;
using Jyx2.Middleware;
using HSFrameWork.Common;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Jyx2
{
    public interface ISkillCastTarget
    {
        void BeHit();
        void ShowDamage();
    }

    /// <summary>
    /// 技能释放逻辑
    /// </summary>
    public class SkillCastHelper
    {
        public Animator Source;
        public IEnumerable<ISkillCastTarget> Targets;
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
            }

            if (Source != null)
            {
                var skill = Zhaoshi.Data.GetSkill();

                //播放攻击特效
                Source.SetFloat("AttackCode", (float)Zhaoshi.Data.GetDisplay().AttackCode);
                CallWithDelay(() =>
                {
                    Source.SetTrigger("attack");
                }, display.AnimaionDelay);
            }


            //普通特效
            if (!string.IsNullOrEmpty(display.CastEft))
            {
                CallWithDelay(DisplayCastEft, display.CastDelay);
            }

            //格子特效
            if(!string.IsNullOrEmpty(display.BlockEft))
            {
                CallWithDelay(DisplayBlockEft, display.BlockDelay);
            }

            //音效
            if(!string.IsNullOrEmpty(display.AudioEft))
            {
                CallWithDelay(ExecuteSoundEffect, display.AudioEftDelay);
            }
            

            //播放受击动画和飘字
            CallWithDelay(() => {
                ExecuteBeHit();
            }, display.HitDelay); 

            //回调
            if(callback != null)
            {
                CallWithDelay(callback, display.Duration);
            }
        }

        //延迟调用
        private void CallWithDelay(Action action, double time)
        {
            if(time == 0)
            {
                action();
                return;
            }

            Observable.Timer(TimeSpan.FromSeconds(time))
            .Subscribe(ms =>
            {
                action();
            });
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
                CastEffectAndWaitSkill(prefab, duration, Source.transform, offset); //默认预留三秒
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
        /// <param name="frame"></param>
        private void ExecuteBeHit()
        {
            //播放对象受击
            foreach (var target in Targets)
            {
                target.BeHit();
                //平均分配，每次hit显示掉一次血
                target.ShowDamage();
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
