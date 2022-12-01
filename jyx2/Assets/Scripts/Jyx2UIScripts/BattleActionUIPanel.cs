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
using Jyx2.Middleware;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jyx2.Battle;
using Jyx2Configs;
using UnityEngine;
using UnityEngine.UI;
using i18n.TranslatorDef;
using Jyx2.Util;
using Jyx2.UINavigation;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

public partial class BattleActionUIPanel : Jyx2_UIBase
{
	[SerializeField]
	private string m_SkillItemPrefabPath;

	[ShowInInspector, ReadOnly]
	private List<Button> m_RightButtons = new List<Button>();
    
    public List<Button> RightActionBtns
	{
		get
		{
            if (m_RightButtons.Count == 0)
            {
				var btns = LeftActions_VerticalLayoutGroup.GetComponentsInChildren<Button>(true);
                m_RightButtons.AddRange(btns);
            }
            return m_RightButtons;
        }
	}

	public RoleInstance GetCurrentRole()
	{
		return m_currentRole;
	}

	RoleInstance m_currentRole;

	List<SkillUIItem> m_CurSkillItems = new List<SkillUIItem>();

	private List<SkillUIItem> m_CacheSkillItems = new List<SkillUIItem>();

	private bool isSelectMove;
	private Action<BattleLoop.ManualResult> OnManualAction;
	private List<BattleBlockVector> moveRange;
	private BattleFieldModel battleModel;
	private SkillCastInstance currentSkill;

	protected override void OnCreate()
	{
		InitTrans();

		BindListener(Move_Button, OnMoveClick);
		BindListener(UsePoison_Button, OnUsePoisonClick);
		BindListener(Depoison_Button, OnDepoisonClick);
		BindListener(Heal_Button, OnHealClick);
		BindListener(Item_Button, OnUseItemClick);
		BindListener(Wait_Button, OnWaitClick);
		BindListener(Rest_Button, OnRestClick);
		BindListener(Surrender_Button, OnSurrenderClick);
		BindListener(Cancel_Button, OnCancelClick);
	}

	protected Image getSkillCastButtonImage(Button button)
	{
		Transform trans = button.gameObject.transform;
		for (var i = 0; i < trans.childCount; i++)
		{
			var image = trans.GetChild(i).GetComponent<Image>();
			if (image != null && image.name == "ActionIcon")
				return image;
		}

		return null;
	}

	protected override void OnShowPanel(params object[] allParams)
	{
		base.OnShowPanel(allParams);
		m_currentRole = allParams[0] as RoleInstance;
		if (m_currentRole == null)
			return;

		moveRange = (List<BattleBlockVector>)allParams[1];
		OnManualAction = (Action<BattleLoop.ManualResult>)allParams[3];
		battleModel = BattleManager.Instance.GetModel();

        BattleboxHelper.Instance.OnControllerSelectBox -= onBattleBlockMove;
        BattleboxHelper.Instance.OnBlockConfirmed -= OnControllerSelectBlock;
        BattleboxHelper.Instance.OnControllerSelectBox += onBattleBlockMove;
		BattleboxHelper.Instance.OnBlockConfirmed += OnControllerSelectBlock;

		SetActionBtnState();
		RefreshSkill();

		TryFocusOnMoveButton();
    }

	private void onBattleBlockMove(BattleBlockData block)
	{
		//hide the skillCast selection
		changeCurrentSkillCastSelection(-1);

		ShowSkillCastHitRange(block);
	}

	private void OnControllerSelectBlock(BattleBlockData block)
	{
		ShowSkillCastHitRange();
		OnBlockConfirmed(block, false);
	}


	//显示攻击范围选择指示器
	void ShowAttackRangeSelector(SkillCastInstance skillCast)
	{
		currentSkill = skillCast;

		isSelectMove = false;

		BattleboxHelper.Instance.HideAllBlocks();
		var blockList = BattleManager.Instance.GetSkillUseRange(m_currentRole, skillCast);

		//prevent reselecting causing not showing hit range
		_lastHitRangeOverBlock = null;
		BattleboxHelper.Instance.ShowBlocks(m_currentRole, blockList, BattleBlockType.AttackZone, true);
	}

	private BattleBlockData _lastHitRangeOverBlock = null;
	private bool rightDpadPressed;
	private bool leftDpadPressed;

