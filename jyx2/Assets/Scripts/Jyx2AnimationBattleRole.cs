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
using Animancer;
using UnityEngine;

namespace Jyx2
{
    /// <summary>
    /// 动画播放者
    /// </summary>
    public abstract class Jyx2AnimationBattleRole : MonoBehaviour
    {
        public abstract Animator GetAnimator();

        private HybridAnimancerComponent _animancer;
        public HybridAnimancerComponent GetAnimancer()
        {
            if (_animancer == null)
            {
                var animator = GetAnimator();
                _animancer = GameUtil.GetOrAddComponent<HybridAnimancerComponent>(animator.transform);
                _animancer.Animator = animator;
                _animancer.Controller = animator.runtimeAnimatorController;
            }
            return _animancer;
        }
        
        /// <summary>
        /// 当前的技能播放
        /// </summary>
        public Jyx2SkillDisplayAsset CurDisplay { get; set; }

        bool IsStandardModelAvata()
        {
            var animator = GetAnimator();
            var controller = animator.runtimeAnimatorController;
            return controller.name == "jyx2humanoidController.controller";
        }
        
        public virtual void Idle()
        {
            if (this == null)
                return;

            PlayAnimation(CurDisplay.LoadAnimation(Jyx2SkillDisplayAsset.Jyx2RoleAnimationType.Idle));
        }

        public virtual void DeadOrIdle()
        {
            Idle();
        }
        
        public virtual void BeHit()
        {
            if (this == null)
                return;

            AnimationClip clip = null;
            if (CurDisplay == null)
            {
                clip = GlobalAssetConfig.Instance.defaultBeHitClip;
            }
            else
            {
                clip = CurDisplay.LoadAnimation(Jyx2SkillDisplayAsset.Jyx2RoleAnimationType.Behit);
            }
            
            PlayAnimation(clip, DeadOrIdle, 0.25f);
        }

        public virtual void Attack()
        {
            if (this == null)
                return;

            PlayAnimation(CurDisplay.LoadAnimation(Jyx2SkillDisplayAsset.Jyx2RoleAnimationType.Attack), 
                Idle, 0.25f);
        }

        public virtual void Run()
        {
            if (this == null)
                return;
            
            AnimationClip clip = null;
            if (CurDisplay == null)
            {
                clip = GlobalAssetConfig.Instance.defaultMoveClip;
            }
            else
            {
                clip = CurDisplay.LoadAnimation(Jyx2SkillDisplayAsset.Jyx2RoleAnimationType.Move);
            }

            PlayAnimation(clip);
        }

        public virtual void ShowDamage()
        {
            //DONOTHING
        }

        public virtual void MarkHpBarIsDirty()
        {
            //DONOTHING
        }

        public virtual void UnmarkHpBarIsDirty()
        {
            //DONOTHING
        }

        protected void PlayAnimation(AnimationClip clip, Action callback = null, float fadeDuration = 0f)
        {
            if (clip == null)
            {
                Debug.LogError("调用了空的动作!");
                callback ? .Invoke();
                return;
            }
            
            var animancer = GetAnimancer();

            //检查动作配置是否正确
            if (clip.isLooping && callback != null)
            {
                Debug.LogError($"动作设置了LOOP但是会有回调！请检查{clip.name}");
            }
            else if (!clip.isLooping && callback == null)
            {
                Debug.LogError($"动作没设置LOOP但是没有回调！请检查{clip.name}");
            }
            
            var state = animancer.Play(clip, 0.25f);

            if (callback != null)
            {
                if (fadeDuration > 0)
                {
                    GameUtil.CallWithDelay(state.Duration - fadeDuration, callback);
                }
                else
                {
                    state.Events.OnEnd = () =>
                    {
                        state.Stop();
                        callback();
                    };
                }
            }
        }
    }
}
    
