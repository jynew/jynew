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
