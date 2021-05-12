using HSFrameWork.ConfigTable;
using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Obsolete("已经使用技能编辑器实现，待删除")]
public class JYX2SkillDebugPanel : MonoBehaviour
{
    public Dropdown skillDropdown;
    public Dropdown levelDropdown;
    public InputField skillDetail;

    List<string> skills = new List<string>();
    List<string> levels = new List<string>();

    List<Jyx2Skill> allSkills = new List<Jyx2Skill>();
    void Start()
    {
        skillDropdown.ClearOptions();
        levelDropdown.ClearOptions();

        foreach(var skill in ConfigTable.GetAll<Jyx2Skill>())
        {
            allSkills.Add(skill);
            skills.Add(skill.Name);
        }
        skillDropdown.AddOptions(skills);

        for(int i = 0; i < 10; ++i)
        {
            levels.Add((i + 1).ToString());
        }
        levelDropdown.AddOptions(levels);

        skillDropdown.onValueChanged.AddListener((index) => {
            var skill = allSkills[index];
            //skillDetail.text = skill.SkillEffectCode;
        });
    }

    public void OnSubmit()
    {
        int level = (levelDropdown.value + 1);
        var skill = allSkills[skillDropdown.value];
        Debug.Log("OnSubmit:" + skill.Name + " " + skillDetail.text + " " + level);

        var currentRole = BattleHelper.Instance.GetCurrentRole();
        
        currentRole.Wugongs[0].Key = int.Parse(skill.Id);
        currentRole.Wugongs[0].Level = level;
        //用于刷新
        BattleHelper.Instance.SwitchStatesTo(BattleHelper.BattleViewStates.SelectMove);
    }
}
