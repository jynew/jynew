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

        m_Head.LoadAsyncForget(role.Data.GetPic());
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
