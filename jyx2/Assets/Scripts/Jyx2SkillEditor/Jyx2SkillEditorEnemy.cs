/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System.Collections;
using System.Collections.Generic;
using Animancer;
using HSFrameWork.ConfigTable;
using UnityEngine;
using Jyx2;


public class Jyx2SkillEditorEnemy : Jyx2AnimationBattleRole
{
    public int SkillId;
    
    Animator animator;

    override public Animator GetAnimator()
    {
        return animator;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Jyx2Skill skill = ConfigTable.Get<Jyx2Skill>(SkillId.ToString());
        var wugong = new WugongInstance(SkillId);

        var display = wugong.GetDisplay();
        this.CurDisplay = display;

        this.Idle();
    }

}
