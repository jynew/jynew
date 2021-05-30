using Jyx2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using HSFrameWork.ConfigTable;
using Jyx2;
using System;

public partial class MainUIPanel : Jyx2_UIBase,IUIAnimator
{
    public override UILayer Layer => UILayer.MainUI;

    protected override void OnCreate()
    {
        InitTrans();

        XiakeButton_Button.onClick.AddListener(OnXiakeBtnClick);
        BagButton_Button.onClick.AddListener(OnBagBtnClick);
        MapButton_Button.onClick.AddListener(OnMapBtnClick);
        SystemButton_Button.onClick.AddListener(OnSystemBtnClick);
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        RefreshNameMapName();
        RefreshDynamic();
    }

    void RefreshDynamic() 
    {
        RoleInstance role = GameRuntimeData.Instance.Player;
        string expText = string.Format("EXP:{0}/{1}", role.Exp, role.GetLevelUpExp());
        Exp_Text.text = expText;
        Level_Text.text = role.Level.ToString();
    }

    void RefreshNameMapName() 
    {
        RoleInstance role = GameRuntimeData.Instance.Player;
        Name_Text.text = role.Name;
        
        GameMap map = LevelMaster.Instance.GetCurrentGameMap();
        if (map != null)
        {
            MapName_Text.text = map.GetShowName();
            bool isWolrd = map.Tags.Contains("WORLDMAP");
            MapButton_Button.gameObject.SetActive(!isWolrd);
			// for change main ui panel background image
			// added by eaphone at 2021/05/27
			var rt = Image_Right.GetComponent<RectTransform>();
　　		rt.sizeDelta = new Vector2(isWolrd?480:640, 100);
			
        }
    }

    void OnXiakeBtnClick() 
    {
        Jyx2_UIManager.Instance.ShowUI("XiakeUIPanel", GameRuntimeData.Instance.Player, GameRuntimeData.Instance.Team);
    }

    void OnBagBtnClick() 
    {
        Jyx2_UIManager.Instance.ShowUI("BagUIPanel", GameRuntimeData.Instance.Items,new Action<int>(OnUseItem));
    }

    void OnUseItem(int id)
    {
        if (id == -1) return;

        var item = ConfigTable.Get<Jyx2Item>(id);
        if (item == null)
        {
            Debug.LogError("use item error, id=" + id);
            return;
        }

        //can use item
        if (item.ItemType != 3)
        {
            GameUtil.DisplayPopinfo("此道具不能在此使用");
            return;
        }

        var runtime = GameRuntimeData.Instance;
        GameUtil.SelectRole(runtime.Team, (selectRole) => {
            if (selectRole == null) return;

            if (selectRole.CanUseItem(id))
            {
                selectRole.UseItem(item);
                runtime.AddItem(id, -1);
                GameUtil.DisplayPopinfo($"{selectRole.Name}使用了{item.Name}");
            }
        });
    }

    void OnMapBtnClick() 
    {
        var levelMaster = LevelMaster.Instance;
        if (!levelMaster.GetCurrentGameMap().Tags.Contains("WORLDMAP"))
        {
            levelMaster.PlayLeaveMusic(levelMaster.GetCurrentGameMap());
            // return to entertrance
			// modified by eaphone at 2021/05/30
            //LevelLoader.LoadGameMap("0_BigMap");
            LevelLoader.LoadGameMap("0_BigMap&transport#" + levelMaster.GetCurrentGameMap().BigMapTriggerName);
        }
    }

    void OnSystemBtnClick() 
    {
        Jyx2_UIManager.Instance.ShowUI("SystemUIPanel");
    }

    public void DoShowAnimator()
    {
        //AnimRoot_RectTransform.anchoredPosition = new Vector2(0, 150);
        //AnimRoot_RectTransform.DOAnchorPosY(-50, 1.0f);
    }

    public void DoHideAnimator()
    {
        
    }
}
