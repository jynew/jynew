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
using Jyx2.UINavigation;
using Jyx2.Util;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillUIItem : MonoBehaviour,IDataContainer<SkillCastInstance>,INavigable
{
    SkillCastInstance m_currentSkill;
    
    Text m_skillText;
    Button m_ClickButton;

    public event Action<SkillUIItem> OnSkillItemClick;
    
    
    void Awake()
    {
        InitTrans();
        m_ClickButton.onClick.AddListener(OnButtonClick);
        OnSkillItemClick = null;
    }

    void OnDestroy()
    {
        m_ClickButton.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        OnSkillItemClick?.Invoke(this);
    }

    void InitTrans() 
    {
        m_ClickButton = transform.GetComponent<Button>();
        m_skillText = transform.Find("SkillText").GetComponent<Text>();
    }

    public void SetData(SkillCastInstance skill) 
    {
        m_currentSkill = skill;
        //TODO 更新icon
        SkillCastInstance.SkillCastStatus state = skill.GetStatus();
        string skillText = $"{skill.Data.Name}\nLv.{skill.Data.GetLevel()}";
        m_skillText.text = skillText;
    }

    public SkillCastInstance GetSkill() 
    {
        return m_currentSkill;
    }

    public void Connect(INavigable up = null, INavigable down = null, INavigable left = null, INavigable right = null)
    {
        if (m_ClickButton == null)
            return;
        Navigation navigation = new Navigation();
        navigation.mode = Navigation.Mode.Explicit;
        navigation.selectOnUp = up?.GetSelectable();
        navigation.selectOnDown = down?.GetSelectable();
        navigation.selectOnLeft = left?.GetSelectable();
        navigation.selectOnRight = right?.GetSelectable();
        m_ClickButton.navigation = navigation;
    }

    public Selectable GetSelectable()
    {
        return m_ClickButton;
    }

    public void Select(bool notifyEvent)
    {
        EventSystem.current.SetSelectedGameObject(m_ClickButton.gameObject);
        if (notifyEvent)
        {
            m_ClickButton.onClick?.Invoke();
        }
    }
}
