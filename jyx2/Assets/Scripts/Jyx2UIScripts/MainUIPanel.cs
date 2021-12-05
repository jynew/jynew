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
using UnityEngine;
using System;
using System.Linq;
using Jyx2Configs;

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

    public void Update()
    {
        if (GameRuntimeData.Instance.isShowCompass != Compass.gameObject.active)
        {
            Compass.gameObject.active = GameRuntimeData.Instance.isShowCompass;
        }

        if (Compass.gameObject.active)
        {
            var p = LevelMaster.Instance.GetPlayerPosition();
            var pString = (p.x + 242).ToString("F0") + "," + (p.z + 435).ToString("F0");
            if (!LevelMaster.Instance.GetPlayer().IsOnBoat)
            {
                var b = LevelMaster.Instance.GetPlayer().GetBoatPosition();
                pString += "("+(b.x + 242).ToString("F0") + "," + (b.z + 435).ToString("F0")+")";
            }
            Compass.text = pString;
        }
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
        
        var map = LevelMaster.GetCurrentGameMap();
        if (map != null)
        {
            MapName_Text.text = map.GetShowName();

            //BY CGGG：小地图不提供传送到大地图的功能 2021/6/13
            //MapButton_Button.gameObject.SetActive(!isWorldMap);
            MapButton_Button.gameObject.SetActive(false);
            
            
            //var rt = Image_Right.GetComponent<RectTransform>();
　　		//rt.sizeDelta = new Vector2(isWorldMap?480:640, 100);
        }
    }

    void OnXiakeBtnClick() 
    {
        Jyx2_UIManager.Instance.ShowUI(nameof(XiakeUIPanel), GameRuntimeData.Instance.Player, GameRuntimeData.Instance.GetTeam().ToList());
    }

    void OnBagBtnClick() 
    {
        Jyx2_UIManager.Instance.ShowUI(nameof(BagUIPanel), GameRuntimeData.Instance.Items,new Action<int>(OnUseItem));
    }

    void OnUseItem(int id)
    {
        if (id == -1) return;

        var item = GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(id);
        if (item == null)
        {
            Debug.LogError("use item error, id=" + id);
            return;
        }

        //剧情类和暗器不能使用
        if ((int)item.ItemType == 0 || (int)item.ItemType == 4)
        {
            GameUtil.DisplayPopinfo("此道具不能在此使用");
            return;
        }

        var runtime = GameRuntimeData.Instance;
        GameUtil.SelectRole(runtime.GetTeam(), (selectRole) => {
            if (selectRole == null) return;

            if (selectRole.GetJyx2RoleId() == runtime.GetItemUser(item.Id)) return;

            if (selectRole.CanUseItem(id))
            {
                //装备
                if ((int)item.ItemType == 1)
                {
                    //武器
                    if ((int)item.EquipmentType == 0)
                    {
                        if (runtime.GetItemUser(item.Id) != -1)
                        {
                            RoleInstance roleInstance = runtime.GetRoleInTeam(runtime.GetItemUser(item.Id));
                            roleInstance.UnequipItem(roleInstance.GetWeapon());
                            roleInstance.Weapon = -1;
                        }

                        selectRole.UnequipItem(selectRole.GetWeapon());
                        selectRole.Weapon = id;
                        selectRole.UseItem(selectRole.GetWeapon());
                        runtime.SetItemUser(item.Id, selectRole.GetJyx2RoleId());
                        GameUtil.DisplayPopinfo($"{selectRole.Name}使用了{item.Name}");
                    }
                    //防具
                    else if ((int)item.EquipmentType == 1)
                    {
                        if (runtime.GetItemUser(item.Id) != -1)
                        {
                            RoleInstance roleInstance = runtime.GetRoleInTeam(runtime.GetItemUser(item.Id));
                            roleInstance.UnequipItem(roleInstance.GetArmor());
                            roleInstance.Armor = -1;
                        }

                        selectRole.UnequipItem(selectRole.GetArmor());
                        selectRole.Armor = id;
                        selectRole.UseItem(selectRole.GetArmor());
                        runtime.SetItemUser(item.Id, selectRole.GetJyx2RoleId());
                        GameUtil.DisplayPopinfo($"{selectRole.Name}使用了{item.Name}");
                    }
                }
                //修炼
                else if ((int)item.ItemType == 2)
                {
                    if (item.NeedCastration)//辟邪剑谱和葵花宝典
                    {
                        GameUtil.ShowYesOrNoCastrate(selectRole, () =>
                        {
                            if (runtime.GetItemUser(item.Id) != -1)
                            {
                                RoleInstance roleInstance = runtime.GetRoleInTeam(runtime.GetItemUser(item.Id));
                                runtime.SetItemUser(item.Id, -1);
                                roleInstance.ExpForItem = 0;
                                roleInstance.Xiulianwupin = -1;
                            }
                            if (selectRole.GetXiulianItem() != null)
                            {
                                runtime.SetItemUser(selectRole.Xiulianwupin, -1);
                                selectRole.ExpForItem = 0;
                            }
                            selectRole.Xiulianwupin = id;
                            runtime.SetItemUser(item.Id, selectRole.GetJyx2RoleId());
                            GameUtil.DisplayPopinfo($"{selectRole.Name}使用了{item.Name}");
                        });
                    }
                    else
                    {
                        if (runtime.GetItemUser(item.Id) != -1)
                        {
                            RoleInstance roleInstance = runtime.GetRoleInTeam(runtime.GetItemUser(item.Id));
                            runtime.SetItemUser(item.Id, -1);
                            roleInstance.ExpForItem = 0;
                            roleInstance.Xiulianwupin = -1;
                        }
                        if (selectRole.GetXiulianItem() != null)
                        {
                            runtime.SetItemUser(selectRole.Xiulianwupin, -1);
                            selectRole.ExpForItem = 0;
                        }
                        selectRole.Xiulianwupin = id;
                        runtime.SetItemUser(item.Id, selectRole.GetJyx2RoleId());
                        GameUtil.DisplayPopinfo($"{selectRole.Name}使用了{item.Name}");
                    }
                }
                //药品
                else if ((int)item.ItemType == 3)
                {
                    selectRole.UseItem(item);
                    runtime.AddItem(id, -1);
                    GameUtil.DisplayPopinfo($"{selectRole.Name}使用了{item.Name}");
                }
            }
            else
            {
                GameUtil.DisplayPopinfo("此人不适合修炼此物品");
                return;
            }
        });
    }

    void OnMapBtnClick() 
    {
        var levelMaster = LevelMaster.Instance;

        if (levelMaster.IsInWorldMap)
            return;
        
        //执行离开事件
        foreach (var zone in FindObjectsOfType<MapTeleportor>())
        {
            if (zone.m_GameMap.Id == GameConst.WORLD_MAP_ID)
            {
                zone.DoTransport();
                break;
            }
        }
    }

    void OnSystemBtnClick() 
    {
        Jyx2_UIManager.Instance.ShowUI(nameof(SystemUIPanel));
    }

    public void DoShowAnimator()
    {
        //AnimRoot_RectTransform.anchoredPosition = new Vector2(0, 150);
        //AnimRoot_RectTransform.DOAnchorPosY(-50, 1.0f);
    }

    public void DoHideAnimator()
    {
        
    }

    private void OnEnable()
    {
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Escape, () =>
        {
            if (LevelMaster.Instance.IsPlayerCanControl())
            {
                OnSystemBtnClick();
            }
        });
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.X, () =>
        {
            if (LevelMaster.Instance.IsPlayerCanControl())
            {
                OnXiakeBtnClick();
            }
        });
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.B, () =>
        {
            if (LevelMaster.Instance.IsPlayerCanControl())
            {
                OnBagBtnClick();
            }
        });
    }

    private void OnDisable()
    {
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Escape);
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.X);
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.B);
    }

}