	private int cur_skillCast = 0;

	private void changeCurrentSkillCastSelection(int number)
	{
		cur_skillCast = number;

		if (number > -1)
		{
			BattleboxHelper.Instance.AnalogMoved = false;
		}
	}

	public override void Update()
	{
		base.Update();

		//显示当前攻击范围
		ShowSkillCastHitRange();

		//寻找玩家点击的格子
		var block = InputManager.Instance.GetMouseUpBattleBlock();

		//没有选择格子
		if (block == null) return;

		//格子隐藏（原则上应该不会出现）
		if (block.gameObject.activeSelf == false) return;

		//选择移动，但位置站人了
		if (isSelectMove && battleModel.BlockHasRole(block.BattlePos.X, block.BattlePos.Y)) return;


		//以下进行回调

		//移动
		OnBlockConfirmed(block, true);
	}

	private void ShowSkillCastHitRange(BattleBlockData block = null)
	{
		if (!isSelectMove)
		{
			var overBlock = block ?? InputManager.Instance.GetMouseOverBattleBlock();
			if (overBlock != null && overBlock != _lastHitRangeOverBlock)
			{
				_lastHitRangeOverBlock = overBlock;
				var range = BattleManager.Instance.GetSkillCoverBlocks(currentSkill, overBlock.BattlePos, m_currentRole.Pos);
				BattleboxHelper.Instance.ShowRangeBlocks(range);
			}
		}
	}

	private void OnBlockConfirmed(BattleBlockData block, bool isMouseClick)
	{
		if (!BattleboxHelper.Instance.AnalogMoved && !isMouseClick)
			return;

		//changeCurrentSelection(-1);

		if (isSelectMove)
		{
			TryCallback(new BattleLoop.ManualResult() { movePos = block.BattlePos }); //移动
		}
		else  //选择攻击
		{
			AIResult rst = new AIResult();
			rst.AttackX = block.BattlePos.X;
			rst.AttackY = block.BattlePos.Y;

			rst.SkillCast = currentSkill;

			TryCallback(new BattleLoop.ManualResult() { aiResult = rst });
		}
	}

	void TryCallback(BattleLoop.ManualResult ret)
	{
		BattleboxHelper.Instance.HideAllBlocks(true);
		OnManualAction?.Invoke(ret);
	}

	//点击了自动
	public void OnAutoClicked()
	{
		TryCallback(new BattleLoop.ManualResult() { isAuto = true });
	}

	protected override void OnHidePanel()
	{
		base.OnHidePanel();
		m_currentRole = null;
		m_CurSkillItems.Clear();
        isSelectMove = false;
        //隐藏格子
        BattleboxHelper.Instance?.HideAllBlocks();
	}

	void SetActionBtnState()
	{
		bool canMove = m_currentRole.movedStep <= 0;
		Move_Button.gameObject.BetterSetActive(canMove);
		bool canPoison = m_currentRole.UsePoison >= 20 && m_currentRole.Tili >= 10;
		UsePoison_Button.gameObject.BetterSetActive(canPoison);
		bool canDepoison = m_currentRole.DePoison >= 20 && m_currentRole.Tili >= 10;
		Depoison_Button.gameObject.BetterSetActive(canDepoison);
		bool canHeal = m_currentRole.Heal >= 20 && m_currentRole.Tili >= 50;
		Heal_Button.gameObject.BetterSetActive(canHeal);

		bool isLastRole = BattleManager.Instance.GetModel().IsLastRole(m_currentRole);
		Wait_Button.gameObject.BetterSetActive(!isLastRole);
		RefreshRightButtonNavigation();

    }

    private List<Selectable> _ActiveButtons = new List<Selectable>();
    void RefreshRightButtonNavigation()
    {
        _ActiveButtons.Clear();
        _ActiveButtons.AddRange(RightActionBtns.Where(btn => btn.gameObject.activeSelf));
        NavigateUtil.SetUpNavigation(_ActiveButtons, _ActiveButtons.Count, 1);
    }


