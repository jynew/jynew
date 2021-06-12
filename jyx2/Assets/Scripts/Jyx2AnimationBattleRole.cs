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
        public Jyx2SkillDisplay CurDisplay { get; set; }

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
            
            string code = CurDisplay.IdleAnim;
            if (!PlayScriptAnimation(code))
            {
                var animator = GetAnimator();
                if (animator != null)
                {
                    animator.SetBool("InBattle", true);
                    animator.SetFloat("PosCode", float.Parse(code));
                    animator.SetTrigger("battle_idle");    
                }
            }
        }
        
        public virtual void BeHit()
        {
            if (this == null)
                return;

            string code = CurDisplay.GetBeHitAnimationCode();
            if (!PlayScriptAnimation(code, Idle, 0.25f))
            {
                var animator = GetAnimator();
                if (animator != null)
                {
                    animator.SetTrigger("hit");
                }
            }
        }

        public virtual void Attack()
        {
            if (this == null)
                return;

            string code = CurDisplay.AttackAnim;
            if (!PlayScriptAnimation(code, Idle, 0.25f))
            {
                var animator = GetAnimator();
                if (animator != null)
                {
                    animator.SetFloat("AttackCode", float.Parse(code));
                    animator.SetTrigger("attack");
                }
            }
        }

        public virtual void Run()
        {
            if (this == null)
                return;

            string code = CurDisplay.RunAnim; //TODO
            if (!PlayScriptAnimation(code))
            {
                var animator = GetAnimator();
                if (animator != null)
                {
                    animator.SetBool("InBattle", true);
                    animator.SetFloat("PosCode", float.Parse(code));
                    animator.ResetTrigger("battle_idle");
                    animator.SetTrigger("move");
                }
            }
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

        private bool PlayScriptAnimation(string animCode, Action callback = null, float fadeDuration = 0f)
        {
            var animancer = GetAnimancer();
            
            //直接指定地址
            if (animCode.StartsWith("@"))
            {
                string path = animCode.TrimStart('@');
                
                Jyx2ResourceHelper.LoadAsset<AnimationClip>(path, (clip) =>
                {
                    //检查动作配置是否正确
                    if (clip.isLooping && callback != null)
                    {
                        Debug.LogError($"动作设置了LOOP但是会有回调！请检查{path}");
                    }
                    else if (!clip.isLooping && callback == null)
                    {
                        Debug.LogError($"动作没设置LOOP但是没有回调！请检查{path}");
                    }

                    var state = animancer.Play(clip);


                    //callback if needed
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
                });
                return true;
            }
            else
            {
                //animancer.Stop(); //force switch to AnimationController
                animancer.PlayController(); //fade to AnimationController
                return false;
            }   
        }
    }
}
    
