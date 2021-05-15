using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSFrameWork;
using Jyx2;
using HSFrameWork.ConfigTable;
using System.IO;
using System.Threading;
using HSFrameWork.Common;
using Jyx2.Middleware;
using Jyx2.Setup;

public class Jyx2SkillEditor : MonoBehaviour
{
    public MapRole player;

    public Jyx2SkillEditorEnemy[] enemys;

    /// <summary>
    /// 技能ID
    /// </summary>
    [Header("技能ID")]
    public int SkillId;

    /// <summary>
    /// 技能等级
    /// </summary>
    [Header("技能等级")]
    public int SkillLevel;

    /// <summary>
    /// 播放启动
    /// </summary>
    [Header("播放技能")]
    public bool DisplaySkill;

    /// <summary>
    /// 切换技能
    /// </summary>
    [Header("切换技能")]
    public bool SwitchSkill;

    
    /// <summary>
    /// 角色ID
    /// </summary>
    [Header("切换的模型ID")]
    public int PlayerRoleId = 0;
    /// <summary>
    /// 切换角色模型
    /// </summary>
    [Header("切换角色模型")]
    public bool SwitchRoleModel;


    [Header("测试奔跑动作")]
    public bool SwitchMove;


    // Start is called before the first frame update
    void Start()
    {
        FileSystemWatcher watcher;

        //监控excel文件夹
        watcher = new FileSystemWatcher();
        watcher.BeginInit();
        watcher.Path = "excel";
        watcher.EnableRaisingEvents = true;
        watcher.IncludeSubdirectories = true;
        watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.Size;
        watcher.Changed += Watcher_Changed;
        watcher.EndInit();


        player.IsInBattle = true;
        DoSwitchRoleModel();
        Container.TryResolve<IXLsReloader>()?.Do();

    }

    private void Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        updateExcel = true;
    }

    bool updateExcel = false;


    void DoSwitchRoleModel()
    {
        var role = new RoleInstance(PlayerRoleId.ToString());
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

        //播放技能
        Jyx2Skill skill = ConfigTable.Get<Jyx2Skill>(SkillId.ToString());
        var wugong = new WugongInstance(SkillId);

        SkillCastHelper helper = new SkillCastHelper();
        helper.Source = player;
        helper.Targets = enemys;
        
        wugong.Level = SkillLevel;
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
        Jyx2Skill skill = ConfigTable.Get<Jyx2Skill>(SkillId.ToString());
        var wugong = new WugongInstance(SkillId);
        //切换武器和动作

        player.SwitchSkillTo(wugong);
    }

    // Update is called once per frame
    void Update()
    {
        if(DisplaySkill)
        {
            DisplaySkill = false;
            TryDisplaySkill();
        }

        if (updateExcel)
        {
            updateExcel = false;
            Container.TryResolve<IXLsReloader>()?.Do();
        }

        if(SwitchSkill)
        {
            SwitchSkill = false;
            SwitchSkillPose();
        }

        if(SwitchRoleModel)
        {
            SwitchRoleModel = false;

            DoSwitchRoleModel();
        }

        if (SwitchMove)
        {
            SwitchMove = false;
            DoSwitchMove();
        }
    }
}