    void RefreshSkill()
	{
		m_CurSkillItems.Clear();
		var skillCastList = m_currentRole.GetSkills(true).ToList();
		Action<int, SkillUIItem, SkillCastInstance> OnSkillItemCreate = (idx, item, data) =>
		{
			m_CurSkillItems.Add(item);
			item.OnSkillItemClick -= OnSelectSkillItem;
			item.OnSkillItemClick += OnSelectSkillItem;
        };

		MonoUtil.GenerateMonoElementsWithCacheList(m_SkillItemPrefabPath, skillCastList, m_CacheSkillItems, Skills_RectTransform, OnSkillItemCreate);
		NavigateUtil.SetUpNavigation(m_CurSkillItems, 1, m_CurSkillItems.Count);
	}

	void OnSelectSkillItem(SkillUIItem item)
	{
		m_currentRole.CurrentSkill = m_CurSkillItems.IndexOf(item);

		m_currentRole.SwitchAnimationToSkill(item.GetSkill().Data);
		ShowAttackRangeSelector(item.GetSkill());
	}

	public void OnCancelClick()
	{
		TryCallback(new BattleLoop.ManualResult() { isRevert = true });
	}

	public void OnMoveClick()
	{
		isSelectMove = true;
        _lastHitRangeOverBlock = null;
        BattleboxHelper.Instance.ShowBlocks(m_currentRole, moveRange, BattleBlockType.MoveZone, false);
    }

	public void OnUsePoisonClick()
	{
		var skillCast = new PoisonSkillCastInstance(m_currentRole.UsePoison);
		ShowAttackRangeSelector(skillCast);
	}

    public void OnDepoisonClick()
	{
		var skillCast = new DePoisonSkillCastInstance(m_currentRole.DePoison);
		ShowAttackRangeSelector(skillCast);
	}

	void OnHealClick()
	{
		if (!Heal_Button.gameObject.activeSelf)
			return;

		var skillCast = new HealSkillCastInstance(m_currentRole.Heal);
		ShowAttackRangeSelector(skillCast);
	}
    
	async void OnUseItemClick()
	{
		Func<Jyx2ConfigItem, bool> itemFilterFunc = item => item.GetItemType() == Jyx2ItemType.Costa 
														 || item.GetItemType() == Jyx2ItemType.Anqi;

		Action<int> OnItemSelected = itemId =>
		{
			if (itemId == -1)
				return;

			var item = GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(itemId);
			if (item == null)
				return;
			if (item.GetItemType() == Jyx2ItemType.Costa) //使用道具逻辑
			{
				if (m_currentRole.CanUseItem(itemId))
				{
					TryCallback(new BattleLoop.ManualResult() { aiResult = new AIResult() { Item = item } });
				}
			}
			else if (item.GetItemType() == Jyx2ItemType.Anqi) //使用暗器逻辑
			{
				var skillCast = new AnqiSkillCastInstance(m_currentRole.Anqi, item);
				ShowAttackRangeSelector(skillCast);
			}

		};

        await Jyx2_UIManager.Instance.ShowUIAsync(nameof(BagUIPanel), OnItemSelected, itemFilterFunc);
	}

	void OnWaitClick()
	{
		TryCallback(new BattleLoop.ManualResult() { isWait = true });
	}

	void OnRestClick()
	{
		TryCallback(new BattleLoop.ManualResult() { aiResult = new AIResult() { IsRest = true } });
	}

	public void OnSurrenderClick()
	{
		Action onConfirm = () => TryCallback(new BattleLoop.ManualResult() { isSurrender = true });

		MessageBox.ConfirmOrCancel("确定投降并放弃本场战斗?".GetContent(nameof(BattleActionUIPanel)), onConfirm);
	}

    public void TryFocusOnCurrentSkill()
	{
		if (m_CurSkillItems.Count == 0)
			return;
        var idx = m_currentRole?.CurrentSkill ?? 0;
        idx = Mathf.Clamp(idx, 0, m_CurSkillItems.Count - 1);
		m_CurSkillItems[idx].Select(false);
    }

    public void TryFocusOnMoveButton()
    {
		EventSystem.current.SetSelectedGameObject(Move_Button.gameObject);
    }

    public bool IsFocusOnSkillsItems
	{
		get
		{
			var curSelect = EventSystem.current.currentSelectedGameObject;
			if (curSelect == null)
				return false;
			if (!curSelect.activeInHierarchy)
				return false;
			return m_CurSkillItems.Any(item => item.gameObject == curSelect);
		}
	}
}
