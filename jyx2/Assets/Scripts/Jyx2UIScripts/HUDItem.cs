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

public class HUDItem : MonoBehaviour
{
    HPBar hpBar;
    RoleInstance currentRole;
    RectTransform rectTrans;

    #region 血条预定义颜色

    private static readonly Color AllyColor = Color.green;

    private static readonly Color AllyPoisonColor = Color.cyan;

    private static readonly Color EnemyColor = Color.red;

    private static readonly Color EnemyPoisonColor = new Color(216f / 255, 0f, 189f / 255);

    #endregion

    public void Init() 
    {
        hpBar = transform.Find("HpBar").GetComponent<HPBar>();
        hpBar.Init();
        rectTrans = transform as RectTransform;
    }

    private bool IsRoleNotValid => currentRole == null || currentRole.View == null || currentRole.IsDead();


    public void BindRole(RoleInstance role) 
    {
        currentRole = role;
        RefreshHpBar();
    }

    int preHp = -1;
    private void Update()
    {
        if (currentRole == null)
            return;
        CheckShouldHide();
        UpdatePosition();
        TryUpdateHealthValue();
    }

    void CheckShouldHide()
    {
        if (IsRoleNotValid)
            gameObject.SetActive(false);
    }


    void TryUpdateHealthValue()
    {
        if (IsRoleNotValid)
            return;
        if (!currentRole.View.HPBarIsDirty)
            return;
        currentRole.View.UnmarkHpBarIsDirty();
        preHp = currentRole.Hp;
        RefreshHpBar();
    }

    void RefreshHpBar()
    {
        if (currentRole == null)
            return;
        hpBar.SetValue((float)currentRole.Hp / currentRole.MaxHp);
        UpdateHpColor();
    }


    void UpdateHpColor()
    {
        if (currentRole == null)
            return;
        if (currentRole.Poison > 0)
        {
            hpBar.fillImage.color = currentRole.team == 0 ? AllyPoisonColor : EnemyPoisonColor;
        }
        else
        {
            hpBar.fillImage.color = currentRole.team == 0 ? AllyColor : EnemyColor;
        }
    }

    void UpdatePosition() 
    {
        if (IsRoleNotValid)
            return;
        rectTrans.position = Jyx2_UIManager.Instance.GetUICamera().WorldToScreenPoint(currentRole.View.transform.position);
    }

    private void OnDisable()
    {
        currentRole = null;
        preHp = -1;
    }
}
