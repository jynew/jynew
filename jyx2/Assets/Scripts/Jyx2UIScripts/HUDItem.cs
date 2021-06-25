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

    public void BindRole(RoleInstance role) 
    {
        currentRole = role;
        UpdateAll();
    }

    int preHp = -1;
    private void Update()
    {
        if (currentRole == null)
            return;
        UpdatePosition();
        if (!currentRole.View.HPBarIsDirty || currentRole.Hp == preHp)
            return;
        currentRole.View.UnmarkHpBarIsDirty();
        UpdateAll();
    }

    void UpdateAll()
    {
        preHp = currentRole.Hp;
        hpBar.SetValue((float)currentRole.Hp / currentRole.MaxHp);
        UpdateHpColor();
        if (currentRole.IsDead())
            transform.gameObject.SetActive(false);
    }

    Vector3 hpPos;
    void UpdatePosition() 
    {
        hpPos = Jyx2_UIManager.Instance.GetUICamera().WorldToScreenPoint(currentRole.View.transform.position);
        rectTrans.position = hpPos;
    }

    void UpdateHpColor()
    {
        //hpBar.fillImage.sprite = null;
        if (currentRole.Poison > 0) 
        {
            hpBar.fillImage.color = Color.cyan;
            return;
        }
        hpBar.fillImage.color = currentRole.team == 0 ? Color.green : Color.red;
    }

    private void OnDisable()
    {
        currentRole = null;
        preHp = -1;
    }
}
