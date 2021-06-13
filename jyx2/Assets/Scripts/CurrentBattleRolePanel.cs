using Jyx2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentBattleRolePanel : MonoBehaviour
{
    public Text m_NameText;
    public Text m_DetailText;
    public Image m_Head;

    public void ShowRole(RoleInstance role)
    {
        this.gameObject.SetActive(true);

        m_NameText.text = role.Name;
        m_DetailText.text = string.Format("体力 {0}/100\n生命 {1}/{2}\n内力 {3}/{4}", role.Tili, role.Hp, role.MaxHp, role.Mp, role.MaxMp);

        Jyx2ResourceHelper.GetRoleHeadSprite(role, m_Head);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
