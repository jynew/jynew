using System.Collections;
using System.Collections.Generic;
using HSFrameWork.ConfigTable;
using Jyx2;
using Jyx2.Setup;
using UnityEngine;
using UnityEngine.UI;

public partial class SkillEditorUIPanel:Jyx2_UIBase
{
    
    public MapRole player;

    public Jyx2SkillEditorEnemy[] enemys;

	protected override void OnCreate()
    {
        InitTrans();
        this.iptSkillid_InputField.text = "1";
        this.iptSkillLevel_InputField.text = "0";
        this.iptModelId_InputField.text = "0";
        
        BindListener(this.btnDisplaySkill_Button,OnDisplaySkill);
        BindListener(this.btnSwitchSkill_Button,OnSwitchSkill);
        BindListener(this.btnRunAnim_Button,OnRunAnim);
        BindListener(this.btnSwitchModel_Button,OnSwitchModel);
        
	}

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        player = allParams[0] as MapRole;
        enemys = allParams[1] as Jyx2SkillEditorEnemy[];
        DoSwitchRoleModel();
    }

    private void OnSwitchModel()
    {
        DoSwitchRoleModel();
    }

    private void OnRunAnim()
    {
        DoSwitchMove();
    }

    private void OnSwitchSkill()
    {
        SwitchSkillPose();
    }

    private void OnDisplaySkill()
    {
        TryDisplaySkill();
    }

    void DoSwitchRoleModel()
    {
        var role = new RoleInstance(this.iptModelId_InputField.text);
        player.BindRoleInstance(role, () =>
        {
            var animator = player.GetAnimator();
            animator.runtimeAnimatorController = player.GetComponent<Animator>().runtimeAnimatorController; //force set animator

            SwitchSkillPose();
        });
    }

    void DoSwitchMove()
    {
        Debug.Log("do switch move");
        player.Run();
    }

    void TryDisplaySkill()
    { 

        
        int skillId = int.Parse(this.iptSkillid_InputField.text);
        int skillLevel = int.Parse(this.iptSkillLevel_InputField.text);
        //播放技能
        Jyx2Skill skill = ConfigTable.Get<Jyx2Skill>(skillId.ToString());
        var wugong = new WugongInstance(skillId);

        SkillCastHelper helper = new SkillCastHelper();
        helper.Source = player;
        helper.Targets = enemys;
        
        wugong.Level = skillLevel;
        helper.Zhaoshi = new BattleZhaoshiInstance(wugong);

        //直接在每个敌人身上受击
        List<Transform> blocks = new List<Transform>();
        foreach(var e in enemys)
        {
            blocks.Add(e.transform);
        }
        helper.CoverBlocks = blocks; 

        helper.Play();


    }

    /// <summary>
    /// 切换技能待机动作
    /// </summary>
    void SwitchSkillPose()
    {
        
       int  skillId = int.Parse(this.iptSkillid_InputField.text);
        Jyx2Skill skill = ConfigTable.Get<Jyx2Skill>(skillId.ToString());
        var wugong = new WugongInstance(skillId);
        //切换武器和动作

        player.SwitchSkillTo(wugong);
    }
}
