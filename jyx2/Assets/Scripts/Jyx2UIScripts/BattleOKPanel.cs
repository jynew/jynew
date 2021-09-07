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
using HSFrameWork.ConfigTable;
using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class BattleOKPanel:Jyx2_UIBase
{
    public override UILayer Layer => UILayer.PopupUI;

    Vector3 followPos;
    Action okCallback;
    Action cancelCallback;
    protected override void OnCreate()
    {
        InitTrans();
        BindListener(OKBtn_Button, OnOKClick);
        BindListener(CancelBtn_Button, OnCancelClick);
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        followPos = (Vector3)allParams[0];
        if (allParams.Length > 1)
            okCallback = allParams[1] as Action;
        if (allParams.Length > 2)
            cancelCallback = allParams[2] as Action;
        ResetPos();
        ShowSkillDamage();
    }

    void ResetPos() 
    {
        Camera uiCamera = Jyx2_UIManager.Instance.GetUICamera();
        Vector3 pos = uiCamera.WorldToScreenPoint(followPos);
        Root_RectTransform.position = pos;
    }

    void ShowSkillDamage() 
    {
        BattleZhaoshiInstance zhaoshi = BattleStateMechine.Instance.CurrentZhaoshi;
        RoleInstance role = BattleStateMechine.Instance.CurrentRole;
        if (zhaoshi == null || role == null)
            return;
        //int level_index = zhaoshi.Data.GetLevel();
        //int damage = role.Attack + zhaoshi.Data.GetSkillLevelInfo(level_index).Attack / 3;
        //if (role.Weapon >= 0)
        //{
        //    var i = ConfigTable.Get<Jyx2Item>(role.Weapon);
        //    damage += i.Attack;
        //}
        //if (role.Armor >= 0)
        //{
        //    var i = ConfigTable.Get<Jyx2Item>(role.Armor);
        //    damage += i.Attack;
        //}
        DamageText_Text.text = $"{zhaoshi.Data.Name} Lv.{zhaoshi.Data.GetLevel()}";
    }

    void OnOKClick() 
    {
        Jyx2_UIManager.Instance.HideUI(nameof(BattleOKPanel));
        if (okCallback != null) 
        {
            okCallback();
            okCallback = null;
        }
    }

    void OnCancelClick() 
    {
        Jyx2_UIManager.Instance.HideUI(nameof(BattleOKPanel));
        if (cancelCallback != null)
        {
            cancelCallback();
            cancelCallback = null;
        }
    }
}
