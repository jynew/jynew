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
        UpdateHealthValue();
    }

    int preHp = -1;
    private void Update()
    {
        if (currentRole == null)
            return;
        CheckShouldHide();
        UpdatePosition();
        UpdateHealthValue();
    }

    void CheckShouldHide()
    {
        if (IsRoleNotValid)
            gameObject.SetActive(false);
    }


    void UpdateHealthValue()
    {
        if (IsRoleNotValid)
            return;
        if (!currentRole.View.HPBarIsDirty || currentRole.Hp == preHp)
            return;
        currentRole.View.UnmarkHpBarIsDirty();
        preHp = currentRole.Hp;
        hpBar.SetValue((float)currentRole.Hp / currentRole.MaxHp);
        UpdateHpColor();
    }

    void UpdateHpColor()
    {
        if (currentRole.Poison > 0)
        {
            hpBar.fillImage.color = Color.cyan;
            return;
        }
        hpBar.fillImage.color = currentRole.team == 0 ? Color.green : Color.red;
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
