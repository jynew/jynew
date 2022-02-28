/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using Jyx2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIItem : MonoBehaviour
{
    SkillCastInstance m_currentSkill;

    private bool hasInit = false;
    Image m_icon;
    Text m_skillText;
    Transform m_select;
    void InitTrans() 
    {
        if (hasInit)
            return;
        hasInit = true;
        m_icon = transform.Find("Icon").GetComponent<Image>();
        m_skillText = transform.Find("SkillText").GetComponent<Text>();
        m_select = transform.Find("CurrentTag");
    }

    public void RefreshSkill(SkillCastInstance skill) 
    {
        InitTrans();
        m_currentSkill = skill;
        //TODO 更新icon
        SkillCastInstance.SkillCastStatus state = skill.GetStatus();
        string skillText = $"{skill.Data.Name}\nLv.{skill.Data.GetLevel()}";
        m_skillText.text = skillText;
    }

    public void SetSelect(bool se) 
    {
        m_select.gameObject.SetActive(se);
    }

    public SkillCastInstance GetSkill() 
    {
        return m_currentSkill;
    }
}
