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
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

public partial class MainUIPanel : Jyx2_UIBase, IUIAnimator
{
	public override UILayer Layer => UILayer.MainUI;

	protected override void OnCreate()
	{
		InitTrans();

		BindListener(XiakeButton_Button, OnXiakeBtnClick);
		BindListener(BagButton_Button, OnBagBtnClick);
		BindListener(MapButton_Button, OnMapBtnClick);
		BindListener(SystemButton_Button, OnSystemBtnClick);

		//pre-load all icon sprites. somehow they don't load the first time
		foreach (var i in Enumerable.Range(0, 4))
			ChatUIPanel.getGamepadIconSprites(i);
	}

	public override void BindListener(UnityEngine.UI.Button button, Action callback, bool supportGamepadButtonsNav = true)
	{
		base.BindListener(button, callback, supportGamepadButtonsNav);
		getButtonImage(button)?.gameObject.SetActive(false);
	}

	static HashSet<string> IgnorePanelTypes = new HashSet<string>(new[]
	{
		"CommonTipsUIPanel"
	});
	private bool initialized;


	private StringBuilder coordinateBuilder = new StringBuilder();

	public override void Update()
	{
		base.Update();

		if (!initialized)
		{
			selectSystemButton();
			initialized = true;
		}

		Compass.gameObject.SetActive(LevelMaster.Instance.IsInWorldMap && Jyx2LuaBridge.HaveItem(182));
		if (Compass.gameObject.activeSelf)
		{
			var player = LevelMaster.Instance.GetPlayer();
			if (player == null)
				return;
			coordinateBuilder.Clear();

            int offsetX = 242, offsetZ = 435;
			var playerPosition = player.transform.position;
            coordinateBuilder.Append(Mathf.Floor(playerPosition.x + offsetX));
            coordinateBuilder.Append(",");
            coordinateBuilder.Append(Mathf.Floor(playerPosition.z + offsetZ));

            if (!player.IsOnBoat)
			{
				var boatPosition = player.GetBoatPosition();
				coordinateBuilder.Append("(");
				coordinateBuilder.Append(Mathf.Round(boatPosition.x + offsetX));
				coordinateBuilder.Append(",");
                coordinateBuilder.Append(Mathf.Round(boatPosition.z + offsetZ));
                coordinateBuilder.Append(")");

            }
			Compass.text = coordinateBuilder.ToString();
        }
	}

	protected override void OnShowPanel(params object[] allParams)
	{
		base.OnShowPanel(allParams);
		RefreshNameMapName();
		RefreshDynamic();
	}

	private void selectSystemButton()
	{
		var systemButtonIndex = activeButtons.ToList().IndexOf(SystemButton_Button);
		if (systemButtonIndex > -1)
		{
			changeCurrentSelection(systemButtonIndex);
		}
	}

	protected override void changeCurrentSelection(int num)
	{
		base.changeCurrentSelection(num);
		for (var i = 0; i < activeButtons.Length; i++)
		{
			var button = activeButtons[i];
			getButtonImage(button)?.gameObject.SetActive(i == num);
		}
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

	async void OnXiakeBtnClick()
	{
		await Jyx2_UIManager.Instance.ShowUIAsync(nameof(XiakeUIPanel), GameRuntimeData.Instance.Player, GameRuntimeData.Instance.GetTeam().ToList());
	}

	async void OnBagBtnClick()
	{
		await Jyx2_UIManager.Instance.ShowUIAsync(nameof(BagUIPanel), GameRuntimeData.Instance.Items, new Action<int>(OnUseItem));
	}

	async void OnUseItem(int id)
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

		async void Action()
		{
			async void Callback(RoleInstance selectRole)
			{
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
						if (item.NeedCastration == 1) //辟邪剑谱和葵花宝典
						{
							await GameUtil.ShowYesOrNoCastrate(selectRole, () =>
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
					GameUtil.DisplayPopinfo((int)item.ItemType == 1 ? "此人不适合配备此物品" : "此人不适合修炼此物品");
					return;
				}
			}

			await GameUtil.SelectRole(runtime.GetTeam(), Callback);
		}

		await GameUtil.ShowYesOrNoUseItem(item, Action);

	}

	void OnMapBtnClick()
	{
		var levelMaster = LevelMaster.Instance;

		if (levelMaster.IsInWorldMap)
			return;

		//执行离开事件
		foreach (var zone in FindObjectsOfType<MapTeleportor>())
		{
			if (LevelMaster.GetCurrentGameMap().Tags.Contains("WORLDMAP"))
			{
				zone.DoTransport();
				break;
			}
		}
	}

	async void OnSystemBtnClick()
	{
		await Jyx2_UIManager.Instance.ShowUIAsync(nameof(SystemUIPanel));
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

	protected override bool captureGamepadAxis => true;

	protected override void OnDirectionalDown()
	{
		//do nothing
	}

	protected override void OnDirectionalUp()
	{
		//do nothing
	}

	protected override void OnDirectionalLeft()
	{
		base.OnDirectionalUp();
	}

	protected override void OnDirectionalRight()
	{
		base.OnDirectionalDown();
	}

	protected override string confirmButtonName()
	{
		return GamepadHelper.START_BUTTON;
	}

	//don't reset to 0 for this main, since it will select the system button automatically
	protected override bool resetCurrentSelectionOnShow => false;
}
