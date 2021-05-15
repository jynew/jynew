using System.Collections;
using System.Collections.Generic;
using Animancer;
using HSFrameWork.ConfigTable;
using UnityEngine;
using Jyx2;
using UnityEngine.AddressableAssets;

public class Jyx2SkillEditorEnemy : MonoBehaviour, ISkillCastTarget
{
    Animator animator;

    public Animator GetAnimator()
    {
        return animator;
    }
    
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

    [HideInInspector]
    public string battleIdlePoseCode = "0";
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static AnimationClip standardBehitAnim = null;
    
    
    public void BeHit()
    {
        if (standardBehitAnim == null)
        {
            string path = "Assets/3D/Animation/Jyx2Anims/标准受击.anim";
            Addressables.LoadAssetAsync<AnimationClip>(path).Completed += r =>
            {
                standardBehitAnim = r.Result;
                PlayBeHit();
            };
        }
        else
        {
            PlayBeHit();
        }
    }

    private void PlayBeHit()
    {
        var animancer = GetAnimancer();
        var state = animancer.Play(standardBehitAnim, 0.25f);
        state.Events.OnEnd = () =>
        {
            state.Stop();
            Idle();
        };
    }

    public void ShowDamage()
    {

       
    }
    public void Idle()
    {
        var animancer = GetAnimancer();
        
        //指定动作
        if (battleIdlePoseCode.StartsWith("@"))
        {
            string path = battleIdlePoseCode.TrimStart('@');
            Addressables.LoadAssetAsync<AnimationClip>(path).Completed += r =>
            {
                animancer.Play(r.Result);
            };
        }
        else
        {
            animancer.Stop(); //使用controller播放
            
            //切换当前的战斗动作
            var animator = GetAnimator();
            if (animator != null)
            {
                animator.SetBool("InBattle", true);
                animator.SetFloat("PosCode", float.Parse(battleIdlePoseCode));
                animator.SetTrigger("battle_idle");
            }
        }
    }
}
