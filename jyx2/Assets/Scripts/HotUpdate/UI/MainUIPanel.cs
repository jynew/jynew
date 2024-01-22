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
using System.Collections.Generic;
using System.Text;

public partial class MainUIPanel : Jyx2_UIBase
{
	public override UILayer Layer => UILayer.MainUI;

	protected override void OnCreate()
	{
		InitTrans();

		BindListener(XiakeButton_Button, OnXiakeBtnClick);
		BindListener(BagButton_Button, OnBagBtnClick);
		BindListener(SystemButton_Button, OnSystemBtnClick);
	}

	static HashSet<string> IgnorePanelTypes = new HashSet<string>(new[]
	{
		"CommonTipsUIPanel"
	});


	private StringBuilder coordinateBuilder = new StringBuilder();

	public override void Update()
	{
		base.Update();

		ShowCompass(Jyx2LuaBridge.jyx2_GetFlagInt("获得罗盘") == 1);
	}

	public void ShowCompass(bool flag)
	{
		ComassText_Text.gameObject.SetActive(LevelMaster.IsInWorldMap && flag);
		if (ComassText_Text.gameObject.activeSelf)
		{
			var player = LevelMaster.Instance.GetPlayer();
			if (player == null)
				return;
			coordinateBuilder.Clear();

			var playerPosition = player.transform.position;

			if (!player.IsOnBoat)
			{
				coordinateBuilder.AppendFormat("人({0},{1})", Mathf.Floor(playerPosition.z),
					Mathf.Floor(playerPosition.x));
				var boatPosition = player.GetBoatPosition();
				coordinateBuilder.AppendFormat(" 船({0},{1})", Mathf.Round(boatPosition.z), Mathf.Round(boatPosition.x));
			}
			else
			{
				coordinateBuilder.AppendFormat("船({0},{1})", Mathf.Floor(playerPosition.z),
					Mathf.Floor(playerPosition.x));
			}

			ComassText_Text.text = coordinateBuilder.ToString();
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
            //BY kk47: 这个按钮删了 2022/11/25
			//MapButton_Button.gameObject.SetActive(!isWorldMap);
		}
	}

	public async void OnXiakeBtnClick()
	{
		await Jyx2_UIManager.Instance.ShowUIAsync(nameof(XiakeUIPanel), GameRuntimeData.Instance.Player, GameRuntimeData.Instance.GetTeam().ToList());
	}

	public async void OnBagBtnClick()
	{
		await Jyx2_UIManager.Instance.ShowUIAsync(nameof(BagUIPanel), new Action<int>(OnUseItem));
	}

	async void OnUseItem(int id)
	{
		if (id == -1) return;

		var item = LuaToCsBridge.ItemTable[id];
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

	public async void OnSystemBtnClick()
	{
		await Jyx2_UIManager.Instance.ShowUIAsync(nameof(SystemUIPanel));
	}
}
